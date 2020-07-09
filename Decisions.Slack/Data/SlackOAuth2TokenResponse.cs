using Decisions.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.Slack
{
    internal class AuthedUser
    {
        public string Id { get; set; }
        public string Scope { get; set; }

        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }
    }

    internal class SlackOAuth2TokenResponse : OAuthTokenService.OAuth2TokenResponse
    {
        [JsonProperty(PropertyName = "authed_user")]
        public AuthedUser AuthedUser { get; set; }
    }
}
