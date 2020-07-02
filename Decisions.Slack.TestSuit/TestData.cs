using DecisionsFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Decisions.Slack.TestSuit
{
    static class TestData
    {
        private static RandomStringGenerator stringGenerator = new RandomStringGenerator(15);


        public static string AccessToken = "xoxp-1240761765984-1217134052291-1223306723748-a8994b1b7c73bfb0952498455fc322cb";

        public static string GetTestMessage()
        {
            return "Test message " + stringGenerator.Generate();
        }

        public static string TextForSearch = "message";

        public static string WelcomeChannelName = "welcome";
        public static string GetTestChannelName()
        {
            var date = DateTime.UtcNow.ToString("u");
            date = Regex.Replace(date, "[ \\.:]", "-").ToLower();
            return $"test-channel-{date}";
        }

    }
}
