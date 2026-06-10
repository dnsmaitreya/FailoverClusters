using ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal interface IHasPropertyPages
{
	PropertyPageCollection PropertyPages { get; }
}

