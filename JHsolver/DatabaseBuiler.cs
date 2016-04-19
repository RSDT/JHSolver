using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace JHsolver
{
	public class Database
	{
		private DatabaseBuilder builder;
		private DatabaseReader reader;
		private readonly string path;
        private bool build = false;
        private static Dictionary<String, Database> databases;
		public Database(string path)
		{
			this.path = path;
			this.builder = new DatabaseBuilder (path);
			this.reader = new DatabaseReader (path);
		}
        public static Database DatabaseMaker(string path)
        {
            ICollection<string> keys = null;
            try {
                if (true)
                keys = databases.Keys;
            } catch (NullReferenceException)
            {
                            }
            bool contains;
            if (keys != null)
            {
                contains = keys.Contains(path);
            } else
            {
                contains = false;
            }
            if (!contains){
                var db = new Database(path);
                databases.Add(path, db );
            }
            return databases[path];
        }
		public void Build ()
		{
            if (!exsits())
            {
                Directory.CreateDirectory(path);
                builder.BuildDatabaseThreaded();
                build = true;
            }else
            {
                Console.Out.WriteLine("please remove the folder: " + path);
            }
			
		}

		/// <summary>
		/// Check of de database al gebouwd is.
		/// </summary>
		/// <returns> of de Database is opgebouwd.</returns>
		public bool exsits (){
            var result = Directory.Exists(path);
            if (!result)
            {
                build = false;
            }
            return result;
		}
	}

	internal class DatabaseReader
	{
		private string path;
		public DatabaseReader(string path){
			this.path = path;
		}
	}

	/// <summary>
	/// Database.
	/// </summary>
    internal class DatabaseBuilder
    {
        private List<ManualResetEvent> _doneEvents;
		/// <summary>
		/// The posible code elems.
		/// </summary>
        private static readonly string[] posibleCodeElems =  { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
        
		/// <summary>
        /// The posible solution elems.
        /// </summary>
		private static readonly string[] posibleSolutionElems = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        
		/// <summary>
        /// The folder.
        /// </summary>
		private readonly string folder;

		/// <summary>
		/// Initializes a new instance of the <see cref="JHsolver.Database"/> class.
		/// </summary>
		/// <param name="folder">Folder.</param>
		public DatabaseBuilder(string folder)
        {
            this.folder = folder;
        }

		/// <summary>
		/// returns all the valid codes.
		/// </summary>
		/// <returns>The codes.</returns>
		private IEnumerable<string> ValidCodes()
        {
            return AllCodes().Where(IsValidCode);
        }

		/// <summary>
		/// Builds the database.
		/// this can take up for a couple of hours,
		/// </summary>
		public void BuildDatabaseThreaded()
        { 
			_doneEvents= new List<ManualResetEvent>();
			foreach (var code in ValidCodes())
			{
				_doneEvents.Add (new ManualResetEvent(false));
				ThreadPool.QueueUserWorkItem (WriteToFileWrapper, new Tuple<string,int>(code,_doneEvents.Count -1));
                if (_doneEvents.Count > 62)
                {
                    var doneEvents2 = new ManualResetEvent[_doneEvents.Count];
                    for (var i = 0; i < _doneEvents.Count; i++)
                    {
                        doneEvents2[i] = _doneEvents[i];
                    }
                    WaitHandle.WaitAll(doneEvents2);
                    _doneEvents = new List<ManualResetEvent>();
                }
			}
			var doneEvents3 = new ManualResetEvent[_doneEvents.Count];
			for (var i = 0; i < _doneEvents.Count; i++) {
				doneEvents3 [i] = _doneEvents [i];
			}
			WaitHandle.WaitAll (doneEvents3);
		}
        /// <summary>
		/// Builds the database.
		/// this can take up for a couple of hours,
		/// </summary>
		public void BuildDatabaseLinear()
        {
            foreach (var code in ValidCodes())
            {
                WriteToFile(code);
            }
        }
        /// <summary>
        /// <see cref="WriteToFile"/>
        /// </summary>
        /// <param name="threadContext">Thread context. wordt gecast naar een string.</param>
        private void WriteToFileWrapper(Object threadContext){
			var pair  = (Tuple<string,int>) threadContext;
			WriteToFile (pair.Item1);
            _doneEvents[pair.Item2].Set();
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
			if (code [0] != 'a' || code.Length != 10) {
				return false;
			}
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

        
		/// <summary>
		/// schrijf alle mogelijke coordinaten voor de code in het bestand folder/{code}.txt
		/// </summary>
		/// <param name="code">Code.</param>
        private void WriteToFile(string code)
        {
            string filename = folder + code + ".txt";
            //var file = new FileStream(filename,FileMode.Append,FileAccess.Write);
            //file.Close();
            //file.Lock(0,file.Length);
           //Console.Out.WriteLine(file.Name);
            //File.Create(filename).Close();
          
                int i = 0;
                Console.Out.WriteLine(code);
                foreach (string line in ToCoords(code)){
                    using (StreamWriter outputFile = new StreamWriter(filename,true)) {
         
                        outputFile.WriteLine(line);
                    Console.Out.WriteLine(code + " "+ i++);
                }
            }
            /*
            foreach (var coord in ToCoords(code))
            {
                foreach (var elem in coord)
                {
                    file.WriteByte((byte)elem);
                }
                file.WriteByte((byte)'\n');
            }
            //file.Unlock(0,file.Length);
            file.Close();*/
        }

		/// <summary>
		/// Check if all the elements in <see cref="x"/>  are different.
		/// </summary>
		/// <returns><c>true</c>, if all the elements are different, <c>false</c> otherwise.</returns>
		/// <param name="x">The x coordinate.</param>
		private bool isAllDifferent(string[] x)
        {
            for (var i = x.Length -1; i >= 0; i--)
            {
                for (var j = x.Length -1; j >= 0 ; j--)
                {
// Console.Out.WriteLine(x[i] + ".  ." + x[j] + "- " + (x[i] == x[j]));
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

		/// <summary>
		/// Alls the possible sol permutations.
		/// </summary>
		/// <returns>possible </returns>
        private IEnumerable<string[]> allPossibleSolPermutations()
        {
            string[] perm;
            foreach (var elem1 in posibleSolutionElems){
                foreach (var elem2 in posibleSolutionElems){
                    perm = new string[] { elem1, elem2};
                    if (isAllDifferent(perm))
                    {
                        foreach (var elem3 in posibleSolutionElems){
                            perm = new string[] { elem1, elem2, elem3};
                            if (isAllDifferent(perm))
                            {
                                foreach (var elem4 in posibleSolutionElems)
                                {
                                    perm = new string[] { elem1, elem2, elem3, elem4 };
                                    if (isAllDifferent(perm))
                                    {
                                        foreach (var elem5 in posibleSolutionElems)
                                        {
                                            perm = new string[] { elem1, elem2, elem3, elem4, elem5 };
                                            if (isAllDifferent(perm))
                                            {
                                                foreach (var elem6 in posibleSolutionElems)
                                                {
                                                    perm = new string[] { elem1, elem2, elem3, elem4, elem5, elem6 };
                                                    if (isAllDifferent(perm))
                                                    {
                                                        foreach (var elem7 in posibleSolutionElems)
                                                        {
                                                            perm = new string[] { elem1, elem2, elem3, elem4, elem5, elem6, elem7 };
                                                            if (isAllDifferent(perm))
                                                            {
                                                                foreach (var elem8 in posibleSolutionElems)
                                                                {
                                                                    perm = new string[] { elem1, elem2, elem3, elem4, elem5, elem6, elem7, elem8 };
                                                                    if (isAllDifferent(perm))
                                                                    {
                                                                        foreach (var elem9 in posibleSolutionElems)
                                                                        {
                                                                            perm = new string[] { elem1, elem2, elem3, elem4, elem5, elem6, elem7, elem8, elem9 };
                                                                            if (isAllDifferent(perm))
                                                                            {
                                                                                foreach (var elem10 in posibleSolutionElems)
                                                                                {

                                                                                    perm = new string[] { elem1, elem2, elem3, elem4, elem5, elem6, elem7, elem8, elem9, elem10 };
                                                                                    if (isAllDifferent(perm))
                                                                                    {
                                                                                        yield return perm;
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
                var list = new HashSet<string>();
                foreach (var perm in allPossibleSolPermutations())
                {
                  string tempcode = (string) Code.Clone();
                    for (int i = 0; i < posibleCodeElems.Length; i++)
                    {
                        tempcode = tempcode.Replace(posibleCodeElems[i], perm[i]);
                    }
                    if (!list.Contains(tempcode))
                    {
                        list.Add(tempcode);
                        yield return tempcode;
                    }
                }
            }
        }

		private IEnumerable<string> AllCodes()
        {
            foreach (var elem1 in posibleCodeElems)
            {
                foreach (var elem2 in posibleCodeElems)
                {
                    foreach (var elem3 in posibleCodeElems)
                    {
                        foreach (var elem4 in posibleCodeElems)
                        {
							foreach (var elem5 in posibleCodeElems)
                            {
								foreach (var elem6 in posibleCodeElems)
                                {
									foreach (var elem7 in posibleCodeElems)
                                    {
										foreach (var elem8 in posibleCodeElems)
                                        {
											foreach (var elem9 in posibleCodeElems)
                                            {
												foreach (var elem10 in posibleCodeElems)
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
