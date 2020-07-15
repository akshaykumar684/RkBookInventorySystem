using System;

namespace BookInventorySystem.Model
{
    public class CheckOutModel
    {
        public string OrderId { get; set; }

        public string BookId { get; set; }


        public string CustomerId { get; set; }

        public DateTime DateTime { get; set; }

        public byte HasBook { get; set; }

        public int Quantity { get; set; }
    }
}
