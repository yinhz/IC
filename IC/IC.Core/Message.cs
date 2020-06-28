using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace IC.Core
{
    public enum MessageType : byte
    {
        Request = 1,
        Response = 2
    }
    public enum MessageFormat : byte
    {
        Binary = 1,
        Json = 2
    }

    [Serializable]
    [DataContract]
    public class MessageRequest
    {
        public MessageRequest()
        {
            this.MessageGuid = System.Guid.NewGuid();
            this.RequestDate = DateTime.UtcNow;
        }
        [DataMember(IsRequired = true)]
        public Guid MessageGuid { get; set; }
        [DataMember(IsRequired = true)]
        public DateTime RequestDate { get; set; }
        [DataMember(IsRequired = true)]
        public string CommandId { get; set; }
        [DataMember(IsRequired = true)]
        public string CommandRequestJson { get; set; }
    }

    [Serializable]
    [DataContract]
    public class MessageResponse
    {
        public MessageResponse()
        {
            this.MessageGuid = System.Guid.NewGuid();
            this.ResponseDate = System.DateTime.Now;
        }
        [DataMember(IsRequired = true)]
        public Guid MessageGuid { get; set; }
        [DataMember(IsRequired = true)]
        public string CommandId { get; set; }
        [DataMember(IsRequired = true)]
        public bool Success { get; set; }
        [DataMember(IsRequired = true)]
        public string ErrorCode { get; set; }
        [DataMember(IsRequired = true)]
        public string ErrorMessage { get; set; }
        [DataMember(IsRequired = true)]
        public DateTime ResponseDate { get; set; }
        [DataMember(IsRequired = true)]
        public string CommandResponseJson { get; set; }
    }

    public static class MessageUtils
    {
        public static string ToJson(this MessageRequest messageRequest)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(messageRequest);
        }
        public static MessageRequest FromMessageRequestJson(string messageRequestJson)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<MessageRequest>(messageRequestJson);
        }
        public static string ToJson(this MessageResponse messageResponse)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(messageResponse);
        }
        public static MessageResponse FromMessageResponseJson(string messageResponseJson)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<MessageResponse>(messageResponseJson);
        }

        public static byte[] ToBinaryFormatter(this MessageRequest messageRequest)
        {
            return messageRequest.SerializeToBinaryFormatter();
        }

        public static T FromBinaryFormatter<T>(byte[] bytes)
            where T : class
        {
            var obj = Utils.DeserializeToObject(bytes);

            if (obj == null || !(obj is T))
                throw new SerializationException();

            return (T)obj;
        }
    }
}