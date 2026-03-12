using System;
using System.Runtime.InteropServices;

namespace Microsoft.FailoverClusters.NativeHelp;

public class PropertyListWrapper : IDisposable
{
	private unsafe CClusPropList* propertyList;

	public unsafe int PropertyListBufferSize => (int)(*(long*)((ulong)(nint)propertyList + 32uL) + 4);

	public unsafe IntPtr PropertyListBuffer => new IntPtr((void*)(*(ulong*)((ulong)(nint)propertyList + 8uL)));

	public unsafe CClusPropList* PropertyList => propertyList;

	internal unsafe PropertyListWrapper(CClusPropList* propertyList)
	{
		this.propertyList = propertyList;
	}

	internal unsafe PropertyListWrapper()
	{
		//IL_001f: Expected I, but got I8
		CClusPropList* ptr = (CClusPropList*)global::_003CModule_003E.@new(112uL);
		CClusPropList* ptr2;
		try
		{
			ptr2 = ((ptr == null) ? null : global::_003CModule_003E.CClusPropList_002E_007Bctor_007D(ptr, 1));
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.delete(ptr);
			throw;
		}
		propertyList = ptr2;
	}

	private unsafe void _007EPropertyListWrapper()
	{
		//IL_0032: Expected I, but got I8
		//IL_003d: Expected I, but got I8
		CClusPropList* ptr = propertyList;
		if (ptr != null)
		{
			global::_003CModule_003E.CClusPropList_002EDeletePropList(ptr);
		}
		CClusPropList* ptr2 = propertyList;
		if (ptr2 != null)
		{
			try
			{
				global::_003CModule_003E.CClusPropList_002EDeletePropList(ptr2);
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<CClusPropValueList*, void>)(&global::_003CModule_003E.CClusPropValueList_002E_007Bdtor_007D), (void*)((ulong)(nint)ptr2 + 64uL));
				throw;
			}
			global::_003CModule_003E.CClusPropValueList_002EDeleteValueList((CClusPropValueList*)((ulong)(nint)ptr2 + 64uL));
			global::_003CModule_003E.delete(ptr2);
		}
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			_007EPropertyListWrapper();
		}
		else
		{
			base.Finalize();
		}
	}

	public virtual sealed void Dispose()
	{
		Dispose(A_0: true);
		GC.SuppressFinalize(this);
	}
}
