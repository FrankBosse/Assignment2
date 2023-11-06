using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    public class Airport
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public Airport(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}
