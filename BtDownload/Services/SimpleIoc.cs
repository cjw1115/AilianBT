using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtDownload.Services
{
    public class SimpleIoc
    {
        private static Dictionary<Type, object> _dic = new Dictionary<Type, object>();
        private static object locker = new object();
        public static void Register<T>() where T : new()
        {
            var instance = _dic.Where(m => m.Key.GetType() == typeof(T)).Select(m => m.Value).FirstOrDefault();
            if (instance == null)
            {
                lock (locker)
                {
                    instance = new T();
                    _dic.Add(typeof(T), instance);
                }
            }
        }
        public static T GetInstance<T>()where T:class
        {
            var instance = _dic.Where(m => m.Key.Equals(typeof(T))).Select(m => m.Value).FirstOrDefault();
            if (instance != null)
            {
                return (T)instance;
            }
            return null;
        }
    }
}
