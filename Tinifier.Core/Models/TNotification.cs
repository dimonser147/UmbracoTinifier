using System;
using Umbraco.Core.Events;

namespace Tinifier.Core.Models
{
    public class TNotification
    {
        public string headline { get; set; }
        public string message { get; set; }
        public string url { get; set; }
        public bool sticky { get; set; } = false;
        public string type { get; set; } = "info";

        public TNotification(string text)
        {
            message = text;
        }

        public TNotification (string header, string text)
            : this(text)
        {
            headline = header;
        }

        public TNotification (string header, string text, EventMessageType type)
            : this(header, text)
        {
            this.type = Enum.GetName(typeof(EventMessageType), type).ToLowerInvariant();
        }

    }
}
