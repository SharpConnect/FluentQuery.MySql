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

    /// <summary>
    /// query helper
    /// </summary>
    public static class Q
    {
        public static FromQry<T> From<T>()
        {
            return new FromQry<T>();
        }

        public static FromQry<T> From<T>(T s)
        {
            return new FromQry<T>();
        }
        /// <summary>
        /// insert into
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static InsertQry<T> Insert<T>()
        {
            return new InsertQry<T>();
        }
        /// <summary>
        /// insert into
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static UpdateQry<T> Update<T>()
        {
            return new UpdateQry<T>();
        }




    }
}