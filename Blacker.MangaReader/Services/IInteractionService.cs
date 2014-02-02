using System;
using System.Windows.Forms;

namespace Blacker.MangaReader.Services
{
    interface IInteractionService
    {
        void ShowOpenFileDialog(string defaultExt, string filter, Action<DialogResult, string> callback);
    }
}
