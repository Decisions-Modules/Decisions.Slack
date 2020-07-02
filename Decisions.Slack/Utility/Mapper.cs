using System.Linq;
using Decisions.Slack.Data;
using Decisions.Slack.Models;

namespace Decisions.Slack
{
    internal static class Mapper
    {
        internal static MessagesRoot Map(MessagesRootModel model)
        {
            return new MessagesRoot
            {
                query = model.query,
                messages = new MessagesSearch
                {
                    total = model.messages.total,
                    matches = model.messages.matches.Select(x => new Matches
                    {
                        channel = new Channel
                        {
                            Id = x.channel.id,
                            Name = x.channel.name
                        },
                        iid = x.iid,
                        permalink = x.permalink,
                        team = x.team,
                        text = x.text,
                        ts = x.ts,
                        type = x.type,
                        user = x.user
                    }).ToArray()
                }
            };
        }
    }
}