using System.IO;
using Newtonsoft.Json;

namespace ExecutableProject
{
    public class SettingsManager
    {
        private string SETTINGS_NAME = "settings.json";
        
        public Settings Settings { get; }
        
        public SettingsManager()
        {
            Settings = GetSettings(SETTINGS_NAME);
        }
        
        private Settings GetSettings(string filepath)
        {
            if (!File.Exists(filepath))
            {
                var settings = new Settings();
                File.WriteAllText(filepath, JsonConvert.SerializeObject(settings));
                return settings;
            }
            
            var readToEnd = string.Empty;
            using (var r = new StreamReader(filepath))
            {
                readToEnd = r.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<Settings>(readToEnd);

        }
    }
}