

using Library.user;
using Library.message;
using System.ComponentModel;

namespace Library.repositories
{/// <summary>
 /// repo of chats
 /// <summary>
    public class InMemoryUserRepository : IUserRepository
    {/// <summary>
     /// dictionary userID,userOBJ
     /// <summary>
        private Dictionary<string, User> _usersById = new();
        /// <summary>
        /// dictionary userNAME,userOBJ
        /// <summary>
        private Dictionary<string, User> _usersByName = new();

        private static short CurentChatId = 1;

        // Core identification
        /// <summary>
        /// Unique identifier of this chat
        /// </summary>
        private string ChatId { get; } = Convert.ToString(CurentChatId);


        /// <summary>
        /// Returns the ID of the current chat .
        /// </summary>
        public string GetChatId()
        {
            CurentChatId++;
            return ChatId;
        }





        /// <summary>
        /// Users currently in this conversation
        /// </summary>
        private List<User> Participants { get; } = new List<User>();

        /// <summary>
        /// Returns the list of participants in the current chat.
        /// </summary>
        /// <returns></returns>
        public List<User> GetParticipants()
        {
            return Participants;
        }

        /// <summary>
        /// All messages in chronological order
        /// </summary>
        private List<Message> Messages { get; } = new List<Message>();

        private List<Message> MediaMessages { get; } = new List<Message>();

        private List<Message> FileMessages { get; } = new List<Message>();


        private List<Message> TextMessages { get; } = new List<Message>();

        /// <summary>
        /// Messages pinned to the top of the chat
        /// </summary>
        private List<Message> PinnedMessages { get; } = new List<Message>();

        /// <summary>
        /// Display name / title of the chat
        /// </summary>
        private string Title { get; set; }

        // Read & notification features
        /// <summary>
        /// Last message each participant has seen
        /// </summary>
        private Dictionary<User, Message> ReadReceipts { get; set; } = new Dictionary<User, Message>();

        /// <summary>
        /// Whether read receipts are enabled in this chat
        /// </summary>

        private bool Settings=false ;  




        /// <summary>
        /// method fo adding user to the repo
        /// <summary>
        public void Add(User user)
        {
            if (user != null)
            {
                _usersById[user.UserId] = user;
                _usersByName[user.DisplayName.ToLowerInvariant()] = user;
            }
        }
        /// <summary>
        ///get user obj by userid
        /// <summary>
        public User GetById(string id) => _usersById.TryGetValue(id, out var u) ? u : null;
        /// <summary>
        ///get user obj by userName
        /// <summary>
        public User GetByName(string name)
        {
            _usersByName.TryGetValue(name.ToLowerInvariant(), out var user);
            return user;
        }
        /// <summary>
        ///get user list
        /// <summary>
        public IReadOnlyList<User> GetAll() => _usersById.Values.ToList().AsReadOnly();

        
    }
}