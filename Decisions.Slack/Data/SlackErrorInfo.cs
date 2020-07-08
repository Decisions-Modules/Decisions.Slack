using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.Slack
{
    [DataContract]
    public class SlackErrorInfo
    {
        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public HttpStatusCode? HttpErrorCode { get; set; }

        internal static SlackErrorInfo FromException(Exception ex)
        {
            return new SlackErrorInfo()
            {
                ErrorMessage = (ex.Message ?? ex.ToString()),
                HttpErrorCode = (ex as SlackException)?.HttpErrorCode
            };
        }

        override public String ToString()
        {
            if (HttpErrorCode == null) return ErrorMessage;
            else return ErrorMessage + "\nHttpErrorCode = " + HttpErrorCode;
        }
    }

    internal class SlackException : Exception
    {
        public HttpStatusCode? HttpErrorCode { get; set; }
        public SlackException(string message, HttpStatusCode? httpStatus = null) : base(message)
        {
            HttpErrorCode = httpStatus;
        }
    }
}
