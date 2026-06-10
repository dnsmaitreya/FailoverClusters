using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

internal class ClusterListComparer<Z> : IComparer<Z>
{
	private enum CustomComparerType
	{
		String = 0,
		LocalizedEnum = 1,
		Unknown = 9
	}

	private readonly PropertyInfo propertyInfo;

	private readonly Type propertyType;

	private readonly OrderByItem item;

	private readonly ClusterListComparer<Z> previousComparer;

	private readonly string fieldName;

	private readonly LocalizedEnum localizedEnum = LocalizedEnum.Unknown;

	private readonly CustomComparerType comparerType = CustomComparerType.Unknown;

	private readonly EnumSortOrder memberEnumSortOrder;

	public ClusterListComparer<Z> ChildComparer => previousComparer;

	public string FieldName => fieldName;

	public ClusterListComparer(OrderByItem item)
		: this(item, (ClusterListComparer<Z>)null)
	{
	}

	public ClusterListComparer(OrderByItem item, ClusterListComparer<Z> previousComparer)
	{
		this.item = item;
		this.previousComparer = previousComparer;
		fieldName = this.item.DataMember.Name;
		Type typeFromHandle = typeof(Z);
		if (typeFromHandle == typeof(PClusterObject))
		{
			if (item.DataMember.Source == typeof(Cluster))
			{
				typeFromHandle = typeof(PCluster);
			}
			else if (item.DataMember.Source == typeof(Group))
			{
				typeFromHandle = typeof(PGroup);
			}
			else if (item.DataMember.Source == typeof(Resource))
			{
				typeFromHandle = typeof(PResource);
			}
			else if (item.DataMember.Source == typeof(Network))
			{
				typeFromHandle = typeof(PNetwork);
			}
			else if (item.DataMember.Source == typeof(NetworkInterface))
			{
				typeFromHandle = typeof(PNetworkInterface);
			}
			else if (item.DataMember.Source == typeof(ResourceType))
			{
				typeFromHandle = typeof(PResourceType);
			}
			else if (item.DataMember.Source == typeof(Node))
			{
				typeFromHandle = typeof(PNode);
			}
		}
		if (typeof(ClusterObject).IsAssignableFrom(typeFromHandle))
		{
			propertyInfo = ClusterObject.GetBackStoreProperty(typeFromHandle, fieldName);
		}
		if (propertyInfo == null)
		{
			propertyInfo = typeFromHandle.GetProperty(fieldName);
		}
		if (propertyInfo == null)
		{
			throw new NotSupportedException();
		}
		propertyType = propertyInfo.PropertyType;
		if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
		{
			NullableConverter nullableConverter = new NullableConverter(propertyType);
			propertyType = nullableConverter.UnderlyingType;
		}
		if (propertyType == typeof(string))
		{
			comparerType = CustomComparerType.String;
		}
		else if (propertyType.IsEnum)
		{
			localizedEnum = EnumLocalization.GetEnumerationType(propertyType);
			comparerType = ((localizedEnum != LocalizedEnum.Unknown) ? CustomComparerType.LocalizedEnum : CustomComparerType.Unknown);
			object obj = propertyType.GetCustomAttributes(inherit: true).FirstOrDefault((object attr) => attr is EnumSortableAttribute);
			if (obj != null)
			{
				memberEnumSortOrder = ((EnumSortableAttribute)obj).SortOrder;
			}
		}
		else
		{
			comparerType = CustomComparerType.Unknown;
		}
	}

	public int Compare(Z x, Z y)
	{
		int num = 0;
		switch (comparerType)
		{
		case CustomComparerType.String:
			try
			{
				num = NativeMethods.StrCmpLogicalW((string)propertyInfo.GetValue(x, null), (string)propertyInfo.GetValue(y, null));
			}
			catch (Exception)
			{
				num = 0;
			}
			break;
		case CustomComparerType.LocalizedEnum:
			try
			{
				object value3 = propertyInfo.GetValue(x, null);
				object value4 = propertyInfo.GetValue(y, null);
				if (value3 != null && value4 != null)
				{
					if (memberEnumSortOrder == EnumSortOrder.Value)
					{
						num = ((int)value3).CompareTo((int)value4);
						break;
					}
					string x3 = ((Enum)value3).Translate(localizedEnum);
					string y3 = ((Enum)value4).Translate(localizedEnum);
					num = NativeMethods.StrCmpLogicalW(x3, y3);
				}
				else if (value3 == null && value4 != null)
				{
					num = -1;
				}
				else
				{
					if (value3 == null)
					{
						return 0;
					}
					num = 1;
				}
			}
			catch (Exception)
			{
				num = 0;
			}
			break;
		case CustomComparerType.Unknown:
			try
			{
				object value = propertyInfo.GetValue(x, null);
				object value2 = propertyInfo.GetValue(y, null);
				if (value != null && value2 != null)
				{
					if (value.GetType().IsEnum && value2.GetType().IsEnum && memberEnumSortOrder == EnumSortOrder.Value)
					{
						num = ((int)value).CompareTo((int)value2);
						break;
					}
					string x2 = (value.GetType().IsEnum ? ((Enum)value).Translate(localizedEnum) : value.ToString());
					string y2 = (value2.GetType().IsEnum ? ((Enum)value2).Translate(localizedEnum) : value2.ToString());
					num = NativeMethods.StrCmpLogicalW(x2, y2);
				}
				else if (value == null && value2 != null)
				{
					num = -1;
				}
				else
				{
					if (value == null)
					{
						return 0;
					}
					num = 1;
				}
			}
			catch (Exception)
			{
				num = 0;
			}
			break;
		}
		if (num != 0)
		{
			if (item.Direction != 0)
			{
				return -num;
			}
			return num;
		}
		if (previousComparer != null)
		{
			return previousComparer.Compare(x, y);
		}
		return num;
	}
}

