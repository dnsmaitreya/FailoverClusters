using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal interface IDeleteable
{
	bool IsDeletable { get; }

	void Delete(object sender, Status status);
}

