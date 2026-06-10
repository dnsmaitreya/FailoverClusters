using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace MS.Internal.ServerClusters;

internal class TypeConverter
{
	private unsafe static void CopyString(ushort* pszDest, ulong destSize, ushort* pszSource, ulong srcSize)
	{
		int num = global::_003CModule_003E.StringCchCopyNW(pszDest, destSize, pszSource, srcSize);
		if (num < 0)
		{
			throw ExceptionHelp.Build<ApplicationException>(num & 0xFFFF, new string[1] { Resources.StringCopyFail_Text });
		}
	}

	private unsafe static StringCollection ConvertMultiStringToManagedType(void* pNativeData, ulong length)
	{
		//IL_0032: Expected I, but got I8
		//IL_0047: Expected I, but got I8
		StringCollection stringCollection = new StringCollection();
		ushort* ptr = (ushort*)pNativeData;
		if (*(ushort*)pNativeData != 0)
		{
			while ((ulong)((long)((nint)((byte*)ptr - (nuint)pNativeData) >> 1) * 2L) <= length)
			{
				stringCollection.Add(InteropHelp.WstrToString(ptr));
				ushort* ptr2 = ptr;
				if (System.Runtime.CompilerServices.Unsafe.ReadUnaligned<short>(ptr) != 0)
				{
					do
					{
						ptr2 = (ushort*)((ulong)(nint)ptr2 + 2uL);
					}
					while (System.Runtime.CompilerServices.Unsafe.ReadUnaligned<short>(ptr2) != 0);
				}
				ptr = (ushort*)((long)((nint)((byte*)ptr2 - (nuint)ptr) >> 1) * 2L + (nint)ptr + 2);
				if (*ptr == 0)
				{
					break;
				}
			}
		}
		return stringCollection;
	}

	private unsafe static byte[] ConvertBinaryBlobToManagedType(void* pNativeData, ulong length)
	{
		byte[] array = new byte[(uint)length];
		ulong num = 0uL;
		if (0 < length)
		{
			do
			{
				array[(uint)num] = *(byte*)(num + (ulong)(nint)pNativeData);
				num++;
			}
			while (num < length);
		}
		return array;
	}

	private TypeConverter()
	{
	}

	public unsafe static object ConvertToManagedType(void* pNativeType, ulong length, DataType type)
	{
		object result = null;
		switch (type)
		{
		case DataType.BinaryBlob:
			result = ConvertBinaryBlobToManagedType(pNativeType, length);
			break;
		case DataType.DateTime:
		{
			long fileTime = *(long*)pNativeType;
			try
			{
				DateTime dateTime = DateTime.FromFileTimeUtc(fileTime);
				DateTime dateTime2 = dateTime;
				DateTime dateTime3 = DateTime.Parse("1/1/1970", CultureInfo.InvariantCulture);
				if (dateTime == dateTime3)
				{
					goto IL_008d;
				}
				DateTime dateTime4 = DateTime.Parse("1/1/1601", CultureInfo.InvariantCulture);
				if (dateTime == dateTime4)
				{
					goto IL_008d;
				}
				result = dateTime;
				goto end_IL_0047;
				IL_008d:
				result = DateTime.MinValue;
				end_IL_0047:;
			}
			catch (ArgumentOutOfRangeException)
			{
				result = DateTime.MaxValue;
			}
			break;
		}
		case DataType.UInt64:
			result = *(ulong*)pNativeType;
			break;
		case DataType.Int64:
			result = *(long*)pNativeType;
			break;
		case DataType.UInt32:
			result = *(uint*)pNativeType;
			break;
		case DataType.Int32:
			result = *(int*)pNativeType;
			break;
		case DataType.MultiString:
			result = ConvertMultiStringToManagedType(pNativeType, length);
			break;
		case DataType.String:
			result = InteropHelp.WstrToString((ushort*)pNativeType);
			break;
		}
		return result;
	}

	public unsafe static object ConvertToManagedType(UnmanagedBuffer buffer, DataType type)
	{
		return ConvertToManagedType(buffer.Pointer, buffer.Size, type);
	}

	public unsafe static UnmanagedBuffer ConvertToNativeType(byte[] managedType)
	{
		//IL_002d: Expected I, but got I8
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		unmanagedBuffer.Allocate((ulong)managedType.LongLength);
		byte* pointer = (byte*)unmanagedBuffer.Pointer;
		int num = 0;
		if (0 < (nint)managedType.LongLength)
		{
			byte* ptr = pointer;
			do
			{
				*ptr = managedType[num];
				num++;
				ptr = (byte*)((ulong)(nint)ptr + 1uL);
			}
			while (num < (nint)managedType.LongLength);
		}
		return unmanagedBuffer;
	}

	public unsafe static UnmanagedBuffer ConvertToNativeType(StringCollection managedType)
	{
		//IL_009e: Expected I, but got I8
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		int num = 0;
		StringEnumerator enumerator = managedType.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				num += current.Length + 1;
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		num++;
		unmanagedBuffer.Allocate((ulong)num * 2uL);
		ushort* ptr = (ushort*)unmanagedBuffer.Pointer;
		StringEnumerator enumerator2 = managedType.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				string current2 = enumerator2.Current;
				ushort* ptr2 = InteropHelp.StringToWstr(current2);
				try
				{
					int num2 = current2.Length + 1;
					CopyString(ptr, (ulong)num, ptr2, (ulong)num2);
					ptr = (ushort*)((long)num2 * 2L + (nint)ptr);
					num -= num2;
				}
				finally
				{
					InteropHelp.FreeWstr(ptr2);
				}
			}
		}
		finally
		{
			if (enumerator2 is IDisposable disposable2)
			{
				disposable2.Dispose();
			}
		}
		*ptr = 0;
		return unmanagedBuffer;
	}

	public unsafe static UnmanagedBuffer ConvertToNativeType(string managedType)
	{
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		ulong num = (ulong)(managedType.Length + 1);
		unmanagedBuffer.Allocate(num * 2);
		ushort* pointer = (ushort*)unmanagedBuffer.Pointer;
		ushort* ptr = InteropHelp.StringToWstr(managedType);
		try
		{
			CopyString(pointer, num, ptr, num);
			return unmanagedBuffer;
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	public unsafe static _FILETIME ConvertToNativeType(ValueType managedType)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _FILETIME result);
		*(long*)(&result) = ((DateTime)managedType).ToFileTimeUtc();
		return result;
	}
}
