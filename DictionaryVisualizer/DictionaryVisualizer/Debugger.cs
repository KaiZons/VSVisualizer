using DictionaryVisualizer;
using DictionaryVisualizer.CommonMethods;
using ListViewVisualizer.View;
using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

[assembly: System.Diagnostics.DebuggerVisualizer(
typeof(DictionaryVisualizer.Debugger),
typeof(DictionaryVisualizerObjectSource),
Target = typeof(Dictionary<,>),
Description = "DataTableVisualizer")]
namespace DictionaryVisualizer
{
    public class Debugger : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            try
            {
                object value = objectProvider.GetObject();
                if (Type.GetTypeCode(value.GetType()) == TypeCode.String)
                {
                    string message = value as string;
                    MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataTable dataTable = value as DataTable;
                if (dataTable == null)
                {
                    return;
                }

                using (ShowData dialog = new ShowData(dataTable))
                {
                    dialog.ShowDialog();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
    }

    public class DictionaryVisualizerObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            try
            {
                Type[] key_valueTypes = target.GetType().GetGenericArguments();
                Type keyType = key_valueTypes[0];
                Type valueType = key_valueTypes[1];

                #region 校验数据类型
                Tuple<bool, string> checkTuple = IsDataTypeAcceptable(keyType, valueType);
                if (!checkTuple.Item1)
                {
                    base.GetData(checkTuple.Item2, outgoingData);
                    return;
                }
                #endregion

                #region 构造表结构
                DictionaryDataStruct dictionaryDataStruct;
                DataTable dt = GetTableStruct(keyType, valueType, out dictionaryDataStruct);
                if (dt.Columns.Count == 0)
                {
                    string errorInfo = "字典中没有可显示的列";
                    base.GetData(errorInfo, outgoingData);
                    return;
                }
                #endregion

                switch (dictionaryDataStruct)
                {
                    case DictionaryDataStruct.SimpleType_SimpleType:
                        dt = FillData_SimpleType_SimpleType(target, keyType, valueType);
                        break;
                    case DictionaryDataStruct.Simpletype_ClassType:
                        dt = FillData_Simpletype_ClassType(target, keyType, valueType);
                        break;
                    case DictionaryDataStruct.SimpleType_ListType:
                        dt = FillData_SimpleType_ListType(target, keyType, valueType);
                        break;
                    case DictionaryDataStruct.ClassType_SimpleType:
                        dt = FillData_ClassType_SimpleType(target, keyType, valueType);
                        break;
                    case DictionaryDataStruct.ClassType_ClassType:
                        dt = FillData_ClassType_ClassType(target, keyType, valueType);
                        break;
                    case DictionaryDataStruct.ClassType_ListType:
                        dt = FillData_ClassType_ListType(target, keyType, valueType);
                        break;
                    default:
                        break;
                }
                base.GetData(dt, outgoingData);
            }
            catch (Exception ex)
            {
                string errorInfo = $"调试器获取数据异常！\r\n{ex.Message}\r\n{ex.StackTrace}";
                base.GetData(errorInfo, outgoingData);
            }
        }

        private Tuple<bool, string> IsDataTypeAcceptable(Type keyType, Type valueType)
        {
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(keyType) && Type.GetTypeCode(keyType) != TypeCode.String)
            {
                string errorInfo = "不支持键为集合的字典";
                return new Tuple<bool, string>(false, errorInfo);
            }
            if (keyType.IsGenericType)
            {
                string errorInfo = "不支持键为泛型的字典";
                return new Tuple<bool, string>(false, errorInfo);
            }
            if (keyType == typeof(object))
            {
                string errorInfo = "不支持键为object的字典";
                return new Tuple<bool, string>(false, errorInfo);
            }

            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(valueType) && valueType.Name != "List`1" && Type.GetTypeCode(valueType) != TypeCode.String)
            {
                string errorInfo = "不支持Value是List<T>之外的集合的字典";
                return new Tuple<bool, string>(false, errorInfo);
            }
            return new Tuple<bool, string>(true, null);
        }

        private DataTable GetTableStruct(Type keyType, Type valueType, out DictionaryDataStruct dictionaryDataStruct)
        {
            Key_ValueType keyDataType = Key_ValueType.Key_SimpleType;
            Key_ValueType valueDataType = Key_ValueType.Value_SimpleType;

            DataTable dt = new DataTable();
            if (keyType.IsValueType || Type.GetTypeCode(keyType) == TypeCode.String)
            {
                keyDataType = Key_ValueType.Key_SimpleType;
                dt.Columns.Add("`(KEY)");
            }
            else
            {
                PropertyInfo[] properties = keyType.GetProperties();
                Array.ForEach<PropertyInfo>(properties, p =>
                {
                    dt.Columns.Add("`(KEY)" + p.Name);
                });
                keyDataType = Key_ValueType.Key_ClassType;
            }

            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(valueType) && Type.GetTypeCode(valueType) != TypeCode.String)
            {
                if (valueType.Name == "List`1")
                {
                    Type elemType = valueType.GetGenericArguments()[0];
                    PropertyInfo[] properties = elemType.GetProperties();
                    Array.ForEach<PropertyInfo>(properties, p =>
                    {
                        dt.Columns.Add("`(VALUE)" + p.Name);
                    });
                    valueDataType = Key_ValueType.Value_ListType;
                }
                else
                {
                    //其他集合类型不做处理
                }
            }
            else
            {
                if (valueType.IsValueType || Type.GetTypeCode(valueType) == TypeCode.String)
                {
                    dt.Columns.Add("`(VALUE)");
                    valueDataType = Key_ValueType.Value_SimpleType;
                }
                else
                {
                    PropertyInfo[] properties = valueType.GetProperties();
                    Array.ForEach<PropertyInfo>(properties, p =>
                    {
                        dt.Columns.Add("`(VALUE)" + p.Name);
                    });
                    valueDataType = Key_ValueType.Value_ClassType;
                }
            }

            dictionaryDataStruct = (DictionaryDataStruct)(int)(keyDataType | valueDataType);
            return dt;
        }

        private static DataTable FillData_SimpleType_SimpleType(object targetData, Type keyType, Type valueType)
        {
            MethodInfo m_convertToDataTableMethod = typeof(ConvertData).GetMethod("ConvertDicToDataTable_SimpleType_SimpleType").MakeGenericMethod(keyType, valueType);
            return m_convertToDataTableMethod.Invoke(null, new object[] { targetData }) as DataTable;
        }

        private static DataTable FillData_Simpletype_ClassType(object targetData, Type keyType, Type valueType)
        {
            MethodInfo m_convertToDataTableMethod = typeof(ConvertData).GetMethod("ConvertDicToDataTable_Simpletype_ClassType").MakeGenericMethod(keyType, valueType);
            return m_convertToDataTableMethod.Invoke(null, new object[] { targetData }) as DataTable;
        }

        private static DataTable FillData_SimpleType_ListType(object targetData, Type keyType, Type valueType)
        {
            MethodInfo m_convertToDataTableMethod = typeof(ConvertData).GetMethod("ConvertDicToDataTable_SimpleType_ListType").MakeGenericMethod(keyType, valueType);
            return m_convertToDataTableMethod.Invoke(null, new object[] { targetData }) as DataTable;
        }

        private static DataTable FillData_ClassType_SimpleType(object targetData, Type keyType, Type valueType)
        {
            MethodInfo m_convertToDataTableMethod = typeof(ConvertData).GetMethod("ConvertDicToDataTable_ClassType_SimpleType").MakeGenericMethod(keyType, valueType);
            return m_convertToDataTableMethod.Invoke(null, new object[] { targetData }) as DataTable;
        }

        private static DataTable FillData_ClassType_ClassType(object targetData, Type keyType, Type valueType)
        {
            MethodInfo m_convertToDataTableMethod = typeof(ConvertData).GetMethod("ConvertDicToDataTable_ClassType_ClassType").MakeGenericMethod(keyType, valueType);
            return m_convertToDataTableMethod.Invoke(null, new object[] { targetData }) as DataTable;
        }

        private static DataTable FillData_ClassType_ListType(object targetData, Type keyType, Type valueType)
        {
            MethodInfo m_convertToDataTableMethod = typeof(ConvertData).GetMethod("ConvertDicToDataTable_ClassType_ListType").MakeGenericMethod(keyType, valueType);
            return m_convertToDataTableMethod.Invoke(null, new object[] { targetData }) as DataTable;
        }

    }

    /// <summary>
    /// 可调试的Dic数据类型
    /// </summary>
    public enum DictionaryDataStruct
    {
        SimpleType_SimpleType = 5,//如Dictionary<int,string>
        Simpletype_ClassType = 9,//如Dictionary<int,Class>
        SimpleType_ListType = 17,//如Dictionary<int,List<T>>

        ClassType_SimpleType = 6,//如Dictionary<Class,int>
        ClassType_ClassType = 10,//如Dictionary<Class,Class>
        ClassType_ListType = 18//如Dictionary<Class,List<T>>
    }

    public enum Key_ValueType
    {
        Key_SimpleType = 1,
        Key_ClassType = 2,
        Value_SimpleType = 4,
        Value_ClassType = 8,
        Value_ListType = 16
    }
}
