using Library.chatField;
using Library.message;
using Library.repositories;
using Library.user;
using System;
using System.Collections.Generic;
using System.Text;


namespace Library.chatField
{        /// <summary>
         /// Groupchat class, inherits from Chat, has an Admin, and allows adding/removing members by the Admin. It also has a custom menu with group-specific options.
         /// </summary>
    public class GroupChat : Chatfeild
    {        /// <summary>
             /// admin of the group chat, who has special permissions to manage members. The admin is set at creation and cannot be changed.
             /// </summary>
        public User Admin { get; }
        /// <summary>
        /// group chat constructor, requires an admin user, a title for the group, and a user repository to manage participants. The admin is automatically added as a participant when the group is created.
        /// </summary>
        public GroupChat(User admin, string title, IUserRepository userRepo) : base(userRepo) 
        {
            Admin = admin;
            Title = title;
            AddParticipant(admin);
        }
        /// <summary>
        /// add a member to the group chat, but only if the requester is the admin. This method checks if the admin parameter matches the Admin property, and if so, adds the member to the Participants list and logs an event. If the requester is not the admin, it logs an error message.
        /// </summary>
        public void AddMember(User admin, User member)
        {
            if (admin == Admin)
            {
                AddParticipant(member);
                Console.WriteLine($"[Event] {member.DisplayName} added by admin");
            }
            else
            {
                Console.WriteLine("[Error] Only admin can add members");
            }
        }
        /// <summary>
        /// remove a member from the group chat, but only if the requester is the admin.
        public void RemoveMember(User admin, User member)
        {
            if (admin == Admin && Participants.Contains(member))
            {
                Participants.Remove(member);
                
                Console.WriteLine($"[Event] {member.DisplayName} removed by admin");
            }
            else
            {
                Console.WriteLine("[Error] Only admin can remove members");
            }
        }


        /// <summary>
        /// shows menu of group chat.
        /// </summary>
        public override void ShowMenu(User currentUser)//---------------------------------------------------------------------------------------service and do it diffrent
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"╔════════════════════════════════════╗");
                Console.WriteLine($"║     Group Chat Menu - {Title,-16} ║");
                Console.WriteLine($"╚════════════════════════════════════╝");
                Console.WriteLine("1. Info about Group");
                Console.WriteLine("2. Media");
                Console.WriteLine("3. Files");
                Console.WriteLine("4. Links");
                Console.WriteLine("5. Chat notifications");
                Console.WriteLine("6. Read receipts (toggle + view)");
                Console.WriteLine("7. Report");
                Console.WriteLine("8. Chat members (admin controls)");
                Console.WriteLine("0. Exit menu");

                Console.Write("\nEnter number (or 0 to exit): ");
                var input = Console.ReadLine()?.Trim();

                if (input == "0" || string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("[Menu closed]");
                    Console.ReadLine();
                    return;
                }

                if (int.TryParse(input, out int choice))
                {
                    switch (choice)
                    {
                        case 1: ShowInfo(); break;
                        case 2: ShowMedia(); break;
                        case 3: ShowFiles(); break;
                        case 4: ShowLinks(); break;
                        case 5: SetNotifications(); break;
                        case 6: ToggleAndShowReadReceipts(); break;
                        case 7:
                            Console.WriteLine("[Reported] Group reported.");
                            Console.ReadLine();
                            break;
                        case 8:
                            ShowMembers(currentUser,userRepo);
                            break;
                        default:
                            Console.WriteLine("Invalid choice.");
                            Console.ReadLine();
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// shows memebr of group.
        /// </summary>
        private void ShowMembers(User currentUser, IUserRepository userRepo)  // Added parameter
        {
            Console.WriteLine("Chat Members:");
            foreach (var p in Participants)
            {
                Console.WriteLine($"- {p.DisplayName} (@{p.DisplayName.ToLower()}) - Invited by: {(p == Admin ? "Creator" : "Admin")}");
            }

            if (currentUser == Admin)
            {
                Console.WriteLine("Admin options: 'add <name>' or 'remove <name>' or 'back'");
                var adminInput = Console.ReadLine()?.Trim();

                if (adminInput?.StartsWith("add ") == true)
                {
                    var name = adminInput.Substring(4).Trim();
                    var member = userRepo.GetByName(name);
                    if (member != null)
                    {
                        AddMember(currentUser, member);
                        Console.WriteLine($"[Success] Added {member.DisplayName}");
                    }
                    else Console.WriteLine("[Error] User not found");
                }
                else if (adminInput?.StartsWith("remove ") == true)
                {
                    var name = adminInput.Substring(7).Trim();
                    var member = userRepo.GetByName(name);
                    if (member != null)
                    {
                        RemoveMember(currentUser, member);
                        Console.WriteLine($"[Success] Removed {member.DisplayName}");
                    }
                    else Console.WriteLine("[Error] User not found");
                }
            }
        }
    }

}
