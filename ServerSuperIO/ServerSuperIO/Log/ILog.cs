using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Log
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        bool IsErrorEnabled { get; }

        bool IsFatalEnabled { get; }

        bool IsInfoEnabled { get; }

        bool IsWarnEnabled { get; }

        void Debug(bool isWriteFile,object message);

        void Debug(bool isWriteFile, object message, Exception exception);

        void DebugFormat(bool isWriteFile, string format, object arg0);

        void DebugFormat(bool isWriteFile, string format, params object[] args);

        void DebugFormat(bool isWriteFile, IFormatProvider provider, string format, params object[] args);

        void DebugFormat(bool isWriteFile, string format, object arg0, object arg1);

        void DebugFormat(bool isWriteFile, string format, object arg0, object arg1, object arg2);

        void Error(bool isWriteFile,object message);

        void Error(bool isWriteFile, object message, Exception exception);

        void ErrorFormat(bool isWriteFile, string format, object arg0);

        void ErrorFormat(bool isWriteFile, string format, params object[] args);

        void ErrorFormat(bool isWriteFile, IFormatProvider provider, string format, params object[] args);

        void ErrorFormat(bool isWriteFile, string format, object arg0, object arg1);

        void ErrorFormat(bool isWriteFile, string format, object arg0, object arg1, object arg2);

        void Fatal(bool isWriteFile, object message);

        void Fatal(bool isWriteFile, object message, Exception exception);

        void FatalFormat(bool isWriteFile, string format, object arg0);

        void FatalFormat(bool isWriteFile, string format, params object[] args);

        void FatalFormat(bool isWriteFile, IFormatProvider provider, string format, params object[] args);

        void FatalFormat(bool isWriteFile, string format, object arg0, object arg1);

        void FatalFormat(bool isWriteFile, string format, object arg0, object arg1, object arg2);

        void Info(bool isWriteFile, object message);

        void Info(bool isWriteFile, object message, Exception exception);

        void InfoFormat(bool isWriteFile, string format, object arg0);

        void InfoFormat(bool isWriteFile, string format, params object[] args);

        void InfoFormat(bool isWriteFile, IFormatProvider provider, string format, params object[] args);

        void InfoFormat(bool isWriteFile, string format, object arg0, object arg1);

        void InfoFormat(bool isWriteFile, string format, object arg0, object arg1, object arg2);

        void Warn(bool isWriteFile, object message);

        void Warn(bool isWriteFile, object message, Exception exception);

        void WarnFormat(bool isWriteFile, string format, object arg0);

        void WarnFormat(bool isWriteFile, string format, params object[] args);

        void WarnFormat(bool isWriteFile, IFormatProvider provider, string format, params object[] args);

        void WarnFormat(bool isWriteFile, string format, object arg0, object arg1);

        void WarnFormat(bool isWriteFile, string format, object arg0, object arg1, object arg2);
    }
}
