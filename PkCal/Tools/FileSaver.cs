using PkCal.Models;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace PkCal.Tools
{
    public class FileSaver
    {
        private string _content;
        public string Path { get; private set; }


        public FileSaver(string saveFilePath, string contentToSave)
        {
            Path = saveFilePath;
            _content = contentToSave;
        }

        public Result SaveFile()
        {
            Result checkFileProperties = ValidateSavingProperties();

            if (!checkFileProperties.Success)
                return checkFileProperties;
            
            try
            {
                File.WriteAllText(Path.ToString(), _content);
            }
            catch (Exception e)
            {
                return new FailedResult(e.Message);
            }

            return new SuccessResult();
        }

        private Result ValidateSavingProperties()
        {       
            if (string.IsNullOrEmpty(_content))
                return new FailedResult("Zawartość zapisanego pliku nie może być pusta!");

            return new SuccessResult();
        }
    }
}
