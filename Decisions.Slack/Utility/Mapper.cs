using System.Linq;
using SlackClient.Entities;
using SlackClient.Models;

namespace SlackClient
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
                            id = x.channel.id,
                            name = x.channel.name
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