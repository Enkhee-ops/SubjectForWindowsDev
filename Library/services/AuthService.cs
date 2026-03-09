using System;
using System.Collections.Generic;
using System.Text;

using Library.repositories;
using Library.user;

namespace Library.services
{    /// <summary>
     ///class for login 
     /// <summary>
    public class AuthService
    {    /// <summary>
         ///repo for info of user
         /// <summary>
        private readonly IUserRepository _repo;
        /// <summary>
        ///constraction for authservice
        /// <summary>
        public AuthService(IUserRepository repo)
        {
            _repo = repo;
        }
        /// <summary>
        ///repo list of obj for info of user
        /// <summary>

        public User Register<T>(string displayName, byte age, string phone, string password)
            where T : User
        {
            // Calls the ctor with params
            var user = (T)Activator.CreateInstance(typeof(T), displayName, age, phone, password);
            _repo.Add(user);
            return user;
        }
        /// <summary>
        ///login method
        /// <summary>
        public User Login(string displayName, string password)
        {
            var user = _repo.GetByName(displayName);
            return user != null && user.ValidatePassword(password) ? user : null;
        }
    }
}