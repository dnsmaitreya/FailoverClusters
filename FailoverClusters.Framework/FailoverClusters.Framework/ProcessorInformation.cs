namespace FailoverClusters.Framework;

public class ProcessorInformation
{
	public string Caption { get; set; }

	public string Description { get; set; }

	public string Name { get; set; }

	public ushort LoadPercentage { get; set; }

	public uint NumberOfCores { get; set; }

	public uint MaxClockSpeed { get; set; }

	public bool IsLoaded { get; set; }
}

