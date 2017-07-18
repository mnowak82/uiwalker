using System;
using System.Globalization;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using WalkerScript.Exceptions;

namespace WalkerScript.Operations
{
    public class ScriptOperations
    {
        private readonly IWebDriver _driver;
        private readonly string _testName;

        public ScriptOperations()
        {
        }

        public ScriptOperations(IWebDriver driver, string testName)
        {
            _driver = driver;
            _testName = testName;
        }
        /// <summary>
        /// wstrzymuje wykonanie skryptu na n sekund
        /// </summary>
        /// <param name="seconds">czas w sekundach, przez który wstrzymane jest wykonanie skryptu</param>
        public void Sleep(int seconds)
        {
            Thread.Sleep(seconds * 1000);
        }

        /// <summary>
        /// test na poprawność danych. Wszystkie aktywne kryteria muszą być spełnione
        /// </summary>
        /// <param name="value">testowana wartość</param>
        /// <param name="eq">wartość ma być równa (null gdy nie używane)</param>
        /// <param name="ne">wartość ma być różna (null gdy nie używane)</param>
        /// <param name="lt">wartość ma być mniejsza niż (null gdy nie używane), gdy value i lt dają się parsować do liczby całkowitej, porównywanie jako liczby</param>
        /// <param name="gt">wartość ma być większa niż (null gdy nie używane), gdy value i gd dają się parsować do liczby całkowitej, porównywanie jako liczby</param>
        public void Assert(string value, string eq = null, string ne = null, string lt = null, string gt = null)
        {
            if (eq != null && value != eq)
                throw new AssertException(value, "should be equal to", eq);

            if (ne != null && value == ne)
                throw new AssertException(value, "should be NOT equal to", ne);

            CompareValues(value, lt, "should be less than", "should be before");
            CompareValues(value, gt, "should be greather than", "should be after", true);
        }

        private void CompareValues(string va, string vb, string assertMessageForIntegers, string assertMessage, bool negate = false)
        {
            if (vb != null)
            {
                int a, b;
                if (int.TryParse(va, out a) && int.TryParse(vb, out b))
                {
                    if (a > b != negate || a == b)
                        throw new AssertException(va, assertMessageForIntegers, vb);
                }
                else
                {
                    var v = string.Compare(va, vb, CultureInfo.CurrentUICulture, CompareOptions.None);
                    if (v > 0 != negate || v == 0)
                        throw new AssertException(va, assertMessage, vb);    
                }
            }
        }

        /// <summary>
        /// ustawia wartość (w połączeniu z set)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Var(string value)
        {
            return value;
        }

        public void Screenshot(string filename = null)
        {
            var screenShot =_driver.TakeScreenshot();

            var path = Path.Combine(Environment.CurrentDirectory, string.Format("progress_{1:yyyy-MM-dd_HH.MM.ss}_{0}.png", _testName, DateTime.Now));
            if (!string.IsNullOrEmpty(filename))
            {
                path = filename.EndsWith(".png") ? filename : (filename + ".png");
            }

            screenShot.SaveAsFile(path, ScreenshotImageFormat.Png);
        }

        public string Date(string format)
        {
            if (string.IsNullOrEmpty(format))
                format = "yyyy-MM-dd hh:mm:ss";
            return DateTime.Now.ToString(format);
        }

        public string Op(string value, string add = null, string subtract = null)
        {
            DateTime dt;
            
            if (DateTime.TryParse(value, out dt))
            {
                DateTime ret = dt;
                TimeSpan ts;
                bool parsed = false;
                if (add != null && TimeSpan.TryParse(add, out ts))
                {
                    ret = ret.Add(ts);
                    parsed = true;
                }

                if (subtract != null && TimeSpan.TryParse(subtract, out ts))
                {
                    ret = ret.Subtract(ts);
                    parsed = true;
                }

                if (parsed)
                    return ret.ToString("yyyy-MM-dd hh:mm:ss");
            }

            int v;
            if (int.TryParse(value, out v))
            {
                int ret = v;
                int o;
                bool parsed = false;
                if (add != null && int.TryParse(add, out o))
                {
                    ret = ret + o;
                    parsed = true;
                }

                if (subtract != null && int.TryParse(subtract, out o))
                {
                    ret = ret - o;
                    parsed = true;
                }

                if (parsed)
                    return ret.ToString();
            }

            var r = value;
            if (add != null)
                r = r + add;
            if (subtract != null)
                r = r + "-" + subtract;
            return r;
        }
    }
}
