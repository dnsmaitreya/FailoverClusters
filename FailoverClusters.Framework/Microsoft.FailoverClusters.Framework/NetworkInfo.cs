using System;
using System.Globalization;

namespace FailoverClusters.Framework;

public class NetworkInfo
{
	public Guid Id { get; internal set; }

	public string Name { get; internal set; }

	public string Address { get; internal set; }

	public int PrefixLength { get; internal set; }

	public NetworkInfo(Guid id, string name, string address, string prefixValue)
	{
		Id = id;
		Name = name;
		Address = address;
		PrefixLength = int.Parse(prefixValue, CultureInfo.CurrentCulture);
	}
}

