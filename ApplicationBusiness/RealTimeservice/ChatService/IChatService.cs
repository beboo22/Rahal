using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using MediatR;
using Application.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Fetures.Authentication.Query;
using Domain.BaseResponce;

namespace ApplicationBusiness.RealTimeservice.ChatService
{
    public class ConversationDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }
    public interface IChatService
    {
        Task<List<ConversationDto>> GetConversationListAsync(string userId);
        Task<ChatMessageDto> SaveMessageAsync(ChatMessageDto chatMessage);
        Task<List<ChatMessageDto>> GetChatHistoryAsync(string senderId, string receiverId);
        Task MarkMessagesAsDeliveredAsync(string receiverId);
        Task MarkMessagesAsDeliveredAsync(string receiverId, string senderId);
        Task MarkMessagesAsReadAsync(string receiverId, string senderId);
        Task UpdateStatusAsync(ChatMessageDto message);
    }
    public class ChatMessageDto
    {
        public string SenderId { get; set; } = default!;
        public string SenderName { get; set; } = default!; // جديد
        public string SenderImage { get; set; } = default!; // جديد
        public string ReceiverId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } = "Sent";
    }


    public class ChatService : IChatService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        public ISender Sender { get; set; }

        public ChatService(IConnectionMultiplexer redis, ISender sender)
        {
            //var connectionString = config["Redis:ConnectionString"];
            //_redis = ConnectionMultiplexer.Connect(
            //new ConfigurationOptions
            //{
            //    EndPoints = { { "redis-19301.c341.af-south-1-1.ec2.redns.redis-cloud.com", 19301 } },
            //    User = "default",
            //    Password = "uJhzvCJD1pjVz9lBh4gKVc9OrKRL9pTR"
            //}
            //);
            //_redis = ConnectionMultiplexer.Connect(connectionString);
            _db = redis.GetDatabase();
            _redis = redis;
            Sender = sender;
        }

        // Generate consistent chat key
        private static string GetChatKey(string user1, string user2)
        {
            var ordered = new[] { user1, user2 }.OrderBy(x => x).ToArray();
            return $"chat:{ordered[0]}:{ordered[1]}";
        }

        private static string GetConversationKey(string userId) => $"conversations:{userId}";

        // ----------------- CORE METHODS -----------------

        //public async Task<ChatMessageDto> SaveMessageAsync(ChatMessageDto chatMessage)
        //{
        //    //var chatMessage = new ChatMessageDto
        //    //{
        //    //    SenderId = senderId,
        //    //    ReceiverId = receiverId,
        //    //    Content = message,
        //    //    Timestamp = DateTime.UtcNow,
        //    //    Status = "Sent"
        //    //};

        //    var chatKey = GetChatKey(chatMessage.SenderId, chatMessage.ReceiverId);
        //    var serialized = JsonSerializer.Serialize(chatMessage);

        //    // Save message to chat list
        //    await _db.ListRightPushAsync(chatKey, serialized);

        //    // Update recent conversations
        //    await UpdateConversationListAsync(chatMessage.SenderId, chatMessage.ReceiverId, chatMessage.Timestamp);
        //    await UpdateConversationListAsync(chatMessage.ReceiverId, chatMessage.SenderId, chatMessage.Timestamp);

        //    return chatMessage;
        //}

        public async Task<ChatMessageDto> SaveMessageAsync(ChatMessageDto chatMessage)
        {
            var chatKey = GetChatKey(
                chatMessage.SenderId,
                chatMessage.ReceiverId);

            var serialized = JsonSerializer.Serialize(chatMessage);

            await _db.ListRightPushAsync(chatKey, serialized);

            // بيانات الريسيفر
            var receiver = await Sender.Send(new GetUserById(int.Parse(chatMessage.ReceiverId))) as ApiResultResponse<TemplateGenericProfile>;
            //await _userManager.FindByIdAsync(chatMessage.ReceiverId);

            var senderConversation = new ConversationDto
            {
                Id = chatMessage.ReceiverId,
                Name = receiver?.Data?.Fname + "" + receiver?.Data?.Lname,
                Image = receiver?.Data?.Traveler is not null ?
                receiver.Data.Traveler.PhotoUrl : receiver?.Data.TourGuide is not null ?
                receiver.Data.TourGuide.PhotoUrl : receiver?.Data?.TravelCompany is not null ?
                receiver.Data.TravelCompany.PhotoUrl : "https://cdn-icons-png.flaticon.com/512/149/149071.png"
            };

            var receiverConversation = new ConversationDto
            {
                Id = chatMessage.SenderId,
                Name = chatMessage.SenderName,
                Image = chatMessage.SenderImage
            };

            await UpdateConversationListAsync(
                chatMessage.SenderId,
                senderConversation,
                chatMessage.Timestamp);

            await UpdateConversationListAsync(
                chatMessage.ReceiverId,
                receiverConversation,
                chatMessage.Timestamp);

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

        private async Task UpdateConversationListAsync(
    string userId,
    ConversationDto otherUser,
    DateTime timestamp)
        {
            var convKey = GetConversationKey(userId);

            double score = new DateTimeOffset(timestamp)
                .ToUnixTimeSeconds();

            var serialized = JsonSerializer.Serialize(otherUser);

            await _db.SortedSetAddAsync(
                convKey,
                serialized,
                score);
        }

        //public async Task<List<ConversationDto> GetConversationListAsync(string userId)
        //{
        //    var convKey = GetConversationKey(userId);
        //    var result = await _db.SortedSetRangeByRankAsync(convKey, 0, -1, Order.Descending);
        //    return result.Select(r => (string)r!).ToList();
        //}

        //public async Task<List<ConversationDto>> GetConversationListAsync(string userId)
        //{
        //    var convKey = GetConversationKey(userId);

        //    var result = await _db.SortedSetRangeByRankAsync(
        //        convKey,
        //        0,
        //        -1,
        //        Order.Descending);

        //    var item = result
        //       .Select(x => JsonSerializer.Deserialize<ConversationDto>(x!)!)
        //       .ToList();
        //    return item;
        //}


        public async Task<List<ConversationDto>> GetConversationListAsync(string userId)
        {
            var convKey = GetConversationKey(userId);

            var result = await _db.SortedSetRangeByRankAsync(
                convKey,
                0,
                -1,
                Order.Descending);

            var conversations = new List<ConversationDto>();

            foreach (var item in result)
            {
                try
                {
                    conversations.Add(
                        JsonSerializer.Deserialize<ConversationDto>(item!)!
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"BAD VALUE = {item}");
                    Console.WriteLine(ex.Message);
                }
            }

            return conversations;
        }



    }



}
