using System;
using System.Runtime.Serialization;

namespace IC.Core
{
    [DataContract]
    public class MessageRequest
    {
        [DataMember(IsRequired = true)]
        public Guid MessageGuid { get; set; }
        [DataMember(IsRequired = true)]
        public DateTime RequestDate { get; set; }
        [DataMember(IsRequired = true)]
        public string CommandRequestJson { get; set; }
    }
    
    [DataContract]
    public class MessageResponse
    {
        [DataMember(IsRequired = true)]
        public Guid MessageGuid { get; set; }
        [DataMember(IsRequired = true)]
        public DateTime ResponseDate { get; set; }
        [DataMember(IsRequired = true)]
        public string CommandResponseJson { get; set; }
    }

    public static class MessageUtils
    {
        public static string ToJson(this ICommand command)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(command);
        }
        public static RequestCommand GetRequestCommand(this MessageRequest messageRequest)
        {
            if (messageRequest == null)
            {
                throw new ArgumentNullException("messageRequest");
            }

            if (string.IsNullOrEmpty(messageRequest.CommandRequestJson))
            {
                throw new ArgumentNullException("messageRequest.JsonContent");
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<RequestCommand>(messageRequest.CommandRequestJson);
        }
        public static ICommand ToCommand(this string jsonCommand)
        {
            if (string.IsNullOrEmpty(jsonCommand))
            {
                throw new ArgumentNullException("jsonCommand");
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<ICommand>(jsonCommand);
        }
    }
}