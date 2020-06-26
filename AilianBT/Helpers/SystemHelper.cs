using System.Security.Cryptography;
using System.Text;

namespace AilianBT.Helpers
{
    public class UtilityHelper
    {
        private MD5 _md5 = MD5.Create();
        private StringBuilder _md5StrBudilder = new StringBuilder(32);

        public string CreateMd5HashString(string content)
        {
            _md5StrBudilder.Clear();
            byte[] stringbuffer = Encoding.UTF8.GetBytes(content);
            byte[] hashBuffer = _md5.ComputeHash(stringbuffer);

            for (int i = 0; i < hashBuffer.Length; i++)
            {
                _md5StrBudilder.Append(hashBuffer[i].ToString("x2"));
            }
            return _md5StrBudilder.ToString();
        }
    }
}
