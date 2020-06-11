using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace IC.Core
{
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

    [DataContract]
    public class MessageResponse
    {
        public MessageResponse()
        {
            this.MessageGuid = System.Guid.NewGuid();
            this.ResponseDate = System.DateTime.Now;
        }
        [DataMember(IsRequired = true)]
        public bool Success { get; set; }
        [DataMember(IsRequired = true)]
        public string ErrorCode { get; set; }
        [DataMember(IsRequired = true)]
        public string ErrorMessage { get; set; }
        [DataMember(IsRequired = true)]
        public Guid MessageGuid { get; set; }
        [DataMember(IsRequired = true)]
        public DateTime ResponseDate { get; set; }
        [DataMember(IsRequired = true)]
        public string CommandResponseJson { get; set; }
    }

    public static class MessageUtils
    {
        public static byte[] ToBinaryFormatter(this MessageRequest messageRequest)
        {
            return messageRequest.SerializeToBinaryFormatter();
        }

        public static MessageRequest FromBinaryFormatter(byte[] bytes)
        {
            var obj = Utils.DeserializeToObject(bytes);

            if (obj == null || !(obj is MessageRequest))
                throw new SerializationException();

            return obj as MessageRequest;
        }
    }
}