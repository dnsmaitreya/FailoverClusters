using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal class QueryTranslator : ExpressionVisitor
{
	private class OrderByTranslator : ExpressionVisitor
	{
		private readonly List<OrderByItem> members;

		private OrderDirection currentDirection;

		internal ReadOnlyCollection<OrderByItem> DataMembers => members.AsReadOnly();

		internal string OrderByClause
		{
			get
			{
				if (members.Count == 0)
				{
					return string.Empty;
				}
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = true;
				foreach (OrderByItem item in Enumerable.Reverse(members))
				{
					if (!flag)
					{
						stringBuilder.Append(", ");
					}
					else
					{
						flag = false;
					}
					stringBuilder.Append(FormatHelper.WrapInBrackets(item.DataMember.MappedName));
					switch (item.Direction)
					{
					case OrderDirection.Ascending:
						stringBuilder.Append(" ASC");
						break;
					case OrderDirection.Descending:
						stringBuilder.Append(" DESC");
						break;
					default:
						throw new NotSupportedException("The selected sorting direction is not supported");
					}
				}
				return stringBuilder.ToString();
			}
		}

		public OrderByTranslator()
		{
			members = new List<OrderByItem>();
		}

		internal virtual Expression Visit(Expression e, OrderDirection direction)
		{
			currentDirection = direction;
			return Visit(e);
		}

		protected override Expression VisitMemberAccess(MemberExpression m)
		{
			if (m.Expression == null || m.Expression.NodeType != ExpressionType.Parameter)
			{
				throw new NotSupportedException(ExceptionResources.TheMemberIsNotSupported.FormatCurrentCulture(m.Member.Name));
			}
			ClusterObjectMetaDataMember clusterObjectMetaDataMember = new ClusterObjectMetaDataMember(m.Expression.Type, m.Member);
			if (clusterObjectMetaDataMember == null)
			{
				throw new EntryPointNotFoundException(ExceptionResources.TheWhereMemberCannotBeFound.FormatCurrentCulture(m.Member.Name, m.Expression.Type.Name));
			}
			members.Add(new OrderByItem
			{
				DataMember = clusterObjectMetaDataMember,
				Direction = currentDirection
			});
			return m;
		}
	}

	private class ProjectionTranslator : ExpressionVisitor
	{
		private readonly List<ClusterObjectMetaDataMember> members;

		private LambdaExpression projectionLambda;

		internal string ProjectionClause => FormatHelper.FormatColumnNamesInSequence(members);

		internal ReadOnlyCollection<ClusterObjectMetaDataMember> DataMembers => members.AsReadOnly();

		internal LambdaExpression ProjectionLambda => projectionLambda;

		public ProjectionTranslator()
		{
			members = new List<ClusterObjectMetaDataMember>();
		}

		public ProjectionTranslator(IEnumerable<ClusterObjectMetaDataMember> dataMemebers)
		{
			members = new List<ClusterObjectMetaDataMember>(dataMemebers);
		}

		public void AddProjectionFields(Type clusterObjectType)
		{
			if (!clusterObjectType.IsSubclassOf(typeof(ClusterObject)))
			{
				throw new ArgumentException("clusterObjectType parameter must be a ClusterObject derived type", "clusterObjectType");
			}
			foreach (MemberInfo member2 in from member in clusterObjectType.GetMembers(BindingFlags.Instance | BindingFlags.Public)
				where member.MemberType == MemberTypes.Property
				let memberAttrs = member.GetCustomAttributes(inherit: true).OfType<ColumnAttribute>()
				where memberAttrs.Count() > 0
				let memberAttr = memberAttrs.First()
				where memberAttr.AutoSync == AutoSync.Always
				select member)
			{
				if (members.Find((ClusterObjectMetaDataMember m) => m.Name == member2.Name) == null)
				{
					members.Add(new ClusterObjectMetaDataMember(clusterObjectType, member2));
				}
			}
		}

		internal void Translate(LambdaExpression lambda)
		{
			projectionLambda = lambda;
			base.VisitLambda(lambda);
		}

		protected override Expression VisitMemberAccess(MemberExpression m)
		{
			if (m.Expression == null || m.Expression.NodeType != ExpressionType.Parameter)
			{
				throw new NotSupportedException(ExceptionResources.TheMemberIsNotSupported.FormatCurrentCulture(m.Member.Name));
			}
			ClusterObjectMetaDataMember clusterObjectMetaDataMember = new ClusterObjectMetaDataMember(m.Expression.Type, m.Member);
			if (clusterObjectMetaDataMember == null)
			{
				throw new Exception(ExceptionResources.TheSelectMemberCannotBeFound.FormatCurrentCulture(m.Member.Name, m.Expression.Type.Name));
			}
			members.Add(clusterObjectMetaDataMember);
			return m;
		}
	}

	private class TakeTranslator : ExpressionVisitor
	{
		private int? count;

		private bool useDefault;

		internal int? Count => count;

		internal bool UseDefault => useDefault;

		internal static TakeTranslator GetNewFirstTranslator(bool useDefault)
		{
			return new TakeTranslator
			{
				count = 1,
				useDefault = useDefault
			};
		}

		internal void Translate(Expression exp)
		{
			Visit(exp);
		}

		protected override Expression VisitConstant(ConstantExpression c)
		{
			if (c.Type == typeof(int))
			{
				count = (int)c.Value;
			}
			return c;
		}
	}

	private class WhereTranslator : ExpressionVisitor
	{
		private readonly StringBuilder sb;

		private readonly List<ClusterObjectMetaDataMember> members;

		private readonly List<IClusterQueryArgument> syntaxis;

		private LambdaExpression lambda;

		internal ReadOnlyCollection<ClusterObjectMetaDataMember> DataMembers => members.AsReadOnly();

		internal List<IClusterQueryArgument> Syntaxis => syntaxis;

		internal LambdaExpression Lambda => lambda;

		internal string WhereClause => sb.ToString();

		public WhereTranslator()
		{
			members = new List<ClusterObjectMetaDataMember>();
			syntaxis = new List<IClusterQueryArgument>();
			sb = new StringBuilder();
		}

		internal void Translate(LambdaExpression lambdaExpression)
		{
			lambda = lambdaExpression;
			base.VisitLambda(lambdaExpression);
		}

		protected override Expression VisitUnary(UnaryExpression u)
		{
			ExpressionType nodeType = u.NodeType;
			if (nodeType == ExpressionType.Not)
			{
				sb.Append(" NOT ");
				Visit(u.Operand);
			}
			else
			{
				Visit(u.Operand);
			}
			return u;
		}

		protected override Expression VisitBinary(BinaryExpression b)
		{
			sb.Append("(");
			syntaxis.Add(new StartEndArgument(StartEndArgumentType.Start));
			Visit(b.Left);
			switch (b.NodeType)
			{
			case ExpressionType.And:
			case ExpressionType.AndAlso:
				sb.Append(" AND ");
				syntaxis.Add(new OperatorArgument(OperatorType.And));
				break;
			case ExpressionType.Or:
			case ExpressionType.OrElse:
				sb.Append(" OR ");
				syntaxis.Add(new OperatorArgument(OperatorType.Or));
				break;
			case ExpressionType.Equal:
				if (IsComparingWithNull(b))
				{
					sb.Append(" IS ");
					syntaxis.Add(new OperatorArgument(OperatorType.Is));
				}
				else
				{
					sb.Append(" = ");
					syntaxis.Add(new OperatorArgument(OperatorType.Equal));
				}
				break;
			case ExpressionType.NotEqual:
				if (IsComparingWithNull(b))
				{
					sb.Append(" IS NOT ");
					syntaxis.Add(new OperatorArgument(OperatorType.IsNot));
				}
				else
				{
					sb.Append(" <> ");
					syntaxis.Add(new OperatorArgument(OperatorType.NotEqual));
				}
				break;
			case ExpressionType.GreaterThan:
				sb.Append(" > ");
				syntaxis.Add(new OperatorArgument(OperatorType.GreaterThan));
				break;
			case ExpressionType.GreaterThanOrEqual:
				sb.Append(" >= ");
				syntaxis.Add(new OperatorArgument(OperatorType.GreaterThanOrEqual));
				break;
			case ExpressionType.LessThan:
				sb.Append(" < ");
				syntaxis.Add(new OperatorArgument(OperatorType.LessThan));
				break;
			case ExpressionType.LessThanOrEqual:
				sb.Append(" <= ");
				syntaxis.Add(new OperatorArgument(OperatorType.LessThanOrEqual));
				break;
			default:
				throw new NotSupportedException(ExceptionResources.WherePredicatedDoesntSupportBinaryOperator.FormatCurrentCulture(b.NodeType));
			}
			Visit(b.Right);
			sb.Append(")");
			syntaxis.Add(new StartEndArgument(StartEndArgumentType.End));
			return b;
		}

		protected override Expression VisitMethodCall(MethodCallExpression m)
		{
			Expression expression = Visit(m.Object);
			if (m.Method.DeclaringType.FullName == "System.Linq.Enumerable")
			{
				string name = m.Method.Name;
				if (name == "Contains")
				{
					sb.Append(" Contains ");
					syntaxis.Add(new OperatorArgument(OperatorType.Contains));
				}
			}
			else if (m.Method.DeclaringType.FullName == "System.String")
			{
				switch (m.Method.Name)
				{
				case "Contains":
					sb.Append(" Contains ");
					syntaxis.Add(new OperatorArgument(OperatorType.Contains));
					break;
				case "StartsWith":
					sb.Append(" StartsWith ");
					syntaxis.Add(new OperatorArgument(OperatorType.StartsWith));
					break;
				case "EndsWith":
					sb.Append(" EndsWith ");
					syntaxis.Add(new OperatorArgument(OperatorType.EndsWith));
					break;
				default:
					return m;
				}
			}
			else if (m.Method.DeclaringType.FullName == "System.Object")
			{
				string name = m.Method.Name;
				if (name == "Equals")
				{
					sb.Append(" = ");
					syntaxis.Add(new OperatorArgument(OperatorType.Equal));
				}
			}
			IEnumerable<Expression> enumerable = VisitExpressionList(m.Arguments);
			if (expression == m.Object && enumerable == m.Arguments)
			{
				return m;
			}
			return Expression.Call(expression, m.Method, enumerable);
		}

		protected override Expression VisitMemberAccess(MemberExpression m)
		{
			if (m.Expression == null || (m.Expression.NodeType != ExpressionType.Parameter && m.Expression.NodeType != ExpressionType.Constant && m.Expression.NodeType != ExpressionType.MemberAccess))
			{
				throw new NotSupportedException(ExceptionResources.TheMemberIsNotSupported.FormatCurrentCulture(m.Member.Name));
			}
			MemberInfo member = m.Member;
			if (m.Expression.NodeType == ExpressionType.Constant)
			{
				if (m.Member is PropertyInfo && ((PropertyInfo)m.Member).PropertyType == typeof(Guid))
				{
					object value = ((ConstantExpression)m.Expression).Value;
					object value2 = value.GetType().GetProperty(m.Member.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(value, null);
					sb.Append(FormatHelper.FormatDbValue(value2));
					syntaxis.Add(new GuidArgument(null, value2));
					return m;
				}
				if (!(m.Member is FieldInfo))
				{
					throw new NotSupportedException(ExceptionResources.TheMemberIsNotSupported.FormatCurrentCulture(m.Member.Name));
				}
				FieldInfo fieldInfo = (FieldInfo)m.Member;
				if (typeof(ClusterObject).IsAssignableFrom(fieldInfo.FieldType))
				{
					object value3 = ((ConstantExpression)m.Expression).Value;
					ClusterObject clusterObject = (ClusterObject)value3.GetType().GetField(m.Member.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(value3);
					if (clusterObject is ResourceType)
					{
						ResourceType resourceType = clusterObject as ResourceType;
						sb.Append(FormatHelper.FormatDbValue(resourceType.Name));
						syntaxis.Add(new GuidArgument(resourceType.Name, resourceType.Name));
					}
					else
					{
						sb.Append(FormatHelper.FormatDbValue(clusterObject.Id));
						syntaxis.Add(new GuidArgument(clusterObject.Name, clusterObject.Id));
					}
					return m;
				}
				object value4 = ((ConstantExpression)m.Expression).Value;
				object value5 = value4.GetType().GetField(m.Member.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(value4);
				sb.Append(FormatHelper.FormatDbValue(value5));
				syntaxis.Add(new ValueArgument(value5));
				return m;
			}
			ClusterObjectMetaDataMember clusterObjectMetaDataMember = new ClusterObjectMetaDataMember(m.Expression.Type, member);
			if (clusterObjectMetaDataMember == null)
			{
				throw new EntryPointNotFoundException(ExceptionResources.TheWhereMemberCannotBeFound.FormatCurrentCulture(m.Member.Name, m.Expression.Type.Name));
			}
			members.Add(clusterObjectMetaDataMember);
			sb.Append(FormatHelper.WrapInBrackets(clusterObjectMetaDataMember.MappedName));
			syntaxis.Add(new FieldArgument(clusterObjectMetaDataMember.MappedName));
			return m;
		}

		protected override Expression VisitConstant(ConstantExpression c)
		{
			if (c.Value is IQueryable)
			{
				throw new NotSupportedException("Sub queries are not supported");
			}
			object value = c.Value;
			if (value is ClusterObject)
			{
				ClusterObject clusterObject = (ClusterObject)value;
				if (clusterObject is ResourceType)
				{
					ResourceType resourceType = clusterObject as ResourceType;
					sb.Append(FormatHelper.FormatDbValue(resourceType.Name));
					syntaxis.Add(new GuidArgument(resourceType.Name, resourceType.Name));
				}
				else
				{
					sb.Append(FormatHelper.FormatDbValue(clusterObject.Id));
					syntaxis.Add(new GuidArgument(clusterObject.Name, clusterObject.Id));
				}
			}
			else
			{
				sb.Append(FormatHelper.FormatDbValue(value));
				syntaxis.Add(new ValueArgument(value));
			}
			return c;
		}

		private static bool IsNullConstant(ConstantExpression ce)
		{
			if (ce != null)
			{
				return ce.Value == null;
			}
			return false;
		}

		internal virtual bool IsComparingWithNull(BinaryExpression b)
		{
			if ((b.NodeType == ExpressionType.Equal || b.NodeType == ExpressionType.NotEqual) && (IsNullConstant(b.Left as ConstantExpression) || IsNullConstant(b.Right as ConstantExpression)))
			{
				return true;
			}
			return false;
		}
	}

	private readonly OrderByTranslator orderByTranslator;

	private WhereTranslator whereTranslator;

	private ProjectionTranslator selectTranslator;

	private TakeTranslator takeTranslator;

	internal QueryTranslator()
	{
		orderByTranslator = new OrderByTranslator();
	}

	internal QueryInfo Translate(Expression query)
	{
		Visit(query);
		QueryInfo queryInfo = ConvertToExecutableQuery(query);
		bool flag;
		do
		{
			flag = false;
			for (int num = queryInfo.WhereSyntaxis.Count - 1; num >= 0; num--)
			{
				IClusterQueryArgument argument = queryInfo.WhereSyntaxis[num];
				if (argument is FieldArgument && queryInfo.WhereFields.Exists((ClusterObjectMetaDataMember item) => item.Name.Equals(argument.Name, StringComparison.OrdinalIgnoreCase) && item.IsMetaDataField))
				{
					queryInfo.WhereSyntaxis.RemoveAt(num);
					flag = true;
					break;
				}
				if (argument is OperatorArgument)
				{
					if ((num > 0 && queryInfo.WhereSyntaxis[num - 1] is StartEndArgument && ((StartEndArgument)queryInfo.WhereSyntaxis[num - 1]).ConditionalType == StartEndArgumentType.Start) || (num < queryInfo.WhereSyntaxis.Count - 1 && queryInfo.WhereSyntaxis[num + 1] is StartEndArgument && ((StartEndArgument)queryInfo.WhereSyntaxis[num + 1]).ConditionalType == StartEndArgumentType.End))
					{
						queryInfo.WhereSyntaxis.RemoveAt(num);
						flag = true;
						break;
					}
					if ((num > 0 && queryInfo.WhereSyntaxis[num - 1] is OperatorArgument) || (num < queryInfo.WhereSyntaxis.Count - 1 && queryInfo.WhereSyntaxis[num + 1] is OperatorArgument))
					{
						queryInfo.WhereSyntaxis.RemoveAt(num);
						flag = true;
						break;
					}
				}
				if (argument is ValueArgument)
				{
					if (num > 0 && queryInfo.WhereSyntaxis[num - 1] is StartEndArgument && ((StartEndArgument)queryInfo.WhereSyntaxis[num - 1]).ConditionalType == StartEndArgumentType.Start && num < queryInfo.WhereSyntaxis.Count - 1 && queryInfo.WhereSyntaxis[num + 1] is StartEndArgument && ((StartEndArgument)queryInfo.WhereSyntaxis[num + 1]).ConditionalType == StartEndArgumentType.End)
					{
						queryInfo.WhereSyntaxis.RemoveAt(num);
						flag = true;
						break;
					}
					if (num > 0 && queryInfo.WhereSyntaxis[num - 1] is OperatorArgument && ((queryInfo.WhereSyntaxis[num - 1] as OperatorArgument).OperatorType == OperatorType.And || (queryInfo.WhereSyntaxis[num - 1] as OperatorArgument).OperatorType == OperatorType.Or) && num < queryInfo.WhereSyntaxis.Count - 1 && queryInfo.WhereSyntaxis[num + 1] is StartEndArgument && (queryInfo.WhereSyntaxis[num + 1] as StartEndArgument).ConditionalType == StartEndArgumentType.End)
					{
						queryInfo.WhereSyntaxis.RemoveAt(num);
						flag = true;
						break;
					}
					if (num > 0 && queryInfo.WhereSyntaxis[num - 1] is StartEndArgument && (queryInfo.WhereSyntaxis[num - 1] as StartEndArgument).ConditionalType == StartEndArgumentType.Start && num < queryInfo.WhereSyntaxis.Count - 1 && queryInfo.WhereSyntaxis[num + 1] is OperatorArgument && ((queryInfo.WhereSyntaxis[num + 1] as OperatorArgument).OperatorType == OperatorType.And || ((OperatorArgument)queryInfo.WhereSyntaxis[num + 1]).OperatorType == OperatorType.Or))
					{
						queryInfo.WhereSyntaxis.RemoveAt(num);
						flag = true;
						break;
					}
				}
				if (num > 0 && queryInfo.WhereSyntaxis[num] is StartEndArgument && ((StartEndArgument)queryInfo.WhereSyntaxis[num]).ConditionalType == StartEndArgumentType.Start && num < queryInfo.WhereSyntaxis.Count && queryInfo.WhereSyntaxis[num + 1] is StartEndArgument && ((StartEndArgument)queryInfo.WhereSyntaxis[num + 1]).ConditionalType == StartEndArgumentType.End)
				{
					queryInfo.WhereSyntaxis.RemoveAt(num);
					queryInfo.WhereSyntaxis.RemoveAt(num);
					flag = true;
					break;
				}
			}
		}
		while (flag);
		for (int num2 = queryInfo.OrderBy.Count - 1; num2 >= 0; num2--)
		{
			OrderByItem orderByItem = queryInfo.OrderBy[num2];
			orderByItem.OrderIndex = num2;
			if (orderByItem.DataMember.IsMetaDataField)
			{
				queryInfo.OrderBy.RemoveAt(num2);
				queryInfo.CustomOrderBy.Add(orderByItem);
			}
		}
		return queryInfo;
	}

	private QueryInfo ConvertToExecutableQuery(Expression query)
	{
		if (!GetSourceTable(query, out var source))
		{
			throw new NotSupportedException("This query expression is not supported!");
		}
		QueryInfo queryInfo = new QueryInfo
		{
			Source = source,
			UseDefault = false
		};
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("SELECT ");
		if (takeTranslator != null && takeTranslator.Count.HasValue)
		{
			queryInfo.UseDefault = takeTranslator.UseDefault;
			stringBuilder.Append("TOP ");
			stringBuilder.Append(takeTranslator.Count);
			stringBuilder.Append(" ");
		}
		if (selectTranslator == null)
		{
			selectTranslator = new ProjectionTranslator();
		}
		selectTranslator.AddProjectionFields(source);
		if (!selectTranslator.DataMembers.Any())
		{
			throw new Exception(ExceptionResources.NoItemsForFieldsInQuery);
		}
		queryInfo.LambdaExpression = selectTranslator.ProjectionLambda;
		queryInfo.ProjectionFields.AddRange(selectTranslator.DataMembers);
		stringBuilder.Append(selectTranslator.ProjectionClause);
		stringBuilder.Append(" FROM ");
		stringBuilder.AppendLine(source.Name);
		if (whereTranslator != null)
		{
			string whereClause = whereTranslator.WhereClause;
			if (!string.IsNullOrEmpty(whereClause))
			{
				stringBuilder.Append(" WHERE ");
				stringBuilder.AppendLine(whereClause);
			}
			queryInfo.WhereFields.AddRange(whereTranslator.DataMembers);
			queryInfo.WhereSyntaxis.AddRange(whereTranslator.Syntaxis);
			queryInfo.WhereLambdaExpression = whereTranslator.Lambda;
		}
		if (orderByTranslator != null)
		{
			string orderByClause = orderByTranslator.OrderByClause;
			if (!string.IsNullOrEmpty(orderByClause))
			{
				stringBuilder.Append(" ORDER BY ");
				stringBuilder.AppendLine(orderByClause);
			}
			queryInfo.OrderBy.AddRange(orderByTranslator.DataMembers);
		}
		queryInfo.ResultShape = GetResultShape(query);
		queryInfo.QueryText = stringBuilder.ToString();
		return queryInfo;
	}

	private QueryResultType GetResultShape(Expression query)
	{
		if (query is LambdaExpression lambdaExpression)
		{
			query = lambdaExpression.Body;
		}
		if (query.Type == typeof(void))
		{
			return QueryResultType.None;
		}
		if (query.Type == typeof(IMultipleResults))
		{
			throw new NotSupportedException("Multiple result shape is not supported");
		}
		if (query is MethodCallExpression methodCallExpression && (methodCallExpression.Method.DeclaringType == typeof(Queryable) || methodCallExpression.Method.DeclaringType == typeof(Enumerable)))
		{
			switch (methodCallExpression.Method.Name)
			{
			case "First":
			case "FirstOrDefault":
			case "Single":
			case "SingleOrDefault":
				return QueryResultType.Singleton;
			}
		}
		return QueryResultType.Sequence;
	}

	private static bool GetSourceTable(Expression query, out Type source)
	{
		source = null;
		MethodCallExpression methodCallExpression = query as MethodCallExpression;
		ConstantExpression constantExpression;
		if (methodCallExpression == null)
		{
			constantExpression = query as ConstantExpression;
		}
		else
		{
			while (methodCallExpression.Arguments[0] is MethodCallExpression)
			{
				methodCallExpression = methodCallExpression.Arguments[0] as MethodCallExpression;
			}
			constantExpression = methodCallExpression.Arguments[0] as ConstantExpression;
		}
		if (constantExpression != null && constantExpression.Value is IClusterLinqList clusterLinqList)
		{
			source = clusterLinqList.ElementType;
			return true;
		}
		return false;
	}

	private void VisitWhere(LambdaExpression predicate)
	{
		if (whereTranslator != null)
		{
			throw new NotSupportedException("You cannot have more than one Where in the expression");
		}
		whereTranslator = new WhereTranslator();
		whereTranslator.Translate(predicate);
	}

	private void VisitSelect(LambdaExpression predicate)
	{
		if (selectTranslator != null)
		{
			throw new NotSupportedException("You cannot have more than 1 Select in the expression");
		}
		selectTranslator = new ProjectionTranslator();
		selectTranslator.Translate(predicate);
	}

	private void VisitOrderBy(LambdaExpression predicate, OrderDirection direction)
	{
		orderByTranslator.Visit(predicate, direction);
	}

	private void VisitTake(Expression takeValue)
	{
		if (takeTranslator != null)
		{
			throw new NotSupportedException("You cannot have more than 1 Take/First/FirstOrDefault in the expression");
		}
		takeTranslator = new TakeTranslator();
		takeTranslator.Translate(takeValue);
	}

	private void VisitFirst(bool useDefault)
	{
		if (takeTranslator != null)
		{
			throw new NotSupportedException("You cannot have more than 1 Take/First/FirstOrDefault in the expression");
		}
		takeTranslator = TakeTranslator.GetNewFirstTranslator(useDefault);
	}

	private LambdaExpression GetLambdaWithParamCheck(MethodCallExpression mc)
	{
		if (mc.Arguments.Count != 2 || !IsLambda(mc.Arguments[1]))
		{
			return null;
		}
		LambdaExpression lambda = GetLambda(mc.Arguments[1]);
		if (lambda.Parameters.Count == 1)
		{
			return lambda;
		}
		return null;
	}

	private bool IsLambda(Expression expression)
	{
		return RemoveQuotes(expression).NodeType == ExpressionType.Lambda;
	}

	private LambdaExpression GetLambda(Expression expression)
	{
		return RemoveQuotes(expression) as LambdaExpression;
	}

	private static Expression RemoveQuotes(Expression expression)
	{
		while (expression.NodeType == ExpressionType.Quote)
		{
			expression = ((UnaryExpression)expression).Operand;
		}
		return expression;
	}

	protected override Expression VisitMethodCall(MethodCallExpression mc)
	{
		if (mc.Method.DeclaringType != typeof(Queryable))
		{
			throw new NotSupportedException("Invalid Sequence Operator Call. The type for the operator is not Queryable!");
		}
		switch (mc.Method.Name)
		{
		case "Where":
		{
			LambdaExpression lambdaWithParamCheck3 = GetLambdaWithParamCheck(mc);
			if (lambdaWithParamCheck3 != null)
			{
				VisitWhere(lambdaWithParamCheck3);
			}
			break;
		}
		case "OrderBy":
		case "ThenBy":
		{
			LambdaExpression lambdaWithParamCheck = GetLambdaWithParamCheck(mc);
			if (lambdaWithParamCheck != null)
			{
				VisitOrderBy(lambdaWithParamCheck, OrderDirection.Ascending);
			}
			break;
		}
		case "OrderByDescending":
		case "ThenByDescending":
		{
			LambdaExpression lambdaWithParamCheck2 = GetLambdaWithParamCheck(mc);
			if (lambdaWithParamCheck2 != null)
			{
				VisitOrderBy(lambdaWithParamCheck2, OrderDirection.Descending);
			}
			break;
		}
		case "Select":
		{
			LambdaExpression lambdaWithParamCheck4 = GetLambdaWithParamCheck(mc);
			if (lambdaWithParamCheck4 != null)
			{
				VisitSelect(lambdaWithParamCheck4);
			}
			break;
		}
		case "Take":
			if (mc.Arguments.Count == 2)
			{
				VisitTake(mc.Arguments[1]);
			}
			break;
		case "First":
			if (mc.Arguments.Count == 1)
			{
				VisitFirst(useDefault: false);
			}
			break;
		case "FirstOrDefault":
			if (mc.Arguments.Count == 1)
			{
				VisitFirst(useDefault: true);
			}
			break;
		default:
			return base.VisitMethodCall(mc);
		}
		Visit(mc.Arguments[0]);
		return mc;
	}
}
