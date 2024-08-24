using DrustvenaPlatformaVideoIgara.Models;
using Microsoft.AspNetCore.SignalR;

namespace DrustvenaPlatformaVideoIgara.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly SteamContext _context;

        public ChatHub(ILogger<ChatHub> logger, SteamContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task SendMessage(int recipientUserId, string message)
        {
            var senderUserId = Context.UserIdentifier;
            var senderNickName = Context.User?.Claims.FirstOrDefault(c => c.Type == "NickName")?.Value;

            if (string.IsNullOrEmpty(senderUserId) || string.IsNullOrEmpty(senderNickName))
            {
                _logger.LogWarning("Sender user identifier or nickname is not set.");
                return;
            }

            // Store message in database
            var chatMessage = new Message
            {
                UserId1 = int.Parse(senderUserId),
                UserId2 = recipientUserId,
                MessageContent = message,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(chatMessage);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Message from {senderNickName} (ID: {senderUserId}) to {recipientUserId}: {message}");

            // Ensure recipientUserId is a valid connection
            var timestamp = chatMessage.Timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ"); // ISO 8601 format
            await Clients.User(recipientUserId.ToString()).SendAsync("ReceiveMessage", senderNickName, message, timestamp);
            await Clients.User(senderUserId).SendAsync("ReceiveMessage", senderNickName, message, timestamp); // Also send to sender
        }
    }
}
