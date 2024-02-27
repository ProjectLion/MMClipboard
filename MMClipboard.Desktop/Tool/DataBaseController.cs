/*🏷️----------------------------------------------------------------
 *📄 文件名：DataBaseController.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/9/22 10:25:40
 *🏷️----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FreeSql;
using HtKit;
using MMClipboard.Model;


namespace MMClipboard.Tool;

public static class DataBaseController
{
    private static string DataPath
    {
        get
        {
            var dp = Path.Combine(AppPath.GetBaseDirectory(), @"Database");
            if (!Directory.Exists(dp))
                Directory.CreateDirectory(dp);
            var dbFilePath = Path.Combine(dp, "MMClipDataBase.db");
            if (!File.Exists(dbFilePath))
                File.Create(dbFilePath).Close();
            return dbFilePath;
        }
    }

    private static Lazy<IFreeSql> sqliteLazy = new(() => new FreeSqlBuilder()
        .UseConnectionString(DataType.Sqlite, $"DataSource={DataPath}")
        .UseAutoSyncStructure(true) //自动同步实体结构到数据库, 只有CRUD时才会生成表。
        // .UseMonitorCommand(cmd => $"Sql：{cmd.CommandText}".Debug()) //监听SQL语句,Trace在输出选项卡中查看
        .Build());

    private static IFreeSql Sqlite => sqliteLazy.Value;

    #region 剪切板历史相关

    /// <summary>
    /// 获取所有剪切板历史数据
    /// </summary>
    /// <returns></returns>
    public static List<ClipItemModel> GetAllHistoryData()
    {
        try
        {
            return Sqlite.Select<ClipItemModel>().OrderByDescending(x => x.date).ToList();
        }
        catch (Exception)
        {
            return [];
        }
    }

    /// <summary>
    /// 根据日期获取历史数据
    /// </summary>
    public static List<ClipItemModel> GetHistoryDataWithDate(DateTime date)
    {
        try
        {
            var res = Sqlite.Select<ClipItemModel>().Where(x => x.date.ToString("yyyy-MM-dd") == date.ToString("yyyy-MM-dd")).OrderByDescending(x => x.date).ToList();
            return res;
        }
        catch (Exception)
        {
            return [];
        }
    }

    /// <summary>
    /// 搜索历史数据
    /// </summary>
    /// <param name="searchText"></param>
    public static List<ClipItemModel> SearchHistoryDataWithText(string searchText)
    {
        try
        {
            var res = Sqlite.Select<ClipItemModel>().Where(x => x.content.Contains(searchText, StringComparison.OrdinalIgnoreCase)).OrderByDescending(x => x.date).ToList();
            return res;
        }
        catch (Exception)
        {
            return [];
        }
    }

    /// <summary>
    /// 获取所有收藏的历史数据
    /// </summary>
    /// <returns></returns>
    public static List<ClipItemModel> GetAllCollectedHistoryData()
    {
        try
        {
            var res = Sqlite.Select<ClipItemModel>().Where(x => x.collect == 1).OrderByDescending(x => x.date).ToList();
            return res;
        }
        catch (Exception)
        {
            return [];
        }
    }

    /// <summary>
    /// 添加历史数据
    /// </summary>
    /// <param name="mod"></param>
    public static bool AddHistoryData(ClipItemModel mod)
    {
        if (string.IsNullOrEmpty(mod.content))
            return false;
        try
        {
            Sqlite.Insert(mod).ExecuteIdentity();
            return true;
        }
        catch (Exception e)
        {
            e.Message.Debug();
            return false;
        }
    }

    /// <summary>
    /// 从列表中添加历史数据
    /// </summary>
    /// <param name="arr"></param>
    public static bool AddHistoryDataFromList(List<ClipItemModel> arr)
    {
        if (arr.Count == 0)
            return false;
        if (string.IsNullOrEmpty(arr[0].content) || string.IsNullOrWhiteSpace(arr[0].content) || arr[0].content == "\r\n")
            return false;
        try
        {
            Sqlite.Insert(arr).ExecuteAffrows();
            return true;
        }
        catch (Exception e)
        {
            e.Message.Debug();
            return false;
        }
    }

    /// <summary>
    /// 更新历史数据的收藏状态
    /// </summary>
    /// <param name="id"></param>
    /// <param name="collect"></param>
    public static bool UpdateHistoryDataCollectState(int id, int collect)
    {
        try
        {
            Sqlite.Update<ClipItemModel>().Where(x => x.id == id).Set(x => x.collect, collect).ExecuteAffrows();
            return true;
        }
        catch (Exception e)
        {
            e.Message.Debug();
            return false;
        }
    }

    /// <summary>
    /// 删除数据项
    /// </summary>
    /// <param name="mod"></param>
    public static bool DeleteHistoryData(ClipItemModel mod)
    {
        try
        {
            Sqlite.Delete<ClipItemModel>().Where(x => x.id == mod.id).ExecuteAffrows();
            return true;
        }
        catch (Exception e)
        {
            e.Message.Debug();
            return false;
        }
    }

    /// <summary>
    /// 删除某一天的所有type历史数据
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="type"></param>
    public static bool DeleteHistoryImageWithDate(DateTime dateTime, ClipType type)
    {
        try
        {
            Sqlite.Delete<ClipItemModel>().Where(x => x.date.ToString("yyyy-MM-dd") == dateTime.ToString("yyyy-MM-dd") && x.clipType == type).ExecuteAffrows();
            return true;
        }
        catch (Exception e)
        {
            e.Message.Debug();
            return false;
        }
    }

    /// <summary>
    /// 删除某一天的所有历史数据
    /// </summary>
    /// <param name="date"></param>
    public static bool DeleteAllHistoryWithDate(DateTime date)
    {
        try
        {
            Sqlite.Delete<ClipItemModel>()
                .Where(x => x.date.ToString("yyyy-MM-dd") == date.ToString("yyyy-MM-dd"))
                .ExecuteAffrows();
            return true;
        }
        catch (Exception e)
        {
            e.Message.Debug();
            return false;
        }
    }

    /// <summary>
    /// 删除数据表中的所有数据
    /// </summary>
    /// <returns></returns>
    public static void DeleteAllHistoryData()
    {
        try
        {
            Sqlite.Delete<ClipItemModel>()
                .Where("1=1")
                .ExecuteAffrows();
        }
        catch (Exception e)
        {
            e.Message.Debug();
        }
    }

    /// <summary>
    /// 获取所有历史数据中时间最早的一条
    /// </summary>
    /// <returns></returns>
    public static DateTime GetFirstDateWithHistory()
    {
        try
        {
            var res = Sqlite.Select<ClipItemModel>().OrderBy(x => x.date).First();
            return res.date;
        }
        catch (Exception)
        {
            return DateTime.Today;
        }
    }

    #endregion

    #region 常用短语相关

    /// <summary>
    /// 获取所有常用短语
    /// </summary>
    /// <returns></returns>
    public static List<ShortcutPhraseModel> GetAllPhrases()
    {
        try
        {
            var res = Sqlite.Select<ShortcutPhraseModel>().ToList();
            return res;
        }
        catch (Exception e)
        {
            e.Message.Debug();
            return [];
        }
    }

    /// <summary>
    /// 获取所有标签名
    /// </summary>
    /// <returns></returns>
    public static List<string> GetAllPhraseTags()
    {
        var res = GetAllPhrases().GroupBy(x => x.tagName).ToList();
        return res.Select(x => x.Key).ToList();
    }

    /// <summary>
    /// 新增一条常用短语
    /// </summary>
    /// <param name="mod"></param>
    /// <returns></returns>
    public static bool AddPhrase(ShortcutPhraseModel mod)
    {
        try
        {
            Sqlite.Insert(mod).ExecuteIdentity();
            return true;
        }
        catch (Exception e)
        {
            e.Message.Debug();
            return false;
        }
    }

    /// <summary>
    /// 更新常用短语
    /// </summary>
    /// <param name="mod"></param>
    /// <returns></returns>
    public static bool UpdatePhrase(ShortcutPhraseModel mod)
    {
        try
        {
            Sqlite.Update<ShortcutPhraseModel>().Where(x => x.id == mod.id).Set(x => x.phrase, mod.phrase).ExecuteAffrows();
            return true;
        }
        catch (Exception e)
        {
            e.Message.Debug();
            return false;
        }
    }

    /// <summary>
    /// 删除常用短语
    /// </summary>
    /// <param name="mod"></param>
    /// <returns></returns>
    public static bool DeletePhrase(ShortcutPhraseModel mod)
    {
        try
        {
            Sqlite.Delete<ShortcutPhraseModel>().Where(x => x.id == mod.id).ExecuteAffrows();
            return true;
        }
        catch (Exception e)
        {
            e.Message.Debug();
            return false;
        }
    }

    /// <summary>
    /// 获取常用短语标签颜色
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static string GetPhraseTagColorWithTag(string tag)
    {
        try
        {
            var res = Sqlite.Select<ShortcutPhraseModel>().Where(x => x.tagName == tag).First();
            return res.tagColor;
        }
        catch (Exception e)
        {
            e.Message.Log();
            return null;
        }
    }

    #endregion

    // 关闭数据库连接
    public static void Close()
    { }
}