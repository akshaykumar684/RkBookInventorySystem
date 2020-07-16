using System;

namespace BookInventorySystem.Model
{
    public class CustomerHistoryModel
    {
        public string CustomerName { get; set; }

        public string BookName { get; set; }

        public string AuthorName { get; set; }

        public DateTime DateTime { get; set; }

        public byte HasBook { get; set; }
    }
}
