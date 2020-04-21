using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ListViewVisualizer.CommonMethods
{
    public class ConvertData
    {
        public static DataTable ConvertToDataTable<T>(List<T> value)
        {
            List<PropertyInfo> pList = new List<PropertyInfo>();//存放每一个列属性
            Type type = typeof(T);
            DataTable dt = new DataTable();
            dt.Columns.Add("序号");
            if (type.IsValueType || Type.GetTypeCode(type) == TypeCode.String)
            {
                dt.Columns.Add(type.Name);
                if (value == null)
                {
                    DataRow row = dt.NewRow();
                    row["序号"] = DBNull.Value;
                    row[type.Name] = DBNull.Value;
                }
                else
                {
                    int i = 0;
                    foreach (var item in value)
                    {
                        DataRow row = dt.NewRow();
                        row["序号"] = i++;
                        if (item == null)
                        {
                            row[type.Name] = DBNull.Value;
                        }
                        else
                        {
                            row[type.Name] = item;
                        }
                        dt.Rows.Add(row);
                    }
                }
                return dt;
            }

            PropertyInfo[] properties = type.GetProperties();
            //获取属性，构造DataTable        
            Array.ForEach<PropertyInfo>(properties, p =>
            {
                pList.Add(p);
                dt.Columns.Add(p.Name);//不指定类型，在表格中的显示效果不算太好

                #region 创建列方法二(为列指定类型，泛型元素不会显示)
                //if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition().Equals(typeof(System.Nullable<>)))//可为空类型
                //{
                //    dt.Columns.Add(p.Name, p.PropertyType.GetGenericArguments()[0]);
                //}
                //else
                //{
                //    dt.Columns.Add(p.Name, p.PropertyType);
                //}
                #endregion
            });
            int j = 0;
            foreach (var item in value)
            {      
                DataRow row = dt.NewRow();
                row["序号"] = j++;
                pList.ForEach(p =>
                {
                    object proValue = p.GetValue(item, null);
                    row[p.Name] = proValue ?? DBNull.Value;
                });
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
