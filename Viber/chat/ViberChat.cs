using Library.chatField;
using Library.repositories;
using Library.user;

namespace Viber.chat
{/// <summary>
///abstract class for viber
/// <summary>
    public abstract class ViberChat : Chat
    {/// <summary>
///constractor for user of messenger
/// <summary>
        protected ViberChat(IUserRepository userRepo) : base(userRepo)
        {
        }
        /// <summary>
        ///show basic menu of chat
        /// <summary>
        public override void ShowMenu(User currentUser)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════╗");
                Console.WriteLine("║        Viber Chat Menu       ║");
                Console.WriteLine("╚══════════════════════════════╝");
                Console.WriteLine("1. Info about user");
                Console.WriteLine("2. Share contact");
                Console.WriteLine("3. Add to favorite");
                Console.WriteLine("4. Media");
                Console.WriteLine("5. GIFs");
                Console.WriteLine("6. Links");
                Console.WriteLine("7. Files");
                Console.WriteLine("8. Participants");
                Console.WriteLine("9. Mute chat");
                Console.WriteLine("10. Set background");
                Console.WriteLine("11. Block this contact");
                Console.WriteLine("12. Report");
                Console.WriteLine("13. Delete conversation");
                Console.WriteLine("0. Back to chat");

                Console.Write("\nEnter number: ");
                var input = Console.ReadLine()?.Trim();

                if (input == "0" || string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("[Back to chat]");
                    Console.ReadLine();
                    return;
                }

                switch (input)
                {
                    case "1":
                        Console.WriteLine("\nUser Info:");
                        Console.WriteLine($"Name: {currentUser.DisplayName}");
                        Console.WriteLine($"Phone: {currentUser.PhoneNumber}");
                        Console.WriteLine($"Status: {currentUser.Status}");
                        break;

                    case "2":
                        Console.WriteLine("[Shared] Your contact info sent to chat.");
                        break;

                    case "3":
                        Console.WriteLine($"[Favorite] {Title} added to favorites.");
                        break;

                    case "4":
                        ShowMedia();
                        break;

                    case "5":
                        Console.WriteLine("GIFs in this chat:");
                        var gifs = Messages.Where(m => m.Content.Contains(".gif") || m.Type == "gif");
                        if (!gifs.Any()) Console.WriteLine("No GIFs yet.");
                        foreach (var g in gifs) Console.WriteLine($" - {g.Content}");
                        break;

                    case "6":
                        ShowLinks();
                        break;

                    case "7":
                        ShowFiles();
                        break;

                    case "8":
                        Console.WriteLine("Participants:");
                        foreach (var p in Participants)
                            Console.WriteLine($" - {p.DisplayName} (@{p.DisplayName.ToLower()})");
                        break;

                    case "9":
                        Console.WriteLine("[Muted] Chat notifications turned off.");
                        break;

                    case "10":
                        Console.WriteLine("[Background] Set new background (placeholder).");
                        break;

                    case "11":
                        Console.WriteLine("[Blocked] Contact blocked.");
                        break;

                    case "12":
                        Console.WriteLine("[Reported] Chat reported.");
                        break;

                    case "13":
                        Console.WriteLine("[Deleted] Conversation deleted (simulation).");
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
            }
        }
        /// <summary>
        ///Get last messeg preview of chat
        /// <summary>
        public override string GetLastMessagePreview()
        {
            if (Messages.Count == 0) return "No messages yet";

            var lastContent = Messages[^1].Content ?? "";
            return lastContent.Length > 20
                ? lastContent.Substring(0, 17) + "..."
                : lastContent;
        }
    }
}