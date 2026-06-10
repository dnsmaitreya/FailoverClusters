using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

public class Property
{
	private ClusterPropertyType m_format;

	private bool m_isReadOnly;

	private bool m_isModified;

	private object m_data;

	private string m_name;

	public string Name => m_name;

	public bool IsReadOnly
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_isReadOnly;
		}
	}

	public object Value
	{
		get
		{
			return m_data;
		}
		set
		{
			if (m_isReadOnly)
			{
				throw ExceptionHelp.Build<ApplicationException>(new string[2]
				{
					Resources.PropertyReadOnly_Text,
					m_name
				});
			}
			VerifyTypeMatches(value);
			m_data = value;
			m_isModified = true;
		}
	}

	public ClusterPropertyType PropertyType => m_format;

	public bool IsModified
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_isModified;
		}
	}

	private void Construct(string name, ClusterPropertyType propertyType, object value, [MarshalAs(UnmanagedType.U1)] bool isReadOnly)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		m_name = name;
		m_format = propertyType;
		m_data = value;
		m_isReadOnly = isReadOnly;
		m_isModified = false;
	}

	private void VerifyTypeMatches(object value)
	{
		ClusterPropertyType propertyTypeFromObject = GetPropertyTypeFromObject(value);
		ClusterPropertyType format = m_format;
		if (propertyTypeFromObject != format && propertyTypeFromObject == ClusterPropertyType.String && format != ClusterPropertyType.ExpandString && format != ClusterPropertyType.ExpandedString)
		{
			throw ExceptionHelp.Build<ApplicationException>(new string[2]
			{
				Resources.PropertyValue_IncorrectType_Text,
				m_name
			});
		}
	}

	private ClusterPropertyType GetPropertyTypeFromObject(object value)
	{
		if (value is byte[])
		{
			return ClusterPropertyType.ByteArray;
		}
		if (null != value as string)
		{
			return ClusterPropertyType.String;
		}
		if (value is DateTime)
		{
			return ClusterPropertyType.DateTime;
		}
		if (value is uint)
		{
			return ClusterPropertyType.UInt32;
		}
		if (value is ulong)
		{
			return ClusterPropertyType.UInt64;
		}
		if (value is long)
		{
			return ClusterPropertyType.Int64;
		}
		if (value is int)
		{
			return ClusterPropertyType.Int32;
		}
		if (value is StringCollection)
		{
			return ClusterPropertyType.StringCollection;
		}
		return (value is ushort) ? ClusterPropertyType.UInt16 : ClusterPropertyType.Unknown;
	}

	public Property(string name, ClusterPropertyType propertyType, object value)
	{
		Construct(name, propertyType, value, isReadOnly: false);
		m_isModified = true;
	}

	public Property(string name, ClusterPropertyType propertyType)
	{
		Construct(name, propertyType, null, isReadOnly: false);
		m_isModified = true;
	}

	internal Property(Property source)
	{
		m_name = source.m_name;
		ClusterPropertyType clusterPropertyType = (m_format = source.m_format);
		m_isReadOnly = source.m_isReadOnly;
		m_isModified = source.m_isModified;
		switch (clusterPropertyType)
		{
		default:
			m_data = source.m_data;
			break;
		case ClusterPropertyType.StringCollection:
		{
			StringCollection stringCollection = new StringCollection();
			StringEnumerator enumerator = ((StringCollection)source.m_data).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					stringCollection.Add(current);
				}
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			m_data = stringCollection;
			break;
		}
		case ClusterPropertyType.ByteArray:
		{
			byte[] array = source.m_data as byte[];
			m_data = array.Clone();
			break;
		}
		}
	}

	internal Property(string name, ClusterPropertyType propertyType, object value, [MarshalAs(UnmanagedType.U1)] bool isReadOnly)
	{
		Construct(name, propertyType, value, isReadOnly);
	}

	internal void ResetModified()
	{
		m_isModified = false;
	}
}
