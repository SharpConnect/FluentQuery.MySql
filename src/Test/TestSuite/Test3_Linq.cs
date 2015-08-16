//MIT 2015,  EngineKit and contributors

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using SharpConnect.FluentQuery;


//**************************
//test concept only
//**************************

namespace MySqlTest.TestQueryBuilder
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
                    //select
                    //arg of select
                    foreach (var arg in callExpr.Arguments)
                    {
                        if (arg.NodeType == ExpressionType.Call)
                        {
                            //go deeper
                            MethodCallExpression argCall = (MethodCallExpression)arg;
                            switch (argCall.Method.Name)
                            {
                                case "Where":
                                    {

                                    }
                                    break;
                                default:
                                    {

                                    }
                                    break;
                            }
                        }
                        else
                        {

                        }
                    }

                }


            }

            return "";
        }
    }





    public class TestSet3
    {

        [Test]
        public static void T_Select_Linq()
        {
            var userList = new MyQueryContext<user_info>();
            var q = from u in userList
                    where u.first_name == "ok"
                    select u.first_name;

            var mysqlString = q.MakeMySqlString();

        }

        class MyQueryContext<T> : IQueryable<T>
        {
            MyQProvider provider;
            Expression expr;
            public MyQueryContext()
            {
                this.provider = new MyQProvider();
                this.expr = Expression.Constant(this);
            }

            public MyQueryContext(MyQProvider provider, Expression expr)
            {
                this.provider = provider;
                this.expr = expr;
            }
            public Expression Expression
            {
                get
                {
                    return this.expr;
                }
            }

            public Type ElementType
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IQueryProvider Provider
            {
                get
                {
                    return this.provider;
                }
            }


            public IEnumerator<T> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }


        class MyQProvider : IQueryProvider
        {

            public IQueryable CreateQuery(Expression expression)
            {
                throw new NotImplementedException();
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return (IQueryable<TElement>)new MyQueryContext<TElement>(this, expression);
            }

            public object Execute(Expression expression)
            {
                throw new NotImplementedException();
            }

            public TResult Execute<TResult>(Expression expression)
            {
                throw new NotImplementedException();
            }
        }

    }


}