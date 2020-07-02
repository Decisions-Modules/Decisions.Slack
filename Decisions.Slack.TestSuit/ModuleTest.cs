using System;
using DecisionsFramework.Data.ORMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Decisions.Slack;
using Decisions.Slack.Data;
using System.Linq;
using DecisionsFramework.ServiceLayer.Services.DBQuery;
using System.Text;

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

            var channels2 = SlackClientApi.GetChannelsList(" ");
        }

        [TestMethod]
        public void TestListUsersInChannelByChannelId()
        {
            Channel[] channels = SlackClientApi.GetChannelsList(AccessToken);

            int userCount = 0;
            foreach (var ch in channels)
            {
                var users = SlackClientApi.ListUsersInChannelByChannelId(AccessToken, ch.Id);
                userCount += users.Length;
            }

            Assert.IsTrue(userCount > 0);
        }

        [TestMethod]
        public void TestListUsersInChannelByChannelName()
        {
            Channel[] channels = SlackClientApi.GetChannelsList(AccessToken);

            int userCount = 0;
            foreach (var ch in channels)
            {
                var users = SlackClientApi.ListUsersInChannelByChannelName(AccessToken, ch.Name);
                userCount += users.Length;
            }

            Assert.IsTrue(userCount > 0);
        }


        [TestMethod]
        public void TestCreateChannel()
        {
            string testChannelName = TestData.GetTestChannelName();
            try
            {
                var res = SlackClientApi.CreateChannel(AccessToken, testChannelName);
                Assert.IsTrue(res);
            }
            finally
            {
                SlackClientApi.ArchiveChannelByChannelName(AccessToken, testChannelName);
            }
        }

        [TestMethod]
        public void TestArchiveChannel()
        {
            string testChannelName = TestData.GetTestChannelName();
            var res = SlackClientApi.CreateChannel(AccessToken, testChannelName);
            try
            {
                string channelId = SlackClientApi.GetChannelIdByName(AccessToken, testChannelName);
                SlackClientApi.ArchiveChannelByChannelId(AccessToken, channelId);
            }
            finally
            {
                try
                {
                    SlackClientApi.ArchiveChannelByChannelName(AccessToken, testChannelName);
                }
                catch { };
            }
        }

        [TestMethod]
        public void TestPostMessage()
        {
            string channelName = TestData.WelcomeChannelName;
            try
            {
                bool res = SlackClientApi.PostMessageToChannelByChannelName(AccessToken, channelName, TestData.GetTestMessage());
                Assert.IsTrue(res);
            }
            finally
            {
            }
        }

        [TestMethod]
        public void TestDeleteMessage()
        {
            string channelName = TestData.WelcomeChannelName;

            SlackClientApi.PostMessageToChannelByChannelName(AccessToken, channelName, TestData.GetTestMessage());
            Message[] messages = SlackClientApi.GetRecentMessagesFromChannelByChannelName(AccessToken, channelName);
            var pinMessage = messages.First();
            bool res = SlackClientApi.DeleteMessageFromChannelByChannelName(AccessToken, channelName, pinMessage.ts);
            Assert.IsTrue(res);
        }


        [TestMethod]
        public void TestGetRecentMessagesFromChannelByChannelName()
        {
            string channelName = TestData.WelcomeChannelName;
            try
            {
                Message[] messages = SlackClientApi.GetRecentMessagesFromChannelByChannelName(AccessToken, channelName);
                Assert.IsTrue(messages.Length > 0);
            }
            finally
            {
            }
        }

        [TestMethod]
        public void TestPinMessage()
        {
            string channelName = TestData.WelcomeChannelName;
            SlackClientApi.PostMessageToChannelByChannelName(AccessToken, channelName, TestData.GetTestMessage());
            Message[] messages = SlackClientApi.GetRecentMessagesFromChannelByChannelName(AccessToken, channelName);
            var pinMessage = messages.First();
            try
            {
                bool res = SlackClientApi.PinMessageToChannelByChannelName(AccessToken, channelName, pinMessage.ts);
                Assert.IsTrue(res);
            }
            finally
            {
                SlackClientApi.DeleteMessageFromChannelByChannelName(AccessToken, channelName, pinMessage.ts);
            }
        }

        [TestMethod]
        public void TestUnpinMessage()
        {
            string channelName = TestData.WelcomeChannelName;
            Message[] messages = SlackClientApi.GetRecentMessagesFromChannelByChannelName(AccessToken, channelName);
            var pinMessage = messages.First();
            SlackClientApi.PinMessageToChannelByChannelName(AccessToken, channelName, pinMessage.ts);

            bool res = SlackClientApi.UnpinMessageToChannelByChannelName(AccessToken, channelName, pinMessage.ts);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void TestSearchForTextInChannels()
        {
            string channelName = TestData.WelcomeChannelName;
            int count = 40;
            int page = 1;
            Matches[] searchResult = new Matches[] { };
            Matches[] items = null;
            do
            {
                items = SlackClientApi.SearchForTextInChannels(AccessToken, TestData.TextForSearch, count, page++);
                searchResult = searchResult.Concat(items).ToArray();
            } while (items.Length > 0);

            Assert.IsTrue(searchResult.Length > 0);

        }

        [TestMethod]
        public void TestOpenChannelWithUser()
        {
            var users = SlackClientApi.ListUsersInChannelByChannelName(AccessToken, TestData.WelcomeChannelName);

            bool res = SlackClientApi.OpenChannelWithUser(AccessToken, users[0].id);
            Assert.IsTrue(res);
        }

    }


}
