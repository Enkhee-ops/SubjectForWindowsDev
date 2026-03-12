/*
    Messenger console application - simulates basic Messenger-like interface
    Uses shared Library (DLL) for core chat/user/message logic
    Implements simple state machine: Login → Main screen → Chat view
*/

using Library.chatField;
using Library.mainScreen;
using Library.previewOfChat;
using Library.repositories;
using Library.services;
using Library.user;
using Messenger.previewOfChat;
using Messenger.user;
using Messenger.user.MessengerUser;
using System;
using System.Linq;
using static Library.chatField.Chatfeild;

namespace Messenger
{
    /// <summary>
    /// Application screen states (finite state machine)
    /// </summary>
    enum ScreenState { LoginRegister, Main, Chat }

    /// <summary>
    /// Main entry point for Messenger console simulation
    /// </summary>
    class Program
    {
        /// <summary>
        /// Program entry point - runs the console UI loop
        /// </summary>
        static void Main()
        {
            // Core shared services from Library
            var repo = new InMemoryUserRepository();
            var auth = new AuthService(repo);
            var mainService = new MainScreenService(repo);
            ChatPreviewGenerator preview = new MessengerPreview();

            User currentUser = null;
            ScreenState currentScreen = ScreenState.LoginRegister;
            bool running = true;
            Chatfeild activeChat = null;

            while (running)
            {
                Console.Clear();
                Console.WriteLine($"[Position] {currentScreen}");

                // ── Login / Register screen ───────────────────────────────────────
                if (currentScreen == ScreenState.LoginRegister)
                {
                    Console.WriteLine("Welcome to Messenger");
                    Console.WriteLine("1 = Register new account");
                    Console.WriteLine("2 = Login");
                    Console.WriteLine("3 = Exit");
                    var choice = Console.ReadLine()?.Trim();

                    if (choice == "1")
                    {
                        // ── Registration flow ──
                        Console.Write("Display Name: "); var name = Console.ReadLine()?.Trim();
                        Console.Write("Age: "); byte.TryParse(Console.ReadLine(), out byte age);
                        Console.Write("Phone (+976...): "); var phone = Console.ReadLine()?.Trim();
                        Console.Write("Password: "); var password = Console.ReadLine()?.Trim();

                        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(password))
                        {
                            currentUser = auth.Register<MessengerUser>(name, age, phone, password);
                            Console.WriteLine($"[Success] Account created for {currentUser.DisplayName}");
                            currentScreen = ScreenState.Main;
                        }
                        else
                        {
                            Console.WriteLine("[Error] Name and password required. Press Enter...");
                            Console.ReadLine();
                        }
                    }
                    else if (choice == "2")
                    {
                        // ── Login flow + test data creation ──
                        Console.Write("Display Name: "); var name = Console.ReadLine()?.Trim();
                        Console.Write("Password: "); var password = Console.ReadLine()?.Trim();

                        currentUser = auth.Login(name, password);
                        if (currentUser != null)
                        {
                            Console.WriteLine($"[Success] Welcome back, {currentUser.DisplayName}");

                            // Test data: auto-add friends if they don't exist
                            var alice = repo.GetByName("Alice") ?? auth.Register<MessengerUser>("Alice", 24, "+97699112233", "alice123");
                            var bob = repo.GetByName("Bob") ?? auth.Register<MessengerUser>("Bob", 28, "+97688112244", "bob456");

                            // Test group chat creation
                            var group = new Library.chatField.GroupChat(currentUser, "Test Group Chat", new InMemoryUserRepository());
                            group.AddMember(currentUser, alice);
                            group.AddMember(currentUser, bob);
                            mainService.AddChat(group);
                            Console.WriteLine("Test group chat created: Test Group Chat (admin: you)");

                            currentUser.AddFriend(alice);
                            currentUser.AddFriend(bob);
                            alice.AddFriend(currentUser);
                            bob.AddFriend(currentUser);
                            Console.WriteLine("Test friends added: Alice, Bob");

                            currentScreen = ScreenState.Main;
                        }
                        else
                        {
                            Console.WriteLine("[Error] Wrong name or password. Press Enter...");
                            Console.ReadLine();
                        }
                    }
                    else if (choice == "3")
                    {
                        running = false;
                    }
                }

                // ── Main screen (chat list) ────────────────────────────────────────
                else if (currentScreen == ScreenState.Main)
                {
                    if (currentUser == null) { currentScreen = ScreenState.LoginRegister; continue; }

                    // Auto create 1:1 chats for friends (Messenger-like behavior)
                    mainService.AutoCreateFriendChats(currentUser);

                    Console.WriteLine($"Messenger - Main Screen ({currentUser.DisplayName})");
                    Console.WriteLine("Stories: " + mainService.GetStoriesPreview(currentUser));
                    Console.WriteLine();

                    var chats = mainService.GetChats();
                    if (chats.Count == 0)
                    {
                        Console.WriteLine("No chats yet.");
                    }
                    else
                    {
                        Console.WriteLine("Your chats:");
                        for (int i = 0; i < chats.Count; i++)
                        {
                            var chat = chats[i];
                            Console.WriteLine($"{i + 1}. {chat.Title}");
                            Console.WriteLine(" • " + preview.GeneratePreview(chat));
                            Console.WriteLine(" " + preview.GenerateAlternatePreview(chat));
                        }
                    }

                    Console.WriteLine();
                    Console.WriteLine("Actions:");
                    Console.WriteLine(" number = open chat");
                    Console.WriteLine(" n = open My Notes / Self Chat");
                    Console.WriteLine(" exit = logout/exit");

                    var input = Console.ReadLine()?.Trim().ToLowerInvariant();

                    if (input == "exit")
                    {
                        currentUser = null;
                        currentScreen = ScreenState.LoginRegister;
                    }
                    else if (input == "n")
                    {
                        // Open or create self-chat (notes)
                        var selfChat = chats.FirstOrDefault(c => c is Library.chatField.SelfChat && c.Participants.Count == 1);
                        if (selfChat == null)
                        {
                            selfChat = new Library.chatField.SelfChat(currentUser, new InMemoryUserRepository());
                            mainService.AddChat(selfChat);
                        }
                        activeChat = selfChat;
                        currentScreen = ScreenState.Chat;
                        Console.WriteLine("[Action] Opened My Notes / Self Chat");
                    }
                    else if (int.TryParse(input, out int num) && num >= 1 && num <= chats.Count)
                    {
                        activeChat = chats[num - 1];
                        currentScreen = ScreenState.Chat;
                    }
                }

                // ── Chat view ──────────────────────────────────────────────────────
                else if (currentScreen == ScreenState.Chat)
                {
                    if (activeChat == null)
                    {
                        currentScreen = ScreenState.Main;
                        continue;
                    }

                    Console.WriteLine($"Chat: {activeChat.Title} ({activeChat.Participants.Count} members)");
                    Console.WriteLine("--------------------------------");

                    foreach (var msg in activeChat.Messages)
                    {
                        Console.WriteLine($"{msg.Sender.DisplayName} ({msg.Type}): {msg.Content}");
                    }

                    Console.WriteLine("--------------------------------");
                    Console.WriteLine("Type message (or prefix: voice / picture / emoji / file)");
                    Console.WriteLine("Special commands: call / video / menu / back");

                    var input = Console.ReadLine()?.Trim();

                    if (input?.ToLowerInvariant() == "back")
                    {
                        currentScreen = ScreenState.Main;
                    }
                    else if (input == "call")
                    {
                        Console.WriteLine("[Call] Starting voice call...");
                    }
                    else if (input == "video")
                    {
                        Console.WriteLine("[Call] Starting video call...");
                    }
                    else if (input == "menu")
                    {
                        activeChat.ShowMenu(currentUser);
                    }
                    else if (!string.IsNullOrWhiteSpace(input))
                    {
                        string type = "text";
                        string content = input;

                        // Support different message types via prefix
                        if (input.StartsWith("voice "))
                        {
                            type = "voice";
                            content = input.Substring(6).Trim();
                        }
                        else if (input.StartsWith("picture "))
                        {
                            type = "picture";
                            content = input.Substring(8).Trim(); // URL
                        }
                        else if (input.StartsWith("emoji "))
                        {
                            type = "emoji";
                            content = input.Substring(6).Trim();
                        }
                        else if (input.StartsWith("file "))
                        {
                            type = "file";
                            content = input.Substring(5).Trim();
                        }

                        activeChat.SendMessage(currentUser, content, type);
                        activeChat.MarkAsRead(currentUser, activeChat.Messages.LastOrDefault());

                        if (activeChat.ReadReceiptsEnabled)
                            activeChat.ShowReadReceipts();
                    }
                }
            }
        }
    }
}