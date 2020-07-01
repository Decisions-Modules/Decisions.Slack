using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackClient;

namespace Decisions.Slack.TestSuit
{


    [TestClass]
    public class ModuleTest
    {
        private string AccessToken { get { return TestData.AccessToken; } }

        [TestMethod]
        public void TestGetChannelsList()
        {
            var channels = SlackClientApi.GetChannelsList(AccessToken);
            Assert.IsTrue(channels.Length > 0);
        }
    }
}
