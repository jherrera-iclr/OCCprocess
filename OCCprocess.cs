using System;
using System.Collections.Generic;
using System.Reflection;
using inteliclear.v1;
using inteliclear.icLoggerNS.v1;
using System.Configuration;

namespace OCCprocess
{
    class OCCprocess
    {
        private static icLogger logger = icLogger.Instance;

        public static int Main(string[] args)
        {
            int retVal = 0;

            try
            {
                var configData = icGeneric.GetConfigData();

                logger.InitLogging(configData.LogDir + ConfigurationManager.AppSettings["LogFilename"]);
                logger.LogInfo($"Version: {Assembly.GetExecutingAssembly().GetName().Version}");
                logger.LogInfo($"Parameters: {string.Join(" ", args)}");

                var argsProcess = new ArgsProcess();

                string sInputFile = string.Empty;
                retVal = argsProcess.GetArgs(args, out sInputFile);

                var objITEntity = new FileEntity();
                retVal = argsProcess.GetFileEntities(sInputFile, out objITEntity);

                logger.LogInfo("Process Started.");
                retVal = FileProcess.LoadFile(objITEntity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                logger.LogError(ex, "Main()");
            }

            if (retVal == 0)
                logger.LogInfo("Process Ended.");
            else
                logger.LogError("Process Ended with errors.");

            return retVal;
        }
    }
}
