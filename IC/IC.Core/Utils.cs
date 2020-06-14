using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace IC.Core
{
    public static class Utils
    {
        #region Bytes <> String
        public static byte[] ToBytes(this string str, System.Text.Encoding encoding) => encoding.GetBytes(str);
        public static byte[] ToBytes(this string str) => str.JsonToBytes(System.Text.Encoding.UTF8);
        public static string BytesToString(this byte[] bytes, System.Text.Encoding encoding) => encoding.GetString(bytes);
        public static string BytesToString(this byte[] bytes) => bytes.ToJson(System.Text.Encoding.UTF8);
        public static string BytesToString(this IEnumerable<byte> bytes, System.Text.Encoding encoding) => bytes.ToArray().BytesToString();
        public static string BytesToString(this IEnumerable<byte> bytes) => bytes.BytesToString(System.Text.Encoding.UTF8);
        #endregion

        #region Object <> Json

        public static string ToJson<T>(this T obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public static T JsonToObject<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        #endregion

        #region Json <> Bytes

        public static byte[] JsonToBytes(this string str, System.Text.Encoding encoding) => encoding.GetBytes(str);
        public static byte[] JsonToBytes(this string str) => str.JsonToBytes(System.Text.Encoding.UTF8);

        public static string ToJson(this byte[] bytes, System.Text.Encoding encoding) => encoding.GetString(bytes);
        public static string ToJson(this byte[] bytes) => bytes.ToJson(System.Text.Encoding.UTF8);

        #endregion

        #region Struct <> Bytes

        public static byte[] StructToBytes<T>(this T structObj)
            where T : struct
        {
            int size = Marshal.SizeOf(structObj);

            byte[] bytes = new byte[size];

            IntPtr structPtr = Marshal.AllocHGlobal(size);

            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);

            //从内存空间拷贝到byte 数组
            Marshal.Copy(structPtr, bytes, 0, size);

            //释放内存空间 
            Marshal.FreeHGlobal(structPtr);

            return bytes;
        }

        public static T BytesToStruct<T>(byte[] bytes)
            where T : struct
        {
            var structType = typeof(T);

            int size = Marshal.SizeOf(structType);

            if (size > bytes.Length)
            {
                throw new Exception("Bytes to struct exception. Struct Size bigger than bytes argument!");
            }

            //分配结构体内存空间 
            IntPtr structPtr = Marshal.AllocHGlobal(size);

            //将byte数组拷贝到分配好的内存空间 
            Marshal.Copy(bytes, 0, structPtr, size);

            //将内存空间转换为目标结构体 
            object obj = Marshal.PtrToStructure(structPtr, structType);

            //释放内存空间 
            Marshal.FreeHGlobal(structPtr);

            if (obj == null)
                throw new Exception("Bytes to struct. result is null!");

            return (T)obj;
        }

        #endregion

        #region Object ([Serializable]) <> Binary

        public static byte[] SerializeToBinaryFormatter(this object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            byte[] bytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    new BinaryFormatter().Serialize(gZipStream, obj);
                }
                bytes = memoryStream.ToArray();
            }
            return bytes;
        }

        public static object DeserializeToObject(this byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                return DeserializeToObject(memoryStream);
            }
        }

        public static T DeserializeToObject<T>(this byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                var obj = DeserializeToObject(memoryStream);

                if (obj == null)
                    throw new System.Runtime.Serialization.SerializationException();

                return (T)obj;
            }
        }

        public static object DeserializeToObject(MemoryStream memoryStream)
        {
            memoryStream.Position = 0;
            using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                return new BinaryFormatter().Deserialize(gZipStream);
            }
        }

        #endregion
    }
}