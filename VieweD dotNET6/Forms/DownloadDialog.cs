using CG.Web.MegaApiClient;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VieweD.Helpers.System;
using YoutubeExplode.Videos.Streams;
using WebClient = System.Net.WebClient;

namespace VieweD.Forms
{
    public partial class DownloadDialog : Form
    {
        public static DownloadDialog? Instance { get; private set; }
        private static CookieAwareWebClient? _webClient = null;

        public static CookieAwareWebClient WebClientInstance
        {
            get
            {
                if (_webClient == null)
                {
                    _webClient = new CookieAwareWebClient();
                    _webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
                }
                return _webClient;
            }
        }

        private string _url = string.Empty;
        private string _targetFile = string.Empty;

        public void SetDownloadJob(string url, string targetFile, string? dialogTitle)
        {
            if (dialogTitle != null)
                Text = dialogTitle;

            _url = url;
            _targetFile = targetFile;
            LabelDownloadUrl.Text = url;
            LabelTargetFile.Text = targetFile;
        }

        public DownloadDialog()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (WebClientInstance != null)
            {
                BtnCancel.Enabled = false;
                WebClientInstance.CancelAsync();
            }
        }

        private void DownloadDialog_Load(object sender, EventArgs e)
        {
            Instance = this;
            bgw.RunWorkerAsync();
        }

        public DialogResult BeginDownload()
        {
            if (File.Exists(_targetFile))
            {
                if (MessageBox.Show($"Target file already exists, do you want to override it?\r\n{_targetFile}",
                        "Download", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return DialogResult.Abort;
            }

            return ShowDialog();
        }

        private void OnProgress(int pos, int max)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                if (pos > 0)
                {
                    DownloadProgress.Style = ProgressBarStyle.Blocks;
                }
                else
                {
                    DownloadProgress.Style = ProgressBarStyle.Marquee;
                }

                DownloadProgress.Minimum = 0;
                DownloadProgress.Maximum = max;
                DownloadProgress.Value = pos;
            }));
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            OnProgress(0, 100);

            var downloadedTempFile = DownloadFileFromUrl(_url, _targetFile);
            if (!string.IsNullOrEmpty(downloadedTempFile))
            {
                var dlExt = Path.GetExtension(downloadedTempFile).ToLower();
                if (dlExt != Path.GetExtension(_targetFile).ToLower())
                {
                    var oldFileName = _targetFile;
                    _targetFile = Path.ChangeExtension(_targetFile, dlExt);
                    if (File.Exists(_targetFile))
                        File.Delete(_targetFile);
                    File.Move(oldFileName, _targetFile);
                }
            }

            if (!File.Exists(_targetFile))
            {
                MessageBox.Show($"Error downloading file !\r\n{_targetFile}", @"Download error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnProgress(100, 100);

            DialogResult = File.Exists(_targetFile) ? DialogResult.OK : DialogResult.Cancel;
        }

        private void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            OnProgress(e.ProgressPercentage, 100);
        }

        public DownloadUrlType GuessUrlType(string URL)
        {
            var u = URL.ToLower();
            if (u.StartsWith("http://") || u.StartsWith("https://"))
            {
                var res = DownloadUrlType.Unknown;

                if (u.Contains(".youtube.com/") || u.Contains("youtu.be/") || u.Contains(".googlevideo.com/"))
                    res = DownloadUrlType.YouTube;
                else
                if (u.Contains("drive.google.com/"))
                    res = DownloadUrlType.GoogleDrive;
                else
                if (u.Contains("mega.nz/"))
                    res = DownloadUrlType.MEGA;

                return res;
            }
            else
            {
                return DownloadUrlType.Invalid;
            }
        }

        public string DownloadFileFromUrl(string URL, string SuggestedFileName)
        {
            var res = string.Empty;
            var URLType = GuessUrlType(URL);
            try
            {
                switch (URLType)
                {
                    case DownloadUrlType.YouTube:
                        // Begin the download process
                        res = SuggestedFileName;
                        var y = DownloadFromYoutubeAsync(URL, res);
                        break;
                    case DownloadUrlType.MEGA:
                        res = SuggestedFileName;
                        var m = DownloadFromMegaAsync(URL, res);
                        res = m.Result;
                        break;
                    case DownloadUrlType.GoogleDrive:
                        OnProgress(0, 100);
                        var fi = WebFileDownloader.DownloadFileFromUrlToPath(URL, SuggestedFileName);
                        if ((fi != null) && (fi.Name != string.Empty))
                        {
                            var downloadFile = WebFileDownloader.GetFileNameFromContentDisposition(WebFileDownloader.LastContentDisposition);
                            if (!string.IsNullOrWhiteSpace(downloadFile))
                            {
                                var dlExt = Path.GetExtension(downloadFile).ToLower();
                                if (dlExt != Path.GetExtension(SuggestedFileName).ToLower())
                                {
                                    var oldFileName = SuggestedFileName;
                                    SuggestedFileName = Path.ChangeExtension(SuggestedFileName, dlExt);
                                    if (File.Exists(SuggestedFileName))
                                        File.Delete(SuggestedFileName);
                                    File.Move(oldFileName, SuggestedFileName);
                                }
                            }
                        }

                        break;
                    default:
                        WebFileDownloader.DownloadFileFromUrlToPath(URL, SuggestedFileName);
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

        public async Task<bool> DownloadFromYoutubeAsync(string URL, string fileName)
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
                    // Console.Error.WriteLine("This videos has no streams");
                    MessageBox.Show(@"This videos has no streams", "Download YouTube Error");
                    res = false;
                }
                else
                {
                    using (var progress = new InlineProgress())
                    {
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

        public async Task<string> DownloadFromMegaAsync(string URL, string fileName)
        {
            string res;
            var destDir = Path.GetDirectoryName(fileName);
            if (destDir == null)
                destDir = "";
            try
            {
                var client = new MegaApiClient();
                await client.LoginAnonymousAsync();

                Uri fileLink = new Uri(URL);
                var node = client.GetNodeFromLinkAsync(fileLink).Result;
                res = Path.Combine(destDir, node.Name);
                //Console.WriteLine($"Downloading {node.Name} => {res}");

                //IProgress<double> progressHandler = new Progress<double>(x => Console.WriteLine("{0}%", x));
                //await client.DownloadFileAsync(fileLink, res, progressHandler);

                /*
                using (var progress = new InlineProgress())
                {
                    progress.Loading.TopMost = true;
                    progress.Loading.Text = "Downloading from MEGA";
                    progress.Loading.pb.Visible = false;
                    progress.Loading.lTextInfo.Text = "Downloading " + (node.Size / 1024 / 1024).ToString() + "MB, please wait ...";
                    progress.Loading.lTextInfo.Visible = true;
                    progress.Loading.Refresh();
                    Application.DoEvents();
                    //await Task.Run(async () => await client.DownloadFileAsync(fileLink, res, null) ).ConfigureAwait(false);
                    //await client.DownloadFileAsync(fileLink, res, progress);
                    client.DownloadFile(fileLink, res);
                    Thread.Sleep(1000);
                }
                */
                Application.DoEvents();
                client.DownloadFile(fileLink, res);


                client.Logout();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Download MEGA Exception");
                res = string.Empty;
            }
            return res;
        }

        private static void WebClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadDialog.Instance?.OnProgress((int)e.BytesReceived, (int)e.TotalBytesToReceive);
        }

        public static class WebFileDownloader
        {
            private const string GOOGLE_DRIVE_DOMAIN = "drive.google.com";
            private const string GOOGLE_DRIVE_DOMAIN2 = "https://drive.google.com";
            public static string LastContentDisposition = string.Empty;

            // Normal example: WebFileDownloader.DownloadFileFromUrlToPath( "http://example.com/file/download/link", @"C:\file.txt" );
            // Drive example: WebFileDownloader.DownloadFileFromUrlToPath( "http://drive.google.com/file/d/FILEID/view?usp=sharing", @"C:\file.txt" );
            public static FileInfo? DownloadFileFromUrlToPath(string url, string path, bool skipTypeCheck = false)
            {
                if ((skipTypeCheck == false) && (url.StartsWith(GOOGLE_DRIVE_DOMAIN) || url.StartsWith(GOOGLE_DRIVE_DOMAIN2)))
                    return DownloadGoogleDriveFileFromUrlToPath(url, path);

                try
                {
                    WebClientInstance.DownloadFile(url, path);
                    LastContentDisposition = WebClientInstance.ResponseHeaders?.Get("content-disposition") ?? "";
                    return new FileInfo(path);
                }
                catch
                {
                    return null;
                }
            }

            // Downloading large files from Google Drive prompts a warning screen and
            // requires manual confirmation. Consider that case and try to confirm the download automatically
            // if warning prompt occurs
            private static FileInfo? DownloadGoogleDriveFileFromUrlToPath(string url, string path)
            {
                // You can comment the statement below if the provided url is guaranteed to be in the following format:
                // https://drive.google.com/uc?id=FILEID&export=download
                url = GetGoogleDriveDownloadLinkFromUrl(url);

                FileInfo? downloadedFile;

                // Sometimes Drive returns an NID cookie instead of a download_warning cookie at first attempt,
                // but works in the second attempt
                for (int i = 0; i < 2; i++)
                {
                    downloadedFile = DownloadFileFromUrlToPath(url, path, true);
                    if (downloadedFile == null)
                    {
                        return null;
                    }

                    // Confirmation page is around 50KB, shouldn't be larger than 60KB
                    if (downloadedFile.Length > 60000)
                    {
                        return downloadedFile;
                    }

                    // Downloaded file might be the confirmation page, check it
                    string content;
                    using (var reader = downloadedFile.OpenText())
                    {
                        // Confirmation page starts with <!DOCTYPE html>, which can be preceeded by a newline
                        char[] header = new char[20];
                        int readCount = reader.ReadBlock(header, 0, 20);
                        if (readCount < 20 || !(new string(header).Contains("<!DOCTYPE html>")))
                        {
                            return downloadedFile;
                        }

                        content = reader.ReadToEnd();
                    }

                    int linkIndex = content.LastIndexOf("href=\"/uc?");
                    if (linkIndex < 0)
                    {
                        return downloadedFile;
                    }

                    linkIndex += 6;
                    int linkEnd = content.IndexOf('"', linkIndex);
                    if (linkEnd < 0)
                    {
                        return downloadedFile;
                    }

                    url = "https://drive.google.com" +
                          content.Substring(linkIndex, linkEnd - linkIndex).Replace("&amp;", "&");
                }

                downloadedFile = DownloadFileFromUrlToPath(url, path, true);
                return downloadedFile;
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
                        if (_cookies.TryGetValue(url.Host, out var cookie))
                            return cookie;

                        return string.Empty;
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
                var request = base.GetWebRequest(address);

                if (request is HttpWebRequest httpRequest)
                {
                    var cookie = cookies[address];
                    if (!string.IsNullOrWhiteSpace(cookie))
                        httpRequest.Headers.Set("cookie", cookie);
                }

                return request;
            }

            protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
            {
                var response = base.GetWebResponse(request, result);

                var getCookies = response.Headers.GetValues("Set-Cookie") ?? Array.Empty<string>();
                if (getCookies.Length <= 0) 
                    return response;

                var cookie = string.Empty;
                foreach (var c in getCookies)
                    cookie += c;

                cookies[response.ResponseUri] = cookie;

                return response;
            }

            protected override WebResponse GetWebResponse(WebRequest request)
            {
                var response = base.GetWebResponse(request);

                var getCookies = response.Headers.GetValues("Set-Cookie") ?? Array.Empty<string>();
                if (getCookies.Length > 0)
                {
                    var cookie = string.Empty;
                    foreach (var c in getCookies)
                        cookie += c;

                    cookies[response.ResponseUri] = cookie;
                }

                return response;
            }
        }

        internal class InlineProgress : IProgress<double>, IDisposable
        {
            public DownloadDialog? DownloadForm { get; set; }

            public InlineProgress()
            {
                DownloadForm?.OnProgress(0, 1);
            }

            public void Report(double progress)
            {
                var newVal = (int)Math.Round(progress * 10000f);
                // Console.WriteLine($"{progress}%");

                if (LoadingForm.Instance?.InvokeRequired ?? false)
                {
                    LoadingForm.Instance?.Invoke(new MethodInvoker(delegate
                    {
                        DownloadForm?.OnProgress(newVal, 10000);
                    }));
                }
                else
                {
                    DownloadForm?.OnProgress(newVal, 10000);
                }
            }

            public void Dispose()
            {
                DownloadForm?.OnProgress(10000, 10000);
            }
        }
    }


}
