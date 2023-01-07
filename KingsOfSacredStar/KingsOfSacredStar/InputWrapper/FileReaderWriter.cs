using System.Collections.Generic;
using System.IO;
using System.Text;


namespace KingsOfSacredStar.InputWrapper
{
    internal static class FileReaderWriter
    {
        private const string DataType = ".koss";
        public static string[] GetFullFile(string path)
        {
            return File.ReadAllLines(path + DataType, Encoding.UTF8);
        }

        public static bool FileExists(string path)
        {
            return File.Exists(path + DataType);
        }

        public static void CreateParentDirectories(string path)
        {
            Directory.CreateDirectory(path);
            Directory.Delete(path);
        }
        public static void WriteFile(string path, IEnumerable<string> data)
        {
            CreateParentDirectories(path);
            File.WriteAllLines(path + DataType, data, Encoding.UTF8);
        }
    }
}
