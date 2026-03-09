using Library.chatField;
using Library.message;
using Library.repositories;
using Library.user;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.chatFeild
{/// <summary>
 /// class for mynote,selfchat.
 /// <summary>
    public class SelfChat : Chat
    {/// <summary>
     /// constractor for mynote/selfchat
     /// <summary>
        public SelfChat(User self,IUserRepository userRepo): base(userRepo)
        {
            Title = "Self";
            AddParticipant(self);
        }
        /// <summary>
        /// method for getting preview of last message of chat
        /// <summary>
        public override string GetLastMessagePreview()
        {
            if (Messages.Count == 0) return "No messages/notes yet";
            var last = Messages[^1].Content;
            return last.Length > 20 ? last.Substring(0, 17) + "..." : last;
        }
    }

}
