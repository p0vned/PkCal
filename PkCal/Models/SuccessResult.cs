namespace PkCal.Models
{
    class SuccessResult : Result
    {
        public SuccessResult()
        {
            Success = true;
        }

        public override string ToString()
        {
            return "Sukces";
        }
    }
}
