using System;
using System.Collections.Generic;
using System.Text;

using Library.chatField;
using Library.previewOfChat;

namespace Viber.previewOfChat
{    /// <summary>
     ///chat previews
     /// <summary

    public class ViberChatPreview : ChatPreviewGenerator
    {    /// <summary>
         ///generate preview of chat
         /// <summary
        public string GeneratePreview(Chatfeild chat)
        {
            var p = chat.Participants.FirstOrDefault();
            if (p == null) return "Empty chat";

            string last = chat.GetLastMessagePreview() ?? "";
            string phone = p.PhoneNumber?.Substring(0, Math.Min(9, p.PhoneNumber.Length)) + "..." ?? "No phone";

            return string.IsNullOrEmpty(last)
                ? $"New contact: {p.DisplayName} ({phone})"
                : $"{p.DisplayName} ({phone}): {last}";
        }

        public string GenerateAlternatePreview(Chatfeild chat)
        {
            var p = chat.Participants.FirstOrDefault();
            if (p == null) return "Empty chat";
            return $"Phone-based: {p.PhoneNumber ?? "—"} • {p.DisplayName}";
        }
    }
}
