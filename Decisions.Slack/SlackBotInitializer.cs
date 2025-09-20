using Decisions.Slack.Services;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.ServiceLayer.Services.Folder;
using DecisionsFramework.ServiceLayer.Services.Projects;
using DecisionsFramework.ServiceLayer.Utilities;
using SlackNet;

namespace Decisions.Slack;

public class SlackBotInitializer : IInitializable
{
    private ISlackSocketModeClient? _client;
    private ISlackApiClient _apiClient;
    private string _botUserId;

    public void Initialize()
    {
        SystemUserContext suc = new();
        
        // Initialize bots for projects
        ProjectDto[] projects = ProjectViewService.Instance.GetProjects(suc);
        foreach (ProjectDto project in projects ?? [])
        {
            string projectId = project?.Id;
            string folderId = SlackModuleDependencyInitializer.GetSlackBotsFolderId(projectId);
            if (!string.IsNullOrEmpty(projectId) && FolderService.Instance.Exists(suc, folderId))
            {
                var folder = new ORM<Folder>().Fetch(folderId);
                if (folder != null)
                {
                    var bots = folder.GetEntitiesOfType<SlackBot>();
                    foreach (var bot in bots)
                    {
                        if (bot.Enabled)
                            SlackBotService.Instance.InitializeBot(suc, bot);
                    }
                }
            }
        }
    }
}