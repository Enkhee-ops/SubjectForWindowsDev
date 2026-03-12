using System;
using System.Collections.Generic;
using System.Text;

using Library.chatField;
/// <summary>
/// interface for generating chat preview.
/// <summary>
namespace Library.previewOfChat
{
    public interface ChatPreviewGenerator
    {
        string GeneratePreview(Chatfeild chat);
        string GenerateAlternatePreview(Chatfeild chat);
    }
}