//MIT, 2015-2016, EngineKit and contributors


using System.Linq;
using SharpConnect.FluentQuery;


//**************************
//test concept only
//**************************
using static SharpConnect.FluentQuery.Q3;

namespace MySqlTest.TestQueryBuilder
{

    public class TestSet3
    {

        [Test]
        public static void T_Select_Linq()
        {

            var q = from u in Q2.Provide<user_info>()
                    where u.first_name == "ok" && u.last_name == "mm"
                    select new { u.first_name, u.last_name };
            var mysqlString = q.MakeMySqlString();
        }
        [Test]
        public static void T_Select_Linq_1()
        {

            var q = from u in _db<user_info>()
                    where u.first_name == "ok" && u.last_name == "mm"
                    select new { u.first_name, u.last_name };
            var mysqlString = q.MakeMySqlString();
        }

        [Test]
        public static void T_Select_Linq2()
        {

            var q = from u in Q2.Provide<user_info>()
                    where u.first_name == "ok" || u.last_name == "mm"
                    select new { u.first_name, u.last_name };
            var mysqlString = q.MakeMySqlString();

        }
        [Test]
        public static void T_Select_Linq2_1()
        {

            var q = from u in _db<user_info>()
                    where u.first_name == "ok" || u.last_name == "mm"
                    select new { u.first_name, u.last_name };
            var mysqlString = q.MakeMySqlString();

        }

        [Test]
        public static void T_Select_Linq2_limit()
        {

            var q = from u in Q2.Provide<user_info>()
                    where u.first_name == "ok" || u.last_name == "mm"
                    select new { u.first_name, u.last_name }; 
            q.Limit(10);  
            var mysqlString = q.MakeMySqlString(); 
        }
        [Test]
        public static void T_Select_Linq2_1_limit()
        {

            var q = from u in _db<user_info>()
                    where u.first_name == "ok" || u.last_name == "mm"
                    select new { u.first_name, u.last_name };
            q.Limit(10);
            var mysqlString = q.MakeMySqlString();
        }
        [Test]
        public static void T_Select_Linq_GroupBy1()
        {

            var q = from u in Q2.Provide<user_info>()
                    group u by u.first_name into g
                    select g.Count(); 

            var mysqlString = q.MakeMySqlString();

        }
        [Test]
        public static void T_Select_Linq_GroupBy2()
        {
            var q = from u in Q2.Provide<user_info>()
                    where u.first_name != ""
                    group u by u.first_name into g
                    where g.Count() > 0
                    select g.Count();

            //select count(*) from user_info
            //where first_name !='' 
            //group by first_name;
            //having count(*) > 0


            var mysqlString = q.MakeMySqlString();

        }
    }


}