using System;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

internal interface ILockable : IDisposable
{
	PClusterObject Owner { get; }

	void Reader();

	void Writer();

	void UpgradeableReader();

	void UnlockReader();

	void UnlockWriter();

	void UnlockUpgradeableReader();
}
