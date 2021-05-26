using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestParse
{
    public class Category
    {
        public int ID;
        public string Name;
        public string russianName;
        public List<Product> Products;

        public Category(string name)
        {
            Name = name;
            Products = new List<Product>();

        }
    }
}
