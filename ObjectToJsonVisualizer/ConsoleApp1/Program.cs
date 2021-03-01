using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            S s = new S();
            s.Name = "sds";
            List<S> lst = new List<S>();
            lst.Add(s);
            Console.ReadKey();
        }

        public class S
        { 
            public string Name { get; set; }
        }
    }
}
