using DecisionsFramework.Design.Projects.FolderStructure;
using DecisionsFramework.ServiceLayer.Services.Folder;
using DecisionsFramework.ServiceLayer.Services.Projects;

namespace Decisions.Slack.FolderBehaviors;

public class SlackFolderBehavior : AbstractProjectManageFolderBehavior, ILookAndFeelProviderFolderBehavior
{
    internal const string NAME = "Slack";
    
    public override string FolderBehaviorName => NAME;
    public override string GetDefaultPageName(Folder folder) => NAME;
    public override bool IsExportable(Folder f) => true;
    public override bool ExportChildrenOnly(Folder f) => false;

    private const string SLACK_LOGO = "../Content/CustomModuleImages/Decisions.Slack/slack.png";
    public LookAndFeel GetLookAndFeel(string folderId)
    {
        LookAndFeel lookandfeel = new LookAndFeel(folderId, FolderStructureHelper.Project_Default_SubFolder_Color, null, 
            new DecisionsFramework.ServiceLayer.Services.Image.ImageInfo()
            {
                ImageType = DecisionsFramework.ServiceLayer.Services.Image.ImageInfoType.Url, ImageUrl = SLACK_LOGO
            });
        return lookandfeel;
    }
}