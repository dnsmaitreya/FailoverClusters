using System;

namespace FailoverClusters.Framework;

public interface IIdentifiable
{
	Guid Id { get; }
}

