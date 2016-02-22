using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.Execution
{
    public static class TestResultUtil
    {

        public static XElement CreateXTestResult(TestResultType resultType, string message, Exception ex, params object[] xcontent)
        {
            XElement xresult = new XElement(XTestResultNames.TestResult
                , new XAttribute(XTestResultNames.ATestResultType, resultType)
                , new XAttribute(XTestResultNames.AExitCode, (int)resultType)
                , new XElement(XTestResultNames.ResultMessage, message ?? resultType.ToString())
                , ex == null ? null : XNames.CreateXError(ex)
                );

            xresult.Add(xcontent);

            return xresult;
        }

        public static XElement CreateErrorXTestResult(XElement xerror)
        {
            if (xerror == null)
                throw new ArgumentNullException();
            if (xerror.Name != XNames.Error)
                throw new ArgumentException(String.Format("Unexpected element {0}. Only {1} elements are accepted.", XNames.Error, xerror.Name));

            return CreateXTestResult(TestResultType.Error, (string)xerror.Element(XConcurrencyNames.ErrorMessage), null, xerror);
        }

        public static XElement CreateErrorXTestResult(String msg)
        {
            return CreateErrorXTestResult(msg, null);
        }

        public static XElement CreateErrorXTestResult(Exception ex)
        {
            return CreateErrorXTestResult(ex.Message, ex);
        }

        public static XElement CreateErrorXTestResult(String msg, Exception ex)
        {
            return CreateXTestResult(TestResultType.Error, msg, ex);
        }

        /// <summary>
        /// Checks to see if a test result file exists and validates it.
        /// </summary>
        /// <param name="resultFilePath"></param>
        /// <returns>
        /// null if the result file doesn't exist; otherwise, a valid test result element.
        /// If the result file is invalid or could not be loaded properly, then an error result
        /// element is returned specifying the information.
        /// </returns>
        public static XElement CheckForResultFile(string resultFilePath)
        {
            if (!File.Exists(resultFilePath))
                return null;

            try
            {
                XDocument xdoc = XDocument.Load(resultFilePath);
                UnitTestingSchemaUtil.ValidateTestResultXml(xdoc);
                XElement xresult = xdoc.Root;
                xresult.Remove();

                return xresult;
            }
            catch (IOException ex)
            {
                // Sometimes, the file exists but is still being written and when we try to
                // open it, we get an IOException. Try to detect this specific case and then
                // return null so it can be looked for later.
                if (ex.Message.StartsWith("The process cannot access the file ", StringComparison.OrdinalIgnoreCase)
                    && ex.Message.EndsWith(" because it is being used by another process.", StringComparison.OrdinalIgnoreCase)
                    )
                {
                    //Console.WriteLine("Exception caught and ignored: {0}", ex.Message);
                    return null;
                }
                else // Otherwise, just report an error
                    return TestResultUtil.CreateErrorXTestResult(ex);
            }
            catch (System.Xml.Schema.XmlSchemaValidationException ex)
            {
                return TestResultUtil.CreateErrorXTestResult("Invalid test result xml: " + ex.Message);
            }
            catch (System.Xml.XmlException ex)
            {
                return TestResultUtil.CreateErrorXTestResult("Invalid test result xml: " + ex.Message);
            }
            catch (Exception ex)
            {
                return TestResultUtil.CreateErrorXTestResult(ex);
            }
        }

    }
}
