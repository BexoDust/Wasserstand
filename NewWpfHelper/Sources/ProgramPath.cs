using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace NGMP.WPF
{
    public static class ProgramPath
    {
        public static Boolean IsUnitTest = 
               (Process.GetCurrentProcess().ProcessName == "VSTestHost")
            || (Process.GetCurrentProcess().ProcessName == "vstest.executionengine.x86")
            || (Process.GetCurrentProcess().ProcessName == "QTAgent32");

        public static string GetPath
        {
            get
            {
                /*
                if (IsUnitTest)
                    return Environment.CurrentDirectory;
                else
                    return AppDomain.CurrentDomain.BaseDirectory;
                 * */
                
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }
    }
}
