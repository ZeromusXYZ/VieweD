using System;
using System.Threading;
using System.Windows.Forms;
using VieweD.Properties;

namespace VieweD.Helpers.System
{
    // Source: https://stackoverflow.com/questions/899350/how-do-i-copy-the-contents-of-a-string-to-the-clipboard-in-c
    /// <summary>
    /// Usage: new ClipboardHelper( DataFormats.Text, "See, I'm on the clipboard" ).Go();
    /// </summary>
    class ClipboardHelper : StaHelper
    {
        readonly string _format;
        readonly object _data;

        public ClipboardHelper(string format, object data)
        {
            _format = format;
            _data = data;
        }

        protected override void Work()
        {
            var obj = new DataObject(
                _format,
                _data
            );

            Clipboard.SetDataObject(obj, true);
        }

        public static void SetClipboard(string clipText)
        {
            try
            {
                // Because nothing is ever as simple as the next line >.>
                // Clipboard.SetText(s);
                // Helper will (try to) prevent errors when copying to clipboard because of threading issues
                var clipHelp = new ClipboardHelper(DataFormats.Text, clipText)
                {
                    DoNotRetryWorkOnFailed = false
                };
                clipHelp.Go();
            }
            catch
            {
                // Ignore
            }
        }
    }

    abstract class StaHelper
    {
        private readonly ManualResetEvent _complete = new (false);

        public void Go()
        {
            var thread = new Thread(DoWork)
            {
                IsBackground = true,
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        // Thread entry method
        private void DoWork()
        {
            try
            {
                _complete.Reset();
                Work();
            }
            catch (Exception ex)
            {
                if (DoNotRetryWorkOnFailed)
                    throw;
                else
                {
                    try
                    {
                        Thread.Sleep(1000);
                        Work();
                    }
                    catch
                    {
                        // ex from first exception
                        MessageBox.Show(ex.Message,Resources.CopyToClipboardTitle,MessageBoxButtons.OK,MessageBoxIcon.Error);
                        // LogAndShowMessage(ex);
                    }
                }
            }
            finally
            {
                _complete.Set();
            }
        }

        public bool DoNotRetryWorkOnFailed { get; set; }

        // Implemented in base class to do actual work.
        protected abstract void Work();
    }


}
