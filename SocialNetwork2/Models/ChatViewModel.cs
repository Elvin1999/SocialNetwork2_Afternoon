using SocialNetwork2.Entities;

namespace SocialNetwork2
{
    public class ChatViewModel
    {
        public string CurrentUserId { get; set; }
        public string CurrentReceiverId { get; set; }
        public Chat CurrentChat { get; set; }
        public List<Chat> Chats { get; set; }
    }
}