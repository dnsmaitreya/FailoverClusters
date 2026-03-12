using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class DependencyRelationship : DependencyRelationshipBase
{
	private sealed class ResourceNode : ParseTreeNode
	{
		private readonly Resource memberResource;

		public override bool IsLeafNode => true;

		public Resource Resource => memberResource;

		public ResourceNode(Resource resource)
			: base(null, null)
		{
			memberResource = resource;
		}
	}

	public Collection<RelatedResource> ChildResources { get; private set; }

	public Collection<DependencyRelationship> ChildRelationships { get; private set; }

	public override bool HasResources => ChildResources.Count != 0;

	protected override bool HasRelationship => ChildRelationships.Count != 0;

	public DependencyRelationship()
	{
		ChildRelationships = new Collection<DependencyRelationship>();
		ChildResources = new Collection<RelatedResource>();
	}

	protected override string BuildResourceString(DependencyStringType dependencyType, bool isChild)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		foreach (RelatedResource childResource in ChildResources)
		{
			Resource resource = childResource.Resource;
			string text = GetResourceString(resource.Id, resource.DisplayName, resource.Name, dependencyType);
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
		foreach (DependencyRelationship childRelationship in ChildRelationships)
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

	public static DependencyRelationship Parse(Resource resource, string expression)
	{
		if (expression.Length == 0)
		{
			return new DependencyRelationship
			{
				RelationType = RelationshipType.And
			};
		}
		Dictionary<Guid, Resource> resourceIdDictionary = new Dictionary<Guid, Resource>();
		IList<Guid> list = (IList<Guid>)resource.Dependencies;
		if (list == null)
		{
			return new DependencyRelationship
			{
				RelationType = RelationshipType.And
			};
		}
		foreach (Guid item in list)
		{
			Guid id = item;
			Resource.Get(resource.Cluster, item, delegate(OperationResult<Resource> operationResult)
			{
				if (operationResult.Error != null)
				{
					ClusterLog.LogException(LogLevel.Info, operationResult.Error, "There was an error getting the dependency '{0}'".FormatCurrentCulture(id));
				}
				else
				{
					resourceIdDictionary.Add(id, operationResult.Result);
				}
			}, OperationType.Sync);
		}
		return Parse(resourceIdDictionary, expression);
	}

	public static void Parse(Group group, string expression, Action<DependencyRelationship> parseResult)
	{
		Parse(group, expression, ResultExecution.DoNotCare, parseResult);
	}

	public static void Parse(Group group, string expression, ResultExecution resultExecution, Action<DependencyRelationship> parseResult)
	{
		Tuple<string, Action<DependencyRelationship>> parameter = new Tuple<string, Action<DependencyRelationship>>(expression, parseResult);
		group.AllResources.ExecuteQuery(resultExecution, AllResourcesQuery, parameter);
	}

	private static void AllResourcesQuery(OperationResult<IClusterList<Resource>> loadResult)
	{
		Tuple<string, Action<DependencyRelationship>> obj = (Tuple<string, Action<DependencyRelationship>>)loadResult.Parameter;
		string item = obj.Item1;
		Action<DependencyRelationship> item2 = obj.Item2;
		DependencyRelationship obj2 = ((item.Length != 0) ? Parse(loadResult.Result.ToDictionary((Resource childResource) => childResource.Id), item) : new DependencyRelationship
		{
			RelationType = RelationshipType.And
		});
		item2(obj2);
	}

	private static DependencyRelationship Parse(Dictionary<Guid, Resource> resourceIdDictionary, string expression)
	{
		ParseTreeNode parseTreeNode = ParseDependencyExpression(resourceIdDictionary, expression);
		if (parseTreeNode == null)
		{
			return null;
		}
		return ConvertParseTree(parseTreeNode);
	}

	private static DependencyRelationship ConvertParseTree(ParseTreeNode parseTree)
	{
		DependencyRelationship dependencyRelationship = null;
		if (parseTree is AndNode)
		{
			dependencyRelationship = new DependencyRelationship
			{
				RelationType = RelationshipType.And
			};
			ProcessChild(dependencyRelationship, parseTree.LeftNode);
			ProcessChild(dependencyRelationship, parseTree.RightNode);
		}
		else if (parseTree is OrNode)
		{
			dependencyRelationship = new DependencyRelationship
			{
				RelationType = RelationshipType.Or
			};
			ProcessChild(dependencyRelationship, parseTree.LeftNode);
			ProcessChild(dependencyRelationship, parseTree.RightNode);
		}
		else if (parseTree is ParenthesisNode)
		{
			parseTree = parseTree.LeftNode;
			dependencyRelationship = ConvertParseTree(parseTree);
		}
		else if (parseTree is ResourceNode)
		{
			dependencyRelationship = new DependencyRelationship
			{
				RelationType = RelationshipType.And
			};
			ProcessChild(dependencyRelationship, parseTree);
		}
		return dependencyRelationship;
	}

	private static void ProcessChild(DependencyRelationship relationship, ParseTreeNode parseTreeNodeChild)
	{
		if (parseTreeNodeChild is ResourceNode resourceNode)
		{
			relationship.ChildResources.Add(new RelatedResource(resourceNode.Resource, resourceNode.Resource.Name));
			return;
		}
		DependencyRelationship dependencyRelationship = ConvertParseTree(parseTreeNodeChild);
		if (dependencyRelationship.RelationType == relationship.RelationType)
		{
			MergeRelationships(relationship, dependencyRelationship);
		}
		else
		{
			relationship.ChildRelationships.Add(dependencyRelationship);
		}
	}

	private static void MergeRelationships(DependencyRelationship relationship, DependencyRelationship childRelationship)
	{
		foreach (RelatedResource childResource in childRelationship.ChildResources)
		{
			relationship.ChildResources.Add(childResource);
		}
		foreach (DependencyRelationship childRelationship2 in childRelationship.ChildRelationships)
		{
			relationship.ChildRelationships.Add(childRelationship2);
		}
	}

	private static ParseTreeNode ParseDependencyExpression(Dictionary<Guid, Resource> resourceIdDictionary, string expression)
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

	private static ResourceNode CreateResourceNode(Dictionary<Guid, Resource> resourceIdDictionary, Guid resourceId)
	{
		resourceIdDictionary.TryGetValue(resourceId, out var value);
		if (value == null)
		{
			return null;
		}
		return new ResourceNode(value);
	}
}
