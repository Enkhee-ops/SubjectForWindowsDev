using Library.repositories;
using Viber.user;

namespace Viber.chat
{/// <summary>
///groupchat of viber
/// <summary>
    public class ViberGroupChat : ViberChat
    {/// <summary>
///which user is admin
/// <summary>
        public ViberUser Admin { get; }
        /// <summary>
        ///constarctor for groupchat of messenger
        /// <summary>
        public ViberGroupChat(ViberUser admin, string title, IUserRepository userRepo)
            : base(userRepo)
        {
            Admin = admin;
            Title = title;
            AddParticipant(admin);
        }
        /// <summary>
        ///add member of groupchat
        /// <summary>
        public void AddMember(ViberUser admin, ViberUser member)
        {
            if (admin == Admin)
            {
                if (!Participants.Contains(member))
                {
                    AddParticipant(member);
                    Console.WriteLine($"[Viber] {member.DisplayName} has been added to the group by admin.");
                }
                else
                {
                    Console.WriteLine($"[Viber] {member.DisplayName} is already in the group.");
                }
            }
            else
            {
                Console.WriteLine("[Viber] Only the group admin can add members.");
            }
        }
        /// <summary>
        ///remove member of groupchat
        /// <summary
        public void RemoveMember(ViberUser admin, ViberUser member)
        {
            if (admin == Admin)
            {
                if (Participants.Contains(member))
                {
                    Participants.Remove(member);
                    Console.WriteLine($"[Viber] {member.DisplayName} has been removed from the group by admin.");
                }
                else
                {
                    Console.WriteLine($"[Viber] {member.DisplayName} is not in the group.");
                }
            }
            else
            {
                Console.WriteLine("[Viber] Only the group admin can remove members.");
            }
        }
        /// <summary>
        ///get last message preview of groupchat
        /// <summary
        public override string GetLastMessagePreview()
        {
            if (Messages.Count == 0) return "No messages yet";
            var last = Messages[^1];
            return $"{last.Sender.DisplayName}: {last.Content.Substring(0, Math.Min(20, last.Content.Length)) + "..."}";
        }
    }
}