

using System;
using System.Collections.Generic;
/// <summary>
///abstract class for all users
/// <summary>
namespace Library.user
{
    public abstract class User
    { // infos of user
        public string UserId { get; } = Guid.NewGuid().ToString();
        public string DisplayName { get; set; }
        public byte Age { get; }
        public string PhoneNumber { get; set; }  // Used more in Viber
        public string ProfilePictureUrl { get; set; } = "https://images.unsplash.com/photo-1633332755192-727a05c4013d?w=150";
        public string Password { get; private set; }
        public string Status { get; set; } = "Offline";  // e.g., "Active", "Last online: 2026-03-07 11:00 PM"
        public DateTime LastOnline { get; set; } = DateTime.UtcNow;

        public List<User> Friends { get; } = new List<User>();
        /// <summary>
        ///constarct func for user
        /// <summary>
        protected User(string displayName, byte age, string phoneNumber, string password)
        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Age = age;
            PhoneNumber = phoneNumber;
            Password = password ?? throw new ArgumentNullException(nameof(password));
            UpdateStatus(true);  // Set to Active on creation
        }
        /// <summary>
        ///adds friend for user
        /// <summary>
        public virtual void AddFriend(User friend)
        {
            if (friend != null && !Friends.Contains(friend))
                Friends.Add(friend);
        }
        /// <summary>
        ///validate pasword
        /// <summary>
        public bool ValidatePassword(string input) => Password == input;
        /// <summary>
        ///updates status (Active, last online :)
        /// <summary>
        public void UpdateStatus(bool isActive)
        {
            if (isActive)
            {
                Status = "Active";
                LastOnline = DateTime.UtcNow;
            }
            else
            {
                Status = "Last online: " + LastOnline.ToString("yyyy-MM-dd hh:mm tt");
            }
        }
    }
}