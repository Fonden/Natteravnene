/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;

namespace NR.Infrastructure
{
    /// <summary>
    /// Writing to a log file
    /// </summary>
    public class LogFile
    {
        public static void Write(string message)
        {
            WriteToFile(DateTime.Now.ToString() + " > > > " + message + "\r\n");
        }

        public static void WriteError(string message)
        {
            WriteToErrorFile(DateTime.Now.ToString() + " > > > " + message + "\r\n");
        }

        public static void Write(Exception e, string message)
        {
            string Message = "-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*" + DateTime.Now.ToString() + " *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\r\n";
            Message += e.ToString() + "\r\n";
            if (!string.IsNullOrWhiteSpace(message)) Message += "---------------\r\n" + message + "\r\n";

            Message += "-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-* " + DateTime.Now.ToString() + " *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\r\n";
            WriteToErrorFile(Message);
        }

        private static void WriteToFile(string message)
        {
            int NumTry = 0;
            while (true)
            {
                try
                {
                    String LogFilePath = HostingEnvironment.MapPath(@"~/log/SystemLog.txt");

                    if (Directory.Exists(Path.GetDirectoryName(LogFilePath)) == false)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
                    }
                    File.AppendAllText(LogFilePath, message);
                    break;
                }
                catch (IOException)
                {
                    // check whether it's a lock problem
                    Thread.Sleep(100);
                }
                NumTry++;
                if (NumTry >= 10) break;
            }

        }


        private static void WriteToErrorFile(string message)
        {
            int NumTry = 0;
            while (true)
            {
                try
                {
                    String LogFilePath = HostingEnvironment.MapPath(@"~/log/ErrorLog.txt");

                    if (Directory.Exists(Path.GetDirectoryName(LogFilePath)) == false)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
                    }
                    File.AppendAllText(LogFilePath, message);
                    break;
                }
                catch (IOException)
                {
                    // check whether it's a lock problem
                    Thread.Sleep(100);
                }
                NumTry++;
                if (NumTry >= 10) break;
            }

        }

    }
}