using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using OpenAC.Net.Core.Extensions;

namespace OpenAC.Net.NFSe.Demo;

public class OpenConfig
{
    #region Fields

    private readonly Configuration config;

    #endregion Fields

    #region Constructors

    private OpenConfig(Configuration config)
    {
        this.config = config;
    }

    #endregion Constructors

    #region Methods

    public void Set(string setting, object value)
    {
        var valor = string.Format(CultureInfo.InvariantCulture, "{0}", value);

        if (config.AppSettings.Settings[setting]?.Value != null)
            config.AppSettings.Settings[setting].Value = valor;
        else
            config.AppSettings.Settings.Add(setting, valor);
    }

    public T Get<T>(string setting, T defaultValue)
    {
        var type = typeof(T);
        var value = config.AppSettings.Settings[setting]?.Value;
        if (value.IsEmpty()) return defaultValue;

        try
        {
            if (type.IsEnum || type.IsGenericType && type.GetGenericArguments()[0].IsEnum)
            {
                return (T)Enum.Parse(type, value);
            }

            return (T)Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    public void Save()
    {
        config.Save(ConfigurationSaveMode.Minimal, true);
    }

    public static OpenConfig CreateOrLoad(string fileName = "opennfse.config")
    {
        if (!File.Exists(fileName))
        {
            var config = "<?xml version='1.0' encoding='utf-8' ?>" + Environment.NewLine +
                         "<configuration>" + Environment.NewLine +
                         "    <appSettings>" + Environment.NewLine +
                         "    </appSettings>" + Environment.NewLine +
                         "</configuration>";
            File.WriteAllText(fileName, config);
        }

        var configFileMap = new ExeConfigurationFileMap
        {
            ExeConfigFilename = fileName
        };

        var configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        return new OpenConfig(configuration);
    }

    #endregion Methods
}