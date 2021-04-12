﻿using System;
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

                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LoadFile()");
                return -1;
            }
            return retVal;
        }
    }
}
