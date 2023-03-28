using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VieweD.Helpers.System;

namespace VieweD.engine.common;

public static class PluginSettingsManager
{
    public static Dictionary<string, Dictionary<string, string>> PluginSettings { get; set; } = new Dictionary<string, Dictionary<string, string>>();

    public static void SavePluginSetting()
    {
        var newSettings = new List<string>();
        // Then add loaded fields
        foreach (var pluginSetting in PluginSettings)
        {
            foreach (var pluginSettingsFields in pluginSetting.Value)
            {
                newSettings.Add(pluginSetting.Key + ";" + pluginSettingsFields.Key + ";" +
                                Helper.Base64Encode(pluginSettingsFields.Value));
            }
        }
        Properties.Settings.Default.PluginSettings = string.Join("\n", newSettings);
    }

    public static void LoadPluginSetting()
    {
        var settings = Properties.Settings.Default.PluginSettings.Split('\n').ToList();
        PluginSettings.Clear();
        // First just copy all things not related to this plugin
        foreach (var settingLine in settings)
        {
            if (string.IsNullOrWhiteSpace(settingLine))
                continue;

            var fields = settingLine.Split(';');
            if (fields.Length != 3)
                continue; // error in the format?

            var pluginName = fields[0];
            var fieldName = fields[1];
            var fieldValue = Helper.Base64Decode(fields[2]);

            if (!PluginSettings.TryGetValue(pluginName, out var plugin))
            {
                plugin = new Dictionary<string, string>();
                PluginSettings.Add(pluginName, plugin);
            }
            plugin.Add(fieldName, fieldValue);
        }
    }

    private static Dictionary<string, string> GetPlugin(string pluginName)
    {
        if (PluginSettings.TryGetValue(pluginName, out var plugin)) 
            return plugin;
        plugin = new Dictionary<string, string>();
        PluginSettings.Add(pluginName, plugin);
        return plugin;
    }

    public static string GetString(string pluginName, string key, string defaultValue = "")
    {
        if (GetPlugin(pluginName).TryGetValue(key, out var val))
            return val;
        return defaultValue;
    }

    public static void SetString(string pluginName, string key, string value)
    {
        var plugin = GetPlugin(pluginName);
        if (plugin.TryGetValue(key, out var _))
            plugin.Remove(key);
        plugin.Add(key, value);
    }

    public static int GetInt(string pluginName, string key, int defaultValue = 0)
    {
        if (GetPlugin(pluginName).TryGetValue(key, out var val))
        {
            if (int.TryParse(val, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number))
                return number;
            return defaultValue;
        }
        return defaultValue;
    }

    public static void SetInt(string pluginName, string key, int value)
    {
        var plugin = GetPlugin(pluginName);
        if (plugin.TryGetValue(key, out var _))
            plugin.Remove(key);
        plugin.Add(key, value.ToString(CultureInfo.InvariantCulture));
    }

    public static float GetFloat(string pluginName, string key, float defaultValue = 0f)
    {
        if (GetPlugin(pluginName).TryGetValue(key, out var val))
        {
            if (float.TryParse(val, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number))
                return number;
            return defaultValue;
        }
        return defaultValue;
    }

    public static void SetFloat(string pluginName, string key, float value)
    {
        var plugin = GetPlugin(pluginName);
        if (plugin.TryGetValue(key, out var _))
            plugin.Remove(key);
        plugin.Add(key, value.ToString(CultureInfo.InvariantCulture));
    }


    public static bool GetBool(string pluginName, string key, bool defaultValue = false)
    {
        if (GetPlugin(pluginName).TryGetValue(key, out var val))
        {
            if (bool.TryParse(val, out var number))
                return number;
            return defaultValue;
        }
        return defaultValue;
    }

    public static void SetBool(string pluginName, string key, bool value)
    {
        var plugin = GetPlugin(pluginName);
        if (plugin.TryGetValue(key, out var _))
            plugin.Remove(key);
        plugin.Add(key, value.ToString(CultureInfo.InvariantCulture));
    }
}