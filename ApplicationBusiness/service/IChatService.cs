using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Text.Json;

namespace ApplicationBusiness.service
{
    public interface IChatService
    {
        Task<List<string>> GetConversationListAsync(string userId);
        Task<ChatMessageDto> SaveMessageAsync(string senderId, string receiverId, string message);
        Task<List<ChatMessageDto>> GetChatHistoryAsync(string senderId, string receiverId);
        Task MarkMessagesAsDeliveredAsync(string receiverId);
        Task MarkMessagesAsDeliveredAsync(string receiverId, string senderId);
        Task MarkMessagesAsReadAsync(string receiverId, string senderId);
        Task UpdateStatusAsync(ChatMessageDto message);
    }
    public class ChatMessageDto
    {
        public string SenderId { get; set; } = default!;
        public string ReceiverId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } = "Sent"; // Sent | Delivered | Read
    }


    public class ChatService : IChatService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public ChatService(IConfiguration config)
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
            _db = _redis.GetDatabase();
        }

        // Generate consistent chat key
        private static string GetChatKey(string user1, string user2)
        {
            var ordered = new[] { user1, user2 }.OrderBy(x => x).ToArray();
            return $"chat:{ordered[0]}:{ordered[1]}";
        }

        private static string GetConversationKey(string userId) => $"conversations:{userId}";

        // ----------------- CORE METHODS -----------------

        public async Task<ChatMessageDto> SaveMessageAsync(string senderId, string receiverId, string message)
        {
            var chatMessage = new ChatMessageDto
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = message,
                Timestamp = DateTime.UtcNow,
                Status = "Sent"
            };

            var chatKey = GetChatKey(senderId, receiverId);
            var serialized = JsonSerializer.Serialize(chatMessage);

            // Save message to chat list
            await _db.ListRightPushAsync(chatKey, serialized);

            // Update recent conversations
            await UpdateConversationListAsync(senderId, receiverId, chatMessage.Timestamp);
            await UpdateConversationListAsync(receiverId, senderId, chatMessage.Timestamp);

            return chatMessage;
        }

        public async Task<List<ChatMessageDto>> GetChatHistoryAsync(string senderId, string receiverId)
        {
            var chatKey = GetChatKey(senderId, receiverId);
            var messages = await _db.ListRangeAsync(chatKey, 0, -1);

            return messages
                .Select(v => JsonSerializer.Deserialize<ChatMessageDto>(v!)!)
                .OrderBy(m => m.Timestamp)
                .ToList();
        }

        public async Task MarkMessagesAsDeliveredAsync(string receiverId)
        {
            var endpoints = _redis.GetEndPoints();
            var server = _redis.GetServer(endpoints.First());
            var keys = server.Keys(pattern: $"chat:*:{receiverId}");

            foreach (var key in keys)
            {
                await UpdateStatusForMessagesAsync(key, "Delivered");
            }
        }

        public async Task MarkMessagesAsDeliveredAsync(string receiverId, string senderId)
        {
            var chatKey = GetChatKey(senderId, receiverId);
            await UpdateStatusForMessagesAsync(chatKey, "Delivered");
        }

        public async Task MarkMessagesAsReadAsync(string receiverId, string senderId)
        {
            var chatKey = GetChatKey(senderId, receiverId);
            await UpdateStatusForMessagesAsync(chatKey, "Read");
        }

        public async Task UpdateStatusAsync(ChatMessageDto message)
        {
            var chatKey = GetChatKey(message.SenderId, message.ReceiverId);
            var messages = await _db.ListRangeAsync(chatKey, 0, -1);

            for (int i = 0; i < messages.Length; i++)
            {
                var msg = JsonSerializer.Deserialize<ChatMessageDto>(messages[i]!)!;
                if (msg.Timestamp == message.Timestamp && msg.SenderId == message.SenderId)
                {
                    msg.Status = message.Status;
                    await _db.ListSetByIndexAsync(chatKey, i, JsonSerializer.Serialize(msg));
                    break;
                }
            }
        }

        private async Task UpdateStatusForMessagesAsync(RedisKey chatKey, string newStatus)
        {
            var messages = await _db.ListRangeAsync(chatKey, 0, -1);

            for (int i = 0; i < messages.Length; i++)
            {
                var msg = JsonSerializer.Deserialize<ChatMessageDto>(messages[i]!)!;
                if (msg.Status != "Read")
                {
                    msg.Status = newStatus;
                    await _db.ListSetByIndexAsync(chatKey, i, JsonSerializer.Serialize(msg));
                }
            }
        }

        // ----------------- CONVERSATION LIST -----------------

        private async Task UpdateConversationListAsync(string userId, string otherUserId, DateTime timestamp)
        {
            var convKey = GetConversationKey(userId);
            double score = new DateTimeOffset(timestamp).ToUnixTimeSeconds();
            await _db.SortedSetAddAsync(convKey, otherUserId, score);
        }

        public async Task<List<string>> GetConversationListAsync(string userId)
        {
            var convKey = GetConversationKey(userId);
            var result = await _db.SortedSetRangeByRankAsync(convKey, 0, -1, Order.Descending);
            return result.Select(r => (string)r!).ToList();
        }
    }



}
