using Microsoft.ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal interface IDeleteable
{
	bool IsDeletable { get; }

	void Delete(object sender, Status status);
}
