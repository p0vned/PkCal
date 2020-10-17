using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PkCal.Models
{
    public class FailedResult : Result
    {
        public string Message { get; private set; }

        public FailedResult(string message)
        {
            Success = false;
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
