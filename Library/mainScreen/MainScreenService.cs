using Library.chatFeild;
using Library.chatField;
using Library.repositories;
using Library.user;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Text;
using static Library.chatField.Chat;

namespace Library.mainScreen
{/// <summary>
 /// class for Mainscreen of application when logs in
 /// <summary>
    public class MainScreenService
    {
        private readonly IUserRepository _userRepo;
        private readonly List<Chat> _chats = new List<Chat>();
        /// <summary>
        /// constauctor for main screen service
        /// <summary>
        public MainScreenService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }
        /// <summary>
        /// add chat to the main screen
        /// <summary>
        public void AddChat(Chat chat) => _chats.Add(chat);
        /// <summary>
        /// it will return the list of chats in the main screen
        /// <summary>
        public IReadOnlyList<Chat> GetChats() => _chats.AsReadOnly();
        /// <summary>
        /// gets preview of stories from friends for the main screen
        /// <summary>
        public string GetStoriesPreview(User user)
        {
            return user.Friends.Count > 0
                ? $"Stories from {string.Join(", ", user.Friends.ConvertAll(f => f.DisplayName))}"
                : "No stories available";
        }

        /// <summary>
        /// For Messenger: automatically create 1:1 chat with every friend if not exists
        /// </summary>
        public void AutoCreateFriendChats(User currentUser)
        {
            foreach (var friend in currentUser.Friends)
            {
                bool alreadyHasChat = _chats.Exists(c =>
                    c.Participants.Count == 2 &&
                    c.Participants.Contains(currentUser) &&
                    c.Participants.Contains(friend));

                if (!alreadyHasChat)
                {
                    IUserRepository repo = new InMemoryUserRepository();

                    var newChat = new OneOnOneChat(currentUser, friend, repo);
                    {
                        newChat.Title = $"Chat with {friend.DisplayName}";
                    };
                    newChat.AddParticipant(friend);
                    AddChat(newChat);
                }
            }
        }
    }
}