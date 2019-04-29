using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Data
{
    public class Address
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public int CustomerId { get; set; }
    }
}
