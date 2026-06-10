using System;
using System.Data.Linq.Mapping;
using System.Reflection;
using FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal class ClusterObjectMetaDataMember
{
	private readonly MemberInfo storageMember;

	private readonly Type source;

	private readonly Type declaringType;

	private readonly Type type;

	private readonly MemberInfo member;

	private readonly bool isNullableType;

	private readonly DataAttribute attr;

	private readonly ColumnAttribute attrColumn;

	private readonly AssociationAttribute attrAssoc;

	public bool IsNullableType => isNullableType;

	public Type Source => source;

	public bool CanBeNull
	{
		get
		{
			if (attrColumn != null)
			{
				return attrColumn.CanBeNull;
			}
			return true;
		}
	}

	public string DbType
	{
		get
		{
			if (attrColumn != null)
			{
				return attrColumn.DbType;
			}
			return null;
		}
	}

	public Type DeclaringType => declaringType;

	public bool IsAssociation => attrAssoc != null;

	public bool IsDbGenerated
	{
		get
		{
			if (attrColumn != null)
			{
				if (!attrColumn.IsDbGenerated)
				{
					return !string.IsNullOrEmpty(attrColumn.Expression);
				}
				return true;
			}
			return false;
		}
	}

	public bool IsPersistent
	{
		get
		{
			if (attrColumn == null)
			{
				return attrAssoc != null;
			}
			return true;
		}
	}

	public bool IsPrimaryKey
	{
		get
		{
			if (attrColumn != null)
			{
				return attrColumn.IsPrimaryKey;
			}
			return false;
		}
	}

	public bool IsMetaDataField
	{
		get
		{
			if (attrColumn != null)
			{
				return attrColumn.Expression != null;
			}
			return false;
		}
	}

	public string MappedName
	{
		get
		{
			if (attrColumn != null && attrColumn.Name != null)
			{
				return attrColumn.Name;
			}
			if (attrAssoc != null && attrAssoc.Name != null)
			{
				return attrAssoc.Name;
			}
			return member.Name;
		}
	}

	public MemberInfo Member => member;

	public string Name => member.Name;

	public MemberInfo StorageMember => storageMember;

	public Type Type => type;

	public UpdateCheck UpdateCheck
	{
		get
		{
			if (attrColumn == null)
			{
				return UpdateCheck.Never;
			}
			return attrColumn.UpdateCheck;
		}
	}

	internal ClusterObjectMetaDataMember(Type source, MemberInfo member)
	{
		this.source = source;
		declaringType = member.DeclaringType;
		this.member = member;
		type = TypeHelper.GetMemberType(member);
		isNullableType = TypeHelper.IsNullableType(type);
		attrAssoc = (AssociationAttribute)Attribute.GetCustomAttribute(member, typeof(AssociationAttribute));
		if (source != declaringType)
		{
			MemberInfo[] array = source.GetMember(member.Name, member.MemberType, BindingFlags.Instance | BindingFlags.Public);
			if (array != null && array.Length == 1)
			{
				attrColumn = (ColumnAttribute)Attribute.GetCustomAttribute(array[0], typeof(ColumnAttribute));
			}
		}
		if (attrColumn == null)
		{
			attrColumn = (ColumnAttribute)Attribute.GetCustomAttribute(member, typeof(ColumnAttribute));
		}
		attr = (DataAttribute)(((object)attrColumn) ?? ((object)attrAssoc));
		if (attr != null && attr.Storage != null)
		{
			MemberInfo[] array2 = member.DeclaringType.GetMember(attr.Storage, BindingFlags.Instance | BindingFlags.NonPublic);
			if (array2 == null || array2.Length != 1)
			{
				throw new TargetException(ExceptionResources.BadStorageProperty);
			}
			storageMember = array2[0];
		}
	}
}

