/*
    Viber console application simulation
    Uses shared Library (DLL) for core user/chat/message logic
    Implements phone-number based contacts + Viber-style UI flow
*/

using System;
using System.Linq;
using Library.repositories;
using Library.mainScreen;
using Library.services;
using Viber.user;
using Viber.chat;
using Viber.previewOfChat;

namespace Viber
{
    /// <summary>
    /// Application screen states (Login → Main → Chat)
    /// </summary>
    enum ScreenState { LoginRegister, Main, Chat }

    /// <summary>
    /// Main entry point for Viber console application
    /// </summary>
    class Program
    {
        /// <summary>
        /// Runs the main application loop (state machine)
        /// </summary>
        static void Main()
        {
            var repo = new InMemoryUserRepository();
            var auth = new AuthService(repo);
            var mainService = new MainScreenService(repo);
            var preview = new ViberChatPreview();

            ViberUser currentUser = null;
            ScreenState currentScreen = ScreenState.LoginRegister;
            bool running = true;
            ViberChat activeChat = null;

            while (running)
            {
                Console.Clear();
                Console.WriteLine($"[Position] {currentScreen}");

                // ── Login / Register screen ───────────────────────────────────────
                if (currentScreen == ScreenState.LoginRegister)
                {
                    Console.WriteLine("Welcome to Viber");
                    Console.WriteLine("1 = Register");
                    Console.WriteLine("2 = Login");
                    Console.WriteLine("3 = Exit");
                    var choice = Console.ReadLine()?.Trim();

                    if (choice == "1")
                    {
                        Console.Write("Name: "); var name = Console.ReadLine()?.Trim();
                        Console.Write("Age: "); byte.TryParse(Console.ReadLine(), out byte age);
                        Console.Write("Phone (+976...): "); var phone = Console.ReadLine()?.Trim();
                        Console.Write("Password: "); var pass = Console.ReadLine()?.Trim();

                        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(pass))
                        {
                            currentUser = (ViberUser)auth.Register<ViberUser>(name, age, phone, pass);
                            Console.WriteLine($"[Success] {currentUser.DisplayName} registered");

                            CreateMyNotes(currentUser, mainService, repo);
                            AutoCreateContactChats(currentUser, repo, mainService);

                            currentScreen = ScreenState.Main;
                        }
                    }
                    else if (choice == "2")
                    {
                        Console.Write("Name: "); var name = Console.ReadLine()?.Trim();
                        Console.Write("Password: "); var pass = Console.ReadLine()?.Trim();

                        var logged = auth.Login(name, pass);
                        if (logged != null)
                        {
                            currentUser = (ViberUser)logged;
                            Console.WriteLine($"[Success] Welcome {currentUser.DisplayName}");

                            CreateMyNotes(currentUser, mainService, repo);
                            AutoCreateContactChats(currentUser, repo, mainService);

                            currentScreen = ScreenState.Main;
                        }
                        else Console.WriteLine("Login failed");
                    }
                    else if (choice == "3") running = false;
                }

                // ── Main screen (chat list + commands) ─────────────────────────────
                else if (currentScreen == ScreenState.Main)
                {
                    if (currentUser == null) { currentScreen = ScreenState.LoginRegister; continue; }

                    Console.WriteLine($"Viber - {currentUser.DisplayName} ({currentUser.PhoneNumber})");
                    Console.WriteLine();

                    var chats = mainService.GetChats();
                    if (chats.Count == 0) Console.WriteLine("No chats yet");
                    else
                    {
                        Console.WriteLine("Chats:");
                        for (int i = 0; i < chats.Count; i++)
                            Console.WriteLine($"{i + 1}. {chats[i].Title} • {preview.GeneratePreview(chats[i])}");
                    }

                    Console.WriteLine("\nCommands:");
                    Console.WriteLine(" number = open chat");
                    Console.WriteLine(" n = My Notes");
                    Console.WriteLine(" g = create group");
                    Console.WriteLine(" 1on1 <name> = start 1-on-1 with user");
                    Console.WriteLine(" exit = logout");

                    var cmd = Console.ReadLine()?.Trim().ToLower();

                    if (cmd == "exit")
                    {
                        currentUser = null;
                        currentScreen = ScreenState.LoginRegister;
                    }
                    else if (cmd == "n")
                    {
                        activeChat = CreateMyNotes(currentUser, mainService, repo);
                        currentScreen = ScreenState.Chat;
                    }
                    else if (cmd == "g")
                    {
                        var group = new ViberGroupChat(currentUser, "Test Group", repo);
                        mainService.AddChat(group);
                        Console.WriteLine("[Success] Group created");
                    }
                    else if (cmd.StartsWith("1on1 "))
                    {
                        var name = cmd.Substring(5).Trim();
                        var other = (ViberUser)repo.GetByName(name);

                        if (other != null)
                        {
                            var chat = new ViberOneOnOneChat(currentUser, other, repo);
                            mainService.AddChat(chat);
                            activeChat = chat;
                            currentScreen = ScreenState.Chat;
                            Console.WriteLine($"[Success] Chat with {other.DisplayName} opened");
                        }
                        else
                        {
                            Console.WriteLine($"[Error] User '{name}' not found");
                        }
                    }
                    else if (int.TryParse(cmd, out int num) && num >= 1 && num <= chats.Count)
                    {
                        activeChat = (ViberChat)chats[num - 1];
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

                    Console.WriteLine($"Chat: {activeChat.Title}");
                    Console.WriteLine("--------------------------------");

                    foreach (var msg in activeChat.Messages)
                    {
                        Console.WriteLine($"[{msg.Type.ToUpper()}] {msg.Sender.DisplayName}: {msg.Content}");
                    }

                    Console.WriteLine("--------------------------------");
                    Console.WriteLine("Type message or prefix:");
                    Console.WriteLine(" voice ... picture <url> emoji ... file ...");
                    Console.WriteLine("Special: call / video / menu / back");

                    var input = Console.ReadLine()?.Trim();

                    if (input?.ToLower() == "back")
                    {
                        currentScreen = ScreenState.Main;
                    }
                    else if (input == "call")
                    {
                        Console.WriteLine("[Viber] Voice call started...");
                    }
                    else if (input == "video")
                    {
                        Console.WriteLine("[Viber] Video call started...");
                    }
                    else if (input == "menu")
                    {
                        activeChat.ShowMenu(currentUser);
                    }
                    else if (!string.IsNullOrWhiteSpace(input))
                    {
                        string type = "text";
                        string content = input;

                        if (input.StartsWith("voice ", StringComparison.OrdinalIgnoreCase))
                        {
                            type = "voice";
                            content = input.Substring(6).Trim();
                        }
                        else if (input.StartsWith("picture ", StringComparison.OrdinalIgnoreCase))
                        {
                            type = "picture";
                            content = input.Substring(8).Trim();
                        }
                        else if (input.StartsWith("emoji ", StringComparison.OrdinalIgnoreCase))
                        {
                            type = "emoji";
                            content = input.Substring(6).Trim();
                        }
                        else if (input.StartsWith("file ", StringComparison.OrdinalIgnoreCase))
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

        /// <summary>
        /// Creates or returns existing "My Notes" self-chat
        /// </summary>
        private static ViberSelfChat CreateMyNotes(ViberUser user, MainScreenService mainService, IUserRepository repo)
        {
            var chats = mainService.GetChats();
            var notes = chats.OfType<ViberSelfChat>().FirstOrDefault(c => c.Title == "My Notes");

            if (notes == null)
            {
                notes = new ViberSelfChat(user, repo);
                mainService.AddChat(notes);
            }

            return notes;
        }

        /// <summary>
        /// Automatically creates 1:1 chats with all known contacts (simulated by all users except self)
        /// </summary>
        private static void AutoCreateContactChats(ViberUser user, IUserRepository repo, MainScreenService mainService)
        {
            var allUsers = repo.GetAll();
            foreach (var other in allUsers)
            {
                if (other == user) continue;

                // Simulate "known contact" by phone number match
                if (other.PhoneNumber == user.PhoneNumber) continue; // same user

                bool alreadyHasChat = mainService.GetChats().Any(c =>
                    c is ViberOneOnOneChat &&
                    c.Participants.Contains(user) &&
                    c.Participants.Contains(other));

                if (!alreadyHasChat)
                {
                    var chat = new ViberOneOnOneChat(user, (ViberUser)other, repo);
                    mainService.AddChat(chat);
                    Console.WriteLine($"[Auto] 1:1 chat created with {other.DisplayName} ({other.PhoneNumber})");
                }
            }
        }
    }
}