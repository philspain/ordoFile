using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace ordoFile.DataAccess
{
    public class Logger
    {
        // Path for file to log errors
        string errorFile = "error-log.txt";

        public Logger()
        {
            CheckFileExists();
        }

        /// <summary>
        /// Attempt to append error message to log file.
        /// </summary>
        /// <param name="error">Error message to be added.</param>
        public void LogError(string error)
        {
            CheckFileExists();

            using (FileStream fs = 
                File.Open(errorFile, FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    string message = "------ Begin Error ------" +
                        "\n" + error + "\n" +
                        "------ End Error ------";

                    sw.Write(message);
                }
            }
        }

        /// <summary>
        /// Check if log file exists, create if it doesn't
        /// </summary>
        void CheckFileExists()
        {
            if (!File.Exists(errorFile))
            {
                try
                {
                    FileStream fs = File.Create(errorFile);
                    fs.Close();
                }
                catch { }
            }
        }
    }
}
