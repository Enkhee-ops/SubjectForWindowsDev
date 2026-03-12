using Library.chatField;
using Library.message;
using Library.repositories;
using Library.user;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.chatField
{/// <summary>
    /// class for one on one chat.
/// <summary>
    public class OneOnOneChat : Chatfeild
    {/// <summary>
     /// constructor for one-on -one
     /// <summary>
        public OneOnOneChat(User user1, User user2, IUserRepository userRepo) : base(userRepo)
        {
            Title = $"Chat with {user2.DisplayName}";
            AddParticipant(user1);
            AddParticipant(user2);
        }
        /// <summary>
        /// preview of the last messege
        /// <summary>

        /// <summary>
        /// shows menu of on one chat
        /// <summary>
        public override void ShowMenu(User currentUser)
        {
            base.ShowMenu(currentUser);
            // In 1on1, info appears directly
            ShowInfo();
        }
    }

}
