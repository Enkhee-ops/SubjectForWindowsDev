using Library.user;

namespace Viber.user
{
    /// <summary>
    ///class for user of viber
    /// <summary
    public class ViberUser : User
    {    /// <summary>
         ///make user of viber
         /// <summary
        public ViberUser(string displayName, byte age, string phone, string password)
            : base(displayName, age, phone, password)
        {

        }
    }
}