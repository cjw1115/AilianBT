using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace AilianBT.Common.Helpers
{
    public class JsonHelper
    {
        public static string SerializeObject<T>(T source)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, source);
                byte[] buffer = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(buffer, 0, buffer.Length);
                var str=Encoding.UTF8.GetString(buffer);
                return str;
            }
        }


        public static T  DerializeObjec<T>(string source)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

            var buffer = Encoding.UTF8.GetBytes(source);
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(buffer, 0, buffer.Length);
                stream.Seek(0, SeekOrigin.Begin);
                var re=serializer.ReadObject(stream);
                return (T)re;
            }
        }
    }
}
