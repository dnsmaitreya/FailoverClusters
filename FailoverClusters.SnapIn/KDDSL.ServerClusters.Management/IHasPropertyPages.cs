using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal interface IHasPropertyPages
{
	PropertyPageCollection PropertyPages { get; }
}

