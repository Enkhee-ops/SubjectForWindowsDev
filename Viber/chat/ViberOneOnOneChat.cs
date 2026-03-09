using System;
using System.Collections.Generic;
using System.Text;

using Library.repositories;
using Library.user;
using Viber.user;

namespace Viber.chat
{        /// <summary>
         /// class for one on one chat
         /// <summary
    public class ViberOneOnOneChat : ViberChat
    {    /// <summary>
         ///constarctor
         /// <summary
        public ViberOneOnOneChat(ViberUser user1, ViberUser user2, IUserRepository userRepo)
            : base(userRepo)
        {
            Title = $"Chat with {user2.DisplayName}";
            AddParticipant(user1);
            AddParticipant(user2);
        }
    }
}
