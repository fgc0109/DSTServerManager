using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoNotStarveManager.Saves
{
    class LicenseManager
    {
        /// <summary>
        /// 读取令牌文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool ReadLicense(string path, out string license)
        {
            license = string.Empty;
            return true;
        }

        /// <summary>
        /// 写入令牌文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool WriteLicense(string path, string license)
        {
            return true;
        }
    }
}
