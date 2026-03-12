using System;
using Library.user;

namespace Library.message
{/// <summary>
 /// class for messega
 /// <summary>
    public class Message
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public User Sender { get; }
        public DateTime SentAt { get; } = DateTime.UtcNow;
        public string Content { get; set; }
        public string Type { get; set; } = "text";  // ← this line was missing or not saved

        public Message(User sender, string content, string type = "text")
        {
            Sender = sender;
            Content = content;
            Type = type;
        }
    }
}