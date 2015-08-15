//MIT 2015, EngineKit

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

//---------------------------------------
//Warning
//concept/test only 
//-------------------------------------
//TODO: use ExpressionTree Walker  ***
//-------------------------------------

namespace SharpConnect.FluentQuery
{

    enum CreationContext
    {
        WhereClause,
        Select,
        From,
        Insert,
    }

    class LinqExpressionTreeWalker : DynamicExpressionVisitor
    {

        StringBuilder stbuilder = new StringBuilder();

        public LinqExpressionTreeWalker()
        {

        }
        public CreationContext CreationContext { get; set; }

        public void Start(Expression node)
        {
            stbuilder.Length = 0;
            this.Visit(node);
        }
        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }
        public string GetWalkResult()
        {
            return stbuilder.ToString();
        }
        protected override Expression VisitBinary(BinaryExpression node)
        {

            switch (CreationContext)
            {
                case CreationContext.WhereClause:
                    {
                        //left and right
                        stbuilder.Append('(');
                        Visit(node.Left);
                        switch (node.NodeType)
                        {
                            case ExpressionType.Equal:
                                stbuilder.Append('=');
                                break;
                            case ExpressionType.NotEqual:
                                stbuilder.Append("!=");
                                break;
                            case ExpressionType.GreaterThan:
                                stbuilder.Append(">");
                                break;
                            case ExpressionType.GreaterThanOrEqual:
                                stbuilder.Append(">=");
                                break;
                            case ExpressionType.LessThanOrEqual:
                                stbuilder.Append("<=");
                                break;
                            case ExpressionType.LessThan:
                                stbuilder.Append("<");
                                break;
                            default:
                                throw new NotSupportedException();
                        }
                        Visit(node.Right);

                        stbuilder.Append(')');
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }

            return node;
        }
        protected override Expression VisitBlock(BlockExpression node)
        {
            return base.VisitBlock(node);
        }
        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            return base.VisitCatchBlock(node);
        }
        protected override Expression VisitConditional(ConditionalExpression node)
        {
            return base.VisitConditional(node);
        }
        protected override Expression VisitConstant(ConstantExpression node)
        {
            //constant value
            if (node.Type == typeof(char) ||
                node.Type == typeof(string))
            {
                stbuilder.Append(string.Concat("'", node.Value.ToString(), "'"));
            }
            else
            {
                //number ?
                stbuilder.Append(node.Value);
            }
            return node;
        }
        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            return base.VisitDebugInfo(node);
        }
        protected override Expression VisitDefault(DefaultExpression node)
        {
            return base.VisitDefault(node);
        }
        protected override Expression VisitDynamic(DynamicExpression node)
        {
            return base.VisitDynamic(node);
        }
        protected override ElementInit VisitElementInit(ElementInit node)
        {
            return base.VisitElementInit(node);
        }
        protected override Expression VisitExtension(Expression node)
        {
            return base.VisitExtension(node);
        }
        protected override Expression VisitGoto(GotoExpression node)
        {
            return base.VisitGoto(node);
        }
        protected override Expression VisitIndex(IndexExpression node)
        {
            return base.VisitIndex(node);
        }
        protected override Expression VisitInvocation(InvocationExpression node)
        {
            return base.VisitInvocation(node);
        }
        protected override Expression VisitLabel(LabelExpression node)
        {
            return base.VisitLabel(node);
        }
        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            return base.VisitLabelTarget(node);
        }
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return base.VisitLambda<T>(node);
        }
        protected override Expression VisitListInit(ListInitExpression node)
        {
            return base.VisitListInit(node);
        }
        protected override Expression VisitLoop(LoopExpression node)
        {
            return base.VisitLoop(node);
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            //u.firstname -> firstname etc

            stbuilder.Append(node.Member.Name);
            return node;
            //return base.VisitMember(node);
        }
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            return base.VisitMemberAssignment(node);
        }
        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            return base.VisitMemberBinding(node);
        }
        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            return base.VisitMemberInit(node);
        }
        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            return base.VisitMemberListBinding(node);
        }
        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            return base.VisitMemberMemberBinding(node);
        }
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return base.VisitMethodCall(node);
        }
        protected override Expression VisitNew(NewExpression node)
        {
            switch (CreationContext)
            {
                case CreationContext.Select:
                    {
                        int j = node.Arguments.Count;
                        int i = 0;
                        foreach (var arg in node.Arguments)
                        {


                            if (i > 0)
                            {
                                stbuilder.Append(',');
                            }

                            Visit(arg);

                            i++;
                        }
                        return node;
                    }
                default:
                    throw new NotSupportedException();

            }
            return base.VisitNew(node);
        }
        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            switch (CreationContext)
            {
                case CreationContext.Select:
                    {
                        int j = node.Expressions.Count;
                        for (int i = 0; i < j; ++i)
                        {

                            if (i > 0)
                            {
                                stbuilder.Append(',');
                            }
                            Visit(node.Expressions[i]);

                        }
                    }
                    return node;
                default:
                    throw new NotSupportedException();
            }


        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(node);
        }
        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            return base.VisitRuntimeVariables(node);
        }
        protected override Expression VisitSwitch(SwitchExpression node)
        {
            return base.VisitSwitch(node);
        }
        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            return base.VisitSwitchCase(node);
        }
        protected override Expression VisitTry(TryExpression node)
        {
            return base.VisitTry(node);
        }
        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            return base.VisitTypeBinary(node);
        }
        protected override Expression VisitUnary(UnaryExpression node)
        {

            switch (CreationContext)
            {
                case CreationContext.Select:
                    switch (node.NodeType)
                    {
                        case ExpressionType.Convert:

                            if (node.Operand.Type.IsPrimitive)
                            {
                                if (node.Operand.Type == typeof(string) ||
                                    node.Operand.Type == typeof(char))
                                {
                                }
                                else
                                {
                                    stbuilder.Append(node.Operand.ToString());

                                }
                            }
                            else
                            {

                            }
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    return node;
                    
                default:
                    throw new NotSupportedException();
            }


        }

    }

}