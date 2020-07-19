using System;
using System.Runtime.CompilerServices;

namespace BILogger
{
    public interface ILogger
    {
      
        void Error(Exception errorData, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);


        void Message(string messageData, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);

    }
}

