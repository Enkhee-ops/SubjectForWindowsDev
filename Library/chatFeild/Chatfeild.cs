/* 
    Base abstract class for all chat types in the social platform library.
    Contains shared logic for messages, participants, read receipts, pinning, notifications.
    Concrete implementations: OneOnOneChat, GroupChat, SelfChat (and platform-specific variants).
*/
using Library.message;
using Library.repositories;
using Library.user;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.chatField
/// <summary>
/// Abstract base class for all chatfeild types (1:1, group, self/notes).
/// Provides common functionality: messaging, participants, read receipts, pinning.
/// </summary>
{
    public abstract class Chatfeild
    {
        // Core identification
        /// <summary>
        /// Unique identifier of this chat
        /// </summary>
        public string ChatId { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Users currently in this conversation
        /// </summary>
        public List<User> Participants { get; } = new List<User>();

        /// <summary>
        /// All messages in chronological order
        /// </summary>
        public List<Message> Messages { get; } = new List<Message>();

        /// <summary>
        /// Messages pinned to the top of the chat
        /// </summary>
        public List<Message> PinnedMessages { get; } = new List<Message>();

        /// <summary>
        /// Display name / title of the chat
        /// </summary>
        public string Title { get; set; }

        // Read & notification features
        /// <summary>
        /// Last message each participant has seen
        /// </summary>
        public Dictionary<User, Message> ReadReceipts { get; set; } = new Dictionary<User, Message>();

        /// <summary>
        /// Whether read receipts are enabled in this chat
        /// </summary>
        public bool ReadReceiptsEnabled { get; set; } = false;

        /// <summary>
        /// User's chosen notification preference for this chat
        /// </summary>
        public enum NotificationSettingTypes { 
            AllChats,
            OnlyMentions,
            NoNotifications
        }
        public NotificationSettingTypes NotificationSetting;
        // Dependency
        protected readonly IUserRepository userRepo;

        // Constructor
        protected Chatfeild(IUserRepository userRepo)
        {
            this.userRepo = userRepo;
        }
        /// <summary>
        /// Adds a participant if not already present
        /// </summary>
        public void AddParticipant(User user)
        {
            if (user != null && !Participants.Contains(user))
                Participants.Add(user);
        }
        /// <summary>
        /// Creates and adds a new message to the chat
        /// </summary>
        /// <summary>
        /// Creates and adds a new message to the chat
        /// </summary>
        public void SendMessage(User sender, string content, string type = "text")
        {
            var msg = new Library.message.Message(sender, content, type);
            Messages.Add(msg);
            Console.WriteLine($"[Event] {type.ToUpper()} sent by {sender.DisplayName}: {content}");


            ReadReceipts[sender] = msg;
        }//---------------------------------------------------------------------------------------------------------------------------------avj service bolgono

        /// <summary>
        /// Records that a user has seen up to this message
        /// </summary>
        public void MarkAsRead(User user, Message message)
        {
            if (Participants.Contains(user) && message != null)
            {
                ReadReceipts[user] = message;
                Console.WriteLine($"[Read] {user.DisplayName} saw message: {message.Content}");
            }
        }

        /// <summary>
        /// Pins an existing message to the top
        /// </summary>
        public void PinMessage(Message message)
        {
            if (Messages.Contains(message) && !PinnedMessages.Contains(message))
                PinnedMessages.Add(message);
        }

        /// <summary>
        /// Returns short preview text of the most recent message
        /// (implementation differs between 1:1, group, self-chat)
        /// </summary>
        public string GetLastMessagePreview()
        {
            if (Messages.Count == 0) return "No messages yet";
            var last = Messages[Messages.Count - 1];
            return $"{last.Sender.DisplayName}: {last.Content.Substring(0, Math.Min(20, last.Content.Length)) + "..."}";
        }
        /// <summary>
        /// Displays interactive menu for this chat
        /// </summary>
        public virtual void ShowMenu(User currentUser)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Chat Menu - {Title}");
                Console.WriteLine("1. Info about User/Chat");
                Console.WriteLine("2. Media");
                Console.WriteLine("3. Files");
                Console.WriteLine("4. Links");
                Console.WriteLine("5. Chat notifications");
                Console.WriteLine("6. Read receipts (toggle + view)");
                Console.WriteLine("7. Report");
                Console.WriteLine("back = return to chat");

                var choice = Console.ReadLine()?.Trim().ToLowerInvariant();

                if (choice == "back") return;

                if (int.TryParse(choice, out int num))
                {
                    switch (num)
                    {
                        case 1:
                            ShowInfo();
                            break;
                        case 2:
                            ShowMedia();
                            break;
                        case 3:
                            ShowFiles();
                            break;
                        case 4:
                            ShowLinks();
                            break;
                        case 5:
                            SetNotifications();
                            break;
                        case 6:
                            ToggleAndShowReadReceipts();
                            break;
                        case 7:
                            Console.WriteLine("[Reported] This chat has been reported to support.");
                            Console.WriteLine("Press Enter to continue...");
                            Console.ReadLine();
                            break;
                        default:
                            Console.WriteLine("Invalid option. Press Enter...");
                            Console.ReadLine();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Press Enter...");
                    Console.ReadLine();
                }
            }
        }//---------------------------------------------------------------------------------------service

        protected void ToggleAndShowReadReceipts()
        {
            Console.Clear();
            ReadReceiptsEnabled = !ReadReceiptsEnabled;
            Console.WriteLine($"Read receipts now: {(ReadReceiptsEnabled ? "Enabled" : "Disabled")}");

            if (ReadReceiptsEnabled)
            {
                ShowReadReceipts();
            }

            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }//---------------------------------------------------------------------------------------service
        protected void ShowInfo()
        {
            Console.Clear();
            Console.WriteLine($"Chat Info: {Title}");
            Console.WriteLine($"Participants ({Participants.Count}):");
            foreach (var p in Participants)
            {
                Console.WriteLine($" - {p.DisplayName} ({p.PhoneNumber ?? "No phone"}) - {p.Status}");
            }
            Console.WriteLine($"Notifications: {NotificationSetting}");
            Console.WriteLine($"Read receipts: {(ReadReceiptsEnabled ? "Enabled" : "Disabled")}");
            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }
        // Shows all media-type messages (picture, voice, file, etc.)
        /// <summary>
        /// Shows all media-type messages (picture, voice, file, etc.)
        /// </summary>

        protected void ShowMedia()
        {
            Console.Clear();
            var media = Messages.Where(m => m.Type == "picture" || m.Type == "voice" || m.Type == "video").ToList();
            if (media.Count == 0)
            {
                Console.WriteLine("No media in this chat yet.");
            }
            else
            {
                Console.WriteLine("Media in chat:");
                foreach (var m in media)
                {
                    Console.WriteLine($"[{m.SentAt:yyyy-MM-dd HH:mm}] {m.Sender.DisplayName} ({m.Type}): {m.Content}");
                }
            }
            Console.WriteLine("\nPress Enter...");
            Console.ReadLine();
        }//---------------------------------------------------------------------------------------service and do it diffrent
        // Shows all files in chat
        /// <summary>
        /// Shows all files in chat
        /// </summary>
        protected void ShowFiles()
        {
            Console.Clear();
            var files = Messages.Where(m => m.Type == "file").ToList();
            if (files.Count == 0)
            {
                Console.WriteLine("No files in this chat yet.");
            }
            else
            {
                Console.WriteLine("Files in chat:");
                foreach (var m in files)
                {
                    Console.WriteLine($"[{m.SentAt:yyyy-MM-dd HH:mm}] {m.Sender.DisplayName}: {m.Content}");
                }
            }
            Console.WriteLine("\nPress Enter...");
            Console.ReadLine();
        }//---------------------------------------------------------------------------------------service and do it diffrent
        // Shows all links in chat
        /// <summary>
        /// Shows all links in chat
        /// </summary>
        protected void ShowLinks()
        {
            Console.Clear();
            var links = Messages.Where(m => m.Content.StartsWith("http") || m.Content.Contains("www.")).ToList();
            if (links.Count == 0)
            {
                Console.WriteLine("No links in this chat yet.");
            }
            else
            {
                Console.WriteLine("Links in chat:");
                foreach (var m in links)
                {
                    Console.WriteLine($"[{m.SentAt:yyyy-MM-dd HH:mm}] {m.Sender.DisplayName}: {m.Content}");
                }
            }
            Console.WriteLine("\nPress Enter...");
            Console.ReadLine();
        }//---------------------------------------------------------------------------------------service and do it diffrent
        // set what notification will reach user
        /// <summary>
        /// Sets notifcation preferences for this chat. 
        /// </summary>
        protected void SetNotifications()
        {
            Console.Clear();
            Console.WriteLine("Current: " + NotificationSetting);
            Console.WriteLine("Choose new setting:");
            Console.WriteLine("1. Only special chats");
            Console.WriteLine("2. All chats");
            Console.WriteLine("3. Only mentions");
            Console.WriteLine("4. No notification");
            Console.WriteLine("back = cancel");

            var choice = Console.ReadLine()?.Trim();
            if (choice == "back") return;

            NotificationSetting = choice switch
            {
                "1" => NotificationSettingTypes.AllChats,
                "2" => NotificationSettingTypes.OnlyMentions,
                "3" => NotificationSettingTypes.NoNotifications,
                _ => NotificationSetting
            };

            Console.WriteLine($"[Updated] Notifications set to: {NotificationSetting}");
            Console.WriteLine("Press Enter...");
            Console.ReadLine();
        }//---------------------------------------------------------------------------------------service and do it diffrent

        protected void ToggleReadReceipts()
        {
            ReadReceiptsEnabled = !ReadReceiptsEnabled;
            Console.WriteLine($"[Updated] Read receipts: {(ReadReceiptsEnabled ? "Enabled" : "Disabled")}");
        }//---------------------------------------------------------------------------------------service and do it diffrent
        /// <summary>
        /// Displays current cuurent receipts for which user saw until which chat
        /// </summary>
        public void ShowReadReceipts()
        {
            if (!ReadReceiptsEnabled)
            {
                Console.WriteLine("Read receipts are disabled.");
                return;
            }

            Console.WriteLine("Read Receipts:");
            foreach (var kv in ReadReceipts)
            {
                var msgPreview = kv.Value != null ? kv.Value.Content.Substring(0, Math.Min(30, kv.Value.Content.Length)) + "..." : "None";
                Console.WriteLine($" - {kv.Key.DisplayName}: {msgPreview} (at {kv.Value?.SentAt})");
            }
        }//---------------------------------------------------------------------------------------service and do it diffrent







    }
}