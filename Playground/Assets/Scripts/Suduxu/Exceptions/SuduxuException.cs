using System;

[Serializable]
public class SuduxuException : Exception
{
    public SuduxuException() { }

    public SuduxuException(string message)
        : base(message)
    { }

    public SuduxuException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
