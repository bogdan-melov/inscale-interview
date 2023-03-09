namespace InScale.Contracts.Exceptions
{
    using FluentResults;
    using System;
    using System.Collections.Generic;

    public static class ErrorLogExtension
    {
        public static void LogErrors(this List<IError> errors)
        {
            foreach (IError error in errors)
            {
                Console.Error.WriteLine(error);
            }
        }
    }
}
