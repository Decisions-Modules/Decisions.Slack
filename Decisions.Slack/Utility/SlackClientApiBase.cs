using DecisionsFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.Slack.Utility
{
    static partial class SlackClientApi
    {
        private static readonly string baseAdress = "https://slack.com/api/";

        private static AuthenticationHeaderValue GetAuthHeader(string accessToken)
        {
            return new AuthenticationHeaderValue("Bearer", accessToken);
        }

        private static HttpClient GetClient(string accessToken)
        {
            HttpClient httpClient = new HttpClient { BaseAddress = new Uri(baseAdress) };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            httpClient.DefaultRequestHeaders.Authorization = GetAuthHeader(accessToken);
            return httpClient;
        }

        private static R ParseResponse<R>(HttpResponseMessage response) where R : SlackResponseModel, new()
        {
            var responseString = response.Content.ReadAsStringAsync().Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new SlackException(response.ReasonPhrase, response.StatusCode);
            };

            var result = JsonConvert.DeserializeObject<R>(responseString);

            if (!result.Ok)
            {
                throw new SlackException(result.Error, response.StatusCode);
            };

            return result;
        }

        private static R GetRequest<R>(string accesssToken, string requestUri) where R : SlackResponseModel, new()
        {
            HttpResponseMessage response = GetClient(accesssToken).GetAsync(requestUri).Result;
            return ParseResponse<R>(response);
        }

        private static R PostRequest<R>(string accesssToken, string requestUri) where R : SlackResponseModel, new()
        {
            HttpResponseMessage response = GetClient(accesssToken).PostAsync(requestUri, null).Result;
            return ParseResponse<R>(response);
        }

        // For Slack text message we need to escape just a few characters. So we do it here without any library
        private static string UnescapeSlackText(string text)
        {
            string res = text;
            if (!string.IsNullOrEmpty(res))
            {
                res = res.Replace("&gt;", ">");
                res = res.Replace("&lt;", "<");
                res = res.Replace("&amp;", "&");
            }
            return res;
        }
    }
}
