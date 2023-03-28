using System;
using System.Collections.Generic;
using System.IO;
using VieweD.engine.common;
using VieweD.Helpers.System;

namespace VieweD.data.aa.engine;

public class AaEncryptionsBase
{
    public virtual string Name => "ArcheAge Encryption Helper Base";

    protected readonly Dictionary<string, (uint, uint)> KeyDictionary = new();

    public static AaEncryptionsBase? CreateEncryptionByName(string name)
    {
        // Load local in-app classes
        var allClasses =
            EngineManager.PluginAssembly?.GetTypes() ?? null; // Assembly.GetExecutingAssembly().GetTypes();
        if (allClasses == null)
            return null;

        try
        {
            foreach (var aClass in allClasses)
            {
                if (
                    (aClass.BaseType == typeof(AaEncryptionsBase)) && 
                    (
                        string.Equals(aClass.Name, name, StringComparison.CurrentCultureIgnoreCase) || 
                        string.Equals(aClass.FullName,name, StringComparison.CurrentCultureIgnoreCase)
                    )
                   )
                {
                    if (Activator.CreateInstance(aClass) is AaEncryptionsBase enc)
                        return enc;
                }
            }
        }
        catch
        {
            // Ignore
        }

        return null;
    }

    /// <summary>
    /// Loads a given key table file
    /// </summary>
    /// <param name="folderName"></param>
    /// <returns></returns>
    public int LoadKeysFromFolder(string folderName)
    {
            KeyDictionary.Clear();
            if (!Directory.Exists(folderName))
                return 0;

            var files = Directory.GetFiles(folderName, "*.key");
            foreach (var fileName in files)
            {
                var lines = File.ReadAllLines(fileName);
                foreach (var line in lines)
                {
                    var fields = line.ToLower().Split(';');
                    if (fields.Length < 3) // 4th field and on, would be comments
                        continue;
                    if (string.IsNullOrWhiteSpace(fields[0]))
                        continue;
                    if (!NumberHelper.TryFieldParse(fields[1], out ulong val1))
                        continue;
                    if (!NumberHelper.TryFieldParse(fields[2], out ulong val2))
                        continue;

                    KeyDictionary.Add(fields[0], ((uint)val1, (uint)val2));
                }
            }

            return KeyDictionary.Count;
    }

    /// <summary>
    /// Check for keys in the loaded table
    /// </summary>
    /// <param name="version"></param>
    /// <param name="val1"></param>
    /// <param name="val2"></param>
    /// <returns></returns>
    public bool GetValuesForVersion(string version, out uint val1, out uint val2)
    {
        if (KeyDictionary.TryGetValue(version.ToLower(), out var keys))
        {
            val1 = keys.Item1;
            val2 = keys.Item2;
            return true;
        }

        val1 = 0;
        val2 = 0;
        return false;
    }

    /// <summary>
    /// Encrypt packet body
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public virtual byte[] S2CEncrypt(byte[] data)
    {
        return data;
    }

    /// <summary>
    /// Encrypts packet body
    /// </summary>
    /// <param name="data"></param>
    /// <param name="xorKey"></param>
    /// <param name="aesKey"></param>
    /// <param name="iv"></param>
    /// <param name="num"></param>
    /// <param name="xorConst1"></param>
    /// <param name="xorConst2"></param>
    /// <returns></returns>
    public virtual byte[] S2CDecrypt(byte[] data, uint xorKey, byte[] aesKey, byte[] iv, uint num, uint xorConst1, uint xorConst2)
    {
        return data;
    }
}