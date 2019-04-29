using ECommerce.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace homework_041019_ECommerce_Website.Models
{
    public class CartViewModel
    {
        public List<CartProduct> CartProducts { get; set; }
        public Customer Customer { get; set; }
        public string Message { get; set; }
    }
}
