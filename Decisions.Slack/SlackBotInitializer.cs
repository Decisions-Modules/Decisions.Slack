using Decisions.Slack.FolderBehaviors;
using Decisions.Slack.Services;
using Decisions.Slack.Utility;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.ServiceLayer.Services.Folder;
using DecisionsFramework.ServiceLayer.Services.Projects;
using DecisionsFramework.ServiceLayer.Utilities;

namespace Decisions.Slack;

public class SlackBotInitializer : IInitializable
{
    public void Initialize()
    {
        SystemUserContext suc = new();
        
        // Initialize bots for projects
        ProjectDto[] projects = ProjectViewService.Instance.GetProjects(suc);
        foreach (ProjectDto project in projects ?? [])
        {
            string projectId = project.Id;
            string folderId = SlackModuleDependencyInitializer.GetSlackBotsFolderId(projectId);
            
            if (!string.IsNullOrEmpty(projectId))
            {
                if (FolderService.Instance.Exists(suc, folderId))
                {
                    Folder folder = new ORM<Folder>().Fetch(folderId);
                    if (folder != null)
                    {
                        SlackBot[] bots = folder.GetEntitiesOfType<SlackBot>();
                        foreach (SlackBot bot in bots)
                        {
                            if (bot.Enabled)
                            {
                                SlackBotService.Instance.InitializeBot(suc, bot);
                            }
                        }
                    }
                }
                else if (ProjectUtility.IsDependentModule(projectId, SlackBotConstants.SLACK_MODULE_NAME))
                {
                    // Main Integrations->Slack Folder
                    string slackFolderId = SlackModuleDependencyInitializer.GetSlackFolderId(projectId);
                    FolderStructureHelper.CreateFolderIfNotExistsAndSendEvent(suc, 
                        FolderStructureHelper.GetIntegrationsFolderId(projectId),
                        slackFolderId,
                        SlackFolderBehavior.NAME,
                        typeof(SlackFolderBehavior).FullName);
        
                    // Integrations->Slack->Bots
                    FolderStructureHelper.CreateFolderIfNotExistsAndSendEvent(suc,
                        slackFolderId,
                        SlackModuleDependencyInitializer.GetSlackBotsFolderId(projectId),
                        SlackBotsFolderBehavior.NAME, 
                        typeof(SlackBotsFolderBehavior).FullName);
                }
            }
        }
    }
}