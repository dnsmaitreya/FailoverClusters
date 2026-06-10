using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KDDSL.ServerClusters;

public class RequiredDependencies
{
	private ReadOnlyCollection<ClusterResourceClass> m_resourceClasses;

	private ReadOnlyCollection<string> m_resourceTypes;

	public ICollection<string> ResourceTypeDependencies => m_resourceTypes;

	public ICollection<ClusterResourceClass> ResourceClassDependencies => m_resourceClasses;

	internal RequiredDependencies(List<ClusterResourceClass> resourceClasses, List<string> resourceTypes)
	{
		m_resourceClasses = new ReadOnlyCollection<ClusterResourceClass>(resourceClasses);
		m_resourceTypes = new ReadOnlyCollection<string>(resourceTypes);
	}
}
