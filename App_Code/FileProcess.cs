using System;
using System.Collections.Generic;
using System.Text;
using inteliclear.v1;
using inteliclear.icLoggerNS.v1;
using System.IO;

namespace OCCprocess
{
    class FileProcess
    {
        public FileProcess() { }

        private static icLogger logger = icLogger.Instance;
        public int LoadFile(FileEntity fileEntity)
        {
            int iPMRetVal = 0;
            try
            {
                var configData = icGeneric.GetConfigData();
                logger.LogInfo($"ParseMsg(): Loading: {configData.LoadDir}{fileEntity.defaultFileName}");
                if (!File.Exists(configData.LoadDir + fileEntity.defaultFileName))
                {
                    logger.LogError(fileEntity.ID + ": Input File " + fileEntity.defaultFileName + " Does not exist");
                    return -4;
                }
                if (File.Exists(configData.LoadDir + fileEntity.outputFileName + "." + fileEntity.outputFileType))
                    File.Delete(configData.LoadDir + fileEntity.outputFileName + "." + fileEntity.outputFileType);

                // Load file...
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ParseMsg()");
                return -1;
            }
            return iPMRetVal;
        }
    }
}
