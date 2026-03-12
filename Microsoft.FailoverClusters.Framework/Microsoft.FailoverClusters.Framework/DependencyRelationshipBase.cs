using System;
using System.Globalization;
using System.Text;

namespace Microsoft.FailoverClusters.Framework;

public abstract class DependencyRelationshipBase
{
	protected class ParseTreeNode
	{
		private readonly ParseTreeNode memberLeftNode;

		private readonly ParseTreeNode memberRightNode;

		public virtual bool IsLeafNode => false;

		public ParseTreeNode LeftNode => memberLeftNode;

		public ParseTreeNode RightNode => memberRightNode;

		protected ParseTreeNode(ParseTreeNode leftNode, ParseTreeNode rightNode)
		{
			memberLeftNode = leftNode;
			memberRightNode = rightNode;
		}
	}

	protected class AndNode : ParseTreeNode
	{
		public AndNode(ParseTreeNode leftNode, ParseTreeNode rightNode)
			: base(leftNode, rightNode)
		{
		}
	}

	protected class OrNode : ParseTreeNode
	{
		public OrNode(ParseTreeNode leftNode, ParseTreeNode rightNode)
			: base(leftNode, rightNode)
		{
		}
	}

	protected class ParenthesisNode : ParseTreeNode
	{
		public ParenthesisNode(ParseTreeNode leftNode)
			: base(leftNode, null)
		{
		}
	}

	protected enum DependencyStringType
	{
		Guid,
		DisplayName,
		Parseable
	}

	public RelationshipType RelationType { get; set; }

	public abstract bool HasResources { get; }

	protected abstract bool HasRelationship { get; }

	public bool IsEmpty
	{
		get
		{
			if (!HasResources)
			{
				return !HasRelationship;
			}
			return false;
		}
	}

	protected DependencyRelationshipBase()
	{
		RelationType = RelationshipType.And;
	}

	public override string ToString()
	{
		return BuildDependencyString(DependencyStringType.Guid, isChild: false);
	}

	public string GetDisplayDependencyString()
	{
		return BuildDependencyString(DependencyStringType.DisplayName, isChild: false);
	}

	public string GetParseDependencyString()
	{
		return BuildDependencyString(DependencyStringType.Parseable, isChild: false);
	}

	public string GetRelationshipTypeString()
	{
		return RelationType switch
		{
			RelationshipType.And => "and", 
			RelationshipType.Or => "or", 
			_ => string.Empty, 
		};
	}

	protected string BuildDependencyString(DependencyStringType dependencyType, bool isChild)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (HasResources)
		{
			AppendItem(stringBuilder, BuildResourceString(dependencyType, isChild));
		}
		if (HasRelationship)
		{
			AppendItem(stringBuilder, BuildRelationshipString(dependencyType, isChild));
		}
		return stringBuilder.ToString();
	}

	protected abstract string BuildResourceString(DependencyStringType dependencyType, bool isChild);

	protected abstract string BuildRelationshipString(DependencyStringType dependencyType, bool isChild);

	protected string GetResourceString(Guid resourceId, string resourceDisplayName, string resourceName, DependencyStringType dependencyType)
	{
		return dependencyType switch
		{
			DependencyStringType.Guid => string.Format(CultureInfo.InvariantCulture, "[{0}]", resourceId), 
			DependencyStringType.DisplayName => string.Format(CultureInfo.InvariantCulture, "'{0}'", resourceDisplayName), 
			DependencyStringType.Parseable => string.Format(CultureInfo.InvariantCulture, "[{0}]", resourceName), 
			_ => null, 
		};
	}

	protected void AppendItem(StringBuilder builder, string item)
	{
		if (builder.Length != 0)
		{
			builder.AppendFormat(CultureInfo.CurrentCulture, " {0} ", GetRelationshipTypeString());
		}
		builder.Append(item);
	}
}
