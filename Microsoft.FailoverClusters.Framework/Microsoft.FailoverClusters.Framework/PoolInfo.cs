using System;

namespace Microsoft.FailoverClusters.Framework;

public class PoolInfo : ICloneable
{
	public string PoolName { get; set; }

	public string PoolDescription { get; set; }

	public object Clone()
	{
		return new PoolInfo
		{
			PoolName = PoolName,
			PoolDescription = PoolDescription
		};
	}
}
