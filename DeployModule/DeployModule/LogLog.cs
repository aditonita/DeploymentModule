using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;

namespace deploy_v_1
{
    internal class LogLog
    {
        public static readonly Logger logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(ConfigurationManager.AppSettings["LogFile"] ?? "Refresh.log")
            .CreateLogger();
        public static void DeleteLogFile()
        {
            try
            {
                File.Delete(ConfigurationManager.AppSettings["LogFile"] ?? "Refresh.log");
            }catch (Exception e)
            {
                MessageBox.Show("ERROR - " + e.Message + "\nApplication will close");
                Environment.Exit(1);
            }
        }
    }
}
