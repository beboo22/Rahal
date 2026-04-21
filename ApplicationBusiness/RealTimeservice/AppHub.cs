using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.RealTimeservice
{
    using ApplicationBusiness.RealTimeservice.ChatService;
    using ApplicationBusiness.service;
    using Microsoft.AspNetCore.SignalR;

    public class AppHub : Hub
    {
        private readonly IChatService _chatService;

        public AppHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        // ---------------- CONNECTION ----------------

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId))
            {
                UserConnectionManager.AddConnection(userId, Context.ConnectionId);

                await _chatService.MarkMessagesAsDeliveredAsync(userId);

                await Clients.Others.SendAsync("UserOnline", userId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId))
            {
                UserConnectionManager.RemoveConnection(userId);

                await Clients.Others.SendAsync("UserOffline", userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // ---------------- CHAT ----------------

        public async Task<List<ChatMessageDto>> GetChatHistory(string receiverId)
        {
            var senderId = Context.UserIdentifier!;
            return await _chatService.GetChatHistoryAsync(senderId, receiverId);
        }

        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = Context.UserIdentifier!;

            var chatMessage = await _chatService.SaveMessageAsync(senderId, receiverId, message);

            var receiverConnection = UserConnectionManager.GetConnection(receiverId);

            if (receiverConnection != null)
            {
                chatMessage.Status = "Delivered";
                await _chatService.UpdateStatusAsync(chatMessage);

                await Clients.Client(receiverConnection)
                    .SendAsync("ReceiveMessage", chatMessage);
            }

            await Clients.Caller.SendAsync("MessageSent", chatMessage);
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
    }
}
