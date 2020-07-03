using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.Slack.Data
{
    internal class SlackException : Exception
    {
        public HttpStatusCode? HttpStatus { get; set; }
        public SlackException(string message, HttpStatusCode? httpStatus = null) : base(message)
        {
            HttpStatus = httpStatus;
        }
    }
}
