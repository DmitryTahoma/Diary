using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Test
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        public void LoggerIsWritesLogs()
        {
            Logger logger = new Logger();
            logger.Log("Start logging");
            string lastLog = logger.Logs.LastByTime().Text;

            Assert.AreEqual("Start logging", lastLog);
        }
    }
}
