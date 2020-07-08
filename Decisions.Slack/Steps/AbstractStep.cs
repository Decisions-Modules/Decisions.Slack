using Decisions.OAuth;
using DecisionsFramework;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.Design.Properties.Attributes;
using DecisionsFramework.ServiceLayer.Services.ContextData;
using DecisionsFramework.ServiceLayer.Services.OAuth2;
using DecisionsFramework.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.Slack
{
    [Writable]
    public abstract class AbstractStep : ISyncStep, IDataConsumer, IDataProducer
    {
        public const string slackCategory = "Integration/Slack Messenger";

        protected const string errorOutcomeLabel = "Error";
        protected const string resultOutcomeLabel = "Result";
        protected const string doneOutcomeLabel = "Done";
        protected const string errorOutcomeDataLabel = "Error info";
        protected const string resultLabel = "RESULT";

        protected const string tokenLabel = "Token";
        protected const string channelLabel = "Channel Name";
        protected const string channelIdLabel = "Channel ID";
        protected const string excludeArchivedLabel = "Exclude Archived";
        protected const string isPrivateLabel = "Private";
        protected const string userIdsLabel = "User Id List";
        protected const string latestMessageTimestampLabel = "Latest Message Timestamp";
        protected const string messageTimestampLabel = "Message Timestamp";
        protected const string messageCountLabel = "Message Count";
        protected const string messageTextLabel = "Мessage text";
        protected const string textToSerachLabel = "Text to Search";
        protected const string matchCountLabel = "Match Count";


        [PropertyHidden]
        public virtual DataDescription[] InputData
        {
            get
            {
                return new DataDescription[] { };
            }
        }

        private const int errorOutcomeIndex = 0;
        private const int resultOutcomeIndex = 1;
        public virtual OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { new OutcomeScenarioData(errorOutcomeLabel, new DataDescription(typeof(SlackErrorInfo), errorOutcomeDataLabel)) };
            }
        }

        [TokenPicker]
        [WritableValue]
        public string Token { get; set; }

        private string FindAccessToken(string id)
        {
            ORM<OAuthToken> orm = new ORM<OAuthToken>();
            var token = orm.Fetch(id);
            if (token != null)
                return token.TokenData;
            throw new EntityNotFoundException($"Can not find token with TokenId=\"{id}\"");
        }

        public ResultData Run(StepStartData data)
        {
            try
            {
                var accessToken = FindAccessToken(Token);

                Object res = ExecuteStep(accessToken, data);

                var outputData = OutcomeScenarios[resultOutcomeIndex].OutputData;
                var exitPointName = OutcomeScenarios[resultOutcomeIndex].ExitPointName;

                if (outputData != null && outputData.Length > 0)
                    return new ResultData(exitPointName, new DataPair[] { new DataPair(outputData[0].Name, res) });
                else
                    return new ResultData(exitPointName);
            }
            catch (Exception ex)
            {
                SlackErrorInfo ErrInfo = SlackErrorInfo.FromException(ex);
                return new ResultData(errorOutcomeLabel, new DataPair[] { new DataPair(errorOutcomeDataLabel, ErrInfo) });
            }
        }

        protected abstract Object ExecuteStep(string token, StepStartData data);

    }
}

