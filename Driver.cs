using System;
using System.IO;
using System.Text;


namespace Falak {

    public class Driver {

        const string VERSION = "0.4";

        //-----------------------------------------------------------
        static readonly string[] ReleaseIncludes = {
            "Lexical analysis",
            "Syntactic analysis",
            "AST construction",
            "Semantic analysis"
        };

        //-----------------------------------------------------------
        void PrintAppHeader() {
            Console.WriteLine("Falak compiler, version " + VERSION);
            Console.WriteLine(
                "Copyright \u00A9 2013-2021 by A. Ortiz, ITESM CEM.");
            Console.WriteLine("This program is free software; you may "
                + "redistribute it under the terms of");
            Console.WriteLine("the GNU General Public License version 3 or "
                + "later.");
            Console.WriteLine("This program has absolutely no warranty.");
        }

        //-----------------------------------------------------------
        void PrintReleaseIncludes() {
            Console.WriteLine("Included in this release:");
            foreach (var phase in ReleaseIncludes) {
                Console.WriteLine("   * " + phase);
            }
        }

        //-----------------------------------------------------------
        void Run(string[] args) {

            PrintAppHeader();
            Console.WriteLine();
            PrintReleaseIncludes();
            Console.WriteLine();

            if (args.Length != 1) {
                Console.Error.WriteLine(
                    "Please specify the name of the input file.");
                Environment.Exit(1);
            }

            try {
                var inputPath = args[0];
                var input = File.ReadAllText(inputPath);
                var parser = new ParserTest(
                    new Scanner(input).Scan().GetEnumerator());
                var program = parser.Program();
                Console.WriteLine("Syntax OK.");

                var semantic = new FirstVisitor();
                semantic.Visit((dynamic) program);
                Console.WriteLine("Global Semantics OK.");
				Console.WriteLine("============");
				Console.WriteLine("\nGlobal: ");
				Console.WriteLine(semantic.GlobalFunctionsTable.ToString());
				Console.WriteLine(semantic.GlobalVars.ToString());
				

				var semantic2 = new SecondVisitor(semantic.GlobalFunctionsTable, semantic.GlobalVars);
				semantic2.Visit((dynamic) program);
    
                Console.WriteLine(semantic2.globalFunctions.ToString());
                Console.WriteLine(semantic2.globalVariables.ToString());
                Console.WriteLine(semantic2.localFuncTable.ToString());

                Console.WriteLine("");
                //Console.WriteLine("Symbol Table");
                
                /*foreach (var entry in semantic.Table) {
                    Console.WriteLine(entry);
                }*/

            } catch (Exception e) {

                if (e is FileNotFoundException
                    || e is SyntaxError
                    || e is SemanticError) {
                    Console.Error.WriteLine(e.Message);
                    Environment.Exit(1);
                }

                throw;
            }
        }

        //-----------------------------------------------------------
        public static void Main(string[] args) {
            new Driver().Run(args);
        }
    }
}

