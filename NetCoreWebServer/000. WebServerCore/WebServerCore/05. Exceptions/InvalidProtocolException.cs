namespace WebServerCore
{
    public class InvalidProtocolException : Exception
    {
        public InvalidProtocolException() { }
        public InvalidProtocolException(string message) : base(message) { }
        public InvalidProtocolException(string message, Exception e) : base(message, e) { }
    }
}
