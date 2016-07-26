# FluentQuery.MySql
This is a simple MySql Query String Builder.

license: MIT

(( under construction ... ))
---


You can build query string with  'using static'

 

 1) select (using static)
 
          using SharpConnect.FluentQuery;
          using static SharpConnect.FluentQuery.Q; //using static method from Q
      
          namespace MySqlTest.TestQueryBuilder
          {
      
            public class TestSet4
            {
        
                [Test]
                public static void T_Select()
                {
        
                    var q = From<user_info>()
                            .Where(u => u.first_name == "a")
                            .Select(u => new { u.uid, u.first_name });
        
                    string sqlStr = q.BuildMySqlString();
                    Report.WriteLine(sqlStr);
                    //output: select uid,first_name from user_info where (first_name='a')
                } 

   
  2) select 2
        
        [Test]
        public static void T_Select_Star()
        {
            var q = From<user_info>()
                   .Where(u => u.first_name == "a")
                   .SelectStar();
      
            string sqlStr = q.BuildMySqlString();
            Report.WriteLine(sqlStr);
            //output: select * from user_info where (first_name='a')
        }

---
(( under construction ... ))

we are developing Linq version too :)
  
  
   


