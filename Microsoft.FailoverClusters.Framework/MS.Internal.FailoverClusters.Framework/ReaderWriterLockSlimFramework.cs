using System;
using System.Threading;

namespace MS.Internal.FailoverClusters.Framework;

internal class ReaderWriterLockSlimFramework : ReaderWriterLockSlim
{
	public string Name { get; set; }

	public Type LockType { get; set; }

	public ReaderWriterLockSlimFramework(LockRecursionPolicy recursionPolicy)
		: base(recursionPolicy)
	{
	}
}
