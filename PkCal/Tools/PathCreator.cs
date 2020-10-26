using System;
using System.Text;
using System.Text.RegularExpressions;

namespace PkCal.Tools
{
    class PathCreator
    {
        private const string DirNamePattern = @"^\w+$";
        private const string FileNamePattern = @"^\w+\.\w+$";

        public string Path { get; private set; }

        public PathCreator(string initialPath)
        {
            Path = initialPath;
        }

        public void AddDirectory(string dirName)
        {
            bool isDirNameValid = Regex.IsMatch(dirName, DirNamePattern);

            if (!isDirNameValid)
                throw new ArgumentException("Wrong directory name!");


            var sb = new StringBuilder(Path);
            sb.Append("\\");
            sb.Append(dirName);

            Path = sb.ToString();
        }

        public void AddFile(string fileName)
        {
            bool isFileNameValid = Regex.IsMatch(fileName, FileNamePattern);

            if (!isFileNameValid)
                throw new ArgumentException("Wrong file name!");


            var sb = new StringBuilder(Path);
            sb.Append("\\");
            sb.Append(fileName);

            Path = sb.ToString();
        }
    }
}
