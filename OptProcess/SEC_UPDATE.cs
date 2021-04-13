using System;
using System.Collections.Generic;
using System.Text;
using inteliclear.v1;
using inteliclear.icLoggerNS.v1;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

namespace OCCprocess
{
    class SEC_UPDATE
    {
        private static icLogger logger = icLogger.Instance;
        private static bool emptyFlag = true;

        public SEC_UPDATE() { }

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

                while (current != null)
                {
                    if (current.Name.LocalName == "SecListUpd")
                    {
                        string updActn = current.Attribute("UpdActn")?.Value;
                        if (String.IsNullOrEmpty(updActn))
                        {
                            logger.LogError("UpdActn is empty or missing - Line " + lineNumber.ToString());
                        }
                        else if (updActn == "M")
                        {
                            ParseXML(current, lineNumber.ToString());
                        }
                    }
                    current = (XElement)current.NextNode;
                    lineNumber += 1;
                }
                if (emptyFlag)
                {
                    logger.LogWarning("No options were updated (no valid updates or empty file).");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LoadFile()");
                return -1;
            }
            return retVal;
        }

        public static void ParseXML(XElement SecListUpd, string lineNumber)
        {
            // Required data
            DateTime MatDt_1, MatDt_2;
            XElement instrmt_1, instrmt_2;
            string Sym_1, CFI_1;
            Double StrkPx_1;
            SecurityUpdate secUpd = new SecurityUpdate();

            // RptID
            if (String.IsNullOrEmpty(SecListUpd.Attribute("RptID")?.Value))
            {
                logger.LogError("RptID is empty or missing. - Line " + lineNumber);
                return;
            }
            secUpd.RptID = SecListUpd.Attribute("RptID").Value;

            // Instrmt 1
            instrmt_1 = (XElement)SecListUpd.FirstNode;
            instrmt_1 = (XElement)instrmt_1.FirstNode;
            if(instrmt_1 == null || instrmt_1.Name.LocalName != "Instrmt") 
            {
                logger.LogError("Invalid SecListUpd format. Instrmt block missing. - Line " + lineNumber);
                return; 
            }

            // Instrmt 2
            instrmt_2 = (XElement)instrmt_1.NextNode;
            if (instrmt_2 == null || instrmt_2.Name.LocalName != "Instrmt")
            {
                logger.LogError("Invalid SecListUpd format. Instrmt block missing. - Line " + lineNumber);
                return;
            }

            // MatDt 1 (Old expiry date)
            if (!DateTime.TryParse(instrmt_1.Attribute("MatDt")?.Value, out MatDt_1)) 
            {
                logger.LogError("Invalid MatDt in Instrmt block 1. - Line " + lineNumber);
                return; 
            }

            // MatDt 2 (New expiry date)
            if (!DateTime.TryParse(instrmt_2.Attribute("MatDt")?.Value, out MatDt_2))
            {
                logger.LogError("Invalid MatDt in Instrmt block 2. - Line " + lineNumber);
                return;
            }

            if (MatDt_1 == MatDt_2) { return; } // Check if MatDt does not change after update

            // Sym 1
            Sym_1 = instrmt_1.Attribute("Sym")?.Value;
            if (String.IsNullOrEmpty(Sym_1))
            {
                logger.LogError("Missing Sym in Instrmt block 1. - Line " + lineNumber);
                return;
            }
            
            // StrkPx 1
            if (!Double.TryParse(instrmt_1.Attribute("StrkPx")?.Value, out StrkPx_1))
            {
                logger.LogError("Invalid StrkPx (\""+ instrmt_1.Attribute("StrkPx")?.Value + "\") in Instrmt block 1. - Line " + lineNumber);
                return;
            }

            // CFI 1
            CFI_1 = instrmt_1.Attribute("CFI")?.Value;
            if (String.IsNullOrEmpty(CFI_1))
            {
                logger.LogError("Missing CFI in Instrmt block 1. - Line " + lineNumber);
                return;
            }
            if (CFI_1[1] == 'C')
            {
                secUpd.Symbol = Sym_1 + "\t" + MatDt_1.ToString("yyMMdd") + "C" + StrkPx_1.ToString("00000.000").Replace(".", "");
            }
            else if (CFI_1[1] == 'P')
            {
                secUpd.Symbol = Sym_1 + "\t" + MatDt_1.ToString("yyMMdd") + "P" + StrkPx_1.ToString("00000.000").Replace(".", "");
            }
            else
            {
                logger.LogError("Invalid CFI in Instrmt block 1. - Line " + lineNumber);
                return;
            }

            ProcessSecUpdate(secUpd);
        }

        public static void ProcessSecUpdate(SecurityUpdate secUpd)
        {
            emptyFlag = false;
            CommandClass command = new CommandClass();
            int retVal = command.UpdateOptionExpDt(secUpd.Symbol, secUpd.MatDt.ToString("MM/dd/yyyy"));
            if (retVal == -2)
            {
                logger.LogError("Symbol not found: \"" + secUpd.Symbol + "\". Update failed for RptID " + secUpd.RptID);
            }
        }
    }
}
