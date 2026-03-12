using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 88)]
[UnsafeValueType]
[NativeCppClass]
internal struct _DnsRecordW
{
	[StructLayout(LayoutKind.Explicit, Size = 4)]
	[CLSCompliant(false)]
	[NativeCppClass]
	public struct _003Cunnamed_002Dtype_002DFlags_003E
	{
		[FieldOffset(0)]
		private int _003Calignment_0020member_003E;
	}

	[StructLayout(LayoutKind.Explicit, Size = 56)]
	[CLSCompliant(false)]
	[UnsafeValueType]
	[NativeCppClass]
	public struct _003Cunnamed_002Dtype_002DData_003E
	{
		[FieldOffset(0)]
		private long _003Calignment_0020member_003E;
	}

	private long _003Calignment_0020member_003E;
}
