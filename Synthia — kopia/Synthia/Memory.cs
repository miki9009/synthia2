using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace Synthia
{
    class Memory
    {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32")]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        //ŚCIEŻKA DO PLIKU
        public Memory(string DataPath = null)
        {
            Path = new FileInfo(DataPath ?? EXE + ".dat").FullName.ToString();
        }
        //ODCZYTAJ WARTOŚć
        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(200);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 2048, Path);
            return RetVal.ToString();
        }
        //ZAPISZ WARTOŚć
        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        }
        //USUŃ KLUCZ
        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }

        //USUŃ SEKCJE
        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        //SPRAWDŹ CZY KLUCZ ISTNIEJE
        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }
    }
}
