using System;
using System.Windows.Forms;
using FailoverClusters.UI.Common;
using ManagementConsole.Advanced;
using WindowsAPICodePack.Dialogs;

namespace MS.Internal.ServerClusters.Management;

internal sealed class ConsoleNotifyUser : NotifyUser
{
	private ManagementConsole.Advanced.Console console;

	public ConsoleNotifyUser(ManagementConsole.Advanced.Console console)
	{
		this.console = console;
	}

	protected override DialogResult InternalShowFormDialog(Form form)
	{
		DialogResult result = DialogResult.OK;
		try
		{
			result = console.ShowDialog(form);
		}
		catch (Exception ex)
		{
			ClusterLog.LogException(ex, "An Error occurred displaying dialog.");
		}
		return result;
	}

	protected override DialogResult InternalShowCommonDialog(CommonDialog dialog)
	{
		return console.ShowDialog(dialog);
	}

	protected override TaskDialogResult InternalShowConfirmationDialog(ConfirmationDialog confirmationDialog)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return confirmationDialog.ShowDialog();
	}
}

