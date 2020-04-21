using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryVisualizer.CommonMethods
{
    public class ConvertData
    {
        public static DataTable ConvertDicToDataTable_SimpleType_SimpleType<Tkey, TValue>(Dictionary<Tkey, TValue> dic)
        {
            Type keyType = typeof(Tkey);
            Type valueType = typeof(TValue);
            DataTable dt = new DataTable();
            dt.Columns.Add("`(KEY)" + keyType.Name);
            dt.Columns.Add("`(VALUE)" + valueType.Name);
            foreach (var item in dic)
            {
                DataRow row = dt.NewRow();
                if (item.Key == null)
                {
                    row["`(KEY)" + keyType.Name] = DBNull.Value;
                }
                else
                {
                    row["`(KEY)" + keyType.Name] = item.Key;
                }

                if (item.Value == null)
                {
                    row["`(VALUE)" + valueType.Name] = DBNull.Value;
                }
                else
                {
                    row["`(VALUE)" + valueType.Name] = item.Value;
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        public static DataTable ConvertDicToDataTable_Simpletype_ClassType<Tkey, TValue>(Dictionary<Tkey, TValue> dic)
        {
            Type keyType = typeof(Tkey);
            Type valueType = typeof(TValue);
            DataTable dt = new DataTable();
            dt.Columns.Add("`(KEY)" + keyType.Name);
            PropertyInfo[] properties = valueType.GetProperties();
            List<PropertyInfo> pList = new List<PropertyInfo>();
            Array.ForEach<PropertyInfo>(properties, p =>
            {
                pList.Add(p);
                dt.Columns.Add("`(VALUE)" + p.Name);
            });

            foreach (var item in dic)
            {
                DataRow row = dt.NewRow();
                if (item.Key == null)
                {
                    row["`(KEY)" + keyType.Name] = DBNull.Value;
                }
                else
                {
                    row["`(KEY)" + keyType.Name] = item.Key;
                }
                pList.ForEach(p =>
                {
                    if (item.Value == null)
                    {
                        row["`(VALUE)" + p.Name] = DBNull.Value;
                    }
                    else
                    {
                        object proValue = p.GetValue(item.Value, null);
                        row["`(VALUE)" + p.Name] = proValue ?? DBNull.Value;
                    }
                });
                dt.Rows.Add(row);
            }
            return dt;
        }

        public static DataTable ConvertDicToDataTable_SimpleType_ListType<Tkey, TValue>(Dictionary<Tkey, TValue> dic)
        {
            Type keyType = typeof(Tkey);
            Type valueType = typeof(TValue);
            DataTable dt = new DataTable();
            dt.Columns.Add("`(KEY)" + keyType.Name);
            Type elemType = valueType.GetGenericArguments()[0];
            PropertyInfo[] properties = elemType.GetProperties();
            List<PropertyInfo> pList = new List<PropertyInfo>();
            Array.ForEach<PropertyInfo>(properties, p =>
            {
                pList.Add(p);
                dt.Columns.Add("`(VALUE)" + p.Name);
            });

            foreach (var item in dic)
            {
                MethodInfo m_convertToDataTableMethod = typeof(ConvertData).GetMethod("ConvertDicValueToDataTable").MakeGenericMethod(elemType);
                DataTable dtValue = m_convertToDataTableMethod.Invoke(null, new object[] { item.Value, "`(VALUE)" }) as DataTable;
                foreach (DataRow valueRow in dtValue.Rows)
                {
                    DataRow row = dt.NewRow();
                    if (item.Key == null)
                    {
                        row["`(KEY)" + keyType.Name] = DBNull.Value;
                    }
                    else
                    {
                        row["`(KEY)" + keyType.Name] = item.Key;
                    }

                    foreach (DataColumn valueColumn in dtValue.Columns)
                    {
                        row[valueColumn.ColumnName] = valueRow[valueColumn.ColumnName];
                    }
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }

        public static DataTable ConvertDicToDataTable_ClassType_SimpleType<Tkey, TValue>(Dictionary<Tkey, TValue> dic)
        {
            Type keyType = typeof(Tkey);
            Type valueType = typeof(TValue);
            DataTable dt = new DataTable();
            PropertyInfo[] properties = keyType.GetProperties();
            List<PropertyInfo> pList = new List<PropertyInfo>();
            Array.ForEach<PropertyInfo>(properties, p =>
            {
                pList.Add(p);
                dt.Columns.Add("`(KEY)" + p.Name);
            });
            dt.Columns.Add("`(VALUE)" + valueType.Name);
            foreach (var item in dic)
            {
                DataRow row = dt.NewRow();
                pList.ForEach(p =>
                {
                    if (item.Key == null)
                    {
                        row["`(KEY)" + p.Name] = DBNull.Value;
                    }
                    else
                    {
                        object proValue = p.GetValue(item.Key, null);
                        row["`(KEY)" + p.Name] = proValue ?? DBNull.Value;
                    }
                });
                if (item.Value == null)
                {
                    row["`(Value)" + valueType.Name] = DBNull.Value;
                }
                else
                {
                    row["`(Value)" + valueType.Name] = item.Value;
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        public static DataTable ConvertDicToDataTable_ClassType_ClassType<Tkey, TValue>(Dictionary<Tkey, TValue> dic)
        {
            Type keyType = typeof(Tkey);
            Type valueType = typeof(TValue);
            DataTable dt = new DataTable();
            PropertyInfo[] keyproperties = keyType.GetProperties();
            List<PropertyInfo> keypList = new List<PropertyInfo>();
            Array.ForEach<PropertyInfo>(keyproperties, p =>
            {
                keypList.Add(p);
                dt.Columns.Add("`(KEY)" + p.Name);
            });

            PropertyInfo[] valueproperties = valueType.GetProperties();
            List<PropertyInfo> valuepList = new List<PropertyInfo>();
            Array.ForEach<PropertyInfo>(valueproperties, p =>
            {
                valuepList.Add(p);
                dt.Columns.Add("`(VALUE)" + p.Name);
            });

            foreach (var item in dic)
            {
                DataRow row = dt.NewRow();
                keypList.ForEach(p =>
                {
                    if (item.Key == null)
                    {
                        row["`(KEY)" + p.Name] = DBNull.Value;
                    }
                    else
                    {
                        object proValue = p.GetValue(item.Key, null);
                        row["`(KEY)" + p.Name] = proValue ?? DBNull.Value;
                    }
                });

                valuepList.ForEach(p =>
                {
                    if (item.Value == null)
                    {
                        row["`(VALUE)" + p.Name] = DBNull.Value;
                    }
                    else
                    {
                        object proValue = p.GetValue(item.Value, null);
                        row["`(VALUE)" + p.Name] = proValue ?? DBNull.Value;
                    }
                });

                dt.Rows.Add(row);
            }
            return dt;
        }

        public static DataTable ConvertDicToDataTable_ClassType_ListType<Tkey, TValue>(Dictionary<Tkey, TValue> dic)
        {
            Type keyType = typeof(Tkey);
            Type valueType = typeof(TValue);
            DataTable dt = new DataTable();
            PropertyInfo[] keyproperties = keyType.GetProperties();
            List<PropertyInfo> keypList = new List<PropertyInfo>();
            Array.ForEach<PropertyInfo>(keyproperties, p =>
            {
                keypList.Add(p);
                dt.Columns.Add("`(KEY)" + p.Name);
            });

            Type elemType = valueType.GetGenericArguments()[0];
            if (elemType.IsValueType || Type.GetTypeCode(elemType) == TypeCode.String)
            {
                dt.Columns.Add("`(VALUE)" + elemType.Name);
            }
            else
            {
                PropertyInfo[] valueproperties = elemType.GetProperties();
                List<PropertyInfo> pList = new List<PropertyInfo>();
                Array.ForEach<PropertyInfo>(valueproperties, p =>
                {
                    pList.Add(p);
                    dt.Columns.Add("`(VALUE)" + p.Name);
                });
            }


            foreach (var item in dic)
            {
                MethodInfo m_convertToDataTableMethod = typeof(ConvertData).GetMethod("ConvertDicValueToDataTable").MakeGenericMethod(elemType);
                DataTable dtValue = m_convertToDataTableMethod.Invoke(null, new object[] { item.Value, "`(VALUE)" }) as DataTable;
                foreach (DataRow valueRow in dtValue.Rows)
                {
                    DataRow row = dt.NewRow();
                    keypList.ForEach(p =>
                    {
                        if (item.Key == null)
                        {
                            row["`(KEY)" + p.Name] = DBNull.Value;
                        }
                        else
                        {
                            object proValue = p.GetValue(item.Key, null);
                            row["`(KEY)" + p.Name] = proValue ?? DBNull.Value;
                        }
                    });

                    foreach (DataColumn valueColumn in dtValue.Columns)
                    {
                        row[valueColumn.ColumnName] = valueRow[valueColumn.ColumnName];
                    }
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }

        public static DataTable ConvertDicValueToDataTable<T>(List<T> value, string prefixColName = "") where T : class
        {
            List<PropertyInfo> pList = new List<PropertyInfo>();//存放每一个列属性
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            DataTable dt = new DataTable();
            //获取属性，构造DataTable        
            Array.ForEach<PropertyInfo>(properties, p =>
            {
                pList.Add(p);
                dt.Columns.Add(prefixColName + p.Name);//不指定类型，在表格中的显示效果不算太好
            });

            if (value == null)//值为空，创建一空行
            {
                DataRow row = dt.NewRow();
                pList.ForEach(p =>
                {
                    row[prefixColName + p.Name] = DBNull.Value;
                });
                dt.Rows.Add(row);
                return dt;
            }

            foreach (var item in value)
            {
                DataRow row = dt.NewRow();
                pList.ForEach(p =>
                {
                    if (item == null)
                    {
                        row[prefixColName + p.Name] = DBNull.Value;
                    }
                    else
                    {
                        object proValue = p.GetValue(item, null);
                        row[prefixColName + p.Name] = proValue ?? DBNull.Value;
                    }
                });
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
