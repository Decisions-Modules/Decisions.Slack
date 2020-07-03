using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Decisions.Slack.Data;
using Decisions.Slack.Utility;
using Microsoft.Win32.SafeHandles;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;
using DecisionsFramework.ServiceLayer.Services.DBQuery;

namespace Decisions.Slack.Utility
{
    internal static class SlackEndpointNames
    {
        public const string postMessage = "chat.postMessage";
        public const string createChannel = "conversations.create";
        public const string inviteToChannel = "conversations.invite";
        public const string conversationsList = "conversations.list";
        public const string conversationsMembers = "conversations.members";
        public const string conversationsHistory = "conversations.history";
        public const string conversationsOpen = "conversations.open"; // post direct message to user-s
        public const string archiveChannel = "conversations.archive";
        public const string searchInChannels = "search.messages";
        public const string deleteMsgFromChannel = "chat.delete";
        public const string usersInfo = "users.info";
        public const string pinMsgToChannel = "pins.add";
        public const string removePinMsgToChannel = "pins.remove";
    }

    public static partial class SlackClientApi
    {
        private static int _paginationLimit = 100;
        public static int PaginationLimit
        {
            get => _paginationLimit;
            set
            {
                if (value > 1000) value = 1000;
                if (value < 1) value = 1;
                _paginationLimit = value;
            }
        }

        /// <summary>
        ///     Get channel id by the channel name. Can be used for convenience
        /// </summary>
        /// <param name="token">Access token</param>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public static string GetChannelIdByName(string token, string channelName)
        {
            var dict = GetChannelsDictionary(token);

            string channelId = null;
            dict.TryGetValue(channelName, out channelId);

            return channelId;
        }

        /// <summary>
        /// Get channels list
        /// https://api.slack.com/methods/conversations.list
        /// </summary>
        /// <param name="token">Access token</param>
        /// <param name="excludeArchived">Do not return archieved channels</param>
        /// <returns>Array of channels model</returns>
        public static SlackChannel[] GetChannelsList(string token, bool excludeArchived)
        {
            List<SlackChannel> channels = new List<SlackChannel>();

            ChannelsListResponseModel response = GetRequest<ChannelsListResponseModel>(token, $"{SlackEndpointNames.conversationsList}?exclude_archived={excludeArchived}&limit={PaginationLimit}");
            channels.AddRange(response.Channels);
            while (response.Metadata != null && !String.IsNullOrEmpty(response.Metadata.NextCursor))
            {
                response = GetRequest<ChannelsListResponseModel>(token, $"{SlackEndpointNames.conversationsList}?exclude_archived={excludeArchived}&limit={PaginationLimit}&cursor={response.Metadata.NextCursor}");
                channels.AddRange(response.Channels);
            }

            return channels.ToArray();
        }

        /// <summary>
        /// Create channel
        /// https://api.slack.com/methods/conversations.create
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="name">name of channel</param>
        /// <param name="isPrivate">create a private channel</param>
        /// <returns></returns>
        public static SlackChannel CreateChannel(string token, string name, bool isPrivate)
        {
            name = Uri.EscapeDataString(name);
            CreateChannelResponseModel channel = PostRequest<CreateChannelResponseModel>(token, $"{SlackEndpointNames.createChannel}?name={name}&is_private={isPrivate}");
            return channel.Channel;
        }

        /// <summary>
        /// Open channel with user(s)
        /// https://api.slack.com/methods/conversations.open
        /// </summary>
        /// <param name="token">Access token</param>
        /// <param name="userIds">Users ids</param>
        /// 
        public static SlackChannel OpenChannelWithUsers(string token, params string[] userIds)
        {
            string users = userIds.Aggregate((acc, it) => { return acc.Length == 0 ? it : acc + "," + it; });
            users = Uri.EscapeDataString(users);
            CreateChannelResponseModel request = PostRequest<CreateChannelResponseModel>(token, $"{SlackEndpointNames.conversationsOpen}?users={users}");
            return request.Channel;
        }

        /// <summary>
        /// Invite user(s) to channel
        /// https://api.slack.com/methods/conversations.invite
        /// </summary>
        /// <param name="token">Access token</param>
        /// <param name="channelId">Id of channel</param>
        /// <param name="userIds">Users ids</param>
        /// 
        public static SlackChannel InviteToChannel(string token, string channelId, params string[] userIds)
        {
            string users = userIds.Aggregate((acc, it) => { return acc.Length == 0 ? it : acc + "," + it; });
            users = Uri.EscapeDataString(users);
            channelId = Uri.EscapeDataString(channelId);
            CreateChannelResponseModel request = PostRequest<CreateChannelResponseModel>(token, $"{SlackEndpointNames.inviteToChannel}?channel={channelId}&users={users}");
            return request.Channel;
        }

        /// <summary>
        /// send channel to archive
        /// https://api.slack.com/methods/conversations.archive
        /// </summary>
        /// <param name="token">Access token</param>
        /// <param name="channelId">Id of channel</param>

        public static void ArchiveChannelByChannelId(string token, string channelId)
        {
            channelId = Uri.EscapeDataString(channelId);
            SlackResponseModel response = PostRequest<SlackResponseModel>(token, $"{SlackEndpointNames.archiveChannel}?channel={channelId}");
        }

        /// <summary>
        /// Get user list from a channel
        /// https://api.slack.com/methods/conversations.members
        /// https://api.slack.com/methods/users.info
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="channelId">Id of channel</param>
        /// <returns></returns>
        public static SlackUser[] ListUsersInChannelByChannelId(string token, string channelId)
        {
            List<string> members = new List<string>();

            channelId = Uri.EscapeDataString(channelId);
            ChannelMembersResponseModel response = GetRequest<ChannelMembersResponseModel>(token, $"{SlackEndpointNames.conversationsMembers}?channel={channelId}&limit={PaginationLimit}");
            members.AddRange(response.Members);

            while (response.Metadata != null && !String.IsNullOrEmpty(response.Metadata.NextCursor))
            {
                response = GetRequest<ChannelMembersResponseModel>(token, $"{SlackEndpointNames.conversationsMembers}?channel={channelId}&limit={PaginationLimit}&cursor={response.Metadata.NextCursor}");
                members.AddRange(response.Members);
            }

            var users = new List<SlackUser>();
            foreach (var id in members)
            {
                UserResponseModel usr = GetRequest<UserResponseModel>(token, $"{SlackEndpointNames.usersInfo}?token={token}&user={id}");
                users.Add(usr.User);
            }

            return users.ToArray();
        }

        /// <summary>
        /// Show recent messages from channel
        /// https://api.slack.com/methods/conversations.history
        /// </summary>
        /// <param name="token">Access token</param>
        /// <param name="channelName">name of channel</param>
        /// <param name="messagesToGetCount">the amount of messages to return</param>
        /// <param name="latestTimeStamp">the message timestamp you have already received</param>
        /// <returns></returns>

        public static SlackMessage[] GetMessagesFromChannelByChannelId(string token, string channelId, int messagesToGetCount = 100, string latestTimeStamp = null)
        {
            int limit = Math.Min(PaginationLimit, messagesToGetCount);
            var messages = new List<SlackMessage>();

            channelId = Uri.EscapeDataString(channelId);
            string request = $"{SlackEndpointNames.conversationsHistory}?channel={channelId}&limit={limit}";
            if (latestTimeStamp != null)
                request += $"&latest={latestTimeStamp}";

            MessageListResponseModel response = GetRequest<MessageListResponseModel>(token, request);
            messages.AddRange(response.Messages);

            while ((messagesToGetCount > messages.Count) && (response.Metadata != null) && !String.IsNullOrEmpty(response.Metadata.NextCursor))
            {
                limit = Math.Min(PaginationLimit, messagesToGetCount - messages.Count);
                response = GetRequest<MessageListResponseModel>(token, $"{SlackEndpointNames.conversationsHistory}?channel={channelId}&limit={limit}&cursor={response.Metadata.NextCursor}");
                messages.AddRange(response.Messages);
            }

            foreach (var msg in messages)
            {
                msg.Text = UnescapeSlackText(msg.Text);
            }
            return messages.ToArray();
        }

        /// <summary>
        /// send message to chanel
        /// https://api.slack.com/methods/chat.postMessage
        /// </summary>
        /// <param name="token">Access token</param>
        /// <param name="channelName">name of channel</param>
        /// <param name="text">message text</param>

        public static SlackMessage PostMessageToChannelByChannelId(string token, string channelId, string text)
        {
            channelId = Uri.EscapeDataString(channelId);
            string escapedText = Uri.EscapeDataString(text);

            PostMessageResponseModel response = PostRequest<PostMessageResponseModel>(token, $"{SlackEndpointNames.postMessage}?channel={channelId}&text={escapedText}");
            if (response.Message != null)
                response.Message.Text = UnescapeSlackText(response.Message.Text);
            return response.Message;
        }


        /// <summary>
        /// Pin message to channel 
        /// https://api.slack.com/methods/pins.add
        /// </summary>
        /// <param name="token">Access token</param>
        /// <param name="channelName">name of channel</param>
        /// <param name="timestamp">timestamp of message</param>
        public static void PinMessageToChannelByChannelId(string token, string channelId, string timestamp)
        {
            channelId = Uri.EscapeDataString(channelId);
            timestamp = Uri.EscapeDataString(timestamp);
            PostRequest<SlackResponseModel>(token, $"{SlackEndpointNames.pinMsgToChannel}?channel={Uri.EscapeDataString(channelId)}&timestamp={timestamp}");
        }

        /// <summary>
        /// Pin message to channel 
        /// </summary>
        /// <param name="token">Access token</param>
        /// <param name="channelName">name of channel</param>
        /// <param name="timestamp">timestamp of message</param>
        public static void UnpinMessageToChannelByChannelId(string token, string channelId, string timestamp)
        {
            channelId = Uri.EscapeDataString(channelId);
            timestamp = Uri.EscapeDataString(timestamp);
            PostRequest<SlackResponseModel>(token, $"{SlackEndpointNames.removePinMsgToChannel}?channel={channelId}&timestamp={timestamp}");
        }

        /// <summary>
        /// Delete message from channel
        /// https://api.slack.com/methods/chat.delete
        /// </summary>
        /// <param name="token">Access token</param>
        /// <param name="channelName">name of channel</param>
        /// <param name="timestamp">timestamp of message</param>
        public static void DeleteMessageFromChannelByChannelId(string token, string channelId, string timestamp)
        {
            channelId = Uri.EscapeDataString(channelId);
            timestamp = Uri.EscapeDataString(timestamp);
            PostRequest<SlackResponseModel>(token, $"{SlackEndpointNames.deleteMsgFromChannel}?channel={channelId}&ts={timestamp}");
        }

        /// <summary>
        /// search message in channels
        /// https://api.slack.com/methods/search.messages
        /// </summary>
        /// <param name="token">Access token</param>
        /// <param name="textToSearch"></param>
        /// <param name="count"> Maximum amount of results</param>
        /// <returns></returns>
        public static SlackMatches[] SearchForTextInChannels(string token, string textToSearch, int matchesToGetCount = 1000)
        {
            int SearchPaginationLimit = Math.Min(100, PaginationLimit); // count cannot be more than 100 for this request
            int count = Math.Min(SearchPaginationLimit, matchesToGetCount);
            var res = new List<SlackMatches>();

            textToSearch = Uri.EscapeDataString(textToSearch);
            MatchesResponseModel response = GetRequest<MatchesResponseModel>(token, $"{SlackEndpointNames.searchInChannels}?query={textToSearch}&count={count}");
            if (response.Messages != null && response.Messages.Matches != null)
            {
                res.AddRange(response.Messages.Matches);

                int currentPage = 2;
                while ((res.Count < matchesToGetCount) && (response.Messages.Pagination.PageCount >= currentPage) &&
                    (currentPage <= 100)) // page cannot be more than 100 for this request
                {
                    count = Math.Min(SearchPaginationLimit, matchesToGetCount - res.Count);
                    response = GetRequest<MatchesResponseModel>(token, $"{SlackEndpointNames.searchInChannels}?query={textToSearch}&count={count}&page={currentPage}");

                    if (response.Messages != null && response.Messages.Matches != null)
                        res.AddRange(response.Messages.Matches);
                    else
                        break;

                    currentPage++;
                }
            }

            foreach (var it in res)
                it.Text = UnescapeSlackText(it.Text);
            return res.ToArray();
        }



        private static Dictionary<string, string> GetChannelsDictionary(string token)
        {
            var channels = GetChannelsList(token, true);

            var channelsNamesDictionary = new Dictionary<string, string>();

            foreach (var item in channels) channelsNamesDictionary.Add(item.Name, item.Id);

            return channelsNamesDictionary;
        }

        // For Slack text message we need to escape just a few characters. So we do it here without any library
        private static string EscapeSlackText(string text)
        {
            StringBuilder res = new StringBuilder(text.Length);
            foreach (Char ch in text)
            {
                switch (ch)
                {
                    case '>':
                        res.Append("&gt;");
                        break;
                    case '<':
                        res.Append("&lt;");
                        break;
                    case '&':
                        res.Append("&amp;");
                        break;

                    default:
                        res.Append(ch);
                        break;
                }
            }
            return res.ToString();
        }

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