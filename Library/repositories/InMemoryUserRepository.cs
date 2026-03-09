using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using Library.user;

namespace Library.repositories
{/// <summary>
 /// repo of chats
 /// <summary>
    public class InMemoryUserRepository : IUserRepository
    {/// <summary>
     /// dictionary userID,userOBJ
     /// <summary>
        private readonly Dictionary<string, User> _usersById = new();
        /// <summary>
        /// dictionary userNAME,userOBJ
        /// <summary>
        private readonly Dictionary<string, User> _usersByName = new();
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