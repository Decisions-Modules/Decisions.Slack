using System.ComponentModel;
using System.Runtime.CompilerServices;
using Decisions.Slack.Services;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.Design.Properties.Attributes;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.ServiceLayer.Actions;
using DecisionsFramework.ServiceLayer.Actions.Common;
using DecisionsFramework.ServiceLayer.Services.ConfigurationStorage;
using DecisionsFramework.ServiceLayer.Utilities;
using SlackNet;

namespace Decisions.Slack;

[ORMEntity, Exportable, Writable]
public class SlackBot : AbstractFolderEntity, INotifyPropertyChanged
{
    [WritableValue, ORMField] private string appToken;
    [WritableValue, ORMField] private string botToken;
    [WritableValue, ORMField] private string handlerFlowId;
    [WritableValue, ORMField] private bool enabled;

    [PropertyHidden] public ISlackSocketModeClient? Client { get; set; }
    [PropertyHidden] public ISlackApiClient? ApiClient { get; set; }
    [PropertyHidden] public string? BotUserId { get; set; }
    
    [ORMPrimaryKeyField]
    private string id;
    
    [ExcludeInDescription]
    [PropertyClassification(0, "Name")]
    public override string EntityName
    {
        get => base.EntityName;
        set => base.EntityName = value;
    }
        
    [PropertyClassification(1, "Enabled")]
    public bool Enabled
    {
        get => enabled;
        set
        {
            enabled = value;
            OnPropertyChanged();
        }
    }
    
    [PropertyHidden]
    public override string EntityDescription { get; set; }
    
    [PropertyClassification(10, "App Token")]
    public string AppToken
    {
        get => appToken;
        set
        {
            appToken = value;
            OnPropertyChanged();
        }
    }
    
    [PropertyClassification(11, "Bot Token")]
    public string BotToken
    {
        get => botToken;
        set
        {
            botToken = value;
            OnPropertyChanged();
        }
    }
    
    [ElementRegistrationPickerEditor(ElementType.Flow, "Pick Message Handler Flow", BehaviorTypeName = "Decisions.Slack.FlowBehaviors.SlackBotMessageHandlerFlowBehavior", ShowEditorWithLinks = true, Actions = PickerActions.Default)]
    [PropertyClassification(12, "Message Handler Flow")]
    public string HandlerFlow 
    {
        get => handlerFlowId;
        set
        {
            handlerFlowId = value;
            OnPropertyChanged();
        } 
    }
    
    public override BaseActionType[] GetActions(AbstractUserContext userContext, EntityActionType[] types)
    {
        return
        [
            new EditEntityAction(this.GetType(), "Edit", "Edits Slack Bot")
        ];
    }

    public override void AfterSave()
    {
        base.AfterSave();

        if (Enabled)
            SlackBotService.Instance.InitializeBot(new SystemUserContext(), this);
        else
            SlackBotService.Instance.DisconnectBot(new SystemUserContext(), this);
    }

    #region IPropertyChanged Members
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}