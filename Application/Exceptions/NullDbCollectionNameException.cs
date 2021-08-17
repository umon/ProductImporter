using System;

namespace Application.Exceptions
{
    public class NullDbCollectionNameException : Exception
    {
        public override string Message => _message;

        private readonly string _message;

        public NullDbCollectionNameException(string message = "CollectionName değeri boş olamaz")
        {
            _message = message;
        }
    }
}
