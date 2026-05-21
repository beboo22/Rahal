using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.RealTimeservice
{
    using ApplicationBusiness.RealTimeservice.ChatService;
    using ApplicationBusiness.RealTimeservice.NotificationService;
    using ApplicationBusiness.service;
    using Microsoft.AspNetCore.SignalR;

    public class AppHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly INotificationService _notificationService;

        public AppHub(IChatService chatService, INotificationService notificationService)
        {
            _chatService = chatService;
            _notificationService = notificationService;
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
            //var senderId = Context.UserIdentifier!;
            var senderId = Context.User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                   ?? Context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(senderId))
            {
                throw new HubException("User ID not found in token.");
            }

            // سحب الاسم والصورة من الـ Claims بتاعة الشخص المتصل حالياً
            var senderName = Context.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown User";
            var senderImage = Context.User.FindFirst("ProfilePicture")?.Value ?? "https://cdn-icons-png.flaticon.com/512/149/149071.png";

            // تمرير الاسم والصورة للـ Service عشان يحفظهم في الـ Redis
            var Message = new ChatMessageDto
            {
                SenderId = senderId,
                SenderName = senderName,
                SenderImage = senderImage,
                ReceiverId = receiverId,
                Content = message,
                Timestamp = DateTime.UtcNow,
                Status = "Sent"
            };


            var chatMessage = await _chatService.SaveMessageAsync(Message);

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

        public async Task<List<NotificationDto>> GetMyNotifications()
        {
            var userId = Context.UserIdentifier!;
            // هنحتاج نعمل Inject للـ INotificationService جوة الـ Hub لو مش معملوله
            // أو نرجعها من الـ Redis علطول زي ما تحب، بس الأفضل نستخدم الـ Service
            return await _notificationService.GetUserNotificationsAsync(userId);
        }
        public async Task MarkNotificationAsRead(string notificationId)
        {
            var userId = Context.UserIdentifier!;
            // بننادي السيرفيس المربوطة بالـ Redis عشان تحول IsRead لـ true وتطرح 1 من الـ Counter
            await _notificationService.MarkAsReadAsync(notificationId, userId);
        }

        // ضيف الميثود دي جوة كلاس AppHub
        public async Task<List<string>> GetConversationListAsync(string userId)
        {
            // بننادي الـ Service اللي متصلة بالـ Redis علطول
            return await _chatService.GetConversationListAsync(userId);
        }
    }

}
