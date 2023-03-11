using System.Net;
using System.Text;
using WebClient = System.Net.WebClient;

namespace VieweD.Helpers.System
{
    public enum DownloadURLType
    {
        Invalid,
        Unknown,
        YouTube,
        GoogleDrive,
        MEGA,
    }

    public static class Helper
    {
        private static List<string> ExpectedLogFileRoots = new List<string>() { "packetviewer", "logs", "packetdb", "wireshark", "packeteer", "idview", "raw", "incoming", "outgoing", "in", "out", "npclogger" };
        private static List<string> ExpectedLogFolderRootsWithCharacterNames = new List<string>() { "packetviewer", "packetdb", "wireshark", "packeteer", "idview", "npclogger" };

        // Source: https://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path
        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromDirectory">Contains the directory that defines the start of the relative path</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path</param>
        /// <returns>The relative path from the start directory to the end path</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string MakeRelative(string fromDirectory, string toPath)
        {
            if (fromDirectory == null)
                throw new ArgumentNullException("fromDirectory");

            if (toPath == null)
                throw new ArgumentNullException("toPath");

            if (TryCompressPath(ref toPath))
                return toPath;

            bool isRooted = (Path.IsPathRooted(fromDirectory) && Path.IsPathRooted(toPath));

            if (isRooted)
            {
                bool isDifferentRoot = (string.Compare(Path.GetPathRoot(fromDirectory), Path.GetPathRoot(toPath), true) != 0);

                if (isDifferentRoot)
                    return toPath;
            }

            List<string> relativePath = new List<string>();
            string[] fromDirectories = fromDirectory.Split(Path.DirectorySeparatorChar);

            string[] toDirectories = toPath.Split(Path.DirectorySeparatorChar);

            int length = Math.Min(fromDirectories.Length, toDirectories.Length);

            int lastCommonRoot = -1;

            // find common root
            for (int x = 0; x < length; x++)
            {
                if (string.Compare(fromDirectories[x], toDirectories[x], true) != 0)
                    break;

                lastCommonRoot = x;
            }

            if (lastCommonRoot == -1)
                return toPath;

            // add relative folders in from path
            for (int x = lastCommonRoot + 1; x < fromDirectories.Length; x++)
            {
                if (fromDirectories[x].Length > 0)
                    relativePath.Add("..");
            }

            // add to folders to path
            for (int x = lastCommonRoot + 1; x < toDirectories.Length; x++)
            {
                relativePath.Add(toDirectories[x]);
            }

            // create relative path
            string[] relativeParts = new string[relativePath.Count];
            relativePath.CopyTo(relativeParts, 0);

            string newPath = string.Join(Path.DirectorySeparatorChar.ToString(), relativeParts);

            return newPath;
        }

        public static string TryMakeFullPath(string ProjectDirectory, string fileName)
        {
            if (fileName == string.Empty)
                return fileName;
            string res = fileName;
            res = res.Replace("app://", Application.StartupPath);

            if (!ProjectDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                ProjectDirectory += Path.DirectorySeparatorChar;

            try
            {
                // If a file is provided, try to expand it to it's full path
                if (!File.Exists(res))
                {
                    var s = Path.GetFullPath(fileName);
                    if (File.Exists(s))
                    {
                        res = s;
                    }
                    else
                    {
                        s = Path.GetFullPath(ProjectDirectory + fileName);
                        if (File.Exists(s))
                        {
                            res = s;
                        }
                    }
                }
            }
            catch
            {
                res = fileName;
            }
            return res;
        }

        public static bool TryCompressPath(ref string filename)
        {
            if (filename.StartsWith(Application.StartupPath))
            {
                filename = filename.Replace(Application.StartupPath, "app://");
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string MakeTabName(string filename)
        {
            const int maxChars = 40;
            string res;
            var fn = Path.GetFileNameWithoutExtension(filename);
            var fnl = fn.ToLower();
            var fel = Path.GetExtension(filename).ToLower();
            if ((fnl == "full") || (fnl == "incoming") || (fnl == "outgoing") || (fel == ".sqlite"))
            {
                var ldir = Path.GetFileName(Path.GetDirectoryName(filename) ?? "").ToLower();
                if (ExpectedLogFileRoots.IndexOf(ldir) >= 0)
                //if ((ldir == "packetviewer") || (ldir == "logs") || (ldir == "packetdb") || (ldir == "wireshark") || (ldir == "packeteer"))
                {
                    res = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(filename) ?? "")??"");
                }
                else
                {
                    res = Path.GetFileName(Path.GetDirectoryName(filename) ?? "");
                }
            }
            else
            {
                res = fn;
            }
            if (res.Length > maxChars)
                res = res.Substring(0, maxChars-4) + "...";
            res += "  ";
            return res;
        }

        public static string MakeProjectDirectoryFromLogFileName(string filename)
        {
            string res;

            var pathSplit = filename.Split(Path.DirectorySeparatorChar).ToList();
            if (pathSplit.Count >= 1)
                pathSplit[0] = pathSplit[0] + Path.DirectorySeparatorChar; // manually add the \ back to the first split

            List<string> elfr = new List<string>();
            elfr.AddRange(ExpectedLogFileRoots);

            while (pathSplit.Count > 1)
            {
                pathSplit.RemoveAt(pathSplit.Count - 1);
                var hDir = pathSplit[^1];

                if ((pathSplit.Count > 2) && (ExpectedLogFolderRootsWithCharacterNames.IndexOf(pathSplit[pathSplit.Count - 2].ToLower()) >= 0))
                {
                    // This is the first dir inside packetviewer, add support for character names
                    // Add "this character name" to expected dirs when downdir is packetviewer and currect dir isn't in expected
                    if ((elfr.IndexOf(hDir.ToLower()) < 0) && (Directory.Exists(Path.Combine(pathSplit.ToArray()))))
                    {
                        elfr.Add(hDir);
                    }
                }

                if ((elfr.IndexOf(hDir.ToLower()) < 0) && (Directory.Exists(Path.Combine(pathSplit.ToArray()))))
                {
                    break;
                }
            }

            if (pathSplit.Count > 1)
            {
                res = Path.Combine(pathSplit.ToArray());
            }
            else
            {
                res = Path.GetDirectoryName(filename) ?? "";
            }

            if (!res.EndsWith(Path.DirectorySeparatorChar.ToString()))
                res += Path.DirectorySeparatorChar;

            return res;
        }

        public static DownloadURLType GuessUrlType(string URL)
        {
            var u = URL.ToLower();
            if (u.StartsWith("http://") || u.StartsWith("https://"))
            {
                var res = DownloadURLType.Unknown;

                if (u.Contains(".youtube.com/") || u.Contains("youtu.be/") || u.Contains(".googlevideo.com/"))
                    res = DownloadURLType.YouTube;
                else
                if (u.Contains("drive.google.com/"))
                    res = DownloadURLType.GoogleDrive;
                else
                if (u.Contains("mega.nz/"))
                    res = DownloadURLType.MEGA;

                return res;
            }
            else
            {
                return DownloadURLType.Invalid;
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
