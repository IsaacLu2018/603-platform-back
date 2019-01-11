using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace MyCommon
{
    [AttributeUsage(AttributeTargets.Property)]

    public sealed class DataFieldAttribute : Attribute

    {
        /// <summary>
        /// 对应的字段
        /// </summary>
        public string ColumnName { set; get; }

        public DataFieldAttribute(string columnName)

        {

            ColumnName = columnName;

        }

    }

    public static class ModelConvert

    {
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="fromModel"></param>
        /// <param name="toModel"></param>
        /// <returns></returns>
        public static T FromTo<F, T>(F fromModel, T toModel)
        where T : new()
        {
            if (fromModel == null)
            {
                return default(T);
            }
            System.Reflection.PropertyInfo[] fProperties = fromModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            System.Reflection.PropertyInfo[] tProperties = toModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            T entity = new T();

            Type info = typeof(T);

            var members = info.GetMembers();

            foreach (var mi in members)

            {

                if (mi.MemberType == MemberTypes.Property)

                {
                    //读取属性上的DataField特性
                    object[] attributes = mi.GetCustomAttributes(typeof(DataFieldAttribute), true);

                    foreach (var attr in attributes)

                    {
                        var dataFieldAttr = attr as DataFieldAttribute;

                        if (dataFieldAttr != null)
                        {
                            var propInfo = info.GetProperty(mi.Name);
                            foreach (System.Reflection.PropertyInfo item in fProperties)
                            {
                                object value = item.GetValue(fromModel, null);
                                if (item.Name.Contains(dataFieldAttr.ColumnName))
                                {
                                    if (value != null)
                                        propInfo.SetValue(entity, Convert.ChangeType(value, GetNicePropertyInfo(propInfo.PropertyType)), null);

                                }
                            }
                        }
                    }
                }
            }
            return entity;

        }
        /// <summary>
        /// 处理可空类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNicePropertyInfo(Type type)
        {
            NullType nullType = new NullType();
            if (type.ToString() == "System.Nullable`1[System.Int32]")
            {
                type = nullType.NullInt.GetType();
            }
            if (type.ToString() == "System.Nullable`1[System.Decimal]")
            {
                type = nullType.NullDecimal.GetType();
            }
            if (type.ToString() == "System.Nullable`1[System.Double]")
            {
                type = nullType.NullDoublel.GetType();
            }
            if (type.ToString() == "System.Nullable`1[System.Long]")
            {
                type = nullType.NullLong.GetType();
            }
            if (type.ToString() == "System.Nullable`1[System.DateTime]")
            {
                type = nullType.NullDateTime.GetType();
            }
            return type;
        }
        public class NullType
        {
            public decimal NullDecimal { get; set; }
            public int NullInt { get; set; }
            public double NullDoublel { get; set; }
            public long NullLong { get; set; }
            public DateTime NullDateTime { get; set; }
        }
    }


}
