using System;

namespace LS.EPiServerNamespaceMigration
{
    public static class LoggerHelper
    {
        /// <summary>
        /// Logs the specified log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static string Log(string log)
        {
            Console.WriteLine(log);
            // Add log4net here

            return log;
        }

        /// <summary>
        /// Logs the specified log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static string Log(object log)
        {
            if (log != null)
            {
                Console.WriteLine("{0} - {1}",DateTime.Now.ToShortTimeString(),log);
                // Add log4net here

                return log.ToString();
            }
            else
            {
                return string.Empty;
            }
            
        }
    }
}
