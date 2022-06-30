using System.Runtime.InteropServices;

namespace i3PackTool
{
    public class i3ResourceFile
    {
        [DllImport("i3BaseDx.dll", EntryPoint = "?Decrypt@i3ResourceFile@@SAXPAE0GH@Z")]
        public static extern void Decrypt(byte[] a1, byte[] a2, ushort dec, int size);

        [DllImport("i3BaseDx.dll", EntryPoint = "?Encrypt@i3ResourceFile@@SAXPAE0GH@Z")]
        public static extern void Encrypt(byte[] a1, byte[] a2, ushort dec, int size);
    }
}
