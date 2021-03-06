﻿using IC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace IC.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(CallbackContract = typeof(IICWcfCallBackService), SessionMode = SessionMode.Required)]
    public interface _ICWcfService : __ICWcfService
    {
    }

    [ServiceContract]
    public interface __ICWcfService
    {
        [OperationContract]
        MessageResponse SendMessage(MessageRequest messageRequest);

        [OperationContract]
        void RegisterClient(string clientId);
    }

    [ServiceContract]
    public interface IICWcfCallBackService
    {
        [OperationContract]
        MessageResponse SendMessageToClient(MessageRequest messageRequest);
    }
}
