using System;
using System.Runtime.CompilerServices;

namespace BILogger
{
    public interface ILogger
    {
        string LogFile { get; }

        void checkDirectory();

        void Error(Exception errorData, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);


        void Message(string messageData, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);


        void Log(string str);

    }
}

