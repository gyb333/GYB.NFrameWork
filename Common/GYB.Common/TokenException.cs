using System;

namespace GYB.Common
{
    /// <summary>
    ///返回异常
    /// </summary>
    public class TokenException : Exception
    {

        public TokenException()
        {
             
        }
         

        public TokenException(string message, Exception innerException):base(message, innerException)
        {

        }

    }
}
