using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using ObjectToJsonVisualizer;
using Newtonsoft.Json;

[assembly: System.Diagnostics.DebuggerVisualizer(
typeof(Debugger),
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

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

    public class ObjectToJsonVisualizerObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            try
            {
                //不做加工
                base.GetData(target, outgoingData);
            }
            catch (Exception ex)
            {
                string errorInfo = $"调试器获取数据异常！\r\n{ex.Message}\r\n{ex.StackTrace}";
                base.GetData(errorInfo, outgoingData);
            }
        }
    }
}
