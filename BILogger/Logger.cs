using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace BILogger
{
    public class Logger : ILogger
    {
        public Logger()
        {
            checkDirectory();
        }
        public string LogFile
        {
            get
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path = Path.Combine(path, "BookInventoryLog\\" + DateTime.Today.ToString("dd-MM-yyyy") + "Log.txt");
                return path;
            }
        }
        public void checkDirectory()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, "BookInventoryLog");
            if (!Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }



        public void Error(Exception errorData, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                var file = Path.GetFileName(sourceFilePath);
                var str = string.Format("{0}: Error {1} : {2} :{3}\n", DateTime.Now.ToShortTimeString(), errorData, file, sourceLineNumber);
                Log(str);
            }
            catch (Exception)
            {
            }
        }

        public void Message(string messageData, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                var file = Path.GetFileName(sourceFilePath);
                var str = string.Format("{0}: {1} {2} :{3}\n", DateTime.Now.ToShortTimeString(), file, sourceLineNumber, messageData);
                Log(str);
            }
            catch (Exception)
            {
            }
        }


        public void Log(string str)
        {
            File.AppendAllText(LogFile, str);
        }
    }
}
