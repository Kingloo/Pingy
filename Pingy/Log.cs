using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pingy
{
    public static class Log
    {
        private static FileInfo logFile = GetLogFile();

        private static FileInfo GetLogFile()
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filename = "logfile.txt";

            string fullPath = Path.Combine(directory, filename);

            return new FileInfo(fullPath);
        }


        public static void LogMessage(string message)
        {
            string text = FormatMessage(message);

            WriteToFile(text);
        }

        public static async Task LogMessageAsync(string message)
        {
            string text = FormatMessage(message);

            await WriteToFileAsync(text).ConfigureAwait(false);
        }


        public static async Task LogExceptionAsync(Exception ex)
            => await LogExceptionAsync(ex, string.Empty, false).ConfigureAwait(false);

        public static async Task LogExceptionAsync(Exception ex, string message)
            => await LogExceptionAsync(ex, message, false).ConfigureAwait(false);

        public static async Task LogExceptionAsync(Exception ex, bool includeStackTrace)
            => await LogExceptionAsync(ex, string.Empty, includeStackTrace).ConfigureAwait(false);

        public static async Task LogExceptionAsync(Exception ex, string message, bool includeStackTrace)
        {
            StringBuilder sb = new StringBuilder();
            
            if (String.IsNullOrWhiteSpace(message))
            {
                sb.AppendLine(string.Format(CultureInfo.CurrentCulture, "{0} - {1}", ex.GetType().FullName, ex.Message));
            }
            else
            {
                sb.AppendLine(string.Format(CultureInfo.CurrentCulture, "{0} - {1} - {2}", ex.GetType().FullName, ex.Message, message));
            }
            
            if (includeStackTrace)
            {
                sb.AppendLine(ex.StackTrace);
            }
            
            await LogMessageAsync(sb.ToString()).ConfigureAwait(false);
        }


        private static string FormatMessage(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss (zzz)", CultureInfo.CurrentCulture);
            string processName = Process.GetCurrentProcess().MainModule.ModuleName;

            return string.Format(CultureInfo.CurrentCulture, "{0} - {1} - {2}", timestamp, processName, message);
        }


        private static void WriteToFile(string text)
        {
            FileStream fs = null;

            try
            {
                fs = new FileStream(
                    logFile.FullName,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.None,
                    4096,
                    FileOptions.None);

                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(text);
                }
            }
            catch (FileNotFoundException) { }
            catch (IOException) { }
            finally
            {
                fs?.Dispose();
            }
        }

        private static async Task WriteToFileAsync(string text)
        {
            FileStream fsAsync = null;

            try
            {
                fsAsync = new FileStream(
                    logFile.FullName,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.None,
                    4096,
                    FileOptions.Asynchronous);

                using (StreamWriter sw = new StreamWriter(fsAsync))
                {
                    await sw.WriteLineAsync(text).ConfigureAwait(false);
                }
            }
            catch (FileNotFoundException) { }
            catch (IOException) { }
            finally
            {
                fsAsync?.Dispose();
            }
        }
    }
}
