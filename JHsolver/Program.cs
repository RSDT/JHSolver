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
            var start = DateTime.Now;
            var database = new Database("./database/");
            database.Build();
            Console.Out.WriteLine(DateTime.Now - start);
            Console.Out.WriteLine("done");
            using (StreamWriter outputFile = new StreamWriter(filename, true))
            {

                outputFile.WriteLine(DateTime.Now);
                outputFile.WriteLine(DateTime.Now - start);
            }
        }
    }
}
