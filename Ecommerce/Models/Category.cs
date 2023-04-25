﻿namespace Ecommerce.Models
{
    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImageId { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public Image Image { get; set; }
    }
}
