namespace InScale.Contracts.Exceptions
{
    using System;
    using System.Net;

    public class NoSqlException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; }

        public NoSqlException(string message) : base(message)
        {
        }
    }
}
