using System;
using System.IO;

namespace PkCal.Models
{
    class DataFile
    {
        public string Path { get; private set; }

        public string Content { get; private set; }

        public bool Exists { get { return File.Exists(Path); } }

        public DataFile(string path)
        {
            Path = path;
        }

        public Result SetContent(string value)
        {
            Content = value;

            return new SuccessResult();
        }

        public Result SaveContentToFile()
        {
            try
            {
                File.WriteAllText(Path, Content);
            }
            catch (Exception e)
            {
                return new FailedResult(e.Message);
            }

            return new SuccessResult();
        }

        public Result ReadContentFromFile()
        {
            if (!Exists)
                return new FailedResult("Nie można odczytać, plik nie istnieje!");

            try
            {
                Content = File.ReadAllText(Path);
            }
            catch (Exception e)
            {
                return new FailedResult(e.Message);
            }

            return new SuccessResult();
        }
    }
}
