using System;
using System.Collections.Generic;
using System.Text;
using inteliclear.v1;
using inteliclear.icLoggerNS.v1;
using System.IO;
using System.Configuration;
using System.Xml.Linq;
using System.Linq;

namespace OCCprocess
{
    class FileProcess
    {
        public FileProcess() { }

        private static icLogger logger = icLogger.Instance;
        private static bool emptyFlag = true;

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

                if(new FileInfo(sInputFilePath).Length == 0)
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
                while(current != null)
                {
                    string updActn = current.Attribute("UpdActn").Value;
                    if (current.Name.LocalName == "SecListUpd" && updActn == "M")
                    {
                        ProcessSecUpdate(current);
                    }
                    current = (XElement)current.NextNode;
                }
                if (emptyFlag)
                {
                    logger.LogWarning("Warning: No options were updated (no valid updates or empty file).");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LoadFile()");
                return -1;
            }
            return retVal;
        }

        public static void ProcessSecUpdate(XElement SecListUpd)
        {   


            XElement instrmt_1 = (XElement)SecListUpd.FirstNode;
            instrmt_1 = (XElement)instrmt_1.FirstNode;

            XElement instrmt_2 = (XElement)instrmt_1.NextNode;

            DateTime MatDt_1 = DateTime.Parse(instrmt_1.Attribute("MatDt").Value); // Old expiry date
            DateTime MatDt_2 = DateTime.Parse(instrmt_2.Attribute("MatDt").Value); // New expiry date

            if (MatDt_1 == MatDt_2) { return; } // Check if MatDt changes after update

            string Sym_1 = instrmt_1.Attribute("Sym").Value;
            string CFI_1 = instrmt_1.Attribute("CFI").Value;
            Double StrkPx_1 = Double.Parse(instrmt_1.Attribute("StrkPx").Value);
            string symbol;

            if (CFI_1.Contains("C"))
            {
                symbol = Sym_1 + "\t" + MatDt_1.ToString("yyMMdd") + "C" + StrkPx_1.ToString("00000.000").Replace(".", "");
            }
            else if (CFI_1.Contains("P"))
            {
                symbol = Sym_1 + "\t" + MatDt_1.ToString("yyMMdd") + "P" + StrkPx_1.ToString("00000.000").Replace(".", "");
            }
            else
            {
                logger.LogError("Invalid CFI: " + CFI_1);
                return;
            }

            emptyFlag = false;

            CommandClass command = new CommandClass();
            int retVal = command.UpdateOptionExpDt(symbol, MatDt_2.ToString("MM/dd/yyyy"));
            if(retVal == -2)
            {
                logger.LogError("Symbol not found: " + symbol + "\n Update failed.");
            }
            else if(retVal==0)
            {
                logger.LogInfo("Option Updated. Symbol: " + symbol);
            }
            
        }
    }
}
