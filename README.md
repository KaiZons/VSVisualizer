一.	安装方法：
	将ListViewVisualizer.dll复制到：VS安装目录下的\Common7\Packages\Debugger\Visualizers
文件夹下即可；
	如VS默认路径
C:\Program Files (x86)\Microsoft VisualStudio\2017\Enterprise\Common7\Packages\Debugger\Visualizers

二.	使用注意点：
1.不支持List<object>集合；
2.泛型参数无需标记可序列化。

三.	使用方法：
	调试状态下，鼠标移至List<>集合上，会出现一个放大镜图标，点击放大镜图标，会弹出一个窗体，窗体中会显示List<>集合的表格视图
