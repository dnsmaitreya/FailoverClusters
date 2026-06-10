using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal interface IRenameable
{
	void Rename(INotifyUser notifyUser, string newName, SyncStatus status);
}

