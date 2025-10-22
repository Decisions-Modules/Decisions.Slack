using Decisions.Slack.FolderBehaviors;
using DecisionsFramework.Design.Projects.Dependency;
using DecisionsFramework.ServiceLayer.Services.Projects;
using DecisionsFramework.ServiceLayer.Utilities;

namespace Decisions.Slack;

public class SlackModuleDependencyInitializer : IModuleDependencyInitializer
{
    public string ModuleName => "Decisions.Slack";
    
    public void OnDependencyAdded(string projectId)
    {
        SystemUserContext suc = new SystemUserContext();
        
        // Main Integrations->Slack Folder
        string slackFolderId = GetSlackFolderId(projectId);
        FolderStructureHelper.CreateFolderIfNotExistsAndSendEvent(suc, 
            FolderStructureHelper.GetIntegrationsFolderId(projectId),
            slackFolderId,
            SlackFolderBehavior.NAME,
            typeof(SlackFolderBehavior).FullName);
        
        // Integrations->Slack->Bots
        FolderStructureHelper.CreateFolderIfNotExistsAndSendEvent(suc,
            slackFolderId,
            GetSlackBotsFolderId(projectId),
            SlackBotsFolderBehavior.NAME, 
            typeof(SlackBotsFolderBehavior).FullName);
    }

    public void OnDependencyRemoved(string projectId) { }
    
    #region ID Helpers
    
    // Integrations->Slack
    public static string GetSlackFolderId(string projectId) => $"{projectId}.slack";
    
    // Integrations->Slack->Bots
    public static string GetSlackBotsFolderId(string projectId) => $"{projectId}.slack.bots";
    
    #endregion
}