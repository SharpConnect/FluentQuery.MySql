//MIT, 2015-2016, brezza92, EngineKit and contributors

using System;
using System.Collections.Generic;
using System.Text;


namespace SharpConnect.FluentQuery.Experiment
{
    public static class Q4
    {
        public static MapperFrom From(string dbName)
        {
            return new Experiment.MapperFrom(dbName);
        }
    }
    public class MapQuery<T>
    {
        MapRecord2<T> record;
        public MapQuery(MapRecord2<T> record)
        {
            this.record = record;
        }
        public MapperFrom MapFromSource
        {
            get; set;
        }

        public string ToSqlString()
        {
            //create query string from record
            return "select " +
             record.GetFieldListString() +
             " from " +
             MapFromSource.SourceName;
        }

    }

    public class MapperFrom
    {
        public MapperFrom(string srcName)
        {
            SourceName = srcName;
        }
        public string SourceName { get; set; }
        public MapQuery<T> Select<T>(MapRecord2<T> record)
        {
            var mapQuery = new MapQuery<T>(record);
            mapQuery.MapFromSource = this;
            return mapQuery;
        }
        public MapQuery<T> Select<T>(MapNew<T> recordMapDel)
        {
            MapRecord2<T> record = new MapRecord2<T>(recordMapDel);
            var mapQuery = new MapQuery<T>(record);
            mapQuery.MapFromSource = this;
            return mapQuery;
        }
        public MapQuery<T> SelectAndMapTo<X, T>(MapNewInto<X, T> recordMapDel)
        {
            var mapDel = new MapNew<T>((T t) => { });
            MapRecord2<T> record = new MapRecord2<T>(mapDel);
            //since we modify the lambda 
            //with new t value so we need to supply the  original here
            var parInfoList = recordMapDel.Method.GetParameters();            //
            int j = parInfoList.Length;
            //excep the last one
            System.Reflection.ParameterInfo[] newParInfoList = new System.Reflection.ParameterInfo[j - 1];
            for (int i = 1; i < j; ++i)
            {
                newParInfoList[i - 1] = parInfoList[i];
            }
            record.SetOriginalParameterInfo(newParInfoList);
            var mapQuery = new MapQuery<T>(record);
            mapQuery.MapFromSource = this;
            return mapQuery;
        }
    }



    public class MapRecord<R, T>
    {
        MapAction<R, T> recordMapDel;
        public MapRecord(MapAction<R, T> recordMapDel)
        {
            this.recordMapDel = recordMapDel;

        }
        public T M1 { get; set; }
        public string GetFieldListString()
        {
            //----------------------------------------
            //create simple selection list
            //TODO: check if we have create the string list or not

            var originalMethod = recordMapDel.Method;
            //get paraemter list
            var parInfos = originalMethod.GetParameters();
            int j = parInfos.Length;
            StringBuilder stbuilder = new StringBuilder();
            for (int i = 1; i < j; ++i)
            {
                //start from 1***
                var parInfo = parInfos[i];
                if (i > 1)
                {
                    stbuilder.Append(',');
                }
                stbuilder.Append(parInfo.Name);
            }
            return stbuilder.ToString();
        }
    }

    public abstract class MapRecordBase
    {
        public abstract string GetFieldListString();
    }
    public class MapRecord2<T> : MapRecordBase
    {
        MapNew<T> recordMapDel;
        System.Reflection.ParameterInfo[] originalParameterInfos;
        public MapRecord2(MapNew<T> recordMapDel)
        {
            this.recordMapDel = recordMapDel;
        }


        public T M1 { get; set; }
        internal void SetOriginalParameterInfo(System.Reflection.ParameterInfo[] originalParameterInfos)
        {
            this.originalParameterInfos = originalParameterInfos;
        }
        public override string GetFieldListString()
        {
            //----------------------------------------
            //create simple selection list
            //TODO: check if we have create the string list or not
            if (originalParameterInfos == null)
            {
                //user this
                var originalMethod = recordMapDel.Method;
                originalParameterInfos = originalMethod.GetParameters();
            }

            //get paraemter list 
            int j = originalParameterInfos.Length;
            StringBuilder stbuilder = new StringBuilder();
            for (int i = 0; i < j; ++i)
            {
                //start from 0***
                var parInfo = originalParameterInfos[i];
                if (i > 0)
                {
                    stbuilder.Append(',');
                }
                stbuilder.Append(parInfo.Name);
            }
            return stbuilder.ToString();
        }
    }



    public class MapRecord<R, T1, T2>
    {
        Action<R, T1, T2> recordMapDel;
        public MapRecord(Action<R, T1, T2> recordMapDel)
        {
            this.recordMapDel = recordMapDel;
        }
        public T1 M1 { get; set; }
        public T2 M2 { get; set; }
    }

    public delegate void MapAction<R, T>(R r, out T t);
    public delegate void MapNew<T>(T t);
    public delegate void MapNewInto<T, X>(T t, X target);
    public static class Mapper
    {
        public static MapRecord2<T> New<T>(MapNew<T> ac)
        {
            return new MapRecord2<T>(ac);
        }

        //----------------------------------------------------------------------
        public static MapRecord<R, T> Map<R, T>(MapAction<R, T> ac)
        {
            return new MapRecord<R, T>(ac);
        }
        public static MapRecord<R, T1, T2> Map<R, T1, T2>(Action<R, T1, T2> ac)
        {
            return new MapRecord<R, T1, T2>(ac);
        }
    }

}