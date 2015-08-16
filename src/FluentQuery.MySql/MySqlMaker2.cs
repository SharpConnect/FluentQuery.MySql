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

    public static class MySqlStringMake2
    {
        public static string MakeMySqlString<T>(this IQueryable<T> q)
        {
            var expr = q.Expression;
            if (expr.NodeType == ExpressionType.Call)
            {
                MethodCallExpression callExpr = (MethodCallExpression)expr;
                if (callExpr.Method.Name == "Select")
                {
                    SelectStatement selectStatement = new SelectStatement();
                    WriteSelectStatement(selectStatement, callExpr);
                    return MySqlStringMaker.BuildMySqlString(selectStatement);
                }
            }
            return "";
        }

        static void WriteSelectStatement(SelectStatement selectStmt, MethodCallExpression callExpr)
        {
            //build select statement 
            //select
            //arg of select
            LinqExpressionTreeWalker2 linqWalker2 = new LinqExpressionTreeWalker2();
            linqWalker2.CreationContext = CreationContext.Select;
            //select 
            WhereExpression whereExpr = null;
            SelectExpression selectExpr = null;



            foreach (var arg in callExpr.Arguments)
            {


                switch (arg.NodeType)
                {
                    case ExpressionType.Call:
                        //go deeper
                        MethodCallExpression argCall = (MethodCallExpression)arg;
                        switch (argCall.Method.Name)
                        {
                            case "Where":
                                {
                                    //where
                                    linqWalker2.CreationContext = CreationContext.WhereClause;
                                    linqWalker2.Start(arg);

                                    //find source of data
                                    var retType = argCall.Method.ReturnType;
                                    if (retType.IsGenericType)
                                    {
                                        var genArgs = retType.GetGenericArguments();
                                        switch (genArgs.Length)
                                        {
                                            case 1:
                                                {
                                                    //source

                                                    FromExpression fromExpr = new FromExpression();
                                                    fromExpr.dataSource = genArgs[0].Name;
                                                    selectStmt.fromExpressions.Add(fromExpr);

                                                }
                                                break;
                                            case 0:
                                            default:
                                                throw new NotSupportedException();
                                        }

                                    }
                                    else
                                    {
                                        throw new NotSupportedException();
                                    }



                                    whereExpr = new WhereExpression();
                                    whereExpr.whereClause = linqWalker2.GetWalkResult();
                                    selectStmt.whereExpressions.Add(whereExpr);
                                }
                                break;
                            default:
                                {

                                }
                                break;
                        }
                        break;
                    case ExpressionType.Quote:
                        {
                            linqWalker2.CreationContext = CreationContext.Select;
                            linqWalker2.Start(arg);
                            //TODO: review here
                            selectExpr = new SelectExpression();
                            selectExpr.selectClause = linqWalker2.GetWalkResult();
                            selectStmt.selectExpressions.Add(selectExpr);

                        }
                        break;
                    default:
                        throw new NotSupportedException();
                }

            }

        }
    }


}
