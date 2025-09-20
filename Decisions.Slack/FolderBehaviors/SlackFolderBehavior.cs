using DecisionsFramework.Design.Projects.FolderStructure;
using DecisionsFramework.ServiceLayer.Services.Folder;

namespace Decisions.Slack.FolderBehaviors;

public class SlackFolderBehavior : AbstractProjectManageFolderBehavior
{
    internal const string NAME = "Slack";
    
    public override string FolderBehaviorName => NAME;
    public override string GetDefaultPageName(Folder folder) => NAME;
    public override bool IsExportable(Folder f) => true;
    public override bool ExportChildrenOnly(Folder f) => false;
}