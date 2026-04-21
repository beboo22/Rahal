using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ApplicationBusiness.RealTimeservice.NotificationService
{
    public static class NotificationTypes
    {
        public const string GuideRequest = "GuideRequest";
        public const string Message = "Message";
        public const string Booking = "Booking";
        public const string Payment = "Payment";
        public const string Like = "Like";
        public const string Comment = "Comment";
        public const string System = "System";
    }
    public class NotificationDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Body { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
    public interface INotificationService
    {
        Task SendAsync(NotificationDto notification);
        Task SendToManyAsync(List<string> userIds, NotificationDto notification);
        Task<List<NotificationDto>> GetUserNotificationsAsync(string userId);
        Task MarkAsReadAsync(string notificationId, string userId);
        Task MarkAllAsReadAsync(string userId);
    }

    public class NotificationService : INotificationService
    {
        private readonly IHubContext<AppHub> _hubContext;
        private readonly IConnectionMultiplexer _redis;

        private readonly IDatabase _db;

        public NotificationService(
            IHubContext<AppHub> hubContext,
            IConfiguration config,
            IConnectionMultiplexer redis)
        {
            var connectionString = config["Redis:ConnectionString"];
            _redis = ConnectionMultiplexer.Connect(
            new ConfigurationOptions
            {
                EndPoints = { { "redis-19301.c341.af-south-1-1.ec2.redns.redis-cloud.com", 19301 } },
                User = "default",
                Password = "uJhzvCJD1pjVz9lBh4gKVc9OrKRL9pTR"
            }
            );
            //_redis = ConnectionMultiplexer.Connect(connectionString);
            _hubContext = hubContext;
            _db = redis.GetDatabase();
        }

        private string GetUnreadCountKey(string userId)
    => $"notification_count:{userId}";
        private string GetKey(string userId)
            => $"notifications:{userId}";

        // ---------------- SEND ----------------

        public async Task SendAsync(NotificationDto notification)
        {
            var key = GetKey(notification.UserId);
            var countKey = GetUnreadCountKey(notification.UserId);

            await _db.ListLeftPushAsync(
                key,
                JsonSerializer.Serialize(notification));

            await _db.StringIncrementAsync(countKey);

            var unreadCount = await _db.StringGetAsync(countKey);

            await _hubContext.Clients
                .User(notification.UserId)
                .SendAsync("ReceiveNotification", notification);

            await _hubContext.Clients
                .User(notification.UserId)
                .SendAsync("UnreadNotificationCount", (int)unreadCount);
        }

        public async Task SendToManyAsync(List<string> userIds, NotificationDto notification)
        {
            foreach (var userId in userIds)
            {
                await SendAsync(new NotificationDto
                {
                    UserId = userId,
                    Title = notification.Title,
                    Body = notification.Body,
                    Type = notification.Type,
                    ReferenceId = notification.ReferenceId
                });
            }
        }

        // ---------------- GET ----------------

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(string userId)
        {
            var key = GetKey(userId);

            var data = await _db.ListRangeAsync(key, 0, -1);

            return data
                .Select(x => JsonSerializer.Deserialize<NotificationDto>(x!)!)
                .ToList();
        }
        public async Task<int> GetUnreadCountAsync(string userId)
        {
            var value = await _db.StringGetAsync(GetUnreadCountKey(userId));

            return value.HasValue ? (int)value : 0;
        }

        // ---------------- READ ----------------
        public async Task MarkAsReadAsync(string notificationId, string userId)
        {
            var key = GetKey(userId);
            var countKey = GetUnreadCountKey(userId);

            var data = await _db.ListRangeAsync(key);

            for (int i = 0; i < data.Length; i++)
            {
                var item = JsonSerializer.Deserialize<NotificationDto>(data[i]!);

                if (item?.Id == notificationId && !item.IsRead)
                {
                    item.IsRead = true;

                    await _db.ListSetByIndexAsync(
                        key,
                        i,
                        JsonSerializer.Serialize(item));

                    await _db.StringDecrementAsync(countKey);

                    break;
                }
            }

            var unread = await GetUnreadCountAsync(userId);

            await _hubContext.Clients
                .User(userId)
                .SendAsync("UnreadNotificationCount", unread);
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var key = GetKey(userId);

            var data = await _db.ListRangeAsync(key);

            for (int i = 0; i < data.Length; i++)
            {
                var item = JsonSerializer.Deserialize<NotificationDto>(data[i]!);

                if (item != null)
                {
                    item.IsRead = true;

                    await _db.ListSetByIndexAsync(
                        key,
                        i,
                        JsonSerializer.Serialize(item));
                }
            }

            await _db.StringSetAsync(GetUnreadCountKey(userId), 0);

            await _hubContext.Clients
                .User(userId)
                .SendAsync("UnreadNotificationCount", 0);
        }
    }
}
