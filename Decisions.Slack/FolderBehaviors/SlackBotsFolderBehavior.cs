using DecisionsFramework.ServiceLayer.Actions;
using DecisionsFramework.ServiceLayer.Actions.Common;
using DecisionsFramework.ServiceLayer.Services.Folder;

namespace Decisions.Slack.FolderBehaviors;

public class SlackBotsFolderBehavior : DefaultFolderBehavior
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
}