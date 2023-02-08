using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtcSync.Helper
{
    public static class LogServices
    {
        private static string fileName = "Log.txt";
        /// <summary>
        /// This method will log things to service's folder
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="text"></param>
        /// <param name="filePath"></param>
        /// <param name="date"></param>        
        public static void LogText(string methodName, string text, string filePath, DateTime date)
        {
            string filePathLog = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), filePath);
            File.AppendAllText(filePathLog, methodName + " __ " + date.ToString("yyyy-MM-dd HH:mm:ss:fff") + "__" + text + Environment.NewLine);
        }

        public static string GetPath(string fileName)
        {
            return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), fileName);
        }
        public static void LogText(string methodName, string text)
        {
            string filePathLog = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), fileName);
            File.AppendAllText(filePathLog, methodName + " __ " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss:fff") + "__" + text + Environment.NewLine);
        }

    }

}
