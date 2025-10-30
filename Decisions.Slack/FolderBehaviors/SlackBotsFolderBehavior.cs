using DecisionsFramework.ServiceLayer.Actions;
using DecisionsFramework.ServiceLayer.Actions.Common;
using DecisionsFramework.ServiceLayer.Services.Folder;
using DecisionsFramework.ServiceLayer.Services.Projects;

namespace Decisions.Slack.FolderBehaviors;

public class SlackBotsFolderBehavior : DefaultFolderBehavior, ILookAndFeelProviderFolderBehavior
{
    internal const string NAME = "Bots";

    public override string FolderBehaviorName => NAME;
    public override bool IsExportable(Folder f) => true;
    public override bool ExportChildrenOnly(Folder f) => false;

    public override BaseActionType[] GetFolderActions(Folder folder, BaseActionType[] proposedActions, EntityActionType[] types)
    {
        List<BaseActionType> actions = new List<BaseActionType>();
        actions.AddRange(base.GetFolderActions(folder, proposedActions, types));

        actions.AddRange([
            new AddEntityAction(typeof(SlackBot), "Add Bot", "Adds a new Slack Bot entity", null, "Add Slack Bot")
                { Order = 5, DisplayType = ActionDisplayType.Primary }

        ]);

        return actions.ToArray();
    }
    
    private const string BOT_FOLDER_LOGO = "../Content/CustomModuleImages/Decisions.Slack/slackbot.png";
    public LookAndFeel GetLookAndFeel(string folderId)
    {
        LookAndFeel lookandfeel = new LookAndFeel(folderId, FolderStructureHelper.Project_Default_SubFolder_Color, null, 
            new DecisionsFramework.ServiceLayer.Services.Image.ImageInfo()
            {
                ImageType = DecisionsFramework.ServiceLayer.Services.Image.ImageInfoType.Url, ImageUrl = BOT_FOLDER_LOGO
            });
        return lookandfeel;
    }
}