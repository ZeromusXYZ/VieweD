using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VieweD.Helpers.System
{
    static class FileHelper
    {
        static bool IsArchiveInFileName(string fileName, out string archiveName, out string inArchiveName)
        {
            archiveName = string.Empty;
            inArchiveName = string.Empty;
            var pathSplit = fileName.Split(Path.DirectorySeparatorChar);
            for(var i = pathSplit.Length-1; i > 0;i--)
            {
                var afn = string.Empty;
                for (var n = 0; n < i; n++)
                    afn = Path.Combine(afn,pathSplit[n]);
                var ifn = string.Empty;
                for (var n = i; n < pathSplit.Length-1; n++)
                    ifn = Path.Combine(ifn, pathSplit[n]);
                if (File.Exists(afn))
                {
                    // Found a file
                    archiveName = afn;
                    inArchiveName = ifn;
                    return true;
                }
            }
            return false;
        }

        public static Stream OpenFileAsReadOnlyStream(string fileName)
        {
            Stream res = null;
            try
            {
                if (File.Exists(fileName))
                {
                    res = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                }
                else
                {
                    if (IsArchiveInFileName(fileName,out var afn, out var ifn))
                    {

                    }

                }
            }
            catch
            {
                res = null;
            }
            return res ;
        }
    }
}
