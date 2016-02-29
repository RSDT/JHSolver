using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JHsolver
{
    class Program
    {
        static void Main(string[] args)
        {
            var databaseBuilder = new DatabaseBuiler("./database/");
            databaseBuilder.fillDatabase();
            Console.Out.WriteLine("done");
            Thread.Sleep(10000);
        }

        public static async Task test()
        {
            
        }
    }
}
