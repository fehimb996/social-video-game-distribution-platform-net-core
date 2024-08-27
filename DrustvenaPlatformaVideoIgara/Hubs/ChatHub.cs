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

            // Fetch sender's profile picture
            var senderProfilePicture = _context.Users
                .Where(u => u.UserId == int.Parse(senderUserId))
                .Select(u => u.ProfilePicture)
                .FirstOrDefault();

            // Store message in the database
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

            var timestamp = chatMessage.Timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ"); // ISO 8601 format

            // Send message to both recipient and sender
            await Clients.User(recipientUserId.ToString()).SendAsync("ReceiveMessage", senderNickName, message, timestamp, senderProfilePicture);
            await Clients.User(senderUserId).SendAsync("ReceiveMessage", senderNickName, message, timestamp, senderProfilePicture);
        }
    }
}
