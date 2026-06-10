using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

internal class SafeServiceHandle : SafeHandleZeroIsInvalid
{
	public SafeServiceHandle()
		: base(ownsHandle: true)
	{
		SetHandle(handle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.CloseServiceHandle(handle);
	}
}

