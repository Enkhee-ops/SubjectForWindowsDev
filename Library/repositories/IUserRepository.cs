using System;
using System.Collections.Generic;
using System.Text;

using Library.user;

namespace Library.repositories
{       /// <summary>
        ///interface for repo
        /// <summary>
    public interface IUserRepository
    {        /// <summary>
             ///add user to repo
             /// <summary>
        void Add(User user);
        /// <summary>
        ///get user obj by userid
        /// <summary>
        User GetById(string id);
        /// <summary>
        ///get user obj by userName
        /// <summary>
        User GetByName(string name); 
        /// <summary>
        ///get user list
        /// <summary>
        IReadOnlyList<User> GetAll();
    }
}