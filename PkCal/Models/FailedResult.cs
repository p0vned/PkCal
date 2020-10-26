namespace PkCal.Models
{
    class FailedResult : Result
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
