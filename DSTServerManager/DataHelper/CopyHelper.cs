using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DSTServerManager.DataHelper
{
    static class  CopyHelper
    {
        /// <summary>
        /// 拷贝所有属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void CopyAllProperties<T>(this object source, T target) where T : class
        {
            if (source == null || target == null)
                return;

            var properties = target.GetType().GetProperties();
            foreach (var targetPro in properties)
            {
                try
                {
                    //判断源对象是否存在与目标属性名字对应的源属性  
                    if (source.GetType().GetProperty(targetPro.Name) == null)
                    {
                        continue;
                    }
                    //数据类型不相等  
                    if (targetPro.PropertyType.FullName != source.GetType().GetProperty(targetPro.Name).PropertyType.FullName)
                    {
                        continue;
                    }
                    var propertyValue = source.GetType().GetProperty(targetPro.Name).GetValue(source, null);
                    if (propertyValue != null)
                    {
                        target.GetType().InvokeMember(targetPro.Name, BindingFlags.SetProperty, null, target, new object[] { propertyValue });
                    }
                }
                catch { }
            }
        }
    }
}
