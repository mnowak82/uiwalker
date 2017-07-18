using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using WalkerScript.Exceptions;

namespace WalkerScript.Execution
{
    public class TestEnvironment
    {
        private readonly string _testName;
        private IWebDriver _driver;
        private readonly OperationFactory _operationFactory;
        private readonly Dictionary<string, Operation> _operations = new Dictionary<string, Operation>();
        private Dictionary<string, string> _variables = new Dictionary<string, string>();

        public delegate object[] OperationFactory(IWebDriver driver);

        public TestEnvironment(string testName, Dictionary<string, string> inheritedVariables, IWebDriver driver, OperationFactory operationFactory)
        {
            _testName = testName;
            _driver = driver;
            _operationFactory = operationFactory;
            InitializeVariables(inheritedVariables);

            LoadOperations();
        }

        public IEnumerable<Operation> AvailableOperations()
        {
            return _operations.Values;
        }

        private void InitializeVariables(Dictionary<string, string> inheritedVariables)
        {
            foreach (var inheritedVariable in inheritedVariables)
            {
                _variables.Add(inheritedVariable.Key, inheritedVariable.Value);
            }
            _variables.Add("testName", _testName);
        }

        /// <summary>
        /// wczytuje operacje z obiektów (tworzy spis operacji)
        /// </summary>
        private void LoadOperations()
        {
            foreach (var operationSource in _operationFactory(_driver))
            {
                var type = operationSource.GetType();
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                foreach (var method in methods.Where(m => type.IsAssignableFrom(m.DeclaringType)))
                {
                    _operations.Add(method.Name, new Operation(method, operationSource));
                }
            }
        }

        /// <summary>
        /// sprawdza poprawność nazw i atrybutów
        /// </summary>
        /// <param name="command"></param>
        public void ValidateCommand(XElement command)
        {
            var opId = command.Name.ToString();
            Operation op;
            if (!_operations.TryGetValue(opId, out op))
                throw new InvalidScriptException("Unknown command '{0}' in test '{1}'", opId, _testName);

            var attr = command.GetAttributes();
            op.Validate(attr);
        }

        /// <summary>
        /// wykonuje dane polecenie
        /// </summary>
        /// <param name="command"></param>
        public void ExecuteCommand(XElement command)
        {
            var opId = command.Name.ToString();
            Operation op;
            if (!_operations.TryGetValue(opId, out op))
                throw new InvalidScriptException("Unknown command '{0}' in test '{1}'", opId, _testName);

            var attr = command.GetAttributes(Evaluate);

            string set = null;
            if (attr.TryGetValue("set", out set))   // czy mamy ustawić wynik w zmiennej
                attr.Remove("set");

            try
            {
                var ret = op.Execute(attr);

                if (ret != null && set != null)
                    _variables[set] = ret;

                var screenShot = _driver.TakeScreenshot();
                var path = Path.Combine(Environment.CurrentDirectory, string.Format("passed_{1:yyyy-MM-dd_HH.mm.ss}_{0}.png", _testName, DateTime.Now));
                screenShot.SaveAsFile(path, ScreenshotImageFormat.Png);
            }
            catch (Exception)
            {
                var screenShot = _driver.TakeScreenshot();
                var path = Path.Combine(Environment.CurrentDirectory, string.Format("failed_{1:yyyy-MM-dd_HH.mm.ss}_{0}.png", _testName, DateTime.Now));
                screenShot.SaveAsFile(path, ScreenshotImageFormat.Png);
                throw;
            }
        }

        private string Evaluate(string input)
        {
            return Regex.Replace(input, "\\${[^${}]+}", m =>
            {
                var key = m.Value.Substring(2, m.Value.Length - 3);
                string v;
                return _variables.TryGetValue(key, out v) ? v : "";
            });
        }

    }
}
