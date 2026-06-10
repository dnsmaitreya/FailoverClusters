using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MS.Internal.FailoverClusters.Framework;

internal static class FormatHelper
{
	internal static Guid UIntToGuid(ulong value)
	{
		return new Guid(value.ToString(CultureInfo.InvariantCulture).PadLeft(32, '0'));
	}

	internal static ulong StringHash(string str)
	{
		ulong num = 5381uL;
		foreach (char c in str)
		{
			num = (num << 5) + num + c;
		}
		return num;
	}

	internal static string EscapeSingleQuotes(string str)
	{
		if (str.IndexOf('\'') < 0)
		{
			return str;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in str)
		{
			if (c == '\'')
			{
				stringBuilder.Append("''");
			}
			else
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	internal static string WrapInBrackets(string column)
	{
		column = column.Trim();
		if (column[0] == '[' && column[column.Length - 1] == ']')
		{
			return column;
		}
		return "[" + column + "]";
	}

	internal static string FormatColumnNamesInSequence(IEnumerable<ClusterObjectMetaDataMember> members)
	{
		return FormatColumnNamesInSequence(members, wrapInBrackets: true, null);
	}

	internal static string FormatColumnNamesInSequence(string[] members)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		foreach (string value in members)
		{
			if (!flag)
			{
				stringBuilder.Append(", ");
			}
			else
			{
				flag = false;
			}
			stringBuilder.Append(value);
		}
		return stringBuilder.ToString();
	}

	internal static string FormatColumnNamesInSequence(IEnumerable<ClusterObjectMetaDataMember> members, bool wrapInBrackets, string[] ignoredFields)
	{
		if (!members.Any())
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		foreach (ClusterObjectMetaDataMember member in members)
		{
			if (ignoredFields == null || !ignoredFields.Contains(member.MappedName, StringComparer.OrdinalIgnoreCase))
			{
				if (!flag)
				{
					stringBuilder.Append(", ");
				}
				else
				{
					flag = false;
				}
				stringBuilder.Append(wrapInBrackets ? WrapInBrackets(member.MappedName) : member.MappedName);
			}
		}
		return stringBuilder.ToString();
	}

	internal static string FormatDbValue(object value)
	{
		if (value == null)
		{
			return "NULL";
		}
		Type type = value.GetType();
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
		{
			type = type.GetGenericArguments()[0];
		}
		switch (Type.GetTypeCode(type))
		{
		case TypeCode.Object:
			if (value is Guid)
			{
				return string.Concat("'", value, "'");
			}
			if (value is List<Guid>)
			{
				StringBuilder stringBuilder = new StringBuilder();
				List<Guid> list = value as List<Guid>;
				stringBuilder.Append("'");
				foreach (Guid item in list)
				{
					stringBuilder.Append("[");
					stringBuilder.Append(item.ToString());
					stringBuilder.Append("]");
				}
				stringBuilder.Append("'");
				return stringBuilder.ToString();
			}
			return value.ToString();
		case TypeCode.Boolean:
			return GetBoolValue((bool)value);
		case TypeCode.Char:
		case TypeCode.DateTime:
		case TypeCode.String:
			return "'" + EscapeSingleQuotes(value.ToString()) + "'";
		default:
			return value.ToString();
		}
	}

	internal static string GetBoolValue(bool value)
	{
		if (!value)
		{
			return "0";
		}
		return "1";
	}
}
