using System.Windows.Forms;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.ManagementConsole;
using Microsoft.ManagementConsole.Advanced;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MS.Internal.ServerClusters.Management;

internal sealed class PropertyPageNotifyUser : NotifyUser
{
	private readonly PropertySheet propertySheet;

	private readonly Console console;

	public PropertyPageNotifyUser(PropertySheet propertySheet)
	{
		this.propertySheet = propertySheet;
	}

	public PropertyPageNotifyUser(Console console)
	{
		this.console = console;
	}

	protected override DialogResult InternalShowFormDialog(Form form)
	{
		if (propertySheet == null)
		{
			return console.ShowDialog(form);
		}
		return propertySheet.ShowDialog(form);
	}

	protected override DialogResult InternalShowCommonDialog(CommonDialog dialog)
	{
		if (propertySheet == null)
		{
			return console.ShowDialog(dialog);
		}
		return propertySheet.ShowDialog(dialog);
	}

	protected override TaskDialogResult InternalShowConfirmationDialog(ConfirmationDialog confirmationDialog)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return confirmationDialog.ShowDialog();
	}
}
