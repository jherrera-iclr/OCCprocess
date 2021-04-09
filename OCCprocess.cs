using System;
using System.Reflection;
using inteliclear.v1;
using inteliclear.icLoggerNS.v1;
using System.Configuration;
using System.IO;

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

                string configFile = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\OCCprocess.dll.config";
                string logName = configData.LogDir + ConfigurationManager.AppSettings["LogFilename"];

                logger.InitLogging(logName, configFile);
                logger.LogInfo($"Version: {Assembly.GetExecutingAssembly().GetName().Version}");
                logger.LogInfo($"Parameters: {string.Join(" ", args)}");

                var argsProcess = new ArgsProcess();

                string sInputFile = string.Empty;
                retVal = argsProcess.GetArgs(args, out sInputFile);

                logger.LogInfo("Process Started.");
                retVal = FileProcess.LoadFile(sInputFile);
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
