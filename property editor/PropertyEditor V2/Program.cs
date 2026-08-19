using System;
using System.Drawing;
using System.Windows.Forms;

namespace PropertyEditor
{
    protected static class Program
    {
        public static PropertyEditorView _propertyEditor;

        [STAThread]
        static void Main()
        {
            Utils.LoadConsoleHeader();
            Utils.LoadSettings();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _propertyEditor = new PropertyEditorView();
            Application.Run(_propertyEditor);
        }
    }
}
