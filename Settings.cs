using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace REO
{
    public enum ChatMessageMethod
    {
        PostMessage,
        CopyToClipboard
    }
    public static class Settings
    {
        private static ConcurrentDictionary<string, string> settings = new ConcurrentDictionary<string, string>();
        private static string version = "1";
        static Settings()
        {
            if (File.Exists("Settings.json"))
            {
                var s = File.ReadAllText("Settings.json");
                settings = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(s); 
                if (!settings.ContainsKey("Version") || settings["Version"] != version)
                {
                    settings = new ConcurrentDictionary<string, string>();
                }
            }
            


            if (!settings.ContainsKey("BrowserWidth"))
                SetSetting("BrowserWidth", "1400");
            if (!settings.ContainsKey("BrowserHeight"))
                SetSetting("BrowserHeight", "900");
            if (!settings.ContainsKey("ToggleOverlay"))
                SetSetting("ToggleOverlay", "CommandOrControl+Shift+X");// keys can be found here: https://www.electronjs.org/docs/api/accelerator#platform-notice
            if (!settings.ContainsKey("RealmEyeStyle"))
                SetSetting("RealmEyeStyle", "default");
            if (!settings.ContainsKey("SettingsStyle"))
                SetSetting("SettingsStyle", "default");
            if (!settings.ContainsKey("Version"))
                SetSetting("Version", version);
            if (!settings.ContainsKey("ChatMessageMethod"))
                SetSetting("ChatMessageMethod", "CopyToClipboard");
            if (!settings.ContainsKey("ResetPassword"))
                SetSetting("ResetPassword", "CommandOrControl+Shift+R");


        }
        /// <summary>Returns the value of the given key. If the key is not found and default value is set it will add the setting then return the value.
        /// If the key is not found and the default value is not provided it will throw a KeyNotFoundException
        /// </summary>
        ///
        public static string GetSetting(string key, string defaultValue = "")
        {
            if (settings.ContainsKey(key))
                return settings[key];
            if (!defaultValue.Equals(""))
            {
                SetSetting(key, defaultValue);
                return defaultValue;
            }
            throw new KeyNotFoundException();
        }
        public static void SetSetting(string setting,string value)
        {
           settings.AddOrUpdate(setting, value,(key, oldValue) => value);
            SaveSettings(); 
        }
        public static void SetSettings(Dictionary<string,string> keyValuePairs)
        {
            foreach (var setting in keyValuePairs)
            {
                settings.AddOrUpdate(setting.Key, setting.Value, (key, oldValue) => setting.Value);
            }
            SaveSettings();
        }
        private static void SaveSettings()
        {
           File.WriteAllText("Settings.json", JsonConvert.SerializeObject(settings));
        }
    }
}
