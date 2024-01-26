/*🏷️----------------------------------------------------------------
 *📄 文件名：StartUp.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/9/22 11:49:40
 *🏷️----------------------------------------------------------------*/


using System;
using MMClipboard;


public class StartUp
{
    [STAThread()]
    private static void Main()
    {
        App app = new();
        app.InitializeComponent();
        app.Run();
    }
}