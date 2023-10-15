using System.Configuration;
using System.Windows;

namespace Project12;

public partial class App : Application
{
    
    public static void SaveConfig(string key, string value)
    {
        try
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;
            if (settings[key] == null)
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }
        catch
        {
            // ignored
        }
    }

    public static string? LoadConfig(string key)
    {
        try
        {
            return ConfigurationManager.AppSettings[key];
        }
        catch
        {
            // ignored
        }

        return null;
    }
}