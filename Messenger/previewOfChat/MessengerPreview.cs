using System;
using System.Collections.Generic;
using System.Text;

using Library.chatField;
using Library.previewOfChat;
/// <summary>
///preview of chat of messenger
/// <summary>
namespace Messenger.previewOfChat
{
    public class MessengerPreview : ChatPreviewGenerator
    {/// <summary>
///preview of chat with lastmessagepreview
/// <summary>
        public string GeneratePreview(Chatfeild chat)
        {
            var p = chat.Participants.Count > 0 ? chat.Participants[0] : null;
            if (p == null) return "Empty chat";
            return $"{p.ProfilePictureUrl} {p.DisplayName}: {chat.GetLastMessagePreview()}";
        }
        /// <summary>
        ///preview of chat without lastmessagepreview
        /// <summary>
        public string GenerateAlternatePreview(Chatfeild chat)
        {
            var p = chat.Participants.Count > 0 ? chat.Participants[0] : null;
            if (p == null) return "Empty chat";
            return $"{p.ProfilePictureUrl} {p.DisplayName}";
        }
    }
}
