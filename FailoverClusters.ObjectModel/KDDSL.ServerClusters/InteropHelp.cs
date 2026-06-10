using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace KDDSL.ServerClusters;

internal sealed class InteropHelp
{
	private InteropHelp()
	{
	}

	public unsafe static ushort* StringToWstr(string str)
	{
		return (ushort*)Marshal.StringToHGlobalUni(str).ToPointer();
	}

	public unsafe static string WstrToString(ushort* str)
	{
		return new string((char*)str);
	}

	public unsafe static void FreeWstr(ushort* str)
	{
		Marshal.FreeHGlobal((IntPtr)str);
	}

	public unsafe static void* AllocateArray(ulong cbSize)
	{
		IntPtr cb = new IntPtr((long)cbSize);
		return Marshal.AllocHGlobal(cb).ToPointer();
	}

	public unsafe static void ReallocateArray(void** pArray, ulong cbSize)
	{
		//IL_0007: Expected I, but got I8
		//IL_001f: Expected I8, but got I
		IntPtr pv = (IntPtr)(void*)(*(ulong*)pArray);
		IntPtr cb = new IntPtr((long)cbSize);
		*(long*)pArray = (nint)Marshal.ReAllocHGlobal(pv, cb).ToPointer();
	}

	public unsafe static ushort* AllocateWCharArray(uint cchSize)
	{
		return (ushort*)AllocateArray((ulong)cchSize * 2uL);
	}

	public unsafe static void ReallocateWCharArray(ushort** pArray, uint cchNewSize)
	{
		//IL_0003: Expected I, but got I8
		//IL_0012: Expected I8, but got I
		void* ptr = (void*)(*(ulong*)pArray);
		ReallocateArray(&ptr, (ulong)cchNewSize * 2uL);
		*(long*)pArray = (nint)ptr;
	}

	public unsafe static void ConvertStringCollectionToPWSTRArray(StringCollection itemCollection, ushort*** pppszItemArray)
	{
		//IL_0003: Expected I, but got I8
		//IL_0059: Expected I8, but got I
		//IL_0062: Expected I, but got I8
		//IL_006e: Expected I8, but got I
		ushort** ptr = null;
		if (itemCollection.Count > 0)
		{
			ulong num = (ulong)itemCollection.Count;
			ptr = (ushort**)global::_003CModule_003E.new_005B_005D((num > 2305843009213693951L) ? ulong.MaxValue : (num * 8));
			int num2 = 0;
			if (0 < itemCollection.Count)
			{
				ushort** ptr2 = ptr;
				do
				{
					*(long*)ptr2 = (nint)Marshal.StringToHGlobalUni(itemCollection[num2]).ToPointer();
					num2++;
					ptr2 = (ushort**)((ulong)(nint)ptr2 + 8uL);
				}
				while (num2 < itemCollection.Count);
			}
		}
		*(long*)pppszItemArray = (nint)ptr;
	}

	public unsafe static void FreeArray(void* pMem)
	{
		if (pMem != null)
		{
			Marshal.FreeHGlobal((IntPtr)pMem);
		}
	}

	public unsafe static void FreePWSTRArray(ushort** ppszItemArray, int count)
	{
		//IL_0017: Expected I, but got I8
		long num = 0L;
		long num2 = count;
		if (0 < num2)
		{
			do
			{
				Marshal.FreeHGlobal((IntPtr)(void*)(*(ulong*)(num * 8 + (nint)ppszItemArray)));
				num++;
			}
			while (num < num2);
		}
		global::_003CModule_003E.delete_005B_005D(ppszItemArray);
	}

	public unsafe static Guid ConvertNativeGuidToManagedGuid(_GUID nativeGuid)
	{
		return new Guid(*(uint*)(&nativeGuid), System.Runtime.CompilerServices.Unsafe.As<_GUID, ushort>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nativeGuid, 4)), System.Runtime.CompilerServices.Unsafe.As<_GUID, ushort>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nativeGuid, 6)), System.Runtime.CompilerServices.Unsafe.As<_GUID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nativeGuid, 8)), System.Runtime.CompilerServices.Unsafe.As<_GUID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nativeGuid, 9)), System.Runtime.CompilerServices.Unsafe.As<_GUID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nativeGuid, 10)), System.Runtime.CompilerServices.Unsafe.As<_GUID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nativeGuid, 11)), System.Runtime.CompilerServices.Unsafe.As<_GUID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nativeGuid, 12)), System.Runtime.CompilerServices.Unsafe.As<_GUID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nativeGuid, 13)), System.Runtime.CompilerServices.Unsafe.As<_GUID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nativeGuid, 14)), System.Runtime.CompilerServices.Unsafe.As<_GUID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nativeGuid, 15)));
	}

	public unsafe static ushort* ConvertStringCollectionToPWSTR(IEnumerable<string> itemCollection, int* size)
	{
		//IL_0009: Expected I, but got I8
		List<byte> list = new List<byte>();
		ushort* result = null;
		if (itemCollection != null)
		{
			UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
			byte item = 0;
			foreach (string item2 in itemCollection)
			{
				list.AddRange(unicodeEncoding.GetBytes(item2));
				list.Add(item);
				list.Add(item);
			}
			list.Add(item);
			list.Add(item);
			IntPtr destination = Marshal.AllocHGlobal(list.Count);
			Marshal.Copy(list.ToArray(), 0, destination, list.Count);
			result = (ushort*)destination.ToPointer();
			*size = list.Count;
		}
		else
		{
			*size = 0;
		}
		return result;
	}

	public unsafe static Guid FromGUID(_GUID* guid)
	{
		return new Guid(*(uint*)guid, *(ushort*)((ulong)(nint)guid + 4uL), *(ushort*)((ulong)(nint)guid + 6uL), *(byte*)((ulong)(nint)guid + 8uL), *(byte*)((ulong)(nint)guid + 9uL), *(byte*)((ulong)(nint)guid + 10uL), *(byte*)((ulong)(nint)guid + 11uL), *(byte*)((ulong)(nint)guid + 12uL), *(byte*)((ulong)(nint)guid + 13uL), *(byte*)((ulong)(nint)guid + 14uL), *(byte*)((ulong)(nint)guid + 15uL));
	}

	public unsafe static _GUID ToGUID(Guid* guid)
	{
		fixed (byte* ptr = &guid->ToByteArray()[0])
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _GUID result);
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ref result, ptr, 16);
			return result;
		}
	}
}
