using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;

namespace AilianBT.DAL
{
    public class LocalSetting
    {
        private StorageFolder _localFolder;
        public LocalSetting()
        {
            _localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        }
        public async Task<T> GetLocalInfo<T>(string fileName) where T : class
        {
            T entity = null;
            try
            {
                var item = await _localFolder.TryGetItemAsync(fileName);
                if (item == null)
                {
                    return entity;
                }
                var file = item as StorageFile;
                using (var ras = await file.OpenAsync(FileAccessMode.Read))
                {
                    using (var stream = ras.AsStream())
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                        var o = serializer.ReadObject(stream);
                        if (o != null)
                        {
                            entity = o as T;
                        }
                    }
                }
            }
            catch (NullReferenceException nullref)
            {
                return null;
            }
            catch (SerializationException ser)
            {
                return null;
            }
            catch (XmlException e)
            {
                return null;
            }
            return entity;
        }
        public async Task SetLocalInfo<T>(string fileName, T entity)
            where T : class
        {

            var item = await _localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            var file = item as StorageFile;
            try
            {
                using (var ras = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (var stream = ras.AsStream())
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                        serializer.WriteObject(stream, entity);
                    }
                }
            }
            catch (NullReferenceException nullref)
            {
                throw;
            }
            catch (SerializationException ser)
            {
                throw;
            }
            catch (XmlException e)
            {
                throw;
            }

        }


    }
}
