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
    public delegate bool QueryPredicate<T1, T2>(T1 t1, T2 t2);

    public delegate TResult QueryProduct<T, TResult>(T t);
    public delegate TResult QueryProduct<T1, T2, TResult>(T1 t1, T2 t2);

    public abstract class QuerySegment
    {
        public QuerySegment PrevSegment { get; internal set; }
        public QuerySegment NextSegment { get; internal set; }
        public abstract QuerySegmentKind SegmentKind { get; }


        internal abstract void WriteToSelectStmt(SelectStatement selectStmt);
        internal abstract void WriteToInsertStmt(InsertStatement insertStmt);
        internal abstract void WriteToUpdateStmt(UpdateStatement updateStmt);

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
        Select,
        Insert,
        Delete,
        Update,
    }

    abstract class ExpressionHolder
    {

        public abstract void WriteToSelectStatement(SelectStatement selectStmt);
        public abstract void WriteToInsertStatement(InsertStatement insertStmt);
    }

    class SelectProductHolder<T, TResult> : ExpressionHolder
    {
        Expression<QueryProduct<T, TResult>> product;
        public SelectProductHolder(Expression<QueryProduct<T, TResult>> product)
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
        public override void WriteToInsertStatement(InsertStatement insertStmt)
        {
            LinqExpressionTreeWalker exprWalker = new LinqExpressionTreeWalker();
            exprWalker.CreationContext = CreationContext.Insert;
            exprWalker.Start(product.Body);

        }
    }
    class SelectProductHolder<T1, T2, TResult> : ExpressionHolder
    {
        Expression<QueryProduct<T1, T2, TResult>> product;
        public SelectProductHolder(Expression<QueryProduct<T1, T2, TResult>> product)
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
        public override void WriteToInsertStatement(InsertStatement insertStmt)
        {
            LinqExpressionTreeWalker exprWalker = new LinqExpressionTreeWalker();
            exprWalker.CreationContext = CreationContext.Insert;
            exprWalker.Start(product.Body);

        }
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
            QuerySegment insertClause = null;

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
                    case QuerySegmentKind.Insert:
                        insertClause = node;
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
            else if (insertClause != null)
            {
                InsertStatement insertStatement = new InsertStatement();
                if (fromSource != null)
                {
                    fromSource.WriteToInsertStmt(insertStatement);
                }

                insertClause.WriteToInsertStmt(insertStatement);
                return insertStatement;

            }
            else
            {
                return null;
            }

        }
    }

    /// <summary>
    /// simple select result
    /// </summary>
    public class R
    {
        public R(params object[] args) { }

    }

    abstract class QuerySegWhereClause
    {
        public abstract void WriteToWhereExpression(WhereExpression whereExpr);

    }
    class QuerySegRawWhereClause : QuerySegWhereClause
    {
        string simpleWhere;
        public QuerySegRawWhereClause(string simpleWhere)
        {
            this.simpleWhere = simpleWhere;
        }
        public override void WriteToWhereExpression(WhereExpression whereExpr)
        {
            whereExpr.whereClause = this.simpleWhere;
        }
    }
    class QuerySegExpressionWhereClause<T> : QuerySegWhereClause
    {
        Expression<QueryPredicate<T>> whereExpression;
        public QuerySegExpressionWhereClause(Expression<QueryPredicate<T>> pred)
        {
            this.whereExpression = pred;
        }
        public override void WriteToWhereExpression(WhereExpression whereExpr)
        {
            var walker = new LinqExpressionTreeWalker();
            walker.CreationContext = CreationContext.WhereClause;
            walker.Start(whereExpression.Body);
            whereExpr.whereClause = walker.GetWalkResult();
        }
    }

    public class FromQry<T> : QuerySegment
    {

        List<QuerySegWhereClause> whereClauses = new List<QuerySegWhereClause>();
        List<LambdaExpression> orderByClauses = new List<LambdaExpression>();

        public FromQry()
        {

        }

        public override QuerySegmentKind SegmentKind
        {
            get
            {
                return QuerySegmentKind.DataSource;
            }
        }
        public FromQry<T> Where(Expression<QueryPredicate<T>> wherePred)
        {
            whereClauses.Add(new QuerySegExpressionWhereClause<T>(wherePred));
            return this;
        }
        public FromQry<T> Where(string rawWhere)
        {
            whereClauses.Add(new QuerySegRawWhereClause(rawWhere));
            return this;
        }
        public FromQry<T> OrderBy<TResult>(Expression<QueryProduct<T, TResult>> orderBy)
        {
            orderByClauses.Add(orderBy);
            return this;
        }

        public SelectQry<T> Select()
        {
            var q = new SelectQry<T>(this);
            return q;
        }
        public SelectQry<T> SelectStar()
        {
            var q = new SelectQry<T>(this);
            q.SelectStar = true;
            return q;
        }
        public SelectQry<TRsult> Select<TRsult>(Expression<QueryProduct<T, TRsult>> product)
        {
            var q = new SelectQry<TRsult>(this);
            q.exprHolder = new SelectProductHolder<T, TRsult>(product);
            return q;
        }
        public SelectQry<T> Select(string rawSelect)
        {
            var q = new SelectQry<T>(this);
            q.rawSelect = rawSelect;
            return q;
        }
        public SelectQry<U> SelectInto<U>()
        {
            //select into another type
            return new SelectQry<U>(this);
        }

        internal override void WriteToSelectStmt(SelectStatement selectStmt)
        {
            FromExpression fromExpr = new FromExpression();
            fromExpr.dataSource = typeof(T).Name;
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
                    var whereExpr = new WhereExpression();
                    whereClauses[i].WriteToWhereExpression(whereExpr);
                    selectStmt.whereExpressions.Add(whereExpr);
                }
            }
            j = orderByClauses.Count;
            if (j > 0)
            {
                LinqExpressionTreeWalker walker = new LinqExpressionTreeWalker();
                walker.CreationContext = CreationContext.OrderBy;

                for (int i = 0; i < j; ++i)
                {
                    var orderByExpress = new OrderByExpression();
                    walker.Start(orderByClauses[i].Body);
                    orderByExpress.orderByClause = walker.GetWalkResult();
                    selectStmt.orderByExpressions.Add(orderByExpress);

                }
            }
        }
        internal override void WriteToInsertStmt(InsertStatement insertStmt)
        {
            insertStmt.targetTable = typeof(T).Name;
        }
        internal override void WriteToUpdateStmt(UpdateStatement updateStmt)
        {
            throw new NotImplementedException();
        }

        public FromQry<T, U> Join<U>(FromQry<U> u)
        {
            return new FromQry<T, U>();
        }
    }

    public class FromQry<T1, T2> : QuerySegment
    {

        List<Expression<QueryPredicate<T1, T2>>> whereClauses = new List<Expression<QueryPredicate<T1, T2>>>();
        public FromQry()
        {

        }

        public override QuerySegmentKind SegmentKind
        {
            get
            {
                return QuerySegmentKind.DataSource;
            }
        }
        public FromQry<T1, T2> Where(Expression<QueryPredicate<T1, T2>> wherePred)
        {
            whereClauses.Add(wherePred);
            return this;
        }
        public FromQry<T1, T2> OrderBy<TResult>(Expression<QueryProduct<T1, T2, TResult>> orderBy)
        {
            //TODO: implement order by
            return this;
        }
        public SelectQry<TRsult> Select<TRsult>(Expression<QueryProduct<T1, T2, TRsult>> product)
        {
            var q = new SelectQry<TRsult>(this);
            q.exprHolder = new SelectProductHolder<T1, T2, TRsult>(product);
            return q;
        }
        public SelectQry<U> SelectInto<U>()
        {
            //select into another type
            return new SelectQry<U>(this);
        }


        internal override void WriteToSelectStmt(SelectStatement selectStmt)
        {
            throw new NotSupportedException();

            //FromExpression fromExpr = new FromExpression();
            //fromExpr.dataSource = typeof(T).Name;
            //selectStmt.fromExpressions.Add(fromExpr);
            ////-------------------------------------------------
            //int j = whereClauses.Count;
            //if (j > 0)
            //{
            //    //create where clause
            //    LinqExpressionTreeWalker walker = new LinqExpressionTreeWalker();
            //    walker.CreationContext = CreationContext.WhereClause;


            //    for (int i = 0; i < j; ++i)
            //    {
            //        WhereExpression whereExpr = new WhereExpression();

            //        var whereClause = whereClauses[i];
            //        walker.Start(whereClause.Body);

            //        whereExpr.whereClause = walker.GetWalkResult();
            //        selectStmt.whereExpressions.Add(whereExpr);
            //    }
            //}

        }
        internal override void WriteToInsertStmt(InsertStatement insertStmt)
        {
            throw new NotImplementedException();
        }
        internal override void WriteToUpdateStmt(UpdateStatement updateStmt)
        {
            throw new NotImplementedException();
        }
    }


    public class InsertQry<T> : QuerySegment
    {
        internal ExpressionHolder exprHolder;
        public InsertQry()
        {
        }
        public InsertQry<T> Values(Expression<QueryProduct<T, T>> product)
        {
            exprHolder = new SelectProductHolder<T, T>(product);
            return this;
        }
        public InsertQry<T> Values<TRsult>(Expression<QueryProduct<T, TRsult>> product)
        {
            exprHolder = new SelectProductHolder<T, TRsult>(product);
            return this;

        }
        public override QuerySegmentKind SegmentKind
        {
            get
            {
                return QuerySegmentKind.Insert;
            }
        }
        internal override void WriteToInsertStmt(InsertStatement insertStmt)
        {
            if (exprHolder != null)
            {
                exprHolder.WriteToInsertStatement(insertStmt);

            }
            else
            {
                throw new NotSupportedException();
            }
        }
        internal override void WriteToSelectStmt(SelectStatement selectStmt)
        {
            throw new NotImplementedException();
        }
        internal override void WriteToUpdateStmt(UpdateStatement updateStmt)
        {
            throw new NotImplementedException();
        }
    }

    public class UpdateQry<T> : QuerySegment
    {
        int limit0 = -1;//default
        internal ExpressionHolder exprHolder;
        List<Expression<QueryPredicate<T>>> whereClauses = new List<Expression<QueryPredicate<T>>>();
        /// <summary>
        /// mysql selection limit
        /// </summary>
        /// <param name="number"></param>
        public UpdateQry<T> Limit(int number)
        {
            limit0 = number;
            return this;
        }
        public override QuerySegmentKind SegmentKind
        {
            get
            {
                return QuerySegmentKind.Update;
            }
        }

        public UpdateQry<T> Where(Expression<QueryPredicate<T>> wherePred)
        {
            whereClauses.Add(wherePred);
            return this;
        }
        public UpdateQry<T> Set(Expression<QueryProduct<T, T>> product)
        {
            this.exprHolder = new SelectProductHolder<T, T>(product);
            return this;
        }
        public UpdateQry<T> Set<TResult>(Expression<QueryProduct<T, TResult>> product)
        {
            this.exprHolder = new SelectProductHolder<T, TResult>(product);
            return this;
        }
        internal override void WriteToInsertStmt(InsertStatement insertStmt)
        {
            throw new NotImplementedException();
        }
        internal override void WriteToSelectStmt(SelectStatement selectStmt)
        {
            throw new NotImplementedException();
        }
        internal override void WriteToUpdateStmt(UpdateStatement updateStmt)
        {
            throw new NotImplementedException();
        }

    }


    public class SelectQry<T> : QuerySegment
    {
        int limit0 = -1;//default
        internal ExpressionHolder exprHolder;
        internal bool SelectStar;
        internal string rawSelect;
        public SelectQry(QuerySegment prev)
        {

            this.PrevSegment = prev;
            prev.NextSegment = this;
        }

        /// <summary>
        /// mysql selection limit
        /// </summary>
        /// <param name="number"></param>
        public SelectQry<T> Limit(int number)
        {
            limit0 = number;
            return this;
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
                selectStmt.limit0 = limit0;
            }
            else if (this.SelectStar)
            {   //then select all public fields or member
                var selExpr = new SelectExpression();
                selExpr.selectClause = "*";
                selectStmt.selectExpressions.Add(selExpr);
            }
            else if (rawSelect != null)
            {
                var selExpr = new SelectExpression();
                selExpr.selectClause = rawSelect;
                selectStmt.selectExpressions.Add(selExpr);
            }
            else
            {
                Type t = typeof(T);
                var fields = t.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                var props = t.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.SetProperty);

                int total = fields.Length + props.Length;
                var stBuilder = new StringBuilder();

                int n = 0;
                int j = fields.Length;
                for (int i = 0; i < j; ++i, ++n)
                {
                    if (n > 0)
                    {
                        stBuilder.Append(',');
                    }
                    stBuilder.Append(fields[i].Name);

                }
                j = props.Length;
                for (int i = 0; i < j; ++i, ++n)
                {
                    if (n > 0)
                    {
                        stBuilder.Append(',');
                    }
                    stBuilder.Append(fields[i].Name);
                }

                var selExpr = new SelectExpression();
                selExpr.selectClause = stBuilder.ToString();
                selectStmt.selectExpressions.Add(selExpr);
            }

        }
        internal override void WriteToInsertStmt(InsertStatement insertStmt)
        {
            throw new NotSupportedException();
        }
        internal override void WriteToUpdateStmt(UpdateStatement updateStmt)
        {
            throw new NotImplementedException();
        }
    }

}
