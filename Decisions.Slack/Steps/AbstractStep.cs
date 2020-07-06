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
        public const string SlackCategory = "Integration/Slack Messenger";

        protected const string ERROR_OUTCOME = "Error";
        protected const string RESULT_OUTCOME = "Result";
        protected const string DONE_OUTCOME = "Done";
        protected const string ERROR_OUTCOME_DATA_NAME = "Error info";
        protected const string RESULT = "RESULT";

        protected const string TOKEN = "Token";
        protected const string CHANNEL_NAME = "Channel Name";
        protected const string CHANNEL_ID = "Channel ID";
        protected const string EXCLUDE_ARCHIVED = "Exclude Archived";
        protected const string IS_PRIVATE = "Private";
        protected const string USER_IDS = "User Id List";
        protected const string LATEST_MESSAGE_TIMESTAMP = "Latest Message Timestamp";
        protected const string MESSAGE_TIMESTAMP = "Message Timestamp";
        protected const string MESSAGE_COUNT = "Message Count";
        protected const string MESSAGE_TEXT = "MESSAGE_TEXT";
        protected const string TEXT_TO_SERACH = "Text to Search";
        protected const string MATCH_COUNT = "Match Count";


        [PropertyHidden]
        public virtual DataDescription[] InputData
        {
            get
            {
                return new DataDescription[] { };
            }
        }

        private const int ERROR_OUTCOME_INDEX = 0;
        private const int RESULT_OUTCOME_INDEX = 1;
        public virtual OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { new OutcomeScenarioData(ERROR_OUTCOME, new DataDescription(typeof(SlackErrorInfo), ERROR_OUTCOME_DATA_NAME)) };
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

                var outputData = OutcomeScenarios[RESULT_OUTCOME_INDEX].OutputData;
                var exitPointName = OutcomeScenarios[RESULT_OUTCOME_INDEX].ExitPointName;

                if (outputData != null && outputData.Length > 0)
                    return new ResultData(exitPointName, new DataPair[] { new DataPair(outputData[0].Name, res) });
                else
                    return new ResultData(exitPointName);
            }
            catch (Exception ex)
            {
                SlackErrorInfo ErrInfo = SlackErrorInfo.FromException(ex);
                return new ResultData(ERROR_OUTCOME, new DataPair[] { new DataPair(ERROR_OUTCOME_DATA_NAME, ErrInfo) });
            }
        }

        protected abstract Object ExecuteStep(string token, StepStartData data);

    }
}

