using System;

namespace Application.Exceptions
{
    public class NotAssignedDbConnectionNameException : Exception
    {
        public override string Message => _message;

        private readonly string _message;

        public NotAssignedDbConnectionNameException(string message = "CollectionName değeri belirtilmedi")
        {
            _message = message;
        }
    }
}
