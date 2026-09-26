namespace HomeLibrary.Services;

public sealed class TocValidationException : Exception
{
    public TocValidationException(string message)
        : base(message)
    {
    }

    public TocValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
