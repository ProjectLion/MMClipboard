/*🏷️----------------------------------------------------------------
 *📄 文件名：HtZipHelper.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.IO;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;


namespace Escher.Tool;

public static class HtZipHelper
{
    public static async Task Unzip(string zipPath, string toDir, Action<int, long> progressAc, Action completeAc, Action<string> failAc)
    {
        if (string.IsNullOrEmpty(toDir)) return;
        if (!Directory.Exists(toDir))
            Directory.CreateDirectory(toDir);
        if (!File.Exists(zipPath))
            return;
        await Task.Run(() =>
        {
            try
            {
                using FileStream fs = new(zipPath, FileMode.Open, FileAccess.Read);
                using ZipFile zf = new(fs);
                var totalEntries = zf.Count;
                var currentEntry = 0;
                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                        continue; // 跳过非文件项
                    currentEntry++;
                    progressAc?.Invoke(currentEntry, totalEntries);
                    var entryFileName = zipEntry.Name;
                    var buffer = new byte[4096];

                    var fullZipToPath = Path.Combine(toDir, entryFileName);
                    var directoryName = Path.GetDirectoryName(fullZipToPath);

                    if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);

                    using var zipStream = zf.GetInputStream(zipEntry);
                    using var streamWriter = File.Create(fullZipToPath);
                    StreamUtils.Copy(zipStream, streamWriter, buffer);
                }
                fs.Close();
                completeAc?.Invoke();
            }
            catch (Exception e)
            {
                failAc?.Invoke(e.Message.Log());
            }
        });
    }
}