using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MS.Internal.ServerClusters;

public class PropertyCollection : ICollection<Property>
{
	private Dictionary<string, Property> m_dictionary;

	private ControlExecutor m_controlExecutor;

	private ClusterPropertyScope m_scope;

	private PropertyCollectionSet m_set;

	private ICommonControlCodes m_commonCodes;

	private ClusterItem m_item;

	public Property this[string index] => GetProperty(index);

	public ClusterItem ClusterItem => m_item;

	public ClusterPropertyScope Scope => m_scope;

	public virtual bool IsReadOnly
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return false;
		}
	}

	public virtual int Count => m_dictionary.Count;

	private unsafe void LoadPropListWithModifiedProperties(CClusPropList* pPropList)
	{
		//IL_0003: Expected I, but got I8
		ushort* ptr = null;
		Dictionary<string, Property>.ValueCollection.Enumerator enumerator = m_dictionary.Values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Property current = enumerator.Current;
			if (!current.IsModified)
			{
				continue;
			}
			try
			{
				ptr = InteropHelp.StringToWstr(current.Name);
				switch (current.PropertyType)
				{
				case ClusterPropertyType.StringCollection:
					SetMultiStringProperty(pPropList, ptr, current.Value);
					break;
				case ClusterPropertyType.String:
					SetStringProperty(pPropList, ptr, current.Value);
					break;
				case ClusterPropertyType.UInt32:
					SetUInt32Property(pPropList, ptr, current.Value);
					break;
				case ClusterPropertyType.ByteArray:
					SetBinaryProperty(pPropList, ptr, current.Value);
					break;
				case ClusterPropertyType.UInt64:
					SetUInt64Property(pPropList, ptr, current.Value);
					break;
				case ClusterPropertyType.DateTime:
					SetDateTimeProperty(pPropList, ptr, current.Value);
					break;
				case ClusterPropertyType.UInt16:
					SetUInt16Property(pPropList, ptr, current.Value);
					break;
				case ClusterPropertyType.Int64:
					SetInt64Property(pPropList, ptr, current.Value);
					break;
				case ClusterPropertyType.ExpandString:
				case ClusterPropertyType.ExpandedString:
					SetExpandSzStringProperty(pPropList, ptr, current.Value);
					break;
				case ClusterPropertyType.Int32:
					SetInt32Property(pPropList, ptr, current.Value);
					break;
				}
				current.ResetModified();
			}
			finally
			{
				InteropHelp.FreeWstr(ptr);
			}
		}
	}

	private unsafe void SetDateTimeProperty(CClusPropList* pPropList, ushort* pszPropertyName, object value)
	{
		_FILETIME fILETIME = TypeConverter.ConvertToNativeType((DateTime)value);
		uint num = global::_003CModule_003E.CClusPropList_002EScAddProp(pPropList, pszPropertyName, fILETIME, fILETIME);
		if (num != 0)
		{
			throw ExceptionHelp.Build<ApplicationException>((int)num, new string[2]
			{
				Resources.DateTimeSaveFail_Text,
				m_item.Name
			});
		}
	}

	private unsafe void SetStringProperty(CClusPropList* pPropList, ushort* pszPropertyName, object value)
	{
		ushort* ptr = InteropHelp.StringToWstr((string)value);
		try
		{
			uint num = global::_003CModule_003E.CClusPropList_002EScAddProp(pPropList, pszPropertyName, ptr, ptr);
			if (num != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)num, new string[2]
				{
					Resources.StringSaveFail_Text,
					m_item.Name
				});
			}
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	private unsafe void SetExpandSzStringProperty(CClusPropList* pPropList, ushort* pszPropertyName, object value)
	{
		ushort* ptr = InteropHelp.StringToWstr((string)value);
		try
		{
			uint num = global::_003CModule_003E.CClusPropList_002EScAddExpandSzProp(pPropList, pszPropertyName, ptr);
			if (num != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)num, new string[2]
				{
					Resources.StringSaveFail_Text,
					m_item.Name
				});
			}
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	private unsafe void SetUInt32Property(CClusPropList* pPropList, ushort* pszPropertyName, object value)
	{
		uint num = (uint)value;
		uint num2 = global::_003CModule_003E.CClusPropList_002EScAddProp(pPropList, pszPropertyName, num, num);
		if (num2 != 0)
		{
			throw ExceptionHelp.Build<ApplicationException>((int)num2, new string[2]
			{
				Resources.IntegerSaveFail_Text,
				m_item.Name
			});
		}
	}

	private unsafe void SetUInt16Property(CClusPropList* pPropList, ushort* pszPropertyName, object value)
	{
		ushort num = (ushort)value;
		uint num2 = global::_003CModule_003E.CClusPropList_002EScAddProp(pPropList, pszPropertyName, num, num);
		if (num2 != 0)
		{
			throw ExceptionHelp.Build<ApplicationException>((int)num2, new string[2]
			{
				Resources.IntegerSaveFail_Text,
				m_item.Name
			});
		}
	}

	private unsafe void SetInt32Property(CClusPropList* pPropList, ushort* pszPropertyName, object value)
	{
		int num = (int)value;
		uint num2 = global::_003CModule_003E.CClusPropList_002EScAddProp(pPropList, pszPropertyName, num, num);
		if (num2 != 0)
		{
			throw ExceptionHelp.Build<ApplicationException>((int)num2, new string[2]
			{
				Resources.IntegerSaveFail_Text,
				m_item.Name
			});
		}
	}

	private unsafe void SetInt64Property(CClusPropList* pPropList, ushort* pszPropertyName, object value)
	{
		long num = (long)value;
		uint num2 = global::_003CModule_003E.CClusPropList_002EScAddProp(pPropList, pszPropertyName, num, num);
		if (num2 != 0)
		{
			throw ExceptionHelp.Build<ApplicationException>((int)num2, new string[2]
			{
				Resources.IntegerSaveFail_Text,
				m_item.Name
			});
		}
	}

	private unsafe void SetUInt64Property(CClusPropList* pPropList, ushort* pszPropertyName, object value)
	{
		ulong num = (ulong)value;
		uint num2 = global::_003CModule_003E.CClusPropList_002EScAddProp(pPropList, pszPropertyName, num, num);
		if (num2 != 0)
		{
			throw ExceptionHelp.Build<ApplicationException>((int)num2, new string[2]
			{
				Resources.IntegerSaveFail_Text,
				m_item.Name
			});
		}
	}

	private unsafe void SetBinaryProperty(CClusPropList* pPropList, ushort* pszPropertyName, object value)
	{
		//IL_0023: Expected I, but got I8
		UnmanagedBuffer unmanagedBuffer = TypeConverter.ConvertToNativeType((byte[])value);
		try
		{
			uint num = global::_003CModule_003E.CClusPropList_002EScAddProp(pPropList, pszPropertyName, (byte*)unmanagedBuffer.Pointer, unmanagedBuffer.Size, null, 0uL);
			if (num != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)num, new string[2]
				{
					Resources.ByteArraySaveFail_Text,
					m_item.Name
				});
			}
		}
		finally
		{
			unmanagedBuffer.Free();
		}
	}

	private unsafe void SetMultiStringProperty(CClusPropList* pPropList, ushort* pszPropertyName, object value)
	{
		//IL_001b: Expected I, but got I8
		UnmanagedBuffer unmanagedBuffer = TypeConverter.ConvertToNativeType((StringCollection)value);
		try
		{
			uint num = global::_003CModule_003E.CClusPropList_002EScAddMultiSzProp(pPropList, pszPropertyName, (ushort*)unmanagedBuffer.Pointer, null);
			if (num != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)num, new string[2]
				{
					Resources.MultiSzStringSaveFail_Text,
					m_item.Name
				});
			}
		}
		finally
		{
			unmanagedBuffer.Free();
		}
	}

	private unsafe void SavePropList(CClusPropList* pPropList)
	{
		//IL_0015: Expected I, but got I8
		ulong size = (ulong)(*(long*)((ulong)(nint)pPropList + 32uL) + 4);
		UnmanagedBuffer inputBuffer = new UnmanagedBuffer((void*)(*(ulong*)((ulong)(nint)pPropList + 8uL)), size);
		uint controlCode = SetPropertyCode();
		m_controlExecutor.ExecuteInControl(controlCode, inputBuffer);
	}

	private uint GetReadWritePropertyCode()
	{
		return m_scope switch
		{
			ClusterPropertyScope.Private => m_commonCodes.GetPrivatePropertiesCode, 
			ClusterPropertyScope.Common => m_commonCodes.GetCommonPropertiesCode, 
			_ => 0u, 
		};
	}

	private uint GetReadOnlyPropertyCode()
	{
		return m_scope switch
		{
			ClusterPropertyScope.Private => m_commonCodes.GetReadOnlyPrivatePropertiesCode, 
			ClusterPropertyScope.Common => m_commonCodes.GetReadOnlyCommonPropertiesCode, 
			_ => 0u, 
		};
	}

	private uint SetPropertyCode()
	{
		return m_scope switch
		{
			ClusterPropertyScope.Private => m_commonCodes.SetPrivatePropertiesCode, 
			ClusterPropertyScope.Common => m_commonCodes.SetCommonPropertiesCode, 
			_ => 0u, 
		};
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool DoLoadReadOnlyProperties()
	{
		PropertyCollectionSet set = m_set;
		if (set != 0 && set != PropertyCollectionSet.Both)
		{
			return false;
		}
		return true;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool DoLoadReadWriteProperties()
	{
		PropertyCollectionSet set = m_set;
		return (uint)(set - 1) <= 1u;
	}

	private unsafe Dictionary<string, Property> GetPropertyDictionary(uint dwCode, [MarshalAs(UnmanagedType.U1)] bool isReadOnly)
	{
		UnmanagedBuffer unmanagedBuffer = null;
		Dictionary<string, Property> dictionary = null;
		try
		{
			unmanagedBuffer = m_controlExecutor.ExecuteOutControl(dwCode, 2048u);
			return ConvertPropertyListToDictionary((CLUSPROP_LIST*)unmanagedBuffer.Pointer, unmanagedBuffer.Size, isReadOnly, m_item.getName);
		}
		finally
		{
			unmanagedBuffer?.Free();
		}
	}

	internal unsafe static Dictionary<string, Property> ConvertPropertyListToDictionary(CLUSPROP_LIST* buffer, ulong bufferSize, [MarshalAs(UnmanagedType.U1)] bool isReadOnly, GetParentName parentObjectName)
	{
		//Discarded unreachable code: IL_0075, IL_0087
		System.Runtime.CompilerServices.Unsafe.SkipInit(out CClusPropList cClusPropList);
		global::_003CModule_003E.CClusPropList_002E_007Bctor_007D(&cClusPropList, 0);
		Dictionary<string, Property> result;
		try
		{
			if (global::_003CModule_003E.CClusPropList_002EScGetBufferProperties(&cClusPropList, buffer, bufferSize) != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>(new string[2]
				{
					Resources.PropList_GetProperties_Failed_Text,
					parentObjectName()
				});
			}
			result = ConvertPropertyListToDictionary(&cClusPropList, isReadOnly, parentObjectName);
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<CClusPropList*, void>)(&global::_003CModule_003E.CClusPropList_002E_007Bdtor_007D), &cClusPropList);
			throw;
		}
		try
		{
			global::_003CModule_003E.CClusPropList_002EDeletePropList(&cClusPropList);
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<CClusPropValueList*, void>)(&global::_003CModule_003E.CClusPropValueList_002E_007Bdtor_007D), System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cClusPropList, 64)));
			throw;
		}
		global::_003CModule_003E.CClusPropValueList_002EDeleteValueList((CClusPropValueList*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cClusPropList, 64)));
		return result;
	}

	private unsafe static Dictionary<string, Property> ConvertPropertyListToDictionary(CClusPropList* pPropList, [MarshalAs(UnmanagedType.U1)] bool isReadOnly, GetParentName parentObjectName)
	{
		//IL_0020: Expected I, but got I8
		//IL_0027: Expected I, but got I8
		//IL_0032: Expected I, but got I8
		//IL_009c: Expected I, but got I8
		//IL_00af: Expected I, but got I8
		//IL_00df: Expected I, but got I8
		//IL_017d: Expected I, but got I8
		Dictionary<string, Property> dictionary = new Dictionary<string, Property>(StringComparer.OrdinalIgnoreCase);
		uint num = global::_003CModule_003E.CClusPropList_002EScMoveToFirstProperty(pPropList);
		if (num == 0)
		{
			CClusPropList* ptr = (CClusPropList*)((ulong)(nint)pPropList + 48uL);
			CClusPropList* ptr2 = (CClusPropList*)((ulong)(nint)pPropList + 72uL);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUSPROP_BUFFER_HELPER cLUSPROP_BUFFER_HELPER);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _FILETIME fILETIME);
			do
			{
				string name = InteropHelp.WstrToString((ushort*)(*(long*)ptr + 8));
				object value = null;
				ClusterPropertyType propertyType = ClusterPropertyType.Unknown;
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(ref cLUSPROP_BUFFER_HELPER, ptr2, 8);
				switch ((uint)(*(int*)(*(ulong*)(&cLUSPROP_BUFFER_HELPER))))
				{
				case 65541u:
					value = GetMultiStringCollection(pPropList);
					propertyType = ClusterPropertyType.StringCollection;
					break;
				case 65540u:
					value = InteropHelp.WstrToString((ushort*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8));
					propertyType = ClusterPropertyType.ExpandString;
					break;
				case 65539u:
					value = InteropHelp.WstrToString((ushort*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8));
					propertyType = ClusterPropertyType.String;
					break;
				case 65538u:
					value = *(uint*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8);
					propertyType = ClusterPropertyType.UInt32;
					break;
				case 65537u:
					value = TypeConverter.ConvertToManagedType((void*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8), *(uint*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 4), DataType.BinaryBlob);
					propertyType = ClusterPropertyType.ByteArray;
					break;
				case 65542u:
					value = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<ulong>((void*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8));
					propertyType = ClusterPropertyType.UInt64;
					break;
				case 65548u:
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ref fILETIME, *(long*)(&cLUSPROP_BUFFER_HELPER) + 8, 8);
					value = TypeConverter.ConvertToManagedType(&fILETIME, 8uL, DataType.DateTime);
					propertyType = ClusterPropertyType.DateTime;
					break;
				case 65547u:
				{
					ushort num2 = *(ushort*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8);
					value = num2;
					propertyType = ClusterPropertyType.UInt16;
					break;
				}
				case 65546u:
					value = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<long>((void*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8));
					propertyType = ClusterPropertyType.Int64;
					break;
				case 65544u:
					value = InteropHelp.WstrToString((ushort*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8));
					propertyType = ClusterPropertyType.ExpandedString;
					break;
				case 65543u:
					value = *(int*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8);
					propertyType = ClusterPropertyType.Int32;
					break;
				}
				Property property = new Property(name, propertyType, value, isReadOnly);
				dictionary[property.Name] = property;
				num = global::_003CModule_003E.CClusPropList_002EScMoveToNextProperty(pPropList);
			}
			while (num == 0);
		}
		if (num != 259 && num != 0)
		{
			throw ExceptionHelp.Build<ApplicationException>((int)num, new string[2]
			{
				Resources.MoveToNextPropertyFail_Text,
				parentObjectName()
			});
		}
		return dictionary;
	}

	private unsafe static StringCollection GetMultiStringCollection(CClusPropList* pPropList)
	{
		//IL_001e: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUSPROP_BUFFER_HELPER cLUSPROP_BUFFER_HELPER);
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlock(ref cLUSPROP_BUFFER_HELPER, (long)(nint)pPropList + 72L, 8);
		return (StringCollection)TypeConverter.ConvertToManagedType((void*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8), *(uint*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 4), DataType.MultiString);
	}

	private Dictionary<string, Property> MergeDictionaries(Dictionary<string, Property> dictionary1, Dictionary<string, Property> dictionary2)
	{
		if (dictionary1.Count == 0)
		{
			return dictionary2;
		}
		if (dictionary2.Count == 0)
		{
			return dictionary1;
		}
		Dictionary<string, Property> dictionary3 = new Dictionary<string, Property>(StringComparer.OrdinalIgnoreCase);
		Dictionary<string, Property>.ValueCollection.Enumerator enumerator = dictionary1.Values.GetEnumerator();
		if (enumerator.MoveNext())
		{
			do
			{
				Property current = enumerator.Current;
				dictionary3[current.Name] = current;
			}
			while (enumerator.MoveNext());
		}
		Dictionary<string, Property>.ValueCollection.Enumerator enumerator2 = dictionary2.Values.GetEnumerator();
		if (enumerator2.MoveNext())
		{
			do
			{
				Property current2 = enumerator2.Current;
				dictionary3[current2.Name] = current2;
			}
			while (enumerator2.MoveNext());
		}
		return dictionary3;
	}

	private void AddModifiedProperties(Dictionary<string, Property> modifiedDictionary, Dictionary<string, Property> newDictionary)
	{
		Property property = null;
		Dictionary<string, Property>.ValueCollection.Enumerator enumerator = modifiedDictionary.Values.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return;
		}
		do
		{
			Property current = enumerator.Current;
			if (current.IsModified)
			{
				property = null;
				if (newDictionary.TryGetValue(current.Name, out property))
				{
					property.Value = current.Value;
				}
				else
				{
					newDictionary.Add(current.Name, current);
				}
			}
		}
		while (enumerator.MoveNext());
	}

	private static Dictionary<string, Property> CreateDictionary()
	{
		return new Dictionary<string, Property>(StringComparer.OrdinalIgnoreCase);
	}

	public PropertyCollection(ClusterItem item, ControlExecutor controlExecutor, ClusterPropertyScope scope, PropertyCollectionSet set)
	{
		m_controlExecutor = controlExecutor;
		m_scope = scope;
		m_set = set;
		m_commonCodes = (ICommonControlCodes)controlExecutor;
		m_item = item;
		Refresh(preserveModifiedProperties: false);
	}

	public PropertyCollection(ClusterItem item, ClusterPropertyScope scope, PropertyCollectionSet set)
	{
		ControlExecutor controlExecutor = (m_controlExecutor = item.GetControlExecutor());
		m_scope = scope;
		m_set = set;
		m_commonCodes = (ICommonControlCodes)controlExecutor;
		m_item = item;
		Refresh(preserveModifiedProperties: false);
	}

	internal PropertyCollection(PropertyCollection oldList)
	{
		m_controlExecutor = oldList.m_controlExecutor;
		m_scope = oldList.m_scope;
		m_set = oldList.m_set;
		m_commonCodes = oldList.m_commonCodes;
		m_item = oldList.m_item;
		m_dictionary = new Dictionary<string, Property>(StringComparer.OrdinalIgnoreCase);
	}

	public virtual void Add(Property property)
	{
		m_dictionary.Add(property.Name, property);
	}

	public virtual void Clear()
	{
		//Discarded unreachable code: IL_0006
		throw new NotSupportedException();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool Contains(string name)
	{
		Property value = null;
		return m_dictionary.TryGetValue(name, out value);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public virtual bool Contains(Property property)
	{
		Property value = null;
		string name = property.Name;
		return m_dictionary.TryGetValue(name, out value);
	}

	public virtual void CopyTo(Property[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex");
		}
		using IEnumerator<Property> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Property current = enumerator.Current;
			array[arrayIndex] = current;
			arrayIndex++;
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe virtual bool Remove(Property property)
	{
		//Discarded unreachable code: IL_00b2, IL_00b4, IL_00bb, IL_00ee
		//IL_001c: Expected I, but got I8
		//IL_00dc: Expected I, but got I8
		//IL_00e7: Expected I, but got I8
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
		CClusPropList* ptr3 = ptr2;
		try
		{
			bool result;
			try
			{
				LoadPropListWithModifiedProperties(ptr2);
				uint num = 0u;
				ulong num2 = *(ulong*)((ulong)(nint)ptr2 + 8uL);
				if (num2 == 0L)
				{
					uint num3 = 0u;
				}
				else if (*(int*)num2 != 0)
				{
					ushort* ptr4 = InteropHelp.StringToWstr(property.Name);
					if (global::_003CModule_003E.CClusPropList_002EScMoveToPropertyByName(ptr2, ptr4) == 0)
					{
						CLUSTER_PROPERTY_FORMAT propfmt = (CLUSTER_PROPERTY_FORMAT)(*(ushort*)(*(ulong*)((ulong)(nint)ptr2 + 72uL)));
						if (global::_003CModule_003E.CClusPropList_002EScSetPropToDefault(ptr2, ptr4, propfmt) == 0)
						{
							goto IL_007c;
						}
					}
					return false;
				}
				goto IL_007c;
				IL_007c:
				SavePropList(ptr2);
				return true;
			}
			catch (Exception caughtException)
			{
				Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException);
				if (firstException == null)
				{
					goto IL_00b0;
				}
				int num4 = -2147019872;
				if (firstException.NativeErrorCode != -2147019872)
				{
					goto IL_00b0;
				}
				result = true;
				goto end_IL_008b;
				IL_00b0:
				throw;
				end_IL_008b:;
			}
			return result;
		}
		finally
		{
			CClusPropList* ptr5 = ptr3;
			if (ptr3 != null)
			{
				try
				{
					global::_003CModule_003E.CClusPropList_002EDeletePropList(ptr3);
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<CClusPropValueList*, void>)(&global::_003CModule_003E.CClusPropValueList_002E_007Bdtor_007D), (void*)((ulong)(nint)ptr5 + 64uL));
					throw;
				}
				global::_003CModule_003E.CClusPropValueList_002EDeleteValueList((CClusPropValueList*)((ulong)(nint)ptr3 + 64uL));
				global::_003CModule_003E.delete(ptr3);
			}
		}
	}

	public virtual IEnumerator<Property> GetEnumerator()
	{
		return m_dictionary.Values.GetEnumerator();
	}

	public virtual IEnumerator GetNonGenericEnumerator()
	{
		return ((IEnumerable)m_dictionary.Values).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		//ILSpy generated this explicit interface implementation from .override directive in GetNonGenericEnumerator
		return this.GetNonGenericEnumerator();
	}

	public Property GetProperty(string name)
	{
		//Discarded unreachable code: IL_001a
		Property value = null;
		if (m_dictionary.TryGetValue(name, out value))
		{
			return value;
		}
		throw new KeyNotFoundException();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool TryGetProperty(string name, out Property property)
	{
		return m_dictionary.TryGetValue(name, out property);
	}

	public StringCollection GetPropertyNames()
	{
		StringCollection stringCollection = new StringCollection();
		Dictionary<string, Property>.KeyCollection.Enumerator enumerator = m_dictionary.Keys.GetEnumerator();
		if (enumerator.MoveNext())
		{
			do
			{
				string current = enumerator.Current;
				stringCollection.Add(current);
			}
			while (enumerator.MoveNext());
		}
		return stringCollection;
	}

	public unsafe PropertySaveStatus SaveChanges()
	{
		//Discarded unreachable code: IL_00aa, IL_00ac, IL_00b4, IL_00e7
		//IL_0019: Expected I, but got I8
		//IL_00d5: Expected I, but got I8
		//IL_00e0: Expected I, but got I8
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
		CClusPropList* ptr3 = ptr2;
		global::_003CModule_003E.CClusPropList_002EScMoveToFirstProperty(ptr2);
		try
		{
			PropertySaveStatus result;
			try
			{
				LoadPropListWithModifiedProperties(ptr2);
				ulong num = *(ulong*)((ulong)(nint)ptr2 + 8uL);
				if (num == 0L)
				{
					uint num2 = 0u;
				}
				else if (*(int*)num != 0)
				{
					SavePropList(ptr2);
				}
				return PropertySaveStatus.Ok;
			}
			catch (Exception ex)
			{
				Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(ex);
				if (firstException == null)
				{
					goto IL_0084;
				}
				int num3 = -2147019872;
				if (firstException.NativeErrorCode != -2147019872)
				{
					goto IL_0084;
				}
				result = PropertySaveStatus.ResourceRequiresRecycle;
				goto end_IL_005b;
				IL_0084:
				throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
				{
					Resources.PropertyCollection_Save_Fail_Text,
					m_item.Name
				});
				end_IL_005b:;
			}
			return result;
		}
		finally
		{
			CClusPropList* ptr4 = ptr3;
			if (ptr3 != null)
			{
				try
				{
					global::_003CModule_003E.CClusPropList_002EDeletePropList(ptr3);
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<CClusPropValueList*, void>)(&global::_003CModule_003E.CClusPropValueList_002E_007Bdtor_007D), (void*)((ulong)(nint)ptr4 + 64uL));
					throw;
				}
				global::_003CModule_003E.CClusPropValueList_002EDeleteValueList((CClusPropValueList*)((ulong)(nint)ptr3 + 64uL));
				global::_003CModule_003E.delete(ptr3);
			}
		}
	}

	public void Refresh([MarshalAs(UnmanagedType.U1)] bool preserveModifiedProperties)
	{
		Dictionary<string, Property> dictionary = new Dictionary<string, Property>(StringComparer.OrdinalIgnoreCase);
		Dictionary<string, Property> dictionary2 = new Dictionary<string, Property>(StringComparer.OrdinalIgnoreCase);
		PropertyCollectionSet set = m_set;
		if ((uint)(set - 1) <= 1u)
		{
			uint readWritePropertyCode = GetReadWritePropertyCode();
			dictionary = GetPropertyDictionary(readWritePropertyCode, isReadOnly: false);
		}
		PropertyCollectionSet set2 = m_set;
		if (set2 == PropertyCollectionSet.ReadOnly || set2 == PropertyCollectionSet.Both)
		{
			uint readOnlyPropertyCode = GetReadOnlyPropertyCode();
			dictionary2 = GetPropertyDictionary(readOnlyPropertyCode, isReadOnly: true);
		}
		Dictionary<string, Property> dictionary3 = MergeDictionaries(dictionary, dictionary2);
		if (preserveModifiedProperties)
		{
			AddModifiedProperties(m_dictionary, dictionary3);
		}
		m_dictionary = dictionary3;
	}

	public void Refresh()
	{
		Refresh(preserveModifiedProperties: false);
	}

	public void LogModifiedProperties(string header)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("Modified properties for {0}", header);
		Dictionary<string, Property>.ValueCollection.Enumerator enumerator = m_dictionary.Values.GetEnumerator();
		if (enumerator.MoveNext())
		{
			do
			{
				Property current = enumerator.Current;
				if (current.IsModified)
				{
					stringBuilder.AppendFormat("\n{0}={1}", current.Name, current.Value);
				}
			}
			while (enumerator.MoveNext());
		}
		DebugLog.LogInfo(stringBuilder.ToString());
	}
}
