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
    public delegate bool QueryPredicate<T>(T t);
    public delegate R QueryProduct<T, R>(T t);

    public abstract class QuerySegment
    {
        public QuerySegment PrevSegment { get; internal set; }
        public QuerySegment NextSegment { get; internal set; }
        public abstract QuerySegmentKind SegmentKind { get; }


        internal virtual void WriteToSelectStmt(SelectStatement selectStmt)
        {

        }

        public CodeStatement MakeCodeStatement()
        {
            QuerySegmentParser parser = new QuerySegmentParser();
            return parser.Parse(this);
        }

    }

    public enum QuerySegmentKind
    {
        DataSource,
        Where,
        Select
    }

    abstract class ExpressionHolder
    {

        public abstract void WriteToSelectStatement(SelectStatement selectStmt);
    }

    class SelectProductHolder<S, R> : ExpressionHolder
    {
        Expression<QueryProduct<S, R>> product;
        public SelectProductHolder(Expression<QueryProduct<S, R>> product)
        {
            this.product = product;
        }
        public override void WriteToSelectStatement(SelectStatement selectStmt)
        {
            LinqExpressionTreeWalker exprWalker = new LinqExpressionTreeWalker();
            exprWalker.CreationContext = CreationContext.Select;
            exprWalker.Start(product.Body);

            SelectExpression selectExpr = new SelectExpression();
            selectExpr.selectClause = exprWalker.GetWalkResult();
            selectStmt.selectExpressions.Add(selectExpr);

        }
        //public override void BuildSql(StringBuilder stbuilder)
        //{
        //    var body = product.Body;
        //    switch (body.NodeType)
        //    {
        //        case ExpressionType.New:
        //            //select to new type

        //            stbuilder.Append("select ");
        //            int i = 0;
        //            NewExpression newExpr = (NewExpression)body;

        //            foreach (var arg in newExpr.Arguments)
        //            {
        //                //expr
        //                //and member
        //                if (i > 0)
        //                {
        //                    stbuilder.Append(',');
        //                }
        //                switch (arg.NodeType)
        //                {
        //                    case ExpressionType.MemberAccess:
        //                        MemberExpression mbExpr = (MemberExpression)arg;
        //                        stbuilder.Append(mbExpr.Member.Name);

        //                        break;
        //                    default:
        //                        throw new NotSupportedException();
        //                }
        //                i++;
        //            }
        //            break;
        //        default:
        //            throw new NotSupportedException();
        //    }
        //}

    }


    class QuerySegmentParser
    {
        public QuerySegmentParser()
        {

        }
        public CodeStatement Parse(QuerySegment segment)
        {
            //goto first segment 
            QuerySegment cNode = segment;
            while (cNode.PrevSegment != null)
            {
                cNode = cNode.PrevSegment;
            }
            //forward collect ..
            List<QuerySegment> nodeList = new List<QuerySegment>();
            while (cNode != null)
            {
                nodeList.Add(cNode);
                cNode = cNode.NextSegment;
            }

            //------------------------------------------------
            //select ...
            //from ...
            int j = nodeList.Count;
            QuerySegment fromSource = null;
            QuerySegment selectClause = null;

            int state = 0;
            for (int i = 0; i < j; ++i)
            {
                var node = nodeList[i];
                switch (node.SegmentKind)
                {
                    case QuerySegmentKind.Select:
                        {

                            selectClause = node;
                        }
                        break;
                    case QuerySegmentKind.Where:
                    case QuerySegmentKind.DataSource:
                        {
                            fromSource = node;
                        }
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            //------------------------------------------------
            if (selectClause != null)
            {
                //create sql select
                SelectStatement selectStmt = new SelectStatement();
                //check data source
                if (fromSource != null)
                {
                    fromSource.WriteToSelectStmt(selectStmt);
                }
                selectClause.WriteToSelectStmt(selectStmt);
                return selectStmt;

            }
            else
            {
                return null;
            }

        }
    }


    public class QuerySegment<S> : QuerySegment
    {


        QuerySegmentKind segmentKind;
        List<Expression<QueryPredicate<S>>> whereClauses = new List<Expression<QueryPredicate<S>>>();
        public QuerySegment()
        {
            this.segmentKind = QuerySegmentKind.DataSource;
        }


        public override QuerySegmentKind SegmentKind
        {
            get
            {
                return this.segmentKind;
            }
        }
        public QuerySegment<S> Where(Expression<QueryPredicate<S>> wherePred)
        {
            whereClauses.Add(wherePred);
            return this;
        }
        public QuerySelectSegment<R> Select<R>(Expression<QueryProduct<S, R>> product)
        {
            var q = new QuerySelectSegment<R>(this);
            q.exprHolder = new SelectProductHolder<S, R>(product);
            return q;
        }
        public QuerySelectSegment<R> SelectInto<R>()
        {
            return new QuerySelectSegment<R>(this);
        }


        internal override void WriteToSelectStmt(SelectStatement selectStmt)
        {
            FromExpression fromExpr = new FromExpression();
            fromExpr.dataSource = typeof(S).Name;
            selectStmt.fromExpressions.Add(fromExpr);
            //-------------------------------------------------
            int j = whereClauses.Count;
            if (j > 0)
            {
                //create where clause
                LinqExpressionTreeWalker walker = new LinqExpressionTreeWalker();
                walker.CreationContext = CreationContext.WhereClause;


                for (int i = 0; i < j; ++i)
                {
                    WhereExpression whereExpr = new WhereExpression();

                    var whereClause = whereClauses[i];
                    walker.Start(whereClause.Body);

                    whereExpr.whereClause = walker.GetWalkResult();
                    selectStmt.whereExpressions.Add(whereExpr);
                }
            }

        }


    }

    public class QuerySelectSegment<S> : QuerySegment<S>
    {
        internal ExpressionHolder exprHolder;
        public QuerySelectSegment(QuerySegment prev)
        {

            this.PrevSegment = prev;
            prev.NextSegment = this;
        }
        public override QuerySegmentKind SegmentKind
        {
            get
            {
                return QuerySegmentKind.Select;
            }
        }

        internal override void WriteToSelectStmt(SelectStatement selectStmt)
        {
            if (exprHolder != null)
            {
                exprHolder.WriteToSelectStatement(selectStmt);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }

}
