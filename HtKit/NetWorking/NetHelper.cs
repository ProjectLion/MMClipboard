/*🏷️----------------------------------------------------------------
 *📄 文件名：NetHelper.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-8-15 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using Newtonsoft.Json;


namespace HtKit.NetWorking;

public delegate void NetSuccessDelegate<T>(T data);

public delegate void NetFailDelegate(string error);

public static class NetHelper
{
    private static HttpClient httpClient
    {
        get
        {
            _httpClient ??= new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(15)
            };
            return _httpClient;
        }
    }

    private static HttpClient _httpClient;

    /// <summary>
    /// Get请求(返回Json字符串)
    /// </summary>
    /// <param name="url"> api </param>
    /// <param name="success"> 成功回调 </param>
    /// <param name="fail"> 失败回调 </param>
    public static async void GetResponse(string url, NetSuccessDelegate<string> success, NetFailDelegate fail = null)
    {
        try
        {
            var response = await httpClient.GetAsync(url);

            var dataStr = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
                success?.Invoke(dataStr);
            else
                fail?.Invoke(dataStr);
        }
        catch (Exception e)
        {
            e.Message.Log();
            fail?.Invoke(e.Message);
        }
    }

    /// <summary>
    /// Get请求(返回指定泛型)
    /// </summary>
    /// <param name="url"> api </param>
    /// <param name="success"> 成功回调 </param>
    /// <param name="fail"> 失败回调 </param>
    public static async void GetResponse<T>(string url, NetSuccessDelegate<T> success, NetFailDelegate fail = null)
    {
        var response = await httpClient.GetAsync(url);

        var dataStr = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.OK)
            success?.Invoke(JsonConvert.DeserializeObject<T>(dataStr)!);
        else
            fail?.Invoke(dataStr);
    }

    /// <summary>
    /// Post请求
    /// </summary>
    /// <param name="url"> api </param>
    /// <param name="parm"> 参数字典 </param>
    /// <param name="success"> 成功回调 </param>
    /// <param name="fail"> 失败回调 </param>
    public static async void PostResponse(string url, Dictionary<string, string> parm, NetSuccessDelegate<string> success, NetFailDelegate fail = null)
    {
        HttpContent content = new StringContent(JsonConvert.SerializeObject(parm));

        var response = await httpClient.PostAsync(url, content);

        var dataStr = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.OK)
            success?.Invoke(dataStr);
        else
            fail?.Invoke(dataStr);
    }

    [DllImport("wininet.dll", EntryPoint = "InternetGetConnectedState")]
    public static extern bool InternetGetConnectedState(out int conState, int reader);

    public static bool IsHaveNet()
    {
        var i = 0;
        return InternetGetConnectedState(out i, i);
    }
}