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
    public abstract class CodeStatement
    {

    }
    public class SelectStatement : CodeStatement
    {
        public List<FromExpression> fromExpressions = new List<FromExpression>();
        public List<SelectExpression> selectExpressions = new List<SelectExpression>();
        public List<WhereExpression> whereExpressions = new List<WhereExpression>();
        public int limit0 = -1;

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
            return MySqlStringMaker.BuildMySqlString((SelectStatement)codeStmt);
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