using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Walker.Properties;
using WalkerInterfaces;
using WalkerInterfaces.Exceptions;
using WalkerScript;
using WalkerScript.Execution;
using WalkerScript.Operations;

namespace Walker
{
    public class Program
    {
        private Dictionary<string, string> _globalVariables;
        private Dictionary<string, string> GlobalVariables
        {
            get
            {
                if (_globalVariables == null)
                {
                    var now = DateTime.Now;
                    _globalVariables = new Dictionary<string, string>();
                    _globalVariables.Add("date", now.ToString("yyyy-MM-dd"));
                    _globalVariables.Add("time", now.ToString("HH.mm.ss"));
                    _globalVariables.Add("datetimelong", now.ToString("F"));
                }
                return _globalVariables;
            }
        }

        private string SelectedTest { get; set; }

        private string ScriptFile { get; set; }

        static void Main(string[] args)
        {
            if (args.Length == 0 || new[] {"/?", "/help", "--help", "-h"}.Contains(args[0]))
            {
                PrintHelp();
                return;
            }

            var p = new Program();
            p.ProcessArguments(args);


            var s = new Script(p.ScriptFile, p.GlobalVariables);
            
            // walidacja dotyczy całego pliku
            bool ok = true;
            foreach (var test in s.Tests)
            {
                ok = test.Validate() && ok;
            }

            var defColor = Console.ForegroundColor;

            if (!ok)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Found errors in file '{0}'.", p.ScriptFile);
                Console.ForegroundColor = defColor;
                return;
            }

            if (!string.IsNullOrEmpty(p.SelectedTest))
            {
                var toExecute = s.Tests.FirstOrDefault(t => t.Name == p.SelectedTest);
                if (toExecute == null)
                    throw new ArgumentException(string.Format("Test '{0}' not found in tests in file '{1}", p.SelectedTest, p.ScriptFile), "-t");
                toExecute.Execute();
                return;
            }


            var passed = 0;
            var failed = 0;
            foreach (var test in s.Tests)
            {
                try
                {
                    Console.ForegroundColor = defColor;
                    Console.Write("Executing test '{0}'... ", test.Name);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    test.Execute();
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(" PASSED");
                    ++passed;
                }
                catch (BusinessApplicationWalkerException e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(" FAILED: ");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("{0}", e.Message);
                    ++failed;
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(" FAILED: ");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("{0}", e);
                    ++failed;
                }
                Console.ForegroundColor = defColor;
            }

            Console.ForegroundColor = failed > 0 ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine("Executed {0} tests: passed {1}, failed {2}.", passed + failed, passed, failed);
            Console.ForegroundColor = defColor;
        }

        private void ProcessArguments(string[] args)
        {
            if (args.Length == 0)
                throw new InvalidOperationException("Missing script file in first argument");

            ScriptFile = args[0];

            for (var i = 1; i < args.Length; ++i)
            {
                var v = args[i];
                switch (v)
                {
                    case "-p":
                        if (i + 2 >= args.Length)
                            throw new InvalidOperationException(string.Format("Unknown argument '{0}'", v));
                        GlobalVariables[args[i + 1]] = args[i + 2];
                        i += 2;
                        break;
                    case "-t":
                        if (i + 1 >= args.Length)
                            throw new InvalidOperationException(string.Format("Unknown argument '{0}'", v));
                        SelectedTest = args[i + 1];
                        i += 1;
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("Unknown argument '{0}'", v));
                }
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine("{0} <skrypt> [-t <nazwa_testu>] [-p <param1> <value1> [-p <param2> <value2> [...]]]", System.AppDomain.CurrentDomain.FriendlyName);
            Console.WriteLine(" - program do uruchamiania skryptów dla aplikacji biznesowych");
            Console.WriteLine();
            Console.WriteLine("Parametry:");
            Console.WriteLine("\t<skrypt>\r- plik xml sterujący przebiegiem testów");
            Console.WriteLine("\t-t <nazwa_testu>\r- wskazuje, który test ma być wykonany");
            Console.WriteLine("\t-p <param1> <value1> [-p <paramN> <valueN> [...]]- pary (nazwa, wartość) przekazywane do skryptu");
            Console.WriteLine();
            Console.WriteLine("Zmienne:");
            Console.WriteLine("W skrypcie istnieje możliwość odwołania się do zmiennych, za pomocą nawiasów klamrowych ${...}");
            Console.WriteLine("Dostępne są zmienne zdefiniowane w parametrach, ustawione w trakcie wykonania skryptu i globalne:");
            Console.WriteLine("\t${{now}}\t\t- zwraca bieżącą godzinę, w formacie {0:HH:mm:ss}", DateTime.Now);
            Console.WriteLine("\t${{nowlong}}\t- zwraca bieżącą datę w formacie {0:F}", DateTime.Now);
            Console.WriteLine("\t${{time}}\t\t- zwraca godzinę uruchomienia skryptu, w formacie {0:HH.mm.ss}", DateTime.Now);
            Console.WriteLine("\t${{date}}\t\t- zwraca godzinę uruchomienia skryptu, w formacie {0:yyyy.MM.dd}", DateTime.Now);
            Console.WriteLine("\t${{datetimelong}}\t- zwraca godzinę uruchomienia skryptu, w formacie {0:F}", DateTime.Now);
            Console.WriteLine("Zmienną można ustawić w trakcie wykonania skryptu, poprzez dodanie atrybutu ... set=\"nazwa_zmiennej\" ...");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Przykładowy skrypt:");
            Console.WriteLine();
            Console.WriteLine("{0}", Properties.Resources.example);
            Console.WriteLine();
            Console.WriteLine("Operacje dostępne w skrypcie:");


            var ee = new TestEnvironment("Operations", new Dictionary<string, string>(), null, driver => 
            new object[] {
                new ScriptOperations(),
                (IBusinessApplicationWalker)Activator.CreateInstance(ScriptHelper.LoadType(ConfigurationManager.AppSettings["Walker"]))
            });
            var ops = ee.AvailableOperations().OrderBy(o => o.Name);

            foreach (var operation in ops)
            {
                var hasReturn = operation.Method.ReturnType != typeof(void);
                var method = string.Format("<{0} {1}{2}/>", operation.Name, 
                    string.Join(" ", operation.Parameters.Select(p => string.Format(p.IsRequired ? "{0}=\"\"" : "[{0}=\"\"]", p.Name))),
                    hasReturn ? " [set=\"\"]" : "");

                var align = 55 - method.Length > 0 ? new String('-', 55 - method.Length) : "-";
                var str = Resources.ResourceManager.GetString(string.Format("Method_{0}", operation.Name));
                Console.WriteLine(str != null ? "{0} {1} {2}" : "{0}", method, align, str);
                bool any = false;
                foreach (var op in operation.Parameters)
                {
                    var desc = Resources.ResourceManager.GetString(string.Format("Method_{0}_{1}", operation.Name, op.Name));
                    if (string.IsNullOrEmpty(desc))
                        continue;
                    any = true;

                    var param = string.Format("        {0}", op.Name);
                    align = 20 - param.Length > 0 ? new String(' ', 20 - param.Length) : " ";
                    Console.WriteLine("{0}{1}- {2} {3}", param, align, desc, op.IsRequired ? "(wymagany)" : "(opcjonalny)");
                }

                if (hasReturn)
                    Console.WriteLine("        set         - wskazuje nazwę zmiennej w której zostanie zachowana zwrócona wartość (opcjonalny)");

                if (any || hasReturn)
                    Console.WriteLine();
            }
        }
   }
}
