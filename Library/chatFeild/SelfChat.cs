using Library.chatField;
using Library.message;
using Library.repositories;
using Library.user;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.chatField
{/// <summary>
 /// class for mynote,selfchat.
 /// <summary>
    public class SelfChat : Chatfeild
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

    }

}
