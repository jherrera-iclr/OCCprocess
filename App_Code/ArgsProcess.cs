using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;
using inteliclear.icLoggerNS.v1;


namespace OCCprocess
{

    class ArgsProcess
    {
        public ArgsProcess(){}
        private static icLogger logger = icLogger.Instance;

        public int GetArgs(string[] args, out string sArgs)
        {
            sArgs = string.Empty;
            try
            {
                if (args.Length != 1)
                {
                    PrintUsage();
                    return -9;
                }
                const string cnst_IPType = "|SEC_UPDATE|SEC_LIST|";
                if (cnst_IPType.IndexOf("|" + args[0].ToUpper() + "|") == -1)
                {
                    logger.LogError("OccProcess: Unknown Parameter(s)");
                    PrintUsage();
                    return -9;
                }
                sArgs = args[0].ToUpper();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message.ToString());
                return -1;
            }

            return 0;
        }

        public int GetFileEntities(string sIpType, out FileEntity objITEntity)
        {
            objITEntity = new FileEntity();
            try
            {
                objITEntity.defaultFileName = sIpType + ".txt";
                objITEntity.inputFileType = "TXT";
                objITEntity.fileContentType = "XML";
                objITEntity.outputFileName = sIpType;
                objITEntity.outputFileType = "CSV";
                switch (sIpType)
                {
                    case "SEC_UPDATE":
                        objITEntity.ID = "Update";
                        break;
                    case "SEC_LIST":
                        objITEntity.ID = "List";
                        break;
                    default:
                        objITEntity = new FileEntity();
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return -1;
            }
            return 0;
        }

        public void PrintUsage()
        {
            logger.LogError("Usage  : OCCprocess <SEC_UPDATE, SEC_LIST>");
        }
    }
}
