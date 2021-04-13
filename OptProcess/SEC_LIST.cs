using System;
using System.Collections.Generic;
using System.Text;
using inteliclear.v1;
using inteliclear.icLoggerNS.v1;
using System.IO;
using System.Xml.Linq;
using System.Linq;

namespace OCCprocess
{
    class SEC_LIST
    {

        private static icLogger logger = icLogger.Instance;
        private static bool emptyFlag = true;

        public SEC_LIST() { }

        public static int LoadFile(string sInputFile)
        {
            int retVal = 0;
            try
            {
                var configData = icGeneric.GetConfigData();
                string sInputFilePath = configData.LoadDir + sInputFile + ".xml";
                logger.LogInfo($"ParseMsg(): Loading: " + sInputFilePath);
                if (!File.Exists(sInputFilePath))
                {
                    logger.LogError(sInputFile + ": Input File " + sInputFilePath + " Does not exist");
                    return -4;
                }

                if (new FileInfo(sInputFilePath).Length == 0)
                {
                    logger.LogWarning("Warning: empty file.");
                    return 0;
                }

                XDocument oldXml = XDocument.Load(sInputFilePath);
                XElement firstChild = oldXml.Root.Elements().First();
                XDocument newXml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                                                 firstChild);

                XElement current = (XElement)newXml.FirstNode;
                current = (XElement)current.FirstNode;
                int lineNumber = 3;

                List<SecurityList> SecLists = new List<SecurityList>();

                while (current != null)
                {
                    if (current.Name.LocalName == "SecList")
                    {
                        ParseXML(current, SecLists, lineNumber.ToString());
                    }
                    current = (XElement)current.NextNode;
                    lineNumber += 1;
                }
                if (SecLists.Count == 0)
                {
                    logger.LogWarning("No options were added (no valid options or empty file).");
                }
                else
                {
                    foreach (var secList in SecLists)
                    {
                        ProcessSecList(secList);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LoadFile()");
                return -1;
            }
            return retVal;
        }

        public static void ParseXML(XElement SecList, List<SecurityList> SecLists, string lineNumber)
        {
            string RptID, CFI, underSym, Symbol;
            DateTime createDt, inactiveDt, MatDt;
            

        }

        public static void ProcessSecList(SecurityList secList)
        {
            CommandClass command = new CommandClass();
            // Call insert SP
        }
    }
}
