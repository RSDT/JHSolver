using System;
using System.CodeDom;
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
		public Database(string path)
		{
			this.path = path;
			this.builder = new DatabaseBuilder (path);
			this.reader = new DatabaseReader (path);
		}

		public void Build ()
		{
			builder.BuildDatabase ();
		}

		/// <summary>
		/// Check of de database al gebouwd is.
		/// </summary>
		/// <returns> of de Database is opgebouwd.</returns>
		public bool exsits (){
			return Directory.Exists (path);
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
		public void BuildDatabase()
		{
			List<ManualResetEvent> doneEvents= new List<ManualResetEvent>();
			foreach (var code in ValidCodes())
			{
				doneEvents.Add (new ManualResetEvent(false));
				ThreadPool.QueueUserWorkItem (WriteToFileWrapper, code);
			}
			var doneEvents2 = new ManualResetEvent[doneEvents.Count];
			for (var i = 0; i < doneEvents.Count; i++) {
				doneEvents2 [i] = doneEvents [i];
			}
			WaitHandle.WaitAll (doneEvents2);
		}
		/// <summary>
		/// <see cref="WriteToFile"/>
		/// </summary>
		/// <param name="threadContext">Thread context. wordt gecast naar een string.</param>
		private void WriteToFileWrapper(Object threadContext){
			string code  = (string) threadContext;
			WriteToFile (code);
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
            var filename = folder + code + ".txt";
            var file = new FileStream(filename,FileMode.Append,FileAccess.Write);
            //file.Lock(0,file.Length);
            Console.Out.WriteLine(file.Name);
			return;
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

		/// <summary>
		/// Alls the possible sol permutations.
		/// </summary>
		/// <returns>possible </returns>
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
                                                    
                                                    var perm = new string[]{ elem1 , elem2 , elem3 , elem4 , elem5 , elem6 , elem7 ,elem8 , elem9 , elem10};
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
