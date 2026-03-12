using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.FailoverClusters.UIFramework;
using Microsoft.ManagementConsole;
using MS.Internal.ServerClusters.Management;

namespace Microsoft.FailoverClusters.ClusterSnapIn;

internal abstract class ViewCommandsProviderBase : IViewCommandsProvider
{
	private readonly Dictionary<UICommandId, ICommand> commandDictionary = new Dictionary<UICommandId, ICommand>();

	protected void Execute(UICommandId commandId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!IsValidViewCommand(commandId))
		{
			return;
		}
		ActionBase actionFromUICommandId = GetActionFromUICommandId(commandId);
		if (actionFromUICommandId != null)
		{
			ActionData actionData = ActionData.GetActionData(actionFromUICommandId);
			try
			{
				actionData.PerformAction(ClusterAdministrator.ActiveFormView);
			}
			catch (Exception ex)
			{
				ClusterDialogException.ShowTaskDialog(ex, Global.DefaultWindowHandle);
			}
		}
	}

	public ICommand GetCommand(UICommandId commandId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (IsValidViewCommand(commandId))
		{
			if (commandDictionary.ContainsKey(commandId))
			{
				return commandDictionary[commandId];
			}
			ICommand command = CreateCommand(commandId);
			commandDictionary.Add(commandId, command);
			return command;
		}
		return null;
	}

	protected abstract ActionBase GetActionFromUICommandId(UICommandId commandId);

	protected abstract bool IsValidViewCommand(UICommandId commandId);

	protected virtual ICommand CreateCommand(UICommandId commandId)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		if (IsValidViewCommand(commandId))
		{
			return (ICommand)new UICommand(((object)(UICommandId)(ref commandId)).ToString(), commandId, (Action<object>)delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				Execute(commandId);
			}, (Predicate<object>)((object x) => true));
		}
		return null;
	}
}
