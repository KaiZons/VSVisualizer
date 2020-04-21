using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ListViewVisualizer.CommonMethods;
using ListViewVisualizer.View;
using System.Reflection;
using System.IO;
using ListViewVisualizer;

[assembly: System.Diagnostics.DebuggerVisualizer(
typeof(ListViewVisualizer.Debugger),
typeof(ListVisualizerObjectSource),
Target = typeof(List<>),
Description = "DataTableVisualizer")]
namespace ListViewVisualizer
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

    public class ListVisualizerObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            try
            {
                Type type = target.GetType().GetGenericArguments()[0];

                //过滤泛型参数类型为值类型，string类型，Object类型(简单类型不需要该功能)
                //type.IsValueType 无法过滤string类型和Object类型，需要附加条件 Type.GetTypeCode(type) == TypeCode.String || type == typeof(object)
                if (type == typeof(object))
                {
                    string errorInfo = "不支持List<object>集合）";
                    base.GetData(errorInfo, outgoingData);
                    return;
                }

                MethodInfo m_convertToDataTableMethod = typeof(ConvertData).GetMethod("ConvertToDataTable").MakeGenericMethod(type);
                DataTable dataTable = m_convertToDataTableMethod.Invoke(null, new object[] { target }) as DataTable;
                base.GetData(dataTable, outgoingData);
            }
            catch (Exception ex)
            {
                string errorInfo = $"调试器获取数据异常！\r\n{ex.Message}\r\n{ex.StackTrace}";
                base.GetData(errorInfo, outgoingData);
            }
        }
    }
}
