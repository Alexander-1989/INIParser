using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace System.IniParser
{
    class INIFile
    {
        readonly string FileName = null;

        public INIFile()
        {
            FileName = ".\\Config.ini";
        }

        public INIFile(string FileName)
        {
            this.FileName = Path.GetFullPath(FileName);
        }

        public bool FileExists
        {
            get { return File.Exists(FileName); }
        }

        public void Write<T>(string Section, string Key, T Value) where T : IConvertible
        {
            if (Section == null) throw new ArgumentNullException("Section");
            if (Key == null) throw new ArgumentNullException("Key");
            if (Value == null) throw new ArgumentNullException("Value");
            SafeNativeMethods.WritePrivateProfileString(Section, Key, Value.ToString(), FileName);
        }

        public string Read(string Section, string Key)
        {
            if (Section == null) throw new ArgumentNullException("Section");
            if (Key == null) throw new ArgumentNullException("Key");
            StringBuilder result = new StringBuilder(255);
            SafeNativeMethods.GetPrivateProfileString(Section, Key, null, result, result.Capacity, FileName);
            return result.ToString();
        }

        public int Parse(string Section, string Key)
        {
            int result;
            int.TryParse(Read(Section, Key), out result);
            return result;
        }

        public T Parse<T>(string Section, string Key) where T : struct, IConvertible
        {
            T result;
            if (typeof(T).IsEnum) Enum.TryParse(Read(Section, Key), out result);
            else result = (T)Convert.ChangeType(Read(Section, Key), typeof(T));
            return result;
        }

        public bool SectionExists(string Section)
        {
            char[] res = new char[255];
            SafeNativeMethods.GetPrivateProfileString(null, null, null, res, res.Length, FileName);
            return new string(res).Contains(Section);
        }

        public string[] GetSections()
        {
            return GetKeys(null);
        }

        public void DeleteSection(string Section)
        {
            SafeNativeMethods.WritePrivateProfileString(Section, null, null, FileName);
        }

        public bool KeyExists(string Section, string Key)
        {
            char[] res = new char[255];
            SafeNativeMethods.GetPrivateProfileString(Section, null, null, res, res.Length, FileName);
            return new string(res).Contains(Key);
        }

        public string[] GetKeys(string Section)
        {
            char[] result = new char[255];
            SafeNativeMethods.GetPrivateProfileString(Section, null, null, result, result.Length, FileName);
            return new string(result).Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public void DeleteKey(string Section, string Key)
        {
            SafeNativeMethods.WritePrivateProfileString(Section, Key, null, FileName);
        }

        private static class SafeNativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "WritePrivateProfileString")]
            internal static extern int WritePrivateProfileString(string Section, string Key, string Value, string FileName);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetPrivateProfileString")]
            internal static extern int GetPrivateProfileString(string Section, string Key, string Default, char[] Result, int Size, string FileName);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetPrivateProfileString")]
            internal static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder Result, int Size, string FileName);
        }
    }
}