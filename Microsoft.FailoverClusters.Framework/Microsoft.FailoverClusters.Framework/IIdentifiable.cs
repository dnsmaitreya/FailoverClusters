using System;

namespace Microsoft.FailoverClusters.Framework;

public interface IIdentifiable
{
	Guid Id { get; }
}
