using System;
using System.Collections.Generic;
using System.Text;

using Library.repositories;
using Viber.user;

namespace Viber.chat
{    /// <summary>
     ///class for Mynote/selfchat
     /// <summary
    public class ViberSelfChat : ViberChat
    {    /// <summary>
         ///make selfchat/mynote
         /// <summary
        public ViberSelfChat(ViberUser self, IUserRepository userRepo)
            : base(userRepo)
        {
            Title = "My Notes";
            AddParticipant(self);
        }
    }
}
