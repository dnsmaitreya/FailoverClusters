using System;

namespace FailoverClusters.Framework;

public class NodeOperatingSystemInformation
{
	public ulong Available { get; set; }

	public ulong Total { get; set; }

	public string CsdVersion { get; set; }

	public string OsName { get; set; }

	public string OsVersion { get; set; }

	public DateTime LocalDateTime { get; set; }

	public DateTime LastBootUptime { get; set; }
}

