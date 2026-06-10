using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace KDDSL.ServerClusters;

public sealed class ClusterResourceRelationship
{
	private class ParseTreeNode
	{
		protected ParseTreeNode m_leftNode;

		protected ParseTreeNode m_rightNode;

		public ParseTreeNode RightNode => m_rightNode;

		public ParseTreeNode LeftNode => m_leftNode;

		public virtual bool IsLeafNode
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return false;
			}
		}

		public ParseTreeNode(ParseTreeNode leftNode, ParseTreeNode rightNode)
		{
			m_leftNode = leftNode;
			m_rightNode = rightNode;
			base._002Ector();
		}
	}

	private class ResourceNode : ParseTreeNode
	{
		protected ClusterResource m_resource;

		public ClusterResource Resource => m_resource;

		public override bool IsLeafNode
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return true;
			}
		}

		public ResourceNode(ClusterResource resource)
		{
			m_resource = resource;
			base._002Ector(null, null);
		}
	}

	private class AndNode : ParseTreeNode
	{
		public AndNode(ParseTreeNode leftNode, ParseTreeNode rightNode)
			: base(leftNode, rightNode)
		{
		}
	}

	private class OrNode : ParseTreeNode
	{
		public OrNode(ParseTreeNode leftNode, ParseTreeNode rightNode)
			: base(leftNode, rightNode)
		{
		}
	}

	private class ParenthesisNode : ParseTreeNode
	{
		public ParenthesisNode(ParseTreeNode leftNode)
			: base(leftNode, null)
		{
		}
	}

	private enum DependencyStringType
	{
		Guid,
		DisplayName,
		Parseable
	}

	private ResourceRelationshipType m_type;

	private Collection<ClusterResourceRelationship> m_childRelationships;

	private Collection<RelatedClusterResource> m_childResources;

	public bool IsEmpty
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			int num = ((m_childResources.Count == 0 && m_childRelationships.Count == 0) ? 1 : 0);
			return (byte)num != 0;
		}
	}

	public bool HasResources
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return (byte)((m_childResources.Count != 0) ? 1u : 0u) != 0;
		}
	}

	public Collection<RelatedClusterResource> ChildResources => m_childResources;

	public Collection<ClusterResourceRelationship> ChildRelationships => m_childRelationships;

	public ResourceRelationshipType RelationshipType
	{
		get
		{
			return m_type;
		}
		set
		{
			m_type = value;
		}
	}

	private string BuildResourceString(DependencyStringType type, [MarshalAs(UnmanagedType.U1)] bool isChild)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		foreach (RelatedClusterResource childResource in m_childResources)
		{
			string text = null;
			ClusterResource relatedResource = childResource.RelatedResource;
			string text2 = GetResourceString(relatedResource, type);
			if (m_type == ResourceRelationshipType.And && (type == DependencyStringType.Guid || type == DependencyStringType.Parseable))
			{
				text2 = string.Format(CultureInfo.InvariantCulture, "({0})", text2);
			}
			AppendItem(stringBuilder, text2);
		}
		flag = m_childResources.Count > 1 || flag;
		if (isChild && flag)
		{
			stringBuilder.Insert(0, "(");
			stringBuilder.Append(")");
		}
		return stringBuilder.ToString();
	}

	private string BuildRelationshipString(DependencyStringType type, [MarshalAs(UnmanagedType.U1)] bool isChild)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		foreach (ClusterResourceRelationship childRelationship in m_childRelationships)
		{
			AppendItem(stringBuilder, childRelationship.BuildDependencyString(type, isChild: true));
		}
		flag = m_childRelationships.Count > 1 || flag;
		if (isChild && flag)
		{
			stringBuilder.Insert(0, "(");
			stringBuilder.Append(")");
		}
		return stringBuilder.ToString();
	}

	private void AppendItem(StringBuilder builder, string item)
	{
		if (builder.Length != 0)
		{
			builder.AppendFormat(" {0} ", GetRelationshipTypeString());
		}
		builder.Append(item);
	}

	private string GetRelationshipTypeString()
	{
		return m_type switch
		{
			ResourceRelationshipType.Or => "or", 
			ResourceRelationshipType.And => "and", 
			_ => "", 
		};
	}

	public static ClusterResourceRelationship Parse(ClusterResource resource, string expression)
	{
		ClusterResourceRelationship clusterResourceRelationship;
		if (expression.Length == 0)
		{
			clusterResourceRelationship = new ClusterResourceRelationship();
			clusterResourceRelationship.m_type = ResourceRelationshipType.And;
		}
		else
		{
			Dictionary<Guid, ClusterResource> dictionary = new Dictionary<Guid, ClusterResource>();
			foreach (ClusterResource dependency in resource.GetDependencies())
			{
				Guid id = dependency.Id;
				dictionary.Add(id, dependency);
			}
			clusterResourceRelationship = ConvertParseTree(ParseDependencyExpression(dictionary, expression));
		}
		return clusterResourceRelationship;
	}

	public static ClusterResourceRelationship Parse(ClusterGroup group, string expression)
	{
		ClusterResourceRelationship clusterResourceRelationship;
		if (expression.Length == 0)
		{
			clusterResourceRelationship = new ClusterResourceRelationship();
			clusterResourceRelationship.m_type = ResourceRelationshipType.And;
		}
		else
		{
			Dictionary<Guid, ClusterResource> dictionary = new Dictionary<Guid, ClusterResource>();
			foreach (ClusterResource resource in group.GetResources())
			{
				Guid id = resource.Id;
				dictionary.Add(id, resource);
			}
			clusterResourceRelationship = ConvertParseTree(ParseDependencyExpression(dictionary, expression));
		}
		return clusterResourceRelationship;
	}

	private static ClusterResourceRelationship Parse(Dictionary<Guid, ClusterResource> resourceDictionary, string expression)
	{
		return ConvertParseTree(ParseDependencyExpression(resourceDictionary, expression));
	}

	private static ClusterResourceRelationship ConvertParseTree(ParseTreeNode parseTree)
	{
		ClusterResourceRelationship clusterResourceRelationship = null;
		if (parseTree is AndNode)
		{
			clusterResourceRelationship = new ClusterResourceRelationship();
			clusterResourceRelationship.m_type = ResourceRelationshipType.And;
			ProcessChild(clusterResourceRelationship, parseTree.LeftNode);
			ProcessChild(clusterResourceRelationship, parseTree.RightNode);
		}
		else if (parseTree is OrNode)
		{
			clusterResourceRelationship = new ClusterResourceRelationship();
			clusterResourceRelationship.m_type = ResourceRelationshipType.Or;
			ProcessChild(clusterResourceRelationship, parseTree.LeftNode);
			ProcessChild(clusterResourceRelationship, parseTree.RightNode);
		}
		else if (parseTree is ParenthesisNode)
		{
			parseTree = parseTree.LeftNode;
			clusterResourceRelationship = ConvertParseTree(parseTree);
		}
		else if (parseTree is ResourceNode)
		{
			clusterResourceRelationship = new ClusterResourceRelationship();
			clusterResourceRelationship.m_type = ResourceRelationshipType.And;
			ProcessChild(clusterResourceRelationship, parseTree);
		}
		return clusterResourceRelationship;
	}

	private static void ProcessChild(ClusterResourceRelationship relationship, ParseTreeNode parseTreeNodeChild)
	{
		if (parseTreeNodeChild is ResourceNode resourceNode)
		{
			relationship.m_childResources.Add(new RelatedClusterResource(resourceNode.Resource, resourceNode.Resource.Name));
			return;
		}
		ClusterResourceRelationship clusterResourceRelationship = ConvertParseTree(parseTreeNodeChild);
		if (clusterResourceRelationship.m_type == relationship.m_type)
		{
			MergeRelationships(relationship, clusterResourceRelationship);
		}
		else
		{
			relationship.m_childRelationships.Add(clusterResourceRelationship);
		}
	}

	private static void MergeRelationships(ClusterResourceRelationship relationship, ClusterResourceRelationship childRelationship)
	{
		foreach (RelatedClusterResource childResource in childRelationship.m_childResources)
		{
			relationship.m_childResources.Add(childResource);
		}
		foreach (ClusterResourceRelationship childRelationship2 in childRelationship.m_childRelationships)
		{
			Collection<ClusterResourceRelationship> childRelationships = relationship.m_childRelationships;
			childRelationships.Add(childRelationship2);
		}
	}

	private static ParseTreeNode ParseDependencyExpression(Dictionary<Guid, ClusterResource> resourceDictionary, string expression)
	{
		string text = expression.ToLower(CultureInfo.InvariantCulture);
		Stack<ushort> stack = new Stack<ushort>();
		Stack<ParseTreeNode> stack2 = new Stack<ParseTreeNode>();
		if (0 != text.Length)
		{
			do
			{
				switch (text[0])
				{
				case 'o':
					if (text.Length > 2 && text.Substring(0, 2).Equals("or", StringComparison.Ordinal))
					{
						stack.Push(text[0]);
						text = text.Remove(0, 2);
						break;
					}
					throw ExceptionHelp.Build<ArgumentException>(new string[3]
					{
						"expression",
						Resources.Exception_ParsingError_Text,
						(expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)
					});
				case 'a':
					if (text.Length > 3 && text.Substring(0, 3).Equals("and", StringComparison.Ordinal))
					{
						stack.Push(text[0]);
						text = text.Remove(0, 3);
						break;
					}
					throw ExceptionHelp.Build<ArgumentException>(new string[3]
					{
						"expression",
						Resources.Exception_ParsingError_Text,
						(expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)
					});
				case '[':
				{
					int num = text.IndexOf(']');
					if (num >= 0)
					{
						if (stack.Count != 0 && stack.Peek() != 40)
						{
							ParseTreeNode parseTreeNode = stack2.Pop();
							if (stack.Peek() == 97)
							{
								if (!parseTreeNode.IsLeafNode && !(parseTreeNode is AndNode) && !(parseTreeNode is ParenthesisNode))
								{
									throw ExceptionHelp.Build<ArgumentException>(new string[3]
									{
										"expression",
										Resources.Exception_MixedOperators_Text,
										(expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)
									});
								}
								ResourceNode rightNode = CreateResourceNode(resourceDictionary, text.Substring(1, num - 1));
								stack2.Push(new AndNode(parseTreeNode, rightNode));
							}
							else
							{
								if (stack.Peek() != 111)
								{
									throw ExceptionHelp.Build<ArgumentException>(new string[3]
									{
										"expression",
										Resources.Exception_ResourceWithoutOperator_Text,
										(expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)
									});
								}
								if (!parseTreeNode.IsLeafNode && !(parseTreeNode is OrNode) && !(parseTreeNode is ParenthesisNode))
								{
									throw ExceptionHelp.Build<ArgumentException>(new string[3]
									{
										"expression",
										Resources.Exception_MixedOperators_Text,
										(expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)
									});
								}
								ResourceNode rightNode2 = CreateResourceNode(resourceDictionary, text.Substring(1, num - 1));
								stack2.Push(new OrNode(parseTreeNode, rightNode2));
							}
							stack.Pop();
						}
						else
						{
							stack2.Push(CreateResourceNode(resourceDictionary, text.Substring(1, num - 1)));
						}
						text = text.Remove(0, num + 1);
						break;
					}
					throw ExceptionHelp.Build<ArgumentException>(new string[3]
					{
						"expression",
						Resources.Exception_InvalidResourceId_Text,
						(expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)
					});
				}
				case ')':
					if (stack.Peek() == 40)
					{
						ParseTreeNode leftNode = stack2.Pop();
						stack2.Push(new ParenthesisNode(leftNode));
						stack.Pop();
						text = text.Remove(0, 1);
						break;
					}
					throw ExceptionHelp.Build<ArgumentException>(new string[3]
					{
						"expression",
						Resources.Exception_MismatchedParenthesis_Text,
						(expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)
					});
				case '(':
					if (!stack.Contains(40))
					{
						stack.Push(text[0]);
						text = text.Remove(0, 1);
						break;
					}
					throw ExceptionHelp.Build<ArgumentException>(new string[3]
					{
						"expression",
						Resources.Exception_NestedParenthesis_Text,
						(expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)
					});
				case '\t':
				case '\n':
				case '\r':
				case ' ':
					text = text.Remove(0, 1);
					break;
				default:
					throw ExceptionHelp.Build<ArgumentException>(new string[4]
					{
						"expression",
						Resources.Exception_InvalidCharacter_Text,
						text[0].ToString(CultureInfo.CurrentCulture),
						(expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)
					});
				}
			}
			while (0 != text.Length);
		}
		if (stack.Count != 0)
		{
			do
			{
				switch (stack.Pop())
				{
				case 111:
					stack2.Push(new OrNode(stack2.Pop(), stack2.Pop()));
					break;
				case 97:
					stack2.Push(new AndNode(stack2.Pop(), stack2.Pop()));
					break;
				}
			}
			while (stack.Count != 0);
		}
		if (stack2.Count != 1)
		{
			throw new ArgumentException(Resources.Exception_BadExpression_Text, "expression");
		}
		return stack2.Pop();
	}

	private static ResourceNode CreateResourceNode(Dictionary<Guid, ClusterResource> resourceDictionary, string resourceId)
	{
		Guid key = new Guid(resourceId);
		return new ResourceNode(resourceDictionary[key]);
	}

	private string BuildDependencyString(DependencyStringType type, [MarshalAs(UnmanagedType.U1)] bool isChild)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (HasResources)
		{
			AppendItem(stringBuilder, BuildResourceString(type, isChild));
		}
		if (m_childRelationships.Count != 0)
		{
			AppendItem(stringBuilder, BuildRelationshipString(type, isChild));
		}
		return stringBuilder.ToString();
	}

	private string GetResourceString(ClusterResource resource, DependencyStringType type)
	{
		switch (type)
		{
		default:
			return null;
		case DependencyStringType.Parseable:
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", resource.Name);
		case DependencyStringType.DisplayName:
			return string.Format(CultureInfo.InvariantCulture, "'{0}'", resource.DisplayName);
		case DependencyStringType.Guid:
		{
			Guid id = resource.Id;
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", id);
		}
		}
	}

	private void MergeRelationship(ClusterResourceRelationship relationship)
	{
		foreach (ClusterResourceRelationship childRelationship in relationship.m_childRelationships)
		{
			m_childRelationships.Add(childRelationship);
		}
		foreach (RelatedClusterResource childResource in relationship.m_childResources)
		{
			m_childResources.Add(childResource);
		}
	}

	public ClusterResourceRelationship()
	{
		m_type = ResourceRelationshipType.And;
		m_childResources = new Collection<RelatedClusterResource>();
		m_childRelationships = new Collection<ClusterResourceRelationship>();
	}

	public string GetDisplayDependencyString()
	{
		return BuildDependencyString(DependencyStringType.DisplayName, isChild: false);
	}

	public string GetParseableDependencyString()
	{
		return BuildDependencyString(DependencyStringType.Parseable, isChild: false);
	}

	public sealed override string ToString()
	{
		return BuildDependencyString(DependencyStringType.Guid, isChild: false);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool AreEqual(ClusterResourceRelationship one, ClusterResourceRelationship two)
	{
		ResourceRelationshipType type = one.m_type;
		ResourceRelationshipType type2 = two.m_type;
		if (type != type2)
		{
			return false;
		}
		Collection<RelatedClusterResource> childResources = one.m_childResources;
		Collection<RelatedClusterResource> childResources2 = two.m_childResources;
		if (childResources.Count != childResources2.Count)
		{
			return false;
		}
		Collection<ClusterResourceRelationship> childRelationships = one.m_childRelationships;
		Collection<ClusterResourceRelationship> childRelationships2 = two.m_childRelationships;
		if (childRelationships.Count != childRelationships2.Count)
		{
			return false;
		}
		foreach (RelatedClusterResource childResource in one.m_childResources)
		{
			bool flag = false;
			IEnumerator<RelatedClusterResource> enumerator2 = two.m_childResources.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					Guid id = enumerator2.Current.RelatedResource.Id;
					if (childResource.RelatedResource.Id == id)
					{
						flag = true;
						break;
					}
				}
			}
			finally
			{
				IEnumerator<RelatedClusterResource> enumerator3 = enumerator2;
				IDisposable disposable = enumerator2;
				enumerator2?.Dispose();
			}
			if (!flag)
			{
				return false;
			}
		}
		foreach (ClusterResourceRelationship childRelationship in one.m_childRelationships)
		{
			bool flag2 = false;
			Collection<ClusterResourceRelationship> childRelationships3 = two.m_childRelationships;
			IEnumerator<ClusterResourceRelationship> enumerator5 = childRelationships3.GetEnumerator();
			try
			{
				while (enumerator5.MoveNext())
				{
					ClusterResourceRelationship current3 = enumerator5.Current;
					if (AreEqual(childRelationship, current3))
					{
						flag2 = true;
						break;
					}
				}
			}
			finally
			{
				IEnumerator<ClusterResourceRelationship> enumerator6 = enumerator5;
				IDisposable disposable2 = enumerator5;
				enumerator5?.Dispose();
			}
			if (!flag2)
			{
				return false;
			}
		}
		return true;
	}

	public void Normalize()
	{
		foreach (ClusterResourceRelationship childRelationship in m_childRelationships)
		{
			childRelationship.Normalize();
		}
		int num = m_childRelationships.Count - 1;
		if (num < 0)
		{
			return;
		}
		do
		{
			ClusterResourceRelationship clusterResourceRelationship = m_childRelationships[num];
			if (clusterResourceRelationship.m_childRelationships.Count == 0 && clusterResourceRelationship.m_childResources.Count == 0)
			{
				m_childRelationships.RemoveAt(num);
			}
			else if (clusterResourceRelationship.m_type == m_type)
			{
				MergeRelationship(clusterResourceRelationship);
				m_childRelationships.RemoveAt(num);
			}
			num += -1;
		}
		while (num >= 0);
	}
}
