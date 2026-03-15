using System;

public class SuduxuException : Exception
{
    public SuduxuException()
    {
    }

    public SuduxuException(string message) : base(message)
    {
    }
}

public class SuduxuInvalidJsonException : SuduxuException { }

public class SuduxuNetworkException : SuduxuException { }

public class SuduxuNotRunningException : SuduxuException { }

public class SuduxuAlreadyRunningException : SuduxuException { }

public class SuduxuInternalException : SuduxuException { }

public class SuduxuClientNotFoundException : SuduxuException { }
public class SuduxuInvalidInputException : SuduxuException { }