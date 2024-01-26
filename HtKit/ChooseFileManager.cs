/*🏷️----------------------------------------------------------------
 *📄 文件名：ChooseFileManager.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;


namespace HtKit;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal class OpenDialogFile
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public string filter = null;
    public string customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public string file = null;
    public int maxFile = 0;
    public string fileTitle = null;
    public int maxFileTitle = 0;
    public string initialDir = null;
    public string title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public string defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public string templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal class OpenDialogDir
{
    public IntPtr hwndOwner = IntPtr.Zero;
    public IntPtr pidlRoot = IntPtr.Zero;
    public string pszDisplayName = null;
    public string lpszTitle = null;
    public uint ulFlags = 0;
    public IntPtr lpfn = IntPtr.Zero;
    public IntPtr lParam = IntPtr.Zero;
    public int iImage = 0;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal struct OpenFileName
{
    public int structSize;
    public IntPtr dlgOwner;
    public IntPtr instance;
    public String filter;
    public String customFilter;
    public int maxCustFilter;
    public int filterIndex;
    public string file;
    public int maxFile;
    public String fileTitle;
    public int maxFileTitle;
    public String initialDir;
    public String title;
    public int flags;
    public short fileOffset;
    public short fileExtension;
    public String defExt;
    public IntPtr custData;
    public IntPtr hook;
    public String templateName;
    public IntPtr reservedPtr;
    public int reservedInt;
    public int flagsEx;
}

public static class ChooseFileManager
{
    #region Window

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName([In] [Out] OpenFileName ofn);

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool GetSaveFileName([In] [Out] OpenFileName ofn);

    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern IntPtr SHBrowseForFolder([In] [Out] OpenDialogDir ofn);

    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In] [Out] char[] fileName);

    #endregion Window

    /*
     筛选器字符串应包含筛选器的说明，后跟竖线(|)和筛选模式。多个筛选器说明和模式对还必须以竖线分隔。在一个筛选器模式中的多个扩展名必须用分号分隔。例如: \"图像文件(*.bmp, *.jpg)|*.bmp;*.jpg|所有文件(*.*)|*.*\"
     */
    public const string AllFILTER = "所有文件(*.*)|*.";
    public const string TxtFILTER = "txt文件(*.txt*)|*.txt";
    public const string ImageFILTER = "图片(*.jpg,*.png)|*.jpg;*.png";
    public const string AudioFILTER = "音乐文件(*.mp3)|*.mp3";
    public const string PDFFILTER = "pdf文件(*.pdf)|*.pdf";
    public const string DOCFILTER = "word文档(*.doc,*.docx)|*.doc;*.docx";

    /// <summary>
    /// 选择文件
    /// </summary>
    /// <param name="callback">返回选择文件夹的路径</param>
    /// <param name="filter">文件类型筛选器</param>
    /// <param name="isChooseMore">是否多选</param>
    public static void SelectFile(Action<string> callback, string filter = AllFILTER, bool isChooseMore = true)
    {
        OpenFileDialog dialog = new()
        {
            DefaultExt = ".png",
            Filter = filter,
            InitialDirectory = @"C:\Users\PC\Pictures\"
        };
        // 打开选择框选择
        var result = dialog.ShowDialog();
        callback?.Invoke(result == true ? dialog.FileName : "");
    }

    /// <summary>
    /// 选择图片
    /// </summary>
    /// <param name="callback"></param>
    /// <param name="isChooseMore"></param>
    public static void SelectImageFile(Action<string> callback, bool isChooseMore = true)
    {
        OpenFileDialog dialog = new()
        {
            DefaultExt = ".png",
            Filter = ImageFILTER,
            InitialDirectory = @"C:\Users\PC\Pictures\"
        };
        // 打开选择框选择
        var result = dialog.ShowDialog();
        callback?.Invoke(result == true ? dialog.FileName : "");
    }

    /// <summary>
    /// 保存文件
    /// </summary>
    /// <param name="fileExt"> 文件默认后缀 </param>
    /// <param name="filter"> 文件类型 </param>
    /// <param name="callBack"></param>
    public static void GetSavePath(Action<string> callBack, string fileExt, string filter = AllFILTER)
    {
        var pth = new OpenFileName();
        pth.structSize = Marshal.SizeOf(pth);
        pth.filter = filter;
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        pth.initialDir = @"C:\User\Desktop"; //默认路径
        pth.title = "保存文件";
        pth.defExt = fileExt;
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        if (!GetSaveFileName(pth)) return;
        var filepath = pth.file; //选择的文件路径;
        callBack?.Invoke(filepath);
    }

    /// <summary>
    /// 调用WindowsExploer 并返回所选文件夹路径
    /// </summary>
    /// <param name="callback"></param>
    /// <param name="dialogTitle">打开对话框的标题</param>
    /// <returns>所选文件夹路径</returns>
    public static void GetPathFromWindowsExplorer(Action<string> callback, string dialogTitle = "请选择保存路径")
    {
        try
        {
            var ofn2 = new OpenDialogDir
            {
                pszDisplayName = new string(new char[2048])
            };
            ; // 存放目录路径缓冲区
            ofn2.lpszTitle = dialogTitle; // 标题
            ofn2.ulFlags = 0x00000040; // 新的样式,带编辑框
            var pidlPtr = SHBrowseForFolder(ofn2);

            var charArray = new char[2048];

            for (var i = 0; i < 2048; i++) charArray[i] = '\0';

            SHGetPathFromIDList(pidlPtr, charArray);
            var res = new string(charArray);
            res = res[..res.IndexOf('\0')];
            callback?.Invoke(res);
        }
        catch (Exception e)
        {
            e.Debug();
            callback?.Invoke("选择出错，请重试");
        }
    }

    /// <summary>
    /// 打开目录
    /// </summary>
    /// <param name="path">将要打开的文件目录</param>
    public static void OpenFolder(string path)
    {
        Process.Start("explorer.exe", path);
    }
}