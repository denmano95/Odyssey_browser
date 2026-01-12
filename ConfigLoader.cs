using System;
using System.IO;
using System.Xml.Linq;

namespace Odyssey
{
    public class BrowserConfig
    {
        public bool Fullscreen { get; set; } = false;
        public string StartupUrl { get; set; } = "http://10.35.49.56:9001";
        public bool IgnoreCookies { get; set; } = false;
        public string ButtonOffset { get; set; } = "";
        public string ButtonSize { get; set; } = "";
    }

    public static class ConfigLoader
    {
        private static string ConfigPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");

        public static BrowserConfig LoadConfig()
        {
            var config = new BrowserConfig();

            try
            {
                if (!File.Exists(ConfigPath))
                {
                    CreateDefaultConfig();
                }

                var doc = XDocument.Load(ConfigPath);
                var root = doc.Root;

                if (root != null)
                {
                    config.Fullscreen = GetBoolValue(root, "fullscreen", false);
                    config.StartupUrl = GetStringValue(root, "startup_url", "http://10.35.49.56:9001");
                    config.IgnoreCookies = GetBoolValue(root, "ignore_cookies", false);
                    config.ButtonOffset = GetStringValue(root, "button_offset", "");
                    config.ButtonSize = GetStringValue(root, "button_size", "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config: {ex.Message}");
            }

            return config;
        }

        private static void CreateDefaultConfig()
        {
            string defaultConfig = @"<?xml version=""1.0""?>
<config>
    <fullscreen>true</fullscreen>
    <startup_url>http://10.35.49.56:9001</startup_url>
    <ignore_cookies>true</ignore_cookies>
    <button_offset>1575,17</button_offset>
    <button_size>120,125</button_size>
</config>";

            try 
            {
                File.WriteAllText(ConfigPath, defaultConfig);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating default config: {ex.Message}");
            }
        }

        private static bool GetBoolValue(XElement root, string elementName, bool defaultValue)
        {
            var element = root.Element(elementName);
            if (element != null && bool.TryParse(element.Value, out bool result))
            {
                return result;
            }
            return defaultValue;
        }

        private static string GetStringValue(XElement root, string elementName, string defaultValue)
        {
            var element = root.Element(elementName);
            return element?.Value ?? defaultValue;
        }
    }
}
