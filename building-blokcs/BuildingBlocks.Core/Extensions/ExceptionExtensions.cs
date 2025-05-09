namespace BuildingBlocks.Core.Extensions
{
    public static class ExceptionExtensions
    {
        public static System.Exception FindInnerExceptionRecursively(this System.Exception exception)
        {
            if (exception.InnerException == null)
                return exception;

            return FindInnerExceptionRecursively(exception.InnerException);
        }
    }
}
