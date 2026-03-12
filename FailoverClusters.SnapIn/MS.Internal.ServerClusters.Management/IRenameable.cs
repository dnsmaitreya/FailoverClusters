using Microsoft.ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal interface IRenameable
{
	void Rename(INotifyUser notifyUser, string newName, SyncStatus status);
}
