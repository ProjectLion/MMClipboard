/*🏷️----------------------------------------------------------------
 *📄 文件名：UserConfig.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/12/15 15:35:34
 *🏷️----------------------------------------------------------------*/

using System.IO;
using System.Windows.Input;
using HtKit;
using Newtonsoft.Json;

namespace MMClipboard.UserConfigs
{
    public class UserConfig
    {
        private static volatile UserConfig defaults;
        private static readonly object lockObj = new();
        private UserConfig()
        { }

        public static UserConfig Default
        {
            get
            {
                if (defaults != null) return defaults;
                lock (lockObj)
                {
                    defaults = new UserConfig();
                }
                return defaults;
            }
        }

        public Config config { get; private set; }

        private static string ConfigFilePath
        {
            get
            {
                var dp = Path.Combine(AppPath.GetBaseDirectory(), @"Config");
                if (!Directory.Exists(dp))
                    Directory.CreateDirectory(dp);
                
                var fp = Path.Combine(dp, "Config.mm");
                if (File.Exists(fp)) return fp;
                File.Create(fp).Close();
                var json = JsonConvert.SerializeObject(new Config()
                {
                    modifierKeys = ModifierKeys.Alt,
                    key = Key.C
                });
                File.WriteAllText(fp, json);
                return fp;
            }
        }

        /// <summary>
        /// 更新用户配置
        /// </summary>
        public static void SaveConfig()
        {
            var json = JsonConvert.SerializeObject(Default.config);
            File.WriteAllText(ConfigFilePath, json);
        }

        /// <summary>
        /// 读取用户配置
        /// </summary>
        /// <returns></returns>
        public static void ReadConfig()
        {
            Default.config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFilePath));
        }
    }
}