using Microsoft.VisualStudio.DebuggerVisualizers;
using Newtonsoft.Json;
using ObjectToJsonVisualizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;


/*
 * 如果提示：未能加载此自定义查看器解决方法
 * 解决方案：在vs中，选择调试-选项-常规中的使用托管兼容模式取消勾选。之后就可以了
 */
[assembly: System.Diagnostics.DebuggerVisualizer(
typeof(ObjectToJsonVisualizer.Debugger),
typeof(ObjectToJsonVisualizerObjectSource),
Target = typeof(object),
Description = "JSONVisualizer")]
namespace ObjectToJsonVisualizer
{
    public class Debugger : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            try
            {
                object value = objectProvider.GetObject();
                // 显示
                using (JsonView jsonView = new JsonView(value.ToString()))
                {
                    jsonView.ShowDialog();
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace +e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    public class ObjectToJsonVisualizerObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            try
            {
                //不做加工
                base.GetData(GetJsonString(target), outgoingData);
            }
            catch (Exception ex)
            {
                string errorInfo = $"调试器获取数据异常！\r\n{ex.Message}\r\n{ex.StackTrace}";
                base.GetData(errorInfo, outgoingData);
            }
        }

        private string GetJsonString(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            StringWriter textWriter = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(textWriter);
            jsonWriter.Formatting = Formatting.Indented;
            jsonWriter.Indentation = 4;
            jsonWriter.IndentChar = ' ';
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(jsonWriter, obj);
            return textWriter.ToString();
        }
    }
}
