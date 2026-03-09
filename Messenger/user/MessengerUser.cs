using Library.user;

namespace Messenger.user.MessengerUser
{/// <summary>
///User class for messenger
/// <summary>
    public class MessengerUser : User
    {
        public MessengerUser(string displayName, byte age, string phone, string password): base(displayName, age, phone, password) 
        { }
    }
}