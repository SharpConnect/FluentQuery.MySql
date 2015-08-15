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
            var q = new QuerySegment<user_info>();
            q.Where(u => u.first_name == "a")
             .Select(u => new { u.uid, u.first_name });

            string sqlStr = MySqlStringMaker.BuildMySqlString(q);

            Report.WriteLine(sqlStr);
        }

        [Test]
        public static void T_Select_Limit()
        {
            var q = new QuerySegment<user_info>();
            q.Where(u => u.first_name == "a")
             .Select(u => new { u.uid, u.first_name })
             .Limit(10); 

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

}