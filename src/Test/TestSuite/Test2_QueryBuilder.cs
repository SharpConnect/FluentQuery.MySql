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
        public static void T_QueryBuilder()
        {
            var q = new QuerySegment<user_info>();
            q.Where(u => u.first_name == "a")
             .Select(u => new { u.uid, u.first_name });

            var codeStmt = q.MakeCodeStatement();
            string sqlStr = MySqlStringMaker.BuildMySqlString((SelectStatement)codeStmt);

        }
    }


    class user_info
    {   
        public int uid;
        public string first_name;
        public string last_name;
    }

}