﻿

namespace Sharebook.Core.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int ProductStock { get; set; }
        public double LocationLatitude { get; set; }
        public double LocationLongitude { get; set; }
    }
}
