using System;
using System.Windows.Forms;
using log4net;

namespace Blacker.MangaReader.Services
{
    class InteractionService : IInteractionService
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(InteractionService));

        public void ShowOpenFileDialog(string defaultExt, string filter, Action<DialogResult, string> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            using (var dlg = new OpenFileDialog())
            {
                dlg.DefaultExt = defaultExt;
                dlg.Filter = filter;

                var result = dlg.ShowDialog();

                try
                {
                    callback(result, dlg.FileName);
                }
                catch (Exception ex)
                {
                    _log.Error("Error invoking callback.", ex);
                }
            }
        }
    }
}
