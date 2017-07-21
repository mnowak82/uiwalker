using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenQA.Selenium.Chrome;
using WalkerScript.Execution;

namespace WalkerScript
{
    public class Test
    {
        private readonly XElement _test;
        private readonly Dictionary<string, string> _globalVariables;
        private readonly Script _script;
        public string Name { get; private set; }

        public Test(XElement test, Dictionary<string, string> globalVariables, Script script)
        {
            _test = test;
            _globalVariables = globalVariables;
            _script = script;
            Name = _test.GetAttribute("name");
        }

        public bool Validate()
        {
            bool noErrors = true;

            var execution = new TestEnvironment(Name, _globalVariables, null, _script.CreateOperations);
            var operations = _test.Descendants();
            foreach (var operation in operations)
            {
                try
                {
                    execution.ValidateCommand(operation);
                }
                catch (Exception e)
                {
                    noErrors = false;
                    Console.WriteLine(e);
                }
            }

            return noErrors;
        }

        public void Execute(string driverName)
        {
            using (var driver = _script.CreateDriver(driverName))
            {
                var execution = new TestEnvironment(Name, _globalVariables, driver, _script.CreateOperations);
                
                var operations = _test.Descendants();
                foreach (var operation in operations)
                {
                    execution.ExecuteCommand(operation);
                }
            }
        }
    }

}
