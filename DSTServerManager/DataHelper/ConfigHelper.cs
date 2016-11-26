using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DSTServerManager.DataHelper
{
    class ConfigHelper
    {
        /// <summary>   
        /// 写入值   
        /// </summary>   
        /// <param name="key"></param>   
        /// <param name="value"></param>   
        public static void SetValue(string key, string value)
        {
            //增加的内容写在appSettings段下 <add key="RegCode" value="0"/>   
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] == null)
            {
                config.AppSettings.Settings.Add(key, value);
            }
            else
            {
                config.AppSettings.Settings[key].Value = value;
            }
            config.Save(ConfigurationSaveMode.Modified);

            //重新加载新的配置文件
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>   
        /// 读取指定Key的值
        /// </summary>   
        /// <param name="key"></param>   
        /// <returns></returns>   
        public static string GetValue(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] == null)
                return "";
            else
                return config.AppSettings.Settings[key].Value;
        }

        /// <summary>
        /// 读取指定Key的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetIntValue(string key)
        {
            int keyValue = -1;
            int.TryParse(GetValue(key), out keyValue);
            return keyValue;
        }

        /// <summary>
        /// 读取指定Key的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetBoolValue(string key)
        {
            bool keyValue = false;
            bool.TryParse(GetValue(key), out keyValue);
            return keyValue;
        }
    }
}
