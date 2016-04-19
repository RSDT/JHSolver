using System;
using System.IO;


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
            using (StreamWriter outputFile = new StreamWriter("out.txt", true))
            {

                outputFile.WriteLine(DateTime.Now);
                outputFile.WriteLine(DateTime.Now - start);
            }
        }
    }
}
