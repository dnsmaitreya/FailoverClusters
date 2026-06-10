using System.Globalization;
using ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class SnapinActionEventArgs
{
	private ActionBase action;

	private Status status;

	internal ActionBase Action => action;

	internal Status Status => status;

	internal SnapinActionEventArgs(ActionBase action, Status status)
	{
		this.action = action;
		this.status = status;
	}

	internal CluadminWaitDialog CreateWaitDialog(string initialStatus)
	{
		return CluadminWaitDialog.Create(action.DisplayName, initialStatus);
	}

	internal CluadminWaitDialog CreateWaitDialog(string initialStatusFormat, params object[] args)
	{
		string initialStatus = string.Format(CultureInfo.CurrentCulture, initialStatusFormat, args);
		return CreateWaitDialog(initialStatus);
	}
}

