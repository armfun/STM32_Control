using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STM32Core
{
    /// <summary> 
    /// dyjson类型集
    /// </summary> 
    public enum JsonType
    {
        /// <summary> 
        /// dyjson类型集
        /// </summary> 
        None,
        /// <summary> 
        /// dyjson类型集
        /// </summary> 
        Object,
        /// <summary> 
        /// dyjson类型集
        /// </summary> 
        Array,
        /// <summary> 
        /// dyjson类型集
        /// </summary> 
        String,
        /// <summary> 
        /// dyjson类型集
        /// </summary> 
        Int,
        /// <summary> 
        /// dyjson类型集
        /// </summary> 
        Long,
        /// <summary> 
        /// dyjson类型集
        /// </summary> 
        Double,
        /// <summary> 
        /// dyjson类型集
        /// </summary> 
        Boolean
    }

    /// <summary> 
    /// dyjson地图
    /// </summary> 
    public interface IJsonWrapper : IList, IDictionary
    {
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        bool IsArray { get; }
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        bool IsBoolean { get; }
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        bool IsDouble { get; }
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        bool IsInt { get; }
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        bool IsLong { get; }
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        bool IsObject { get; }
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        bool IsString { get; }

        /// <summary> 
        /// dyjson地图
        /// </summary> 
        bool GetBoolean();
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        double GetDouble();
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        int GetInt();
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        JsonType GetJsonType();
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        long GetLong();
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        string GetString();
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        void SetBoolean(bool val);
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        void SetDouble(double val);
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        void SetInt(int val);
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        void SetJsonType(JsonType type);
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        void SetLong(long val);
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        void SetString(string val);
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        string ToJson();
        /// <summary> 
        /// dyjson地图
        /// </summary> 
        void ToJson(JsonWriter writer);
    }
}
