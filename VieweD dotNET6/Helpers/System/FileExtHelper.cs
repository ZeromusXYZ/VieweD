using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Windows.Forms;

namespace VieweD.Helpers.System
{
    public class FileAssociation
    {
        public string Extension { get; set; } = string.Empty;
        public string ProgramId { get; set; } = string.Empty;
        public string FileTypeDescription { get; set; } = string.Empty;
        public string ExecutableFilePath { get; set; } = string.Empty;
    }


    // source: https://stackoverflow.com/questions/2681878/associate-file-extension-with-application
    public class FileAssociations
    {
        // needed so that Explorer windows get refreshed after the registry is updated
        [DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        private const int SHCNE_ASSOCCHANGED = 0x8000000;
        private const int SHCNF_FLUSH = 0x1000;

        public static void EnsureAssociationsSet()
        {
            var filePath = Process.GetCurrentProcess()?.MainModule?.FileName ?? Application.ExecutablePath;
            EnsureAssociationsSet(
                new FileAssociation
                {
                    Extension = ".pvd",
                    ProgramId = "PVD_Project_File",
                    FileTypeDescription = "VieweD Project File",
                    ExecutableFilePath = filePath
                });
        }

        /*
        public static void EnsureURIAssociationsSet()
        {
            var filePath = Process.GetCurrentProcess().MainModule.FileName;
            SetURIAssociation("aelcf", "AAEmu Launcher Protocol", filePath);
        }
        */

        public static void EnsureAssociationsSet(params FileAssociation[] associations)
        {
            bool madeChanges = false;
            foreach (var association in associations)
            {
                madeChanges |= SetAssociation(
                    association.Extension,
                    association.ProgramId,
                    association.FileTypeDescription,
                    association.ExecutableFilePath);
            }

            if (madeChanges)
            {
                SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
            }
        }

        public static bool SetAssociation(string extension, string progId, string fileTypeDescription, string applicationFilePath)
        {
            bool madeChanges = false;
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + extension, progId);
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + progId, fileTypeDescription);
            madeChanges |= SetKeyDefaultValue($@"Software\Classes\{progId}\shell\open\command", "\"" + applicationFilePath + "\" \"%1\"");
            return madeChanges;
        }

        private static bool SetKeyDefaultValue(string keyPath, string value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(keyPath))
            {
                if (key.GetValue(null) as string != value)
                {
                    key.SetValue(null, value);
                    return true;
                }
            }

            return false;
        }

        private static bool SetKeyValue(string keyPath, string keyName, string value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(keyPath))
            {
                if (key.GetValue(keyName) as string != value)
                {
                    key.SetValue(keyName, value);
                    return true;
                }
            }

            return false;
        }

        public static bool SetURIAssociation(string protocolID, string protocolName, string applicationFilePath)
        {
            bool madeChanges = false;
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + protocolID, "URL:" + protocolName);
            madeChanges |= SetKeyValue(@"Software\Classes\" + protocolID, "URL PRotocol", "");
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + protocolID + @"\DefaultIcon", "\"" + applicationFilePath + ",1\"");
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + protocolID + @"\shell\open\command", "\"" + applicationFilePath + "\" \"%1\"");
            return madeChanges;
        }


    }
}
