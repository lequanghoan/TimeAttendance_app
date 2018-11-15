using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Caching
{
    public interface IRedisObjectSerializer
    {
        byte[] Serialize(object item);
        Task<byte[]> SerializeAsync(object item);
        object Deserialize(byte[] serializedObject);
        Task<object> DeserializeAsync(byte[] serializedObject);
        T Deserialize<T>(byte[] serializedObject);
        Task<T> DeserializeAsync<T>(byte[] serializedObject);
    }
}
