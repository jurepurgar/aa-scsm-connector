using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PurgarNET.AAConnector.Shared
{
    public static class Serializer
    {

        public static string ToJson(object obj)
        {
            string res = null;
            var t = obj.GetType();
            var serializer = new DataContractJsonSerializer(t);
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                res = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
            }
            return res;
        }

    }

    [Serializable]
    public class ConnectedObject : ISerializable
    {
        private Dictionary<string, object> _store = new Dictionary<string, object>();

        public bool ContainsKey(string key)
        {
            return _store.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            _store.Add(key, value);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var i in _store)
                info.AddValue(i.Key, i.Value);
        }
    }
}
