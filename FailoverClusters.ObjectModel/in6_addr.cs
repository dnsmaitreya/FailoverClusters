using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 16)]
[NativeCppClass]
[UnsafeValueType]
internal struct in6_addr
{
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	[CLSCompliant(false)]
	[NativeCppClass]
	[UnsafeValueType]
	public struct _003Cunnamed_002Dtype_002Du_003E
	{
		[FieldOffset(0)]
		private short _003Calignment_0020member_003E;
	}

	private short _003Calignment_0020member_003E;
}
