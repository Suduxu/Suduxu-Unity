public enum FFIStatus : int
{
    Success = 0,
    InvalidJson = 1,
    NetworkError = 2,
    NotRunning = 3,
    AlreadyRunning = 4,
    InternalError = 5,
    ClientNotFound = 6,
    InvalidInput = 7,
}

public static class FFIStatusExtensions
{
    public static FFIStatus ToFFIStatus(this int code)
    {
        return (FFIStatus)code;
    }

    public static void ThrowIfException(this FFIStatus status)
    {
        SuduxuException exception = status switch
        {
            FFIStatus.Success => null,
            FFIStatus.InvalidJson => new SuduxuInvalidJsonException(),
            FFIStatus.NetworkError => new SuduxuNetworkException(),
            FFIStatus.NotRunning => new SuduxuNotRunningException(),
            FFIStatus.AlreadyRunning => new SuduxuAlreadyRunningException(),
            FFIStatus.InternalError => new SuduxuInternalException(),
            FFIStatus.ClientNotFound => new SuduxuClientNotFoundException(),
            FFIStatus.InvalidInput => new SuduxuInvalidInputException(),
            _ => new SuduxuException()
        };

        if (exception != null)
        {
            throw exception;
        }
    }
}