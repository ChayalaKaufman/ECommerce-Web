﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Data
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
    }
}
