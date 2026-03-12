using System;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal abstract class StorageResourceContextBase : ResourceContext, IDeleteable
{
	private string groupName;

	protected string GroupName
	{
		get
		{
			return groupName;
		}
		set
		{
			groupName = value;
		}
	}

	internal StorageResourceContextBase(ClusterResource resource)
		: base(resource)
	{
		GroupName = null;
	}

	protected override void UpdateStandardVerbs()
	{
		base.EnabledStandardVerbs &= ~StandardVerbs.Delete;
	}

	protected abstract void PerformRemoveStorage(CluadminWaitDialog waitDialog);

	protected void PerformRemoveStorageAction(object sender, SnapinActionEventArgs e, string confirmationText)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		try
		{
			if (notifyUserFromSender.ShowYesNoQuestion(MessageBoxDefaultButton.Button2, confirmationText) != DialogResult.Yes)
			{
				return;
			}
			VerifyActionCanBePerformedData verifyActionData = ActionVerification.BuildVerifyActionData(VerifyAction.QuorumLoss, string.Format(CultureInfo.CurrentCulture, Resources.ClusterShutdownByResourceMove_Text, base.DisplayName), QuorumLossCheck.RemoveStorage, string.Empty, string.Empty);
			if (VerifyActionCanBePerformed(notifyUserFromSender, e, null, verifyActionData, new ConfirmationMessage(string.Format(CultureInfo.CurrentCulture, Resources.RemoveStorageActionConfirm_Text, base.DisplayName))))
			{
				CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.RemovingStorage_Text, base.DisplayName);
				cluadminWaitDialog.DisplayDelay = TimeSpan.Zero;
				using (cluadminWaitDialog)
				{
					cluadminWaitDialog.ShowDialog(notifyUserFromSender, PerformRemoveStorage);
					return;
				}
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error removing storage");
			notifyUserFromSender.ShowError(ex, Resources.FailureRemovingStorage_Text);
		}
	}

	protected abstract void OnRemoveStorage(object sender, SnapinActionEventArgs e);

	public override void Delete(object sender, Status status)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.Delete_Resource_Text, Resources.VerifyResourceDelete_Text, base.DisplayName);
		using (cluadminWaitDialog)
		{
			bool flag = cluadminWaitDialog.ShowDialog<object, bool>(notifyUserFromSender, ResourceInAvailableStorage, null);
			if (!cluadminWaitDialog.IsCanceled)
			{
				if (flag)
				{
					base.Delete(sender, status);
					return;
				}
				ActionBase action = ActionFactory.CreateDisabledAction(string.Format(CultureInfo.CurrentCulture, StringExtensions.ReplaceAccelerator(CommandResources.RemoveCommand_Text), GroupName), Resources.RemoveStorageActionDescription_Text, Icons.RemoveIndex);
				OnRemoveStorage(sender, new SnapinActionEventArgs(action, status));
			}
		}
	}

	private bool ResourceInAvailableStorage(CluadminWaitDialog waitDialog, object data)
	{
		return base.Resource.GetOwnerGroup().GroupType == GroupType.AvailableStorage;
	}

	public override void Dispose()
	{
		if (!isDisposed)
		{
			base.Dispose();
			GC.SuppressFinalize(this);
		}
	}

	public override void Refresh()
	{
		base.Refresh();
	}
}
