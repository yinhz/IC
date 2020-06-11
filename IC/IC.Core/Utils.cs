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
        public static byte[] StructToBytes<T>(T structObj)
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
    }
}
