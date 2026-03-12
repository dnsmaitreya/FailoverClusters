using System;
using System.Runtime.InteropServices;

namespace Microsoft.FailoverClusters.Framework;

internal abstract class SafeHandleZeroIsInvalid : SafeHandle
{
	public override bool IsInvalid => handle == IntPtr.Zero;

	internal SafeHandleZeroIsInvalid(bool ownsHandle)
		: base(IntPtr.Zero, ownsHandle)
	{
	}
}
