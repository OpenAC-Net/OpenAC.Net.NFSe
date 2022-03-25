using System.Windows.Forms;

namespace OpenAC.Net.NFSe.Demo
{
    public static class Helpers
    {
        public static string OpenFile(string filters, string title = "Abrir")
        {
            using var ofd = new OpenFileDialog();
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            ofd.Multiselect = false;
            ofd.Filter = filters;
            ofd.Title = title;

            if (ofd.ShowDialog().Equals(DialogResult.Cancel))
                return null;

            return ofd.FileName;
        }

        public static string[] OpenFiles(string filters, string title = "Abrir")
        {
            using var ofd = new OpenFileDialog();
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            ofd.Multiselect = true;
            ofd.Filter = filters;
            ofd.Title = title;

            if (ofd.ShowDialog().Equals(DialogResult.Cancel))
                return null;

            return ofd.FileNames;
        }

        public static string SelectFolder()
        {
            using var fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            return fbd.ShowDialog().Equals(DialogResult.Cancel) ? string.Empty : fbd.SelectedPath;
        }
    }
}