using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static VieweD.Forms.ProjectInfoForm;
using VieweD.Forms;
using YoutubeExplode.Videos.Streams;
using CG.Web.MegaApiClient;
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

    static class Helper
    {
        private static List<string> ExpectedLogFileRoots = new List<string>() { "packetviewer", "logs", "packetdb", "wireshark", "packeteer", "idview", "raw", "incoming", "outgoing", "in", "out", "npclogger" };
        private static List<string> ExpectedLogFolderRootsWithCharacterNames = new List<string>() { "packetviewer", "packetdb", "wireshark", "packeteer", "idview", "npclogger" };

        // Source: https://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path
        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromDirectory">
        /// Contains the directory that defines the start of the relative path.
        /// </param>
        /// <param name="toPath">
        /// Contains the path that defines the endpoint of the relative path.
        /// </param>
        /// <returns>
        /// The relative path from the start directory to the end path.
        /// </returns>
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
            string res;
            string fn = Path.GetFileNameWithoutExtension(filename);
            string fnl = fn.ToLower();
            string fel = Path.GetExtension(filename).ToLower();
            if ((fnl == "full") || (fnl == "incoming") || (fnl == "outgoing") || (fel == ".sqlite"))
            {
                string ldir = Path.GetFileName(Path.GetDirectoryName(filename)).ToLower();
                if (ExpectedLogFileRoots.IndexOf(ldir) >= 0)
                //if ((ldir == "packetviewer") || (ldir == "logs") || (ldir == "packetdb") || (ldir == "wireshark") || (ldir == "packeteer"))
                {
                    res = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(filename)));
                }
                else
                {
                    res = Path.GetFileName(Path.GetDirectoryName(filename));
                }
            }
            else
            {
                res = fn;
            }
            if (res.Length > 20)
                res = res.Substring(0, 16) + "...";
            res += "  ";
            return res;
        }

        public static string MakeProjectDirectoryFromLogFileName_Old(string filename)
        {
            string res;
            string fnl = Path.GetFileNameWithoutExtension(filename).ToLower();
            string fel = Path.GetExtension(filename).ToLower();
            if ((fnl == "full") || (fnl == "incoming") || (fnl == "outgoing") || (fel == ".sqlite"))
            {
                string ldir = Path.GetFileName(Path.GetDirectoryName(filename)).ToLower();
                // Expected "root" folders of where logs might be stored
                if (ExpectedLogFileRoots.IndexOf(ldir) >= 0)
                // if ((ldir == "packetviewer") || (ldir == "logs") || (ldir == "packetdb") || (ldir == "wireshark") || (ldir == "packeteer"))
                {
                    res = Path.GetDirectoryName(Path.GetDirectoryName(filename));
                }
                else
                {
                    res = Path.GetDirectoryName(filename);
                }
            }
            else
            {
                res = Path.GetDirectoryName(filename);
            }

            if (!res.EndsWith(Path.DirectorySeparatorChar.ToString()))
                res += Path.DirectorySeparatorChar;

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
                var hDir = pathSplit[pathSplit.Count - 1];

                if ((pathSplit.Count > 2) && (ExpectedLogFolderRootsWithCharacterNames.IndexOf(pathSplit[pathSplit.Count - 2]) >= 0))
                {
                    // This is the first dir inside packetviewer, add support for character names
                    // Add "this character name" to expected dirs when downdir is packetviewer and currect dir isn't in expected
                    if ((elfr.IndexOf(hDir) < 0) && (Directory.Exists(Path.Combine(pathSplit.ToArray()))))
                    {
                        elfr.Add(hDir);
                    }
                }

                if ((elfr.IndexOf(hDir) < 0) && (Directory.Exists(Path.Combine(pathSplit.ToArray()))))
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
                res = Path.GetDirectoryName(filename);
            }

            if (!res.EndsWith(Path.DirectorySeparatorChar.ToString()))
                res += Path.DirectorySeparatorChar;

            return res;
        }

        public static DownloadURLType GuessURLType(string URL)
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

        public static string DownloadFileFromURL(string URL, string SuggestedFileName)
        {
            var res = string.Empty;
            var URLType = GuessURLType(URL);
            try
            {
                switch (URLType)
                {
                    case DownloadURLType.YouTube:
                        // Begin the download process
                        res = SuggestedFileName;
                        var y = DownloadFromYoutubeAsync(URL, res);
                        break;
                    case DownloadURLType.MEGA:
                        res = SuggestedFileName;
                        var m = DownloadFromMEGAAsync(URL, res);
                        res = m.Result ;
                        break;
                    case DownloadURLType.GoogleDrive:
                        using (var loadform = new LoadingForm(MainForm.ThisMainForm))
                        {
                            loadform.pb.Hide();
                            loadform.lTextInfo.Show();
                            loadform.Show();
                            loadform.Refresh();
                            var fi = FileDownloader.DownloadFileFromURLToPath(URL, SuggestedFileName);
                            if ((fi.Name != null) && (fi.Name != string.Empty))
                            {
                                string dlFile = FileDownloader.GetFileNameFromContentDisposition(FileDownloader.LastContentDisposition);
                                if ((dlFile != null) && (dlFile != string.Empty))
                                {
                                    var dlExt = Path.GetExtension(dlFile).ToLower();
                                    if (dlExt != Path.GetExtension(SuggestedFileName).ToLower())
                                    {
                                        var oldFN = SuggestedFileName;
                                        SuggestedFileName = Path.ChangeExtension(SuggestedFileName, dlExt);
                                        if (File.Exists(SuggestedFileName))
                                            File.Delete(SuggestedFileName);
                                        File.Move(oldFN, SuggestedFileName);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        FileDownloader.DownloadFileFromURLToPath(URL, SuggestedFileName);
                        res = SuggestedFileName;
                        break;
                }
            }
            catch
            {
                res = string.Empty;
            }
            return res;
        }

        public static async Task<bool> DownloadFromYoutubeAsync(string URL, string fileName)
        {
            bool res = false;
            try
            {
                var youtube = new YoutubeExplode.YoutubeClient();
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(URL);
                // Get media streams & choose the best muxed stream
                var streamInfo = streamManifest.GetMuxedStreams().TryGetWithHighestVideoQuality();

                if (streamInfo == null)
                {
                    Console.Error.WriteLine("This videos has no streams");
                    MessageBox.Show(@"This videos has no streams", "Download YouTube Error");
                    res = false;
                }
                else
                {
                    using (var progress = new InlineProgress())
                    {
                        progress.Loading.Text = "Downloading from YouTube";
                        await youtube.Videos.Streams.DownloadAsync(streamInfo, fileName, progress);
                    }
                    res = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Download YouTube Exception");
            }
            return res;
        }

        public static async Task<string> DownloadFromMEGAAsync(string URL, string fileName)
        {
            string res;
            var destDir = Path.GetDirectoryName(fileName);
            try
            {
                var client = new MegaApiClient();
                client.LoginAnonymous();

                Uri fileLink = new Uri(URL);
                var node = client.GetNodeFromLink(fileLink);
                res = Path.Combine(destDir, node.Name);
                Console.WriteLine($"Downloading {node.Name} => {res}");

                //IProgress<double> progressHandler = new Progress<double>(x => Console.WriteLine("{0}%", x));
                //await client.DownloadFileAsync(fileLink, res, progressHandler);

                using (var progress = new InlineProgress())
                {
                    progress.Loading.TopMost = true;
                    progress.Loading.Text = "Downloading from MEGA";
                    progress.Loading.pb.Visible = false ;
                    progress.Loading.lTextInfo.Text = "Downloading " + (node.Size / 1024 / 1024).ToString() + "MB, please wait ...";
                    progress.Loading.lTextInfo.Visible = true;
                    progress.Loading.Refresh();
                    Application.DoEvents();
                    //await Task.Run(async () => await client.DownloadFileAsync(fileLink, res, null) ).ConfigureAwait(false);
                    //await client.DownloadFileAsync(fileLink, res, progress);
                    client.DownloadFile(fileLink, res);
                    Thread.Sleep(1000);
                }
                
                client.Logout();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Download MEGA Exception");
                res = string.Empty;
            }
            return res;
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

    // Might need this later at some point
    // Source: https://gist.github.com/yasirkula/d0ec0c07b138748e5feaecbd93b6223c
    public static class FileDownloader
    {
        private const string GOOGLE_DRIVE_DOMAIN = "drive.google.com";
        private const string GOOGLE_DRIVE_DOMAIN2 = "https://drive.google.com";
        public static string LastContentDisposition = string.Empty;

        // Normal example: FileDownloader.DownloadFileFromURLToPath( "http://example.com/file/download/link", @"C:\file.txt" );
        // Drive example: FileDownloader.DownloadFileFromURLToPath( "http://drive.google.com/file/d/FILEID/view?usp=sharing", @"C:\file.txt" );
        public static FileInfo DownloadFileFromURLToPath(string url, string path)
        {
            if (url.StartsWith(GOOGLE_DRIVE_DOMAIN) || url.StartsWith(GOOGLE_DRIVE_DOMAIN2))
                return DownloadGoogleDriveFileFromURLToPath(url, path);
            else
                return DownloadFileFromURLToPath(url, path, null);
        }

        private static FileInfo DownloadFileFromURLToPath(string url, string path, WebClient webClient)
        {
            try
            {
                if (webClient == null)
                {
                    using (webClient = new WebClient())
                    {
                        webClient.DownloadFile(url, path);
                        LastContentDisposition = webClient.ResponseHeaders.Get("content-disposition");
                        return new FileInfo(path);
                    }
                }
                else
                {
                    webClient.DownloadFile(url, path);
                    LastContentDisposition = webClient.ResponseHeaders.Get("content-disposition");
                    return new FileInfo(path);
                }
            }
            catch
            {
                return null;
            }
        }

        // Downloading large files from Google Drive prompts a warning screen and
        // requires manual confirmation. Consider that case and try to confirm the download automatically
        // if warning prompt occurs
        private static FileInfo DownloadGoogleDriveFileFromURLToPath(string url, string path)
        {
            // You can comment the statement below if the provided url is guaranteed to be in the following format:
            // https://drive.google.com/uc?id=FILEID&export=download
            url = GetGoogleDriveDownloadLinkFromUrl(url);

            using (var webClient = new CookieAwareWebClient())
            {
                FileInfo downloadedFile;

                // Sometimes Drive returns an NID cookie instead of a download_warning cookie at first attempt,
                // but works in the second attempt
                for (int i = 0; i < 2; i++)
                {
                    downloadedFile = DownloadFileFromURLToPath(url, path, webClient);
                    if (downloadedFile == null)
                        return null;

                    // Confirmation page is around 50KB, shouldn't be larger than 60KB
                    if (downloadedFile.Length > 60000)
                        return downloadedFile;

                    // Downloaded file might be the confirmation page, check it
                    string content;
                    using (var reader = downloadedFile.OpenText())
                    {
                        // Confirmation page starts with <!DOCTYPE html>, which can be preceeded by a newline
                        char[] header = new char[20];
                        int readCount = reader.ReadBlock(header, 0, 20);
                        if (readCount < 20 || !(new string(header).Contains("<!DOCTYPE html>")))
                            return downloadedFile;

                        content = reader.ReadToEnd();
                    }

                    int linkIndex = content.LastIndexOf("href=\"/uc?");
                    if (linkIndex < 0)
                        return downloadedFile;

                    linkIndex += 6;
                    int linkEnd = content.IndexOf('"', linkIndex);
                    if (linkEnd < 0)
                        return downloadedFile;

                    url = "https://drive.google.com" + content.Substring(linkIndex, linkEnd - linkIndex).Replace("&amp;", "&");
                }

                downloadedFile = DownloadFileFromURLToPath(url, path, webClient);

                return downloadedFile;
            }
        }

        // Handles 3 kinds of links (they can be preceeded by https://):
        // - drive.google.com/open?id=FILEID
        // - drive.google.com/file/d/FILEID/view?usp=sharing
        // - drive.google.com/uc?id=FILEID&export=download
        public static string GetGoogleDriveDownloadLinkFromUrl(string url)
        {
            int index = url.IndexOf("id=");
            int closingIndex;
            if (index > 0)
            {
                index += 3;
                closingIndex = url.IndexOf('&', index);
                if (closingIndex < 0)
                    closingIndex = url.Length;
            }
            else
            {
                index = url.IndexOf("file/d/");
                if (index < 0) // url is not in any of the supported forms
                    return string.Empty;

                index += 7;

                closingIndex = url.IndexOf('/', index);
                if (closingIndex < 0)
                {
                    closingIndex = url.IndexOf('?', index);
                    if (closingIndex < 0)
                        closingIndex = url.Length;
                }
            }

            return string.Format("https://drive.google.com/uc?id={0}&export=download", url.Substring(index, closingIndex - index));
        }

        public static string GetFileNameFromContentDisposition(string cd)
        {
            string fn = string.Empty;

            var cdFields = cd.Split(';');
            string fnTag = "filename=";
            foreach (var cdf in cdFields)
            {
                if (cdf.StartsWith(fnTag))
                {
                    fn = cdf.Substring(fnTag.Length);
                    fn = fn.Trim('\"');
                    return fn;
                }

            }
            return fn;
        }

    }

    // Web client used for Google Drive
    public class CookieAwareWebClient : WebClient
    {
        private class CookieContainer
        {
            Dictionary<string, string> _cookies;

            public string this[Uri url]
            {
                get
                {
                    string cookie;
                    if (_cookies.TryGetValue(url.Host, out cookie))
                        return cookie;

                    return null;
                }
                set
                {
                    _cookies[url.Host] = value;
                }
            }

            public CookieContainer()
            {
                _cookies = new Dictionary<string, string>();
            }
        }

        private CookieContainer cookies;

        public CookieAwareWebClient() : base()
        {
            cookies = new CookieContainer();
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);

            if (request is HttpWebRequest)
            {
                string cookie = cookies[address];
                if (cookie != null)
                    ((HttpWebRequest)request).Headers.Set("cookie", cookie);
            }

            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse response = base.GetWebResponse(request, result);

            string[] cookies = response.Headers.GetValues("Set-Cookie");
            if (cookies != null && cookies.Length > 0)
            {
                string cookie = "";
                foreach (string c in cookies)
                    cookie += c;

                this.cookies[response.ResponseUri] = cookie;
            }

            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);

            string[] cookies = response.Headers.GetValues("Set-Cookie");
            if (cookies != null && cookies.Length > 0)
            {
                string cookie = "";
                foreach (string c in cookies)
                    cookie += c;

                this.cookies[response.ResponseUri] = cookie;
            }

            return response;
        }
    }

}
