using System;
using DecisionsFramework.Data.ORMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Decisions.Slack.Utility;
using System.Linq;

namespace Decisions.Slack.TestSuit
{
    [TestClass]
    public class ModuleTest
    {
        private string AccessToken { get { return TestData.AccessToken; } }

        [TestInitialize]
        public void Initialize()
        {
            SlackClientApi.PaginationLimit = 2;
        }

        [TestMethod]
        public void TestGetChannelsList()
        {
            var channels = SlackClientApi.GetChannelList(AccessToken, false);
            Assert.IsTrue(channels.Length > 0);
        }

        [TestMethod]
        public void TestListUsersInChannelByChannelId()
        {
            SlackChannel[] channels = SlackClientApi.GetChannelList(AccessToken, true);

            int userCount = 0;
            foreach (var ch in channels)
            {
                var users = SlackClientApi.GetUserListInChannel(AccessToken, ch.Id);
                userCount += users.Length;
            }

            Assert.IsTrue(userCount > 0);
        }

        [TestMethod]
        public void TestCreateChannel()
        {
            string testChannelName = TestData.GetTestChannelName();
            SlackChannel channel = null;
            try
            {
                channel = SlackClientApi.CreateChannel(AccessToken, testChannelName, false);
                Assert.IsNotNull(channel);
            }
            finally
            {
                if (channel != null)
                    SlackClientApi.ArchiveChannel(AccessToken, channel.Id);
            }
        }

        [TestMethod]
        public void TestOpenChannelWithUser()
        {
            string channelId = SlackClientApi.GetChannelIdByName(AccessToken, TestData.WelcomeChannelName);
            var users = SlackClientApi.GetUserListInChannel(AccessToken, channelId);

            SlackChannel channel = SlackClientApi.OpenChannelWithUsers(AccessToken, users[0].Id);
            Assert.IsNotNull(channel);
        }

        [TestMethod]
        public void TestInviteToChannel()
        {
            var users = SlackClientApi.GetUserListInChannel(AccessToken, SlackClientApi.GetChannelIdByName(AccessToken, TestData.WelcomeChannelName));

            string invitedUserId = users[0].Id;
            string testChannelName = TestData.GetTestChannelName();
            SlackChannel channel = null;
            try
            {
                channel = SlackClientApi.CreateChannel(AccessToken, testChannelName, false);

                SlackClientApi.InviteToChannel(AccessToken, channel.Id, invitedUserId);

                var invitedUsers = SlackClientApi.GetUserListInChannel(AccessToken, channel.Id);
                var u = invitedUsers.FirstOrDefault((it) => { return it.Id == invitedUserId; });
                Assert.IsNotNull(u);
            }
            finally
            {
                if (channel != null)
                    SlackClientApi.ArchiveChannel(AccessToken, channel.Id);
            }
        }

        [TestMethod]
        public void TestArchiveChannel()
        {
            string testChannelName = TestData.GetTestChannelName();
            SlackChannel channel = SlackClientApi.CreateChannel(AccessToken, testChannelName, false);
            try
            {
                string channelId = SlackClientApi.GetChannelIdByName(AccessToken, testChannelName);
                SlackClientApi.ArchiveChannel(AccessToken, channelId);
                channel = null;
            }
            finally
            {
                if (channel != null)
                    SlackClientApi.ArchiveChannel(AccessToken, channel.Id);
            }
        }

        [TestMethod]
        public void TestGetRecentMessagesFromChannelByChannelId()
        {
            string channelId = SlackClientApi.GetChannelIdByName(AccessToken, TestData.WelcomeChannelName);
            try
            {
                string lastTimestamp = null;

                SlackMessage[] messages = SlackClientApi.GetMessagesFromChannel(AccessToken, channelId, 10000);
                Assert.IsTrue(messages.Length > 0);

                if (messages.Length < 5)
                    for (int i = 0; i < 5; i++)
                        SlackClientApi.PostMessageToChannel(AccessToken, channelId, TestData.GetTestMessage());

                SlackMessage[] messages1 = SlackClientApi.GetMessagesFromChannel(AccessToken, channelId, 1);
                Assert.IsTrue(messages1.Length == 1);
                lastTimestamp = messages1.Last().Timestamp;

                SlackMessage[] messages2 = SlackClientApi.GetMessagesFromChannel(AccessToken, channelId, 2, lastTimestamp);
                Assert.IsTrue(messages2.Length == 2);
                Assert.AreNotEqual(lastTimestamp, messages2[0].Timestamp);
                lastTimestamp = messages2.Last().Timestamp;

                SlackMessage[] messages3 = SlackClientApi.GetMessagesFromChannel(AccessToken, channelId, 3, lastTimestamp);
                Assert.IsTrue(messages3.Length == 3);
                Assert.AreNotEqual(lastTimestamp, messages3[0].Timestamp);
            }
            finally
            {
            }
        }

        [TestMethod]
        public void TestPostMessage()
        {
            string channelId = SlackClientApi.GetChannelIdByName(AccessToken, TestData.WelcomeChannelName);
            try
            {
                var text = TestData.GetTestMessage();
                var message = SlackClientApi.PostMessageToChannel(AccessToken, channelId, text);
                Assert.AreEqual(text, message.Text);

                var specialText = ">Special characters: \" ' & < > \n        should look good";
                var message2 = SlackClientApi.PostMessageToChannel(AccessToken, channelId, specialText);
                Assert.AreEqual(specialText, message2.Text);

                SlackMessage lastMessage = SlackClientApi.GetMessagesFromChannel(AccessToken, channelId, 1)[0];
                Assert.AreEqual(specialText, lastMessage.Text);
            }
            finally
            {
            }
        }

        [TestMethod]
        public void TestDeleteMessage()
        {
            string channelId = SlackClientApi.GetChannelIdByName(AccessToken, TestData.WelcomeChannelName);

            var newMessage = SlackClientApi.PostMessageToChannel(AccessToken, channelId, TestData.GetTestMessage());
            SlackClientApi.DeleteMessageFromChannel(AccessToken, channelId, newMessage.Timestamp);

        }


        [TestMethod]
        public void TestPinMessage()
        {
            string channelId = SlackClientApi.GetChannelIdByName(AccessToken, TestData.WelcomeChannelName);
            var pinnedMessage = SlackClientApi.PostMessageToChannel(AccessToken, channelId, TestData.GetTestMessage());

            try
            {
                SlackClientApi.PinMessage(AccessToken, channelId, pinnedMessage.Timestamp);
                var lastMessage = SlackClientApi.GetMessagesFromChannel(AccessToken, channelId, 1)[0];
                Assert.IsNotNull(lastMessage.PinnedTo);
                Assert.AreEqual(channelId, lastMessage.PinnedTo[0]);
            }
            finally
            {
                SlackClientApi.DeleteMessageFromChannel(AccessToken, channelId, pinnedMessage.Timestamp);
            }
        }

        [TestMethod]
        public void TestUnpinMessage()
        {
            string channelId = SlackClientApi.GetChannelIdByName(AccessToken, TestData.WelcomeChannelName);
            var pinnedMessage = SlackClientApi.PostMessageToChannel(AccessToken, channelId, TestData.GetTestMessage());
            try
            {
                SlackClientApi.PinMessage(AccessToken, channelId, pinnedMessage.Timestamp);
                SlackClientApi.UnpinMessage(AccessToken, channelId, pinnedMessage.Timestamp);

                var lastMessage = SlackClientApi.GetMessagesFromChannel(AccessToken, channelId, 1)[0];
                Assert.IsNull(lastMessage.PinnedTo);
            }
            finally
            {
                SlackClientApi.DeleteMessageFromChannel(AccessToken, channelId, pinnedMessage.Timestamp);
            }
        }

        [TestMethod]
        public void TestSearchForTextInChannels()
        {
            string channelName = TestData.WelcomeChannelName;

            SlackMatches[] searchResult = SlackClientApi.SearchForTextInChannels(AccessToken, TestData.TextForSearch, 10000);
            Assert.IsTrue(searchResult.Length <= 10000);

            SlackMatches[] searchResult2 = SlackClientApi.SearchForTextInChannels(AccessToken, TestData.TextForSearch, 33);
            Assert.IsTrue(searchResult.Length <= 33);

            SlackMatches[] searchResult3 = SlackClientApi.SearchForTextInChannels(AccessToken, TestData.TextForSearch, 1);
            Assert.IsTrue(searchResult3.Length <= 1);

            SlackMatches[] searchResult4 = SlackClientApi.SearchForTextInChannels(AccessToken, "& <", 10000);
            Assert.IsTrue(searchResult4.Length > 0);
            foreach (var it in searchResult4)
            {
                Assert.IsTrue(it.Text.Contains("& <"));
            }

        }


    }


}
