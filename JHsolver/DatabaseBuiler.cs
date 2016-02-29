using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace JHsolver
{
    class Pool
    {
        private Thread MainThread;
        private Thread[] workers;
        private Dictionary<string, Action<string>> queue;
        private bool running = false;
        private List<string> keys; 
        public Pool(int nWorkers)
        {
            workers = new Thread[nWorkers];
            queue = new Dictionary<string, Action<string>>();
            keys = new List<string>();
        }

        public void add(Action<string> target, string code)
        {
            queue.Add(code,target);
            keys.Add(code);
        }

        public void Start()
        {
            running = true;
            MainThread = new Thread(run);
            MainThread.Start();
        }
        private void run()
        {
            Console.Out.WriteLine("pool");
            while (running)
            {
                foreach (var worker in workers)
                {
                    if (worker != null)
                    {
                        worker.Join(100);
                    }
                }
                if (!keys.Count.Equals(0))
                {
                    for (var i =0; i < workers.Length; i++)
                    {
                        if (workers[i] == null)
                        {

                            workers[i] = new Thread(threadstringwrapper);
                            var code = keys[0];
                            keys.Remove(code);
                            workers[i].Start(code);
                        }else if (workers[i].ThreadState != ThreadState.Running)
                        {
                            workers[i] = new Thread(threadstringwrapper);
                            var code = keys[0];
                            keys.Remove(code);
                            workers[i].Start(code);
                        }
                    }
                }
            }
        }

        private void threadstringwrapper(object parameter)
        {
            var code = (string) parameter;
            var target = queue[code];
            try
            {
                target(code);
            }
            catch (Exception)
            {
                keys.Add(code);
            }
            
        }

        public void Join()
        {
            while (keys.Count!=0) Thread.Sleep(200);
            foreach (var worker in workers)
            {
                worker?.Join();
            }
            running = false;
            MainThread.Join();
        }
    }
    internal class DatabaseBuiler
    {
        private static readonly string[] posibleCodeElems =  { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
        private static readonly string[] posibleSolutionElems = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        private readonly string folder;

        public DatabaseBuiler(string folder)
        {
            this.folder = folder;
        }

        public IEnumerable<string> ValidCodes()
        {
            return AllCodes().Where(IsValidCode);
        }

        /// <summary>
        /// check of de code een valid code is.
        /// doe dit door te kijken of er geen equivalente code is aan de code.
        /// dit is bijvoorbeeld het geval bij:
        /// abbbcd en abbbce
        /// maar niet bij
        /// abbbcd en abbacd
        /// </summary>
        /// <param name="code">een code van 1 vos.(voor x en y) dus 10 lang.</param>
        /// <returns>of er geen equivalente code is aan code</returns>
        private bool IsValidCode(string code)
        {
            var highest = code[0];
            foreach (var letter in code)
            {
                if (letter == highest+1)
                {
                    highest = letter;
                }
                else if (letter > highest + 1)
                {
                    return false;
                }
            }
            return true;
        }

        public void fillDatabase()
        {
            Pool pool = new Pool(4);
            pool.Start();
            foreach (var code in ValidCodes())
            {
                pool.add(WriteToFile, code);
            }
            pool.Join();
        }
        private void WriteToFile(string code)
        {
            var filename = folder + code + ".txt";
            var file = new FileStream(filename,FileMode.Append,FileAccess.Write);
            //file.Lock(0,file.Length);
            Console.Out.WriteLine(file.Name);
           
            foreach (var coord in ToCoords(code))
            {
                foreach (var elem in coord)
                {
                    file.WriteByte((byte)elem);
                }
                file.WriteByte((byte)'\n');
            }
            //file.Unlock(0,file.Length);
            file.Close();
        }

        private bool all_different(string[] x)
        {
            for (var i = 0; i < x.Length; i++)
            {
                for (var j = 0; j < x.Length; j++)
                {
                    if (i != j)
                    {
                        if (x[i] == x[j])
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        private IEnumerable<string[]> allPossibleSolPermutations()
        {
            foreach (var elem1 in posibleSolutionElems)
            {
                foreach (var elem2 in posibleSolutionElems)
                {
                    foreach (var elem3 in posibleSolutionElems)
                    {
                        foreach (var elem4 in posibleSolutionElems)
                        {
                            foreach (var elem5 in posibleSolutionElems)
                            {
                                foreach (var elem6 in posibleSolutionElems)
                                {
                                    foreach (var elem7 in posibleSolutionElems)
                                    {
                                        foreach (var elem8 in posibleSolutionElems)
                                        {
                                            foreach (var elem9 in posibleSolutionElems)
                                            {
                                                foreach (var elem10 in posibleSolutionElems)
                                                {
                                                    
                                                    var code = new string[]{ elem1 , elem2 , elem3 , elem4 , elem5 , elem6 , elem7 ,elem8 , elem9 , elem10};
                                                    if (all_different(code))
                                                    {
                                                        yield return code;
                                                    }
                                                    
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        } 

        private IEnumerable<string> ToCoords(string Code)
        {

            if (Code.Length != 10)
                throw new ArgumentException();
            else
            { 
                foreach (var perm in allPossibleSolPermutations())
                {
                  string tempcode = (string) Code.Clone();
                    for (int i = 0; i < posibleCodeElems.Length; i++)
                    {
                        tempcode = tempcode.Replace(posibleCodeElems[i], perm[i]);
                    }
                    yield return tempcode;
                }
            }
        }

        private IEnumerable<string> AllCodes()
        {
            string[] posibleElems = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j"};
            foreach (var elem1 in posibleElems)
            {
                foreach (var elem2 in posibleElems)
                {
                    foreach (var elem3 in posibleElems)
                    {
                        foreach (var elem4 in posibleElems)
                        {
                            foreach (var elem5 in posibleElems)
                            {
                                foreach (var elem6 in posibleElems)
                                {
                                    foreach (var elem7 in posibleElems)
                                    {
                                        foreach (var elem8 in posibleElems)
                                        {
                                            foreach (var elem9 in posibleElems)
                                            {
                                                foreach (var elem10 in posibleElems)
                                                {
                                                    var code = elem1 + elem2 + elem3 + elem4 + elem5 + elem6 + elem7 + elem8 +
                                                        elem9 + elem10;
                                                    yield return code;

                                                    if (code == "abcdefghij")
                                                    {
                                                        yield break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
         
    }
}
