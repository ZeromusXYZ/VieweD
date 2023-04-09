using CG.Web.MegaApiClient;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VieweD.Helpers.System;
using VieweD.Properties;
using YoutubeExplode.Videos.Streams;

namespace VieweD.Forms
{
    public partial class DownloadDialog : Form
    {
        public static DownloadDialog? Instance { get; private set; }
        private static HttpClient? _webClient;
        private static readonly CancellationToken CancellationToken = new();

        public static HttpClient WebClientInstance
        {
            get
            {
                if (_webClient == null)
                {
                    _webClient = new HttpClient();
                    _webClient.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (Windows NT 10; Win64; x64; rv:56.0) Gecko/20100101 Firefox/56.0");
                    _webClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                    _webClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                    // Keep true if you download resources from different collections of URLs each time
                    // Remove or set to false if you use the same URLs multiple times and frequently
                    _webClient.DefaultRequestHeaders.ConnectionClose = true;
                    // _webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
                }
                return _webClient;
            }
        }

        private string _url = string.Empty;
        private string _targetFile = string.Empty;

        public string TargetFile => _targetFile;

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
            BtnCancel.Enabled = false;
            WebClientInstance.CancelPendingRequests();
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
                if (MessageBox.Show(string.Format(Resources.FileExistsOverrideQuestion, _targetFile),
                        Resources.Download, MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return DialogResult.Abort;
            }

            return ShowDialog();
        }

        private void OnProgress(int pos, int max)
        {
            try
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
                    if (max < DownloadProgress.Value)
                        DownloadProgress.Value = max;
                    DownloadProgress.Maximum = max;
                    DownloadProgress.Value = Math.Clamp(pos, 0, max);
                }));
            }
            catch
            {
                // Ignore
            }
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            OnProgress(0, 100);

            var downloadedTempFile = DownloadFileFromUrl(_url, _targetFile);
            /*
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
            */

            var error = false;

            if (string.IsNullOrWhiteSpace(downloadedTempFile) || !File.Exists(downloadedTempFile))
            {
                error = true;
            }
            else if (File.Exists(downloadedTempFile))
            {
                var fi = new FileInfo(downloadedTempFile);
                if (fi.Length <= 0)
                {
                    error = true;
                    File.Delete(downloadedTempFile);
                }
            }
            else
            {
                error = true;
            }

            if (error)
            {
                MessageBox.Show(string.Format(Resources.DownloadFileError, downloadedTempFile), Resources.DownloadErrorTitle,  MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                _targetFile = downloadedTempFile;
            }
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnProgress(100, 100);

            DialogResult = File.Exists(_targetFile) ? DialogResult.OK : DialogResult.Cancel;
        }

        private void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // OnProgress(e.ProgressPercentage, 100);
        }

        public DownloadUrlType GuessUrlType(string url)
        {
            var u = url.ToLower();
            if (u.StartsWith("http://") || u.StartsWith("https://"))
            {
                var res = DownloadUrlType.Unknown;

                if (u.Contains(".youtube.com/") || u.Contains("youtu.be/") || u.Contains(".googlevideo.com/"))
                    res = DownloadUrlType.YouTube;
                else
                if (u.Contains("drive.google.com/"))
                    res = DownloadUrlType.GoogleDrive;
                else
                if (u.Contains("dropbox.com"))
                    res = DownloadUrlType.DropBox;
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

        public string DownloadFileFromUrl(string url, string suggestedFileName)
        {
            var res = string.Empty;
            var urlType = GuessUrlType(url);
            try
            {
                if (urlType == DownloadUrlType.DropBox)
                {
                    // For dropbox to download as a normal file, it's enough to put dl=1 as option
                    var tempUri = new UriBuilder(url);
                    tempUri.Query = tempUri.Query.Replace("dl=0", "dl=1");
                    url = tempUri.Uri.AbsoluteUri;
                }

                switch (urlType)
                {
                    case DownloadUrlType.YouTube:
                        // Begin the download process
                        if (DownloadFromYoutubeAsync(url, suggestedFileName).Result)
                            res = suggestedFileName;
                        break;
                    case DownloadUrlType.MEGA:
                        res = DownloadFromMegaAsync(url, suggestedFileName).Result;
                        break;
                    case DownloadUrlType.DropBox:
                    case DownloadUrlType.GoogleDrive:
                        OnProgress(0, 100);
                        var fi = WebFileDownloader.DownloadFileFromUrlToPath(url, suggestedFileName);
                        if ((fi != null) && (fi.Name != string.Empty))
                        {
                            var downloadFile = WebFileDownloader.LastContentDisposition; // .GetFileNameFromContentDisposition(WebFileDownloader.LastContentDisposition);
                            if (!string.IsNullOrWhiteSpace(downloadFile))
                            {
                                var dlExt = Path.GetExtension(downloadFile).ToLower();
                                if (dlExt != Path.GetExtension(suggestedFileName).ToLower())
                                {
                                    var oldFileName = suggestedFileName;
                                    suggestedFileName = Path.ChangeExtension(suggestedFileName, dlExt);
                                    if (File.Exists(suggestedFileName))
                                        File.Delete(suggestedFileName);
                                    File.Move(oldFileName, suggestedFileName);
                                    res = suggestedFileName;
                                    break;
                                }
                            }
                            res = fi.FullName;
                        }

                        break;
                    default:
                        var fid = WebFileDownloader.DownloadFileFromUrlToPath(url, suggestedFileName);
                        if (fid != null)
                            res = fid.FullName;
                        break;
                }
            }
            catch
            {
                res = string.Empty;
            }
            return res;
        }

        public async Task<bool> DownloadFromYoutubeAsync(string url, string fileName)
        {
            bool res = false;
            try
            {
                var youtube = new YoutubeExplode.YoutubeClient();
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
                // Get media streams & choose the best muxed stream
                var streamInfo = streamManifest.GetMuxedStreams().TryGetWithHighestVideoQuality();

                if (streamInfo == null)
                {
                    // Console.Error.WriteLine("This videos has no streams");
                    MessageBox.Show(Resources.YouTubeErrorNoStreams, Resources.DownloadErrorTitle);
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
                MessageBox.Show(ex.Message, Resources.DownloadErrorTitle);
            }
            return res;
        }

        public async Task<string> DownloadFromMegaAsync(string url, string fileName)
        {
            string res;
            var destDir = Path.GetDirectoryName(fileName);
            destDir ??= "";

            try
            {
                var client = new MegaApiClient();
                await client.LoginAnonymousAsync();

                Uri fileLink = new Uri(url);
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
                MessageBox.Show(ex.Message, Resources.DownloadErrorTitle);
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
            private const string GoogleDriveDomain = "drive.google.com";
            private const string GoogleDriveDomain2 = "https://drive.google.com";
            public static string LastContentDisposition = string.Empty;

            // Normal example: WebFileDownloader.DownloadFileFromUrlToPath( "http://example.com/file/download/link", @"C:\file.txt" );
            // Drive example: WebFileDownloader.DownloadFileFromUrlToPath( "http://drive.google.com/file/d/FILEID/view?usp=sharing", @"C:\file.txt" );
            public static FileInfo? DownloadFileFromUrlToPath(string url, string path, bool skipTypeCheck = false)
            {
                if ((skipTypeCheck == false) && (url.StartsWith(GoogleDriveDomain) || url.StartsWith(GoogleDriveDomain2)))
                    return DownloadGoogleDriveFileFromUrlToPath(url, path);

                try
                {
                    var progress = new InlineProgress();

                    // Create a file stream to store the downloaded data.
                    // This really can be any type of writeable stream.
                    using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        // Use the custom extension method below to download the data.
                        // The passed progress-instance will receive the download status updates.
                        var response = WebClientInstance.DownloadAsync(url, fileStream, progress, CancellationToken).Result;
                        LastContentDisposition = response?.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "";
                    }
                    /*
                    using(var response = WebClientInstance.GetAsync(url).Result)
                    {
                        using (var fileStream = File.Open(path, FileMode.Create))
                        {
                            response.Content.ReadAsStream().CopyTo(fileStream);
                        }
                        LastContentDisposition = response.Headers.GetValues("content-disposition").FirstOrDefault() ?? "";
                    }
                    */
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

                    var linkIndex = content.LastIndexOf("href=\"/uc?", StringComparison.InvariantCulture);
                    if (linkIndex < 0)
                    {
                        return downloadedFile;
                    }

                    linkIndex += 6;
                    var linkEnd = content.IndexOf('"', linkIndex);
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
                int index = url.IndexOf("id=", StringComparison.InvariantCulture);
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
                    index = url.IndexOf("file/d/", StringComparison.InvariantCulture);
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

                return $"https://drive.google.com/uc?id={url.Substring(index, closingIndex - index)}&export=download";
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

        internal class InlineProgress : IProgress<double>, IDisposable
        {
            public InlineProgress()
            {
                DownloadDialog.Instance?.OnProgress(0, 1);
            }

            public void Report(double progress)
            {
                var newVal = (int)Math.Round(progress * 10000f);
                // Console.WriteLine($"{progress}%");

                if (DownloadDialog.Instance?.InvokeRequired ?? false)
                {
                    DownloadDialog.Instance.Invoke(new MethodInvoker(delegate
                    {
                        DownloadDialog.Instance?.OnProgress(newVal, 10000);
                    }));
                }
                else
                {
                    DownloadDialog.Instance?.OnProgress(newVal, 10000);
                }
            }

            public void Dispose()
            {
                DownloadDialog.Instance?.OnProgress(10000, 10000);
            }
        }
    }

    public static class HttpClientExtensions
    {
        // source: https://stackoverflow.com/questions/20661652/progress-bar-with-httpclient

        public static async Task<HttpResponseMessage?> DownloadAsync(this HttpClient client, string requestUri,
            Stream destination, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
        {
            // Get the http headers first to examine the content length
            var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            var contentLength = response.Content.Headers.ContentLength ?? 0;

            using (var download = await response.Content.ReadAsStreamAsync(cancellationToken))
            {
                // Ignore progress reporting when no progress reporter was 
                // passed or when the content length is unknown
                if (progress == null || contentLength <= 0)
                {
                    await download.CopyToAsync(destination, cancellationToken);
                    return null;
                }

                // Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
                // var relativeProgress = new Progress<long>(totalBytes => progress.Report((double)totalBytes / contentLength));
                // Use extension method to report progress while downloading
                // await download.CopyToAsync(destination, 81920, relativeProgress, cancellationToken);
                await download.CopyToAsync2(destination, 81920, progress, contentLength, cancellationToken);
                progress.Report(1);
            }

            return response;
        }
    }

    public static class StreamExtensions
    {
        public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long>? progress = null, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!source.CanRead)
                throw new ArgumentException("Has to be readable", nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite)
                throw new ArgumentException("Has to be writable", nameof(destination));
            if (bufferSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }

        public static async Task CopyToAsync2(this Stream source, Stream destination, int bufferSize, IProgress<double>? progress = null, long? expectedFileSize = null, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!source.CanRead)
                throw new ArgumentException("Has to be readable", nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite)
                throw new ArgumentException("Has to be writable", nameof(destination));
            if (bufferSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                if ((expectedFileSize != null) && (expectedFileSize > 0))
                {
                    progress?.Report(((double)totalBytesRead / (double)expectedFileSize));
                    progress?.Report(totalBytesRead);
                }
            }
        }
    }
}
