using System;
using System.Collections.Generic;
using System.Reflection;

namespace MS.Internal.FailoverClusters.Framework;

internal class TypeHelper
{
	internal static bool IsNullableType(Type type)
	{
		if (type != null && type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
		return false;
	}

	internal static Type GetElementType(Type seqType)
	{
		Type type = FindIEnumerable(seqType);
		if (type == null)
		{
			return seqType;
		}
		return type.GetGenericArguments()[0];
	}

	private static Type FindIEnumerable(Type seqType)
	{
		if (seqType != null && seqType != typeof(string))
		{
			if (seqType.IsArray)
			{
				return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
			}
			if (seqType.IsGenericType)
			{
				Type[] genericArguments = seqType.GetGenericArguments();
				foreach (Type type in genericArguments)
				{
					Type type2 = typeof(IEnumerable<>).MakeGenericType(type);
					if (type2.IsAssignableFrom(seqType))
					{
						return type2;
					}
				}
			}
			Type[] interfaces = seqType.GetInterfaces();
			if (interfaces != null && interfaces.Length != 0)
			{
				Type[] genericArguments = interfaces;
				for (int i = 0; i < genericArguments.Length; i++)
				{
					Type type3 = FindIEnumerable(genericArguments[i]);
					if (type3 != null)
					{
						return type3;
					}
				}
			}
			if (seqType.BaseType != null && seqType.BaseType != typeof(object))
			{
				return FindIEnumerable(seqType.BaseType);
			}
		}
		return null;
	}

	internal static Type GetMemberType(MemberInfo mi)
	{
		FieldInfo fieldInfo = mi as FieldInfo;
		if (fieldInfo != null)
		{
			return fieldInfo.FieldType;
		}
		PropertyInfo propertyInfo = mi as PropertyInfo;
		if (propertyInfo != null)
		{
			return propertyInfo.PropertyType;
		}
		EventInfo eventInfo = mi as EventInfo;
		if (eventInfo != null)
		{
			return eventInfo.EventHandlerType;
		}
		return null;
	}

	internal static void SetMemberValue(object instance, MemberInfo mi, object value)
	{
		FieldInfo fieldInfo = mi as FieldInfo;
		if (fieldInfo != null)
		{
			fieldInfo.SetValue(instance, value, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null);
			return;
		}
		PropertyInfo propertyInfo = mi as PropertyInfo;
		if (propertyInfo != null)
		{
			propertyInfo.SetValue(instance, value, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
			return;
		}
		throw new NotSupportedException("The member type is not supported!");
	}

	internal static object GetMemberValue(object instance, MemberInfo mi)
	{
		FieldInfo fieldInfo = mi as FieldInfo;
		if (fieldInfo != null)
		{
			return fieldInfo.GetValue(instance);
		}
		PropertyInfo propertyInfo = mi as PropertyInfo;
		if (propertyInfo != null)
		{
			return propertyInfo.GetValue(instance, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
		}
		throw new NotSupportedException("The member type is not supported!");
	}
}
