using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Entity.Identity;
using Microsoft.AspNetCore.SignalR;

namespace ApplicationBusiness.service
{
    public static class UserConnectionManager
    {
        private static readonly Dictionary<string, string> _connections = new();
        // userId -> connectionId

        public static void AddConnection(string userId, string connectionId)
        {
            lock (_connections)
            {
                _connections[userId] = connectionId;
            }
        }

        public static void RemoveConnection(string userId)
        {
            lock (_connections)
            {
                _connections.Remove(userId);
            }
        }

        public static string? GetConnection(string userId)
        {
            lock (_connections)
            {
                return _connections.TryGetValue(userId, out var conn) ? conn : null;
            }
        }

        public static bool IsUserConnected(string userId)
        {
            lock (_connections)
            {
                return _connections.ContainsKey(userId);
            }
        }
    }


    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        // ------------------ Connection Management ------------------

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier; // or from query string
            if (userId is not null)
            {
                UserConnectionManager.AddConnection(userId, Context.ConnectionId);

                // Mark undelivered messages as delivered now that user is online
                await _chatService.MarkMessagesAsDeliveredAsync(userId);

                // Optionally, notify others the user is online
                await Clients.Others.SendAsync("UserOnline", userId);
            }
            else
                Console.WriteLine("userId == null");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (userId is not null)
            {
                UserConnectionManager.RemoveConnection(userId);
                await Clients.Others.SendAsync("UserOffline", userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // ------------------ Chat Logic ------------------

        public async Task<List<ChatMessageDto>> GetChatHistory(string receiverId)
        {
            var senderId = Context.UserIdentifier!;
            if(string.IsNullOrEmpty(senderId))
                Console.WriteLine("SenderId is null");
            return await _chatService.GetChatHistoryAsync(senderId, receiverId);
        }

        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = Context.UserIdentifier!;
            var chatMessage = await _chatService.SaveMessageAsync(senderId, receiverId, message);

            // Check if receiver online
            var receiverConnection = UserConnectionManager.GetConnection(receiverId);
            if (receiverConnection != null)
            {
                // Deliver message
                chatMessage.Status = "Delivered";
                await _chatService.UpdateStatusAsync(chatMessage);

                await Clients.Client(receiverConnection)
                    .SendAsync("ReceiveMessage", chatMessage);
            }

            // Always send confirmation to sender
            await Clients.Caller.SendAsync("MessageSent", chatMessage);
        }

        public async Task MarkAsDelivered(string senderId)
        {
            var receiverId = Context.UserIdentifier!;
            await _chatService.MarkMessagesAsDeliveredAsync(receiverId, senderId);

            var senderConn = UserConnectionManager.GetConnection(senderId);
            if (senderConn != null)
            {
                await Clients.Client(senderConn)
                    .SendAsync("MessagesDelivered", receiverId);
            }
        }

        public async Task MarkAsRead(string senderId)
        {
            var receiverId = Context.UserIdentifier!;
            await _chatService.MarkMessagesAsReadAsync(receiverId, senderId);

            var senderConn = UserConnectionManager.GetConnection(senderId);
            if (senderConn != null)
            {
                await Clients.Client(senderConn)
                    .SendAsync("MessagesRead", receiverId);
            }
        }

        public async Task<List<string>> GetConversationList()
        {
            var userId = Context.UserIdentifier!;
            return await _chatService.GetConversationListAsync(userId);
        }

    }

}
