namespace MS.Internal.ServerClusters;

public class ResourceCharacteristics
{
	private ClusterResourceClass resourceClass;

	private uint characteristics;

	internal uint Characteristics => characteristics;

	internal ClusterResourceClass ResourceClass => resourceClass;

	internal ResourceCharacteristics(ClusterResourceClass resourceClass, uint characteristics)
	{
		this.resourceClass = resourceClass;
		this.characteristics = characteristics;
	}
}
