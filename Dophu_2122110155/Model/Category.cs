﻿namespace Dophu_2122110155.Model
{
    public class Category
    {
        public int ID { get; set; }
        public string Name { get; set; }


        public List<Product>? Products { get; set; }
    }
}
