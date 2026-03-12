using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.FailoverClusters.Framework;

public class RequiredDependencies
{
	public ReadOnlyCollection<ResourceClass> ResourceClassDependencies { get; private set; }

	public ReadOnlyCollection<string> ResourceTypeDependencies { get; private set; }

	internal RequiredDependencies(IList<ResourceClass> resourceClasses, IList<string> resourceTypes)
	{
		ResourceClassDependencies = new ReadOnlyCollection<ResourceClass>(resourceClasses);
		ResourceTypeDependencies = new ReadOnlyCollection<string>(resourceTypes);
	}
}
