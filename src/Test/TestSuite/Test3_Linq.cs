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

    public class TestSet3
    {

        [Test]
        public static void T_Select_Linq()
        {

            var q = from u in Q2.Table<user_info>()
                    where u.first_name == "ok"
                    select new { u.first_name, u.last_name };


            var mysqlString = q.MakeMySqlString();

        }



    }


}