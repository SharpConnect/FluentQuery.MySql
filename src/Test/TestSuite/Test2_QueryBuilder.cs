//MIT 2015,  EngineKit and contributors

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

using SharpConnect.FluentQuery;
namespace MySqlTest.TestQueryBuilder
{

    public class TestSet2
    {



        [Test]
        public static void T_Select()
        {
            var q = Q.From<user_info>()
                     .Where(u => u.first_name == "a")
                     .Select(u => new { u.uid, u.first_name });

            string sqlStr = MySqlStringMaker.BuildMySqlString(q);

            Report.WriteLine(sqlStr);
        }
        [Test]
        public static void T_Select1()
        {

            var q = From<user_info>
                     .Where(u => u.first_name == "a")
                     .Select(); //TODO: implement simple select

            string sqlStr = MySqlStringMaker.BuildMySqlString(q);

            Report.WriteLine(sqlStr);
        }

        [Test]
        public static void T_Select2()
        {

            var q = From<user_info>
                     .Where(u => u.first_name == "a")
                     .Select(u => new { u.uid, u.first_name });

            string sqlStr = MySqlStringMaker.BuildMySqlString(q);

            Report.WriteLine(sqlStr);
        }


        [Test]
        public static void T_Select_Limit()
        {
            var q = From<user_info>
                     .Where(u => u.first_name == "a")
                     .Select(u => new { u.uid, u.first_name })
                     .Limit(10);

            string sqlStr = MySqlStringMaker.BuildMySqlString(q);

            Report.WriteLine(sqlStr);
        }



        [Test]
        public static void T_Select_With_Const()
        {

            var q = From<user_info>
                     .Where(u => u.first_name == "a")
                     .Select(u => new R(u.first_name, u.last_name, 20 + 5));


            string sqlStr = MySqlStringMaker.BuildMySqlString(q);

            Report.WriteLine(sqlStr);
        }

        [Test]
        public static void T_Select_OrderBy()
        {

            var q = From<user_info>
                     .Where(u => u.first_name == "a")
                     .OrderBy(u => u.first_name)
                     .Select(u => new R(u.first_name, u.last_name, 20 + 5));



            string sqlStr = MySqlStringMaker.BuildMySqlString(q);

            Report.WriteLine(sqlStr);
        }


        [Test]
        public static void T_Join()
        {
            //?
            var q = From<user_info>
                     .Where(u => u.first_name == "a002")
                     .Join(
                            From<user_address>
                                .Where(a => a.areadCode == "101"));


        }
        [Test]
        public static void T_Join2()
        {
            //?
            var q = Join<user_info, user_address>
                      .On((u, a) => u.first_name == a.areadCode)
                      .Where((u, a) => u.first_name == "m")
                      .Select((u, a) => new { u.last_name, a.streetNo });



        }
        //------------------------------------------------------------------------
        [Test]
        public static void T_Insert()
        {
            var q = Q.InsertInto<user_info>()
                    .Values(i => new user_info { first_name = "ok", last_name = "001" });

            string sqlStr = MySqlStringMaker.BuildMySqlString(q);

            Report.WriteLine(sqlStr);
        }



        [Test]
        public static void T_Insert2()
        {
            var q = Q.InsertInto<user_info>()
                     .Values(i => new { first_name = "ok", last_name = "001" });

            string sqlStr = MySqlStringMaker.BuildMySqlString(q);

            Report.WriteLine(sqlStr);
        }
        [Test]
        public static void T_Insert3()
        {
            var q = InsertInto<user_info>
                    .Values(i => new user_info { first_name = "ok", last_name = "001" });

            string sqlStr = MySqlStringMaker.BuildMySqlString(q);

            Report.WriteLine(sqlStr);
        }



        [Test]
        public static void T_Insert4()
        {
            var q = InsertInto<user_info>
                     .Values(i => new { first_name = "ok", last_name = "001" });

            string sqlStr = MySqlStringMaker.BuildMySqlString(q);

            Report.WriteLine(sqlStr);
        }
        //------------------------------------------------------------------------
        [Test]
        public static void T_Update()
        {
            var q = Q.Update<user_info>()
                     .Set(u => new user_info { first_name = "ok" });

            string sqlStr = MySqlStringMaker.BuildMySqlString(q);

            Report.WriteLine(sqlStr);
        }
        [Test]
        public static void T_Update2()
        {
            var q = Q.Update<user_info>()
                     .Where(u => u.first_name == "mmm")
                     .Set(u => new R(u.first_name, "ok",
                                     u.last_name, "12345"));

            string sqlStr = MySqlStringMaker.BuildMySqlString(q);

            Report.WriteLine(sqlStr);
        }
        [Test]
        public static void T_Update3()
        {
            var q = Update<user_info>
                     .Set(u => new user_info { first_name = "ok" });

            string sqlStr = MySqlStringMaker.BuildMySqlString(q);

            Report.WriteLine(sqlStr);
        }
        [Test]
        public static void T_Update24()
        {
            var q = Update<user_info>
                     .Where(u => u.first_name == "mmm")
                     .Set(u => new R(u.first_name, "ok",
                                     u.last_name, "12345"));

            string sqlStr = MySqlStringMaker.BuildMySqlString(q);

            Report.WriteLine(sqlStr);
        }

    }


    class user_info
    {
        public int uid;
        public string first_name;
        public string last_name;
    }
    class user_address
    {
        public string no;
        public string streetNo;
        public string areadCode;
    }

}