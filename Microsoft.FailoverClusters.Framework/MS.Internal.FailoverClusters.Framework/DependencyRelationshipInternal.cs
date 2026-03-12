using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal class DependencyRelationshipInternal : DependencyRelationshipBase
{
	private sealed class ResourceNode : ParseTreeNode
	{
		private readonly PResource memberResource;

		public override bool IsLeafNode => true;

		public PResource Resource => memberResource;

		public ResourceNode(PResource resource)
			: base(null, null)
		{
			memberResource = resource;
		}
	}

	public Collection<RelatedResourceInternal> ChildResources { get; private set; }

	public Collection<DependencyRelationshipInternal> ChildRelationships { get; private set; }

	public override bool HasResources => ChildResources.Count != 0;

	protected override bool HasRelationship => ChildRelationships.Count != 0;

	public DependencyRelationshipInternal()
	{
		ChildRelationships = new Collection<DependencyRelationshipInternal>();
		ChildResources = new Collection<RelatedResourceInternal>();
	}

	protected override string BuildResourceString(DependencyStringType dependencyType, bool isChild)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		foreach (RelatedResourceInternal childResource in ChildResources)
		{
			PResource resource = childResource.Resource;
			string text = GetResourceString(resource.Id, resource.Name, resource.Name, dependencyType);
			if (base.RelationType == RelationshipType.And && (dependencyType == DependencyStringType.Guid || dependencyType == DependencyStringType.Parseable))
			{
				text = string.Format(CultureInfo.InvariantCulture, "({0})", text);
			}
			AppendItem(stringBuilder, text);
		}
		if (ChildResources.Count > 1)
		{
			flag = true;
		}
		if (isChild && flag)
		{
			stringBuilder.Insert(0, "(");
			stringBuilder.Append(")");
		}
		return stringBuilder.ToString();
	}

	protected override string BuildRelationshipString(DependencyStringType dependencyType, bool isChild)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		foreach (DependencyRelationshipInternal childRelationship in ChildRelationships)
		{
			AppendItem(stringBuilder, childRelationship.BuildDependencyString(dependencyType, isChild: true));
		}
		if (ChildRelationships.Count > 1)
		{
			flag = true;
		}
		if (isChild && flag)
		{
			stringBuilder.Insert(0, "(");
			stringBuilder.Append(")");
		}
		return stringBuilder.ToString();
	}

	public static DependencyRelationshipInternal Parse(PResource resource, string expression)
	{
		if (expression.Length == 0)
		{
			return new DependencyRelationshipInternal
			{
				RelationType = RelationshipType.And
			};
		}
		Dictionary<Guid, PResource> dictionary = new Dictionary<Guid, PResource>();
		IList<Guid> list = (IList<Guid>)resource.Dependencies;
		if (list == null)
		{
			return new DependencyRelationshipInternal
			{
				RelationType = RelationshipType.And
			};
		}
		foreach (Guid item in list)
		{
			Guid key = item;
			PResource value = resource.Cluster.Server.Resource.Open(item);
			dictionary.Add(key, value);
		}
		return Parse(dictionary, expression);
	}

	private static DependencyRelationshipInternal Parse(Dictionary<Guid, PResource> resourceIdDictionary, string expression)
	{
		ParseTreeNode parseTreeNode = ParseDependencyExpression(resourceIdDictionary, expression);
		if (parseTreeNode == null)
		{
			return null;
		}
		return ConvertParseTree(parseTreeNode);
	}

	private static DependencyRelationshipInternal ConvertParseTree(ParseTreeNode parseTree)
	{
		DependencyRelationshipInternal dependencyRelationshipInternal = null;
		if (parseTree is AndNode)
		{
			dependencyRelationshipInternal = new DependencyRelationshipInternal
			{
				RelationType = RelationshipType.And
			};
			ProcessChild(dependencyRelationshipInternal, parseTree.LeftNode);
			ProcessChild(dependencyRelationshipInternal, parseTree.RightNode);
		}
		else if (parseTree is OrNode)
		{
			dependencyRelationshipInternal = new DependencyRelationshipInternal
			{
				RelationType = RelationshipType.Or
			};
			ProcessChild(dependencyRelationshipInternal, parseTree.LeftNode);
			ProcessChild(dependencyRelationshipInternal, parseTree.RightNode);
		}
		else if (parseTree is ParenthesisNode)
		{
			parseTree = parseTree.LeftNode;
			dependencyRelationshipInternal = ConvertParseTree(parseTree);
		}
		else if (parseTree is ResourceNode)
		{
			dependencyRelationshipInternal = new DependencyRelationshipInternal
			{
				RelationType = RelationshipType.And
			};
			ProcessChild(dependencyRelationshipInternal, parseTree);
		}
		return dependencyRelationshipInternal;
	}

	private static void ProcessChild(DependencyRelationshipInternal relationship, ParseTreeNode parseTreeNodeChild)
	{
		if (parseTreeNodeChild is ResourceNode resourceNode)
		{
			relationship.ChildResources.Add(new RelatedResourceInternal(resourceNode.Resource, resourceNode.Resource.Name));
			return;
		}
		DependencyRelationshipInternal dependencyRelationshipInternal = ConvertParseTree(parseTreeNodeChild);
		if (dependencyRelationshipInternal.RelationType == relationship.RelationType)
		{
			MergeRelationships(relationship, dependencyRelationshipInternal);
		}
		else
		{
			relationship.ChildRelationships.Add(dependencyRelationshipInternal);
		}
	}

	private static void MergeRelationships(DependencyRelationshipInternal relationship, DependencyRelationshipInternal childRelationship)
	{
		foreach (RelatedResourceInternal childResource in childRelationship.ChildResources)
		{
			relationship.ChildResources.Add(childResource);
		}
		foreach (DependencyRelationshipInternal childRelationship2 in childRelationship.ChildRelationships)
		{
			relationship.ChildRelationships.Add(childRelationship2);
		}
	}

	private static ParseTreeNode ParseDependencyExpression(Dictionary<Guid, PResource> resourceIdDictionary, string expression)
	{
		string text = expression.ToLowerInvariant();
		Stack<char> stack = new Stack<char>();
		Stack<ParseTreeNode> stack2 = new Stack<ParseTreeNode>();
		while (text.Length != 0)
		{
			switch (text[0])
			{
			case '\t':
			case '\n':
			case '\r':
			case ' ':
				text = text.Remove(0, 1);
				break;
			case '[':
			{
				int num = text.IndexOf(']');
				if (num < 0)
				{
					throw new ArgumentException(ExceptionResources.InvalidResourceId_Default.FormatCurrentCulture((expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)), "expression");
				}
				if (stack.Count == 0 || stack.Peek() == '(')
				{
					ResourceNode resourceNode = CreateResourceNode(resourceIdDictionary, new Guid(text.Substring(1, num - 1)));
					if (resourceNode == null)
					{
						return null;
					}
					stack2.Push(resourceNode);
				}
				else
				{
					ParseTreeNode parseTreeNode = stack2.Pop();
					if (stack.Peek() == 'a')
					{
						if (!parseTreeNode.IsLeafNode && !(parseTreeNode is AndNode) && !(parseTreeNode is ParenthesisNode))
						{
							throw new ArgumentException(ExceptionResources.MixedOperators_Default.FormatCurrentCulture((expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)), "expression");
						}
						ResourceNode resourceNode2 = CreateResourceNode(resourceIdDictionary, new Guid(text.Substring(1, num - 1)));
						if (resourceNode2 == null)
						{
							return null;
						}
						stack2.Push(new AndNode(parseTreeNode, resourceNode2));
					}
					else
					{
						if (stack.Peek() != 'o')
						{
							throw new ArgumentException(ExceptionResources.ResourceWithoutOperator_Default.FormatCurrentCulture((expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)), "expression");
						}
						if (!parseTreeNode.IsLeafNode && !(parseTreeNode is OrNode) && !(parseTreeNode is ParenthesisNode))
						{
							throw new ArgumentException(ExceptionResources.MixedOperators_Default.FormatCurrentCulture((expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)), "expression");
						}
						ResourceNode resourceNode3 = CreateResourceNode(resourceIdDictionary, new Guid(text.Substring(1, num - 1)));
						if (resourceNode3 == null)
						{
							return null;
						}
						stack2.Push(new OrNode(parseTreeNode, resourceNode3));
					}
					stack.Pop();
				}
				text = text.Remove(0, num + 1);
				break;
			}
			case '(':
				if (stack.Contains('('))
				{
					throw new ArgumentException(ExceptionResources.NestedParenthesis_Default.FormatCurrentCulture((expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)), "expression");
				}
				stack.Push(text[0]);
				text = text.Remove(0, 1);
				break;
			case ')':
			{
				if (stack.Peek() != '(')
				{
					throw new ArgumentException(ExceptionResources.MismatchedParenthesis_Default.FormatCurrentCulture((expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)), "expression");
				}
				ParseTreeNode leftNode = stack2.Pop();
				stack2.Push(new ParenthesisNode(leftNode));
				stack.Pop();
				text = text.Remove(0, 1);
				break;
			}
			case 'a':
				if (text.Length <= 3 || !text.Substring(0, 3).Equals("and", StringComparison.Ordinal))
				{
					throw new ArgumentException(ExceptionResources.ParsingError_Default.FormatCurrentCulture((expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)), "expression");
				}
				stack.Push(text[0]);
				text = text.Remove(0, 3);
				break;
			case 'o':
				if (text.Length <= 2 || !text.Substring(0, 2).Equals("or", StringComparison.Ordinal))
				{
					throw new ArgumentException(ExceptionResources.ParsingError_Default.FormatCurrentCulture((expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)), "expression");
				}
				stack.Push(text[0]);
				text = text.Remove(0, 2);
				break;
			default:
				throw new ArgumentException(ExceptionResources.InvalidCharacter_Default.FormatCurrentCulture((expression.Length - text.Length).ToString(CultureInfo.CurrentCulture)), "expression");
			}
		}
		while (stack.Count != 0)
		{
			switch (stack.Pop())
			{
			case 'a':
				stack2.Push(new AndNode(stack2.Pop(), stack2.Pop()));
				break;
			case 'o':
				stack2.Push(new OrNode(stack2.Pop(), stack2.Pop()));
				break;
			}
		}
		if (stack2.Count != 1)
		{
			throw new ArgumentException(ExceptionResources.BadExpression_Default, "expression");
		}
		return stack2.Pop();
	}

	private static ResourceNode CreateResourceNode(Dictionary<Guid, PResource> resourceIdDictionary, Guid resourceId)
	{
		resourceIdDictionary.TryGetValue(resourceId, out var value);
		if (value == null)
		{
			return null;
		}
		return new ResourceNode(value);
	}
}
