/*🏷️----------------------------------------------------------------
 *📄 文件名：FileHelper.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HtKit;

public static class FileHelper
{
    /// <summary>
    /// 将字节流写入到文件
    /// </summary>
    /// <param name="path"> 文件地址 </param>
    /// <param name="data"> 数据 </param>
    /// <param name="complete"> 完成回调 </param>
    /// <param name="isAppend"> 是否为新增 </param>
    /// <exception cref="Exception"></exception>
    public static async void WriteBytesToFile(string path, byte[] data, Action complete = null, bool isAppend = false)
    {
        if (path is null || path.Length == 0)
        {
            "地址不能为空".Debug();
            return;
        }

        if (!File.Exists(path))
        {
            await using Stream fs = File.Create(path);
            await fs.FlushAsync();
            fs.Close();
        }

        var mode = FileMode.OpenOrCreate;
        if (isAppend) mode = FileMode.Append;

        await Task.Run(async () =>
        {
            await using Stream stream = new FileStream(path, mode, FileAccess.Write);
            await stream.WriteAsync(data);
            await stream.FlushAsync();
            stream.Close();
            Dispatch.BackToMainThreadSync(() => complete?.Invoke());
        });
    }

    /// <summary>
    /// 将字符串写入到文件(UTF8编码)
    /// </summary>
    /// <param name="path"> 文件地址 </param>
    /// <param name="dataStr"> 数据字符串 </param>
    /// <param name="encoding"></param>
    /// <param name="complete"> 完成回调 </param>
    /// <param name="isAppend"> 是否为新增 </param>
    /// <exception cref="Exception"></exception>
    public static void WriteStringToFile(string path, string dataStr, Encoding encoding, Action complete = null, bool isAppend = false)
    {
        if (path.Ht_IsEmpty())
            throw new Exception("地址不能为空");

        WriteBytesToFile(path, encoding.GetBytes(dataStr), complete, isAppend);
    }

    /// <summary>
    /// 从指定文件读取字节流(异步)
    /// </summary>
    /// <param name="path"> 文件地址 </param>
    /// <param name="ac"> 完成回调 </param>
    /// <exception cref="Exception"></exception>
    public static async void ReadBytesFromFileAsync(string path, Action<byte[]> ac)
    {
        if (path.Ht_IsEmpty())
            throw new Exception("地址不能为空");

        if (!File.Exists(path))
            throw new Exception("文件不存在");

        await Task.Run(async () =>
        {
            var bytes = new byte[1024];
            var result = new List<byte>();
            await using Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            while (true)
            {
                var r = await stream.ReadAsync(bytes);
                if (r == 0)
                {
                    Dispatch.BackToMainThreadSync(() => ac.Invoke(result.ToArray()));
                    break;
                }
                result.AddRange(bytes.ToList().GetRange(0, r));
            }
            await stream.FlushAsync();
            stream.Close();
        });
    }

    /// <summary>
    /// 从文件读取字符串
    /// </summary>
    /// <param name="path"> 文件地址 </param>
    /// <param name="ac"> 完成回调 </param>
    /// <param name="encoding"> 字符串编码方式 </param>
    /// <exception cref="Exception"></exception>
    public static void ReadStringFromFileAsync(string path, Action<string> ac, Encoding encoding)
    {
        if (path.Ht_IsEmpty())
            throw new Exception("地址不能为空");

        if (!File.Exists(path))
            throw new Exception("文件不存在");

        ReadBytesFromFileAsync(path, (bt) => ac(encoding.GetString(bt)));
    }

    /// <summary>
    /// 获取文件信息 byte
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static long GetFileSize(string filePath)
    {
        if (!File.Exists(filePath))
            throw new Exception("文件不存在");
        var info = new FileInfo(filePath);
        return info.Length;
    }
}