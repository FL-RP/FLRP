using System;
using System.Collections.Generic;

namespace outRp.OtherSystem.Phone
{
    public class pClass
    {
        public class Contact
        {
            public string Name { get; set; }
            public int Number { get; set; }
        }

        public class Messages
        {
            public int Number { get; set; }
            public List<Message> message { get; set; }
        }
        public class Message
        {
            public int type { get; set; } = 1;
            public bool isOwner { get; set; }
            public string text { get; set; }
            public DateTime DateTime { get; set; } = new();
        }

        public class MessagesTiny
        {
            public int Number { get; set; }
        }

        public class image
        {
            public string link { get; set; }
        }

    }
}
