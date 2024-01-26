/*🏷️----------------------------------------------------------------
 *📄 文件名：DataBaseController.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/9/22 10:25:40
 *🏷️----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.IO;
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

    /// <summary>
    /// 获取所有数据
    /// </summary>
    /// <returns></returns>
    public static List<ClipItemModel> GetAllData()
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
    /// 根据日期获取数据
    /// </summary>
    public static List<ClipItemModel> GetDataWithDate(DateTime date)
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
    /// 搜索内容
    /// </summary>
    /// <param name="searchText"></param>
    public static List<ClipItemModel> SearchContent(string searchText)
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
    /// 获取所有收藏的数据
    /// </summary>
    /// <returns></returns>
    public static List<ClipItemModel> GetAllCollectedData()
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
    /// 添加数据
    /// </summary>
    /// <param name="mod"></param>
    public static bool AddData(ClipItemModel mod)
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
    /// 添加数据列表
    /// </summary>
    /// <param name="arr"></param>
    public static bool AddDataFromList(List<ClipItemModel> arr)
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
    /// 更新数据项
    /// </summary>
    /// <param name="id"></param>
    /// <param name="collect"></param>
    public static bool UpdateItemCollectState(int id, int collect)
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
    public static bool DeleteData(ClipItemModel mod)
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
    /// 删除某一天的所有数据
    /// </summary>
    /// <param name="date"></param>
    public static bool DeleteAllWithDate(DateTime date)
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
    public static bool DeleteAllData()
    {
        try
        {
            Sqlite.Delete<ClipItemModel>()
                .Where("1=1")
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
    /// 获取所有数据的最早时间
    /// </summary>
    /// <returns></returns>
    public static DateTime GetFirstDataDate()
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

    // 关闭数据库连接
    public static void Close()
    { }
}