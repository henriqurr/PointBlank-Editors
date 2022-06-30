using PropertyEditor.Models;
using PropertyEditor.Models.Enums;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PropertyEditor
{
    public class Utils
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static void ShowConsole(bool visible)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, visible ? SW_SHOW : SW_HIDE);
        }

        public static void LoadSettings()
        {
            try
            {
                if (File.Exists(string.Format("{0}/config.data", Application.StartupPath)))
                {
                    using (FileStream fs = new FileStream(string.Format("{0}/config.data", Application.StartupPath), FileMode.Open))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            Settings.ShowConsole = br.ReadBoolean();
                            Settings.Nation = (Nation)br.ReadInt32();
                        }
                        fs.Close();
                    }
                }
                else
                {
                    using (FileStream fs = new FileStream(string.Format("{0}/config.data", Application.StartupPath), FileMode.Create))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            bw.Write(Settings.ShowConsole);
                            bw.Write((int)Settings.Nation);
                        }
                        fs.Close();
                    }
                }

                //Load all global configs
                ShowConsole(Settings.ShowConsole);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void LoadConsoleHeader()
        {
            Console.Title = Application.ProductName + " - Debug Console";
            Console.CursorVisible = false;
            Console.WriteLine(@"
______                          _           _____    _ _ _             
| ___ \                        | |         |  ___|  | (_) |            
| |_/ / __ ___  _ __   ___ _ __| |_ _   _  | |__  __| |_| |_ ___  _ __ 
|  __/ '__/ _ \| '_ \ / _ \ '__| __| | | | |  __|/ _` | | __/ _ \| '__|
| |  | | | (_) | |_) |  __/ |  | |_| |_| | | |__| (_| | | || (_) | |   
\_|  |_|  \___/| .__/ \___|_|   \__|\__, | \____/\__,_|_|\__\___/|_|   
               | |                   __/ |                             
               |_|                  |___/                              
");
            Console.WriteLine("Developed by Exploit Network");
            Console.WriteLine("DEVS: Coyote, PISTOLA, Uzumendiz\n");
        }

        public static Color HexToColor(string hexString)
        {
            //replace # occurences
            if (hexString.IndexOf('#') != -1)
                hexString = hexString.Replace("#", "");

            int r, g, b, a;

            r = int.Parse(hexString.Substring(0, 2), NumberStyles.AllowHexSpecifier);
            g = int.Parse(hexString.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            b = int.Parse(hexString.Substring(4, 2), NumberStyles.AllowHexSpecifier);
            a = int.Parse(hexString.Substring(6, 2), NumberStyles.AllowHexSpecifier);

            return Color.FromArgb(a, r, g, b);
        }

        public static string ArgbToHex(Color myColor)
        {
            return myColor.R.ToString("X2") + myColor.G.ToString("X2") + myColor.B.ToString("X2") + myColor.A.ToString("X2");
        }
    }
}
