using System;

namespace GYB.Common
{
    /// <summary>
    ///     远程接口返回异常
    /// </summary>
    public class RemoteApiException : Exception
    {
        public RemoteApiException(string url, string resMsg)
        {
            Url = url;
            ResponseMsg = resMsg;
        }

        public string Url { get; set; }

        public string ResponseMsg { get; set; }
    }
}
