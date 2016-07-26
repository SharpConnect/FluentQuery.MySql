//MIT, 2015-2016, EngineKit and contributors 
using SharpConnect.FluentQuery;
using SharpConnect.FluentQuery.Experiment;
using static SharpConnect.FluentQuery.Experiment.Q4;

namespace MySqlTest.TestQueryBuilder
{

    public class TestSet5
    {
        [Test]
        public static void T_Select()
        {

            //create  custom record class
            //not use linq
            var record = Mapper.New((int col_id) => { });
            var q = From("test001")
                    .Select(record);

            string toSqlString = q.ToSqlString();

        }
        [Test]
        public static void T_Select2()
        {
            //not use linq           
            //create custom record class            
            var q = From("test001")
                    .Select((int col_id) => { });

            string toSqlString = q.ToSqlString();
            //after exec we can manual map data to target 
        }

        [Test]
        public static void T_Select3()
        {
            //create custom record class
            //not use linq            
            var q = From("test001")
                   .SelectAndMapTo((UserInfo3 u, int col_id) =>
                   {
                       //we bunddle mapper here
                       //so we can access both src field record name 
                       //and target field 
                       //and we can create custom map logic here
                       u.userId = col_id;
                   });

            //create query string 
            string toSqlString = q.ToSqlString();
            //we can map data from source to the target with map func



        }

        class UserInfo3
        {
            public int userId;
            public string FirstName;
            public string LastName;
        }
    }
}