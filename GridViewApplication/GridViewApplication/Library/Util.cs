using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace GridViewApplication.Library
{
    public class Util
    {
        private static readonly object _locker = new object();
        private static string _logPath = HttpRuntime.AppDomainAppPath + "Log\\";
        private static string _logExt = "log";
        public static void CreateLog(string LogEntry)
        {
            try
            {
                string PageName = "";
                try
                {
                    if (HttpContext.Current != null)
                    {
                        System.Web.UI.Page page = HttpContext.Current.Handler as System.Web.UI.Page;
                        PageName = System.IO.Path.GetFileName(page.Request.Path);
                    }
                }
                catch { }

                if (!Directory.Exists(_logPath))
                {
                    Directory.CreateDirectory(_logPath);
                }

                string BasePath = _logPath;
                string FilePath = BasePath + "\\" + string.Format("{0:yyyy-MM-dd}", DateTime.Now) + "." + _logExt;
                LogEntry = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + " : " + PageName + "," + LogEntry + System.Environment.NewLine;
                lock (_locker)
                {
                    File.AppendAllText(FilePath, LogEntry);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static string getDetail(Exception ex)
        {
            try
            {
                //Get a StackTrace object for the exception
                StackTrace st = new StackTrace(ex, true);

                //Get the first stack frame
                //StackFrame frame = st.GetFrame(0);
                //Get stack frame - sebelum error - dapat berguna jika error null refference!
                StackFrame frame = st.GetFrame(st.FrameCount - 1);
                //Get the file name
                string fileName = frame.GetFileName();
                //Get the method name
                string methodName = frame.GetMethod().Name;
                //Get the line number from the stack frame
                int line = frame.GetFileLineNumber();
                //Get the column number
                int col = frame.GetFileColumnNumber();

                string str = string.Format("{0} - {1} - line:{2}  - col:{3} - {4}", fileName, methodName, line, col, ex.Message);
                return str;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}