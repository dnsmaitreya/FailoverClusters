using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace MS.Internal.ServerClusters;

[DefaultMember("Item")]
internal class ReadOnlyNetworkInfoCollection : ReadOnlyCollection<NetworkInfo>
{
	public ReadOnlyNetworkInfoCollection()
		: base((IList<NetworkInfo>)new List<NetworkInfo>())
	{
	}

	public void InternalAdd(NetworkInfo netInfo)
	{
		base.Items.Add(netInfo);
	}
}
