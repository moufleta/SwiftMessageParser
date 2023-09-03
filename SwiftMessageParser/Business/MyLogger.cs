using NLog;
using SwiftMessageParser.Business.Contracts;

namespace SwiftMessageParser.Business
{
    public class MyLogger : IMyLogger
    {
        private static Logger logger;
        private static MyLogger instance;

        private MyLogger()
        {
        }

        public static MyLogger GetInstance()
        {
            if (instance == null)
            {
                instance = new MyLogger();
            }

            return instance;
        }

        public void Debug(string message, string argument = null)
        {
            if (argument == null)
            {
                GetLogger("myAppLoggerRules").Debug(message);
            }
            else
            {
                GetLogger("myAppLoggerRules").Debug(message, argument);
            }
        }

        public void Error(string message, string argument = null)
        {
            if (argument == null)
            {
                GetLogger("myAppLoggerRules").Error(message);
            }
            else
            {
                GetLogger("myAppLoggerRules").Error(message, argument);
            }
        }

        public void Info(string message, string argument = null)
        {
            if (argument == null)
            {
                GetLogger("myAppLoggerRules").Info(message);
            }
            else
            {
                GetLogger("myAppLoggerRules").Info(message, argument);
            }
        }

        public void Warn(string message, string argument = null)
        {
            if (argument == null)
            {
                GetLogger("myAppLoggerRules").Warn(message);
            }
            else
            {
                GetLogger("myAppLoggerRules").Warn(message, argument);
            }
        }

        private Logger GetLogger(string theLogger)
        {
            if (logger == null)
            {
                logger = LogManager.GetLogger(theLogger);
            }

            return logger;
        }
    }
}