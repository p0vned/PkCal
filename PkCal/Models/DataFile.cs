using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PkCal.Models
{
    public class DataFile
    {
        public string Path { get; private set; }

        public string Content { get; private set; }

        public bool Exists { get; private set; }

        public DataFile(string path)
        {
            Path = path;
            Exists = false;
        }

        public void CheckIfExists()
        {
            if (string.IsNullOrEmpty(Path))
                throw new Exception("File path is empty!");

            Exists = File.Exists(Path);
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
