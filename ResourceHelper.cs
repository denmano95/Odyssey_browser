using System;
using System.IO;
using System.Reflection;

namespace Odyssey
{
    public static class ResourceHelper
    {
        /// <summary>
        /// Get absolute path to resource, works for dev and for published builds
        /// </summary>
        public static string GetResourcePath(string relativePath)
        {
            try
            {
                // Try to get the directory where the executable is located
                string? baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                
                if (string.IsNullOrEmpty(baseDirectory))
                {
                    baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                }

                return Path.Combine(baseDirectory, relativePath);
            }
            catch
            {
                // Fallback to current directory
                return Path.Combine(Directory.GetCurrentDirectory(), relativePath);
            }
        }
    }
}
