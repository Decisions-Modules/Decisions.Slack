using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Decisions.Slack.Data;
using Decisions.Slack.Models;

namespace Decisions.Slack
{
    internal static class SlackEndpointNames
    {
        public const string postMessage = "chat.postMessage";
        public const string addChannel = "conversations.create";
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

    public static class SlackClientApi
    {
        private static readonly string baseAdress = "https://slack.com/api/";


        /// <summary>
        ///     Get channel id by the channel name. Can be used for convenience
        /// </summary>
        /// <param name="token"></param>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public static string GetChannelIdByName(string token, string channelName)
        {
            var dict = GetChannelsDictionary(token);

            var channelId = dict.FirstOrDefault(x => x.Value == channelName).Key;

            return channelId;
        }

        /// <summary>
        /// Create channel
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="name">name of channel</param>
        /// <returns></returns>
        public static bool CreateChannel(string token, string name)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(baseAdress) };

            var responseMessage = httpClient
                .PostAsync($"{SlackEndpointNames.addChannel}?token={token}&name={name}", null)
                .GetAwaiter().GetResult();
            var resultStr = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            var model = CheckErrorAndReturnModel(resultStr);
            return model.ok;
        }

        /// <summary>
        /// Get channels list
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Array of channels model</returns>
        public static Channel[] GetChannelsList(string token)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(baseAdress) };

            /*var response = httpClient.GetStringAsync($"{SlackEndpointNames.conversationsList}?token={token}")
                .GetAwaiter()
                .GetResult();*/
            var rowResponse = httpClient.GetAsync($"{SlackEndpointNames.conversationsList}?token={token}").Result;
            var response = rowResponse.Content.ReadAsStringAsync().Result;

            CheckErrorAndReturnModel(response);

            var channels =
                JsonConvert.DeserializeObject<ChannelsListWithOkModel>(response);
            var list = channels.channels.Select(x => new Channel { Id = x.id, Name = x.name }).ToList();

            return list.ToArray();
        }

        public static User[] ListUsersInChannelByChannelId(string token, string channelId)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(baseAdress) };

            var responseMessage = httpClient
                .GetStringAsync($"{SlackEndpointNames.conversationsMembers}?token={token}&channel={channelId}")
                .GetAwaiter().GetResult();

            CheckErrorAndReturnModel(responseMessage);

            var ids = JsonConvert.DeserializeObject<MembersRootModel>(responseMessage);

            var names = new List<User>();

            foreach (var item in ids.members)
            {
                var userInfoModel = JsonConvert.DeserializeObject<RootUserInfoModel>(httpClient
                    .GetStringAsync($"{SlackEndpointNames.usersInfo}?token={token}&user={item}").GetAwaiter()
                    .GetResult());
                names.Add(new User
                {
                    real_name = userInfoModel.user.real_name,
                    id = userInfoModel.user.id,
                    name = userInfoModel.user.name
                });
            }

            return names.ToArray();
        }

        public static User[] ListUsersInChannelByChannelName(string token, string channelName)
        {
            string channelId = GetChannelIdByName(token, channelName);
            return ListUsersInChannelByChannelId(token, channelId);
        }

        /// <summary>
        /// Show recent messages from channel
        /// </summary>
        /// <param name="token"></param>
        /// <param name="channelName">name of channel</param>
        /// <returns></returns>
        public static Message[] GetRecentMessagesFromChannelByChannelName(string token, string channelName)
        {
            string channelId = GetChannelIdByName(token, channelName);
            return GetRecentMessagesFromChannelByChannelId(token, channelId);
        }

        public static Message[] GetRecentMessagesFromChannelByChannelId(string token, string channelId)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(baseAdress) };

            var responseMessage = httpClient
                .GetStringAsync($"{SlackEndpointNames.conversationsHistory}?token={token}&channel={channelId}")
                .GetAwaiter().GetResult();

            CheckErrorAndReturnModel(responseMessage);
            var historyRoot = JsonConvert.DeserializeObject<HistoryRootModel>(responseMessage);

            return historyRoot.messages.Select(x => new Message
            {
                text = x.text,
                ts = x.ts,
                type = x.type,
                user = x.user
            }).ToArray();
        }


        /// <summary>
        /// Send message to users
        /// </summary>
        /// <param name="token"></param>
        /// <param name="users">Users ids</param>
        public static bool OpenChannelWithUser(string token, string users)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(baseAdress) };

            var responseMessage = httpClient
                .PostAsync($"{SlackEndpointNames.conversationsOpen}?token={token}&users={users}", null)
                .GetAwaiter().GetResult();
            var model = CheckErrorAndReturnModel(responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            return model.ok;
        }


        /// <summary>
        /// Pin message to channel 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="channelName">name of channel</param>
        /// <param name="timestamp">timestamp of message</param>
        public static bool PinMessageToChannelByChannelName(string token, string channelName, string timestamp)
        {
            string channelId = GetChannelIdByName(token, channelName);
            return PinMessageToChannelByChannelId(token, channelId, timestamp);
        }

        public static bool PinMessageToChannelByChannelId(string token, string channelId, string timestamp)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(baseAdress) };

            var responseMessage = httpClient
                .PostAsync(
                    $"{SlackEndpointNames.pinMsgToChannel}?token={token}&channel={channelId}&timestamp={timestamp}",
                    null)
                .GetAwaiter().GetResult();
            var response = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var model = CheckErrorAndReturnModel(response);
            return model.ok;
        }

        /// <summary>
        /// Pin message to channel 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="channelName">name of channel</param>
        /// <param name="timestamp">timestamp of message</param>
        public static bool UnpinMessageToChannelByChannelName(string token, string channelName, string timestamp)
        {
            string channelId = GetChannelIdByName(token, channelName);
            return UnpinMessageToChannelByChannelId(token, channelId, timestamp);
        }

        public static bool UnpinMessageToChannelByChannelId(string token, string channelId, string timestamp)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(baseAdress) };

            var responseMessage = httpClient
                .PostAsync(
                    $"{SlackEndpointNames.removePinMsgToChannel}?token={token}&channel={channelId}&timestamp={timestamp}",
                    null)
                .GetAwaiter().GetResult();
            var response = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var model = CheckErrorAndReturnModel(response);
            return model.ok;
        }



        /// <summary>
        /// Delete message from channel
        /// </summary>
        /// <param name="token"></param>
        /// <param name="channelName">name of channel</param>
        /// <param name="timestamp">timestamp of message</param>
        public static bool DeleteMessageFromChannelByChannelName(string token, string channelName, string timestamp)
        {
            string channelId = GetChannelIdByName(token, channelName);
            return DeleteMessageFromChannelByChannelId(token, channelId, timestamp);
        }

        public static bool DeleteMessageFromChannelByChannelId(string token, string channelId, string timestamp)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(baseAdress) };

            var responseMessage = httpClient
                .PostAsync(
                    $"{SlackEndpointNames.deleteMsgFromChannel}?token={token}&channel={channelId}&ts={timestamp}",
                    null)
                .GetAwaiter().GetResult();

            var response = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var model = CheckErrorAndReturnModel(response);
            return model.ok;
        }



        /// <summary>
        /// send message to chanel
        /// </summary>
        /// <param name="token"></param>
        /// <param name="channelName">name of channel</param>
        /// <param name="text">message text</param>
        public static bool PostMessageToChannelByChannelName(string token, string channelName, string text)
        {
            string channelId = GetChannelIdByName(token, channelName);
            return PostMessageToChannelByChannelId(token, channelId, text);
        }

        public static bool PostMessageToChannelByChannelId(string token, string channelId, string text)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(baseAdress) };

            var responseMessage =
                httpClient.PostAsync($"{SlackEndpointNames.postMessage}?token={token}&channel={channelId}&text={text}",
                    null).GetAwaiter().GetResult();
            var response = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var model = CheckErrorAndReturnModel(response);
            return model.ok;
        }

        /// <summary>
        /// search message in channels
        /// </summary>
        /// <param name="token"></param>
        /// <param name="textToSearch"></param>
        /// <returns></returns>
        public static Matches[] SearchForTextInChannels(string token, string textToSearch, int count = 20, int page = 1)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(baseAdress) };

            var responseMessage = httpClient
                .GetStringAsync($"{SlackEndpointNames.searchInChannels}?token={token}&query={textToSearch}&count={count}&page={page}")
                .GetAwaiter()
                .GetResult();
            CheckErrorAndReturnModel(responseMessage);

            var model = JsonConvert.DeserializeObject<MessagesRootModel>(responseMessage);
            var result = Mapper.Map(model);
            return result.messages.matches;
        }

        /// <summary>
        /// send channel to archive
        /// </summary>
        /// <param name="token"></param>
        /// <param name="channelName">name of channel</param>
        public static bool ArchiveChannelByChannelName(string token, string channelName)
        {
            string channelId = GetChannelIdByName(token, channelName);
            return ArchiveChannelByChannelId(token, channelId);
        }

        public static bool ArchiveChannelByChannelId(string token, string channelId)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(baseAdress) };

            var responseMessage = httpClient
                .PostAsync($"{SlackEndpointNames.archiveChannel}?token={token}&channel={channelId}", null)
                .GetAwaiter()
                .GetResult();
            var response = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var model = CheckErrorAndReturnModel(response);
            return model.ok;
        }


        private static Dictionary<string, string> GetChannelsDictionary(string token)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(baseAdress) };

            var responseMessage = httpClient.GetStringAsync($"{SlackEndpointNames.conversationsList}?token={token}")
                .GetAwaiter()
                .GetResult();
            CheckErrorAndReturnModel(responseMessage);

            var channels = JsonConvert.DeserializeObject<ChannelsListWithOkModel>(responseMessage);

            var channelsNamesDictionary = new Dictionary<string, string>();

            foreach (var item in channels.channels) channelsNamesDictionary.Add(item.id, item.name);

            return channelsNamesDictionary;
        }

        private static ErrorModel CheckErrorAndReturnModel(string response)
        {
            ErrorModel model = new ErrorModel();
            try
            {
                model = JsonConvert.DeserializeObject<ErrorModel>(response);
                if (!model.ok) throw new ArgumentException($"Server return {model.error}");
            }
            catch (JsonException)
            {
                // all are okey, go next step
            }

            return model;
        }
    }
}