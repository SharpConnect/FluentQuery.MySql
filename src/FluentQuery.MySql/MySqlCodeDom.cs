//MIT 2015, EngineKit

using System;
using System.Collections.Generic;
using System.Text;

//---------------------------------------
//Warning
//concept/test only 
//-------------------------------------

namespace SharpConnect.FluentQuery
{

    public abstract class CodeExpression
    {

    }
    public enum CodeStatementKind
    {
        Select,
        Insert,
        Update,
        Delete,
        CreateTable,
        DropTable,
    }
    public abstract class CodeStatement
    {
        public abstract CodeStatementKind StatementKind { get; }
    }
    public class SelectStatement : CodeStatement
    {
        public List<FromExpression> fromExpressions = new List<FromExpression>();
        public List<SelectExpression> selectExpressions = new List<SelectExpression>();
        public List<WhereExpression> whereExpressions = new List<WhereExpression>();
        public int limit0 = -1;
        public override CodeStatementKind StatementKind
        {
            get
            {
                return CodeStatementKind.Select;
            }
        }

    }
    public class InsertStatement : CodeStatement
    {
        public string targetTable;
        public override CodeStatementKind StatementKind
        {
            get
            {
                return CodeStatementKind.Insert;
            }
        }
    }

    public class WhereExpression : CodeExpression
    {
        public string whereClause;

    }

    public class FromExpression : CodeExpression
    {
        public string dataSource;

    }
    public class SelectExpression : CodeExpression
    {
        public string selectClause;
    }

    public static class MySqlStringMaker
    {
        public static string BuildMySqlString(QuerySegment q)
        {
            var codeStmt = q.MakeCodeStatement();
            switch (codeStmt.StatementKind)
            {
                case CodeStatementKind.Select:
                    return MySqlStringMaker.BuildMySqlString((SelectStatement)codeStmt);
                case CodeStatementKind.Insert:

                default:
                    throw new NotSupportedException();

            }

        }
        public static string BuildMySqlString(SelectStatement selectStmt)
        {
            StringBuilder stbuilder = new StringBuilder();

            stbuilder.Append("select ");
            int j = selectStmt.selectExpressions.Count;
            int limit0 = -1;
            if (j > 0)
            {
                stbuilder.Append(selectStmt.selectExpressions[0].selectClause);
                limit0 = selectStmt.limit0;
            }
            j = selectStmt.fromExpressions.Count;
            if (j > 0)
            {
                stbuilder.Append(" from ");
                stbuilder.Append(selectStmt.fromExpressions[0].dataSource);
            }
            j = selectStmt.whereExpressions.Count;
            if (j > 0)
            {
                stbuilder.Append(" where ");
                stbuilder.Append(selectStmt.whereExpressions[0].whereClause);
            }

            if (limit0 >= 0)
            {
                stbuilder.Append(" limit " + limit0);
            }
            return stbuilder.ToString();
        }
    }

}