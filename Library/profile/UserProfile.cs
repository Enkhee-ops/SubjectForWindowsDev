using System;
using System.Collections.Generic;
using System.Text;

using Library.user;

namespace Library.profile
{/// <summary>
 /// user profile class for getting profile of user
 /// <summary>
    public class UserProfile
    {/// <summary>
     /// user of the profile, it is read-only and set through the constructor. It provides access to the user's basic information like display name and age, which can be used in the profile display.
     /// <summary>
        public User User { get; }
        public string Bio { get; set; } = "No bio yet";
        public string Status { get; set; } = "Online";
        /// <summary>
        /// constarctar of user profile
        /// <summary>
        public UserProfile(User user)
        {
            User = user;
        }

        public override string ToString() => $"{User.DisplayName} ({User.Age}) - {Status}";
    }
}