using System;

namespace FailoverClusters.NativeHelp;

public class NativeResourceHelp
{
	[Flags]
	public enum SubStatus
	{
		None = 0,
		Locked = 1,
		EmbeddedFailure = 2,
		FailedDueToInsufficientCpu = 4,
		FailedDueToInsufficientMemory = 8,
		FailedDueToInsufficientGenericResources = 0x10,
		NetworkFailure = 0x20,
		Unmonitored = 0x40
	}
}

