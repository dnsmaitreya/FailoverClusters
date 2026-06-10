using System.Globalization;
using ManagementConsole;
using KDDSL.ServerClusters.Management;

namespace FailoverClusters.SnapIn;

internal static class Extensions
{
	internal static CluadminWaitDialog CreateWaitDialog(this ActionEventArgs eventArgs, string initialStatus)
	{
		return CluadminWaitDialog.Create(eventArgs.Action.DisplayName, initialStatus);
	}

	internal static CluadminWaitDialog CreateWaitDialog(this ActionEventArgs eventArgs, string initialStatusFormat, params object[] args)
	{
		string initialStatus = string.Format(CultureInfo.CurrentCulture, initialStatusFormat, args);
		return eventArgs.CreateWaitDialog(initialStatus);
	}
}

