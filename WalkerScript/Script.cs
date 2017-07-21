using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WalkerInterfaces;
using WalkerScript.Exceptions;
using WalkerScript.Operations;

namespace WalkerScript
{
    public class Script
    {
        public List<Test> Tests;
        public Dictionary<string, string> GlobalVariables { get; private set; }

        public string BaseUrl { get; private set; }
        public TimeSpan Timeout { get; private set; }

        public Script(string fileName, Dictionary<string, string> globalVariables)
        {
            GlobalVariables = globalVariables ?? new Dictionary<string, string>();

            var root = LoadScript(fileName);
            BaseUrl = root.GetAttribute("url");
            var timeoutStr = root.GetAttribute("timeout", false);
            Timeout = new TimeSpan(0, 0, 5);
            if (!string.IsNullOrEmpty(timeoutStr))
                Timeout = TimeSpan.Parse(timeoutStr);

            var testScripts = root.Descendants("Test");
            Tests = new List<Test>();

            foreach (var test in testScripts)
            {
                Tests.Add(new Test(test, GlobalVariables, this));
            }
        }

        public IWebDriver CreateDriver(string driver)
        {
            if (!String.IsNullOrEmpty(driver))
            {
                if (String.Compare(driver, "chrome", true) == 0)
                    return new ChromeDriver();
                if (String.Compare(driver, "ff", true) == 0)
                {
                    FirefoxProfile profile = new FirefoxProfile();
                    profile.SetPreference("network.automatic-ntlm-auth.trusted-uris", BaseUrl);
                    return new FirefoxDriver(profile);
                }
                if (String.Compare(driver, "ie", true) == 0)
                    return new InternetExplorerDriver();
            }
            return new ChromeDriver();
        }

        public object[] CreateOperations(IWebDriver driver)
        {
            var type = ScriptHelper.LoadType(ConfigurationManager.AppSettings["Walker"]);
            if (type == null)
                throw new NotSupportedException(string.Format("Unknown walker '{0}'", ConfigurationManager.AppSettings["Walker"]));

            return new object[]
            {
                new ScriptOperations(),
                (IBusinessApplicationWalker)Activator.CreateInstance(type, driver != null ? new object[] { driver, BaseUrl, Timeout } : new object[] {})
            };
        }
  
        XElement LoadScript(string fileName)
        {
            try
            {
                using (var stream = new FileStream(fileName, FileMode.Open))
                {
                    var doc = XDocument.Load(stream);
                    if (doc.Root == null)
                        throw new Exception("Missing root element <Tests />");
                    return doc.Root;
                }
            }
            catch (Exception e)
            {
                throw new InvalidScriptException(e, "Can't load script file '{0}' - {1}", fileName, e.Message);
            }
        }


    }
}
