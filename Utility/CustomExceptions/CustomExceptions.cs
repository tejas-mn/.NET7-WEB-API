using System;
namespace asp_net_web_api.API.ErrorHandling {

    public class MyAppException : Exception
    {
        public MyAppException()
        {
        }

        public MyAppException(string message) : base(message)
        {
        }

        public MyAppException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class ItemNotFoundException : MyAppException
    {
        public ItemNotFoundException()
        {
        }

        public ItemNotFoundException(string message) : base(message)
        {
        }

        public ItemNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class CategoryNotFoundException : MyAppException
    {
        public CategoryNotFoundException()
        {
        }

        public CategoryNotFoundException(string message) : base(message)
        {
        }

        public CategoryNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}