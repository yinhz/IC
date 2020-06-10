﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IC.WCF.ConsoleHost.PM.ICWcfService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="MessageRequest", Namespace="http://schemas.datacontract.org/2004/07/IC.Core")]
    [System.SerializableAttribute()]
    public partial class MessageRequest : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string CommandIdField;
        
        private string CommandRequestJsonField;
        
        private System.Guid MessageGuidField;
        
        private System.DateTime RequestDateField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string CommandId {
            get {
                return this.CommandIdField;
            }
            set {
                if ((object.ReferenceEquals(this.CommandIdField, value) != true)) {
                    this.CommandIdField = value;
                    this.RaisePropertyChanged("CommandId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string CommandRequestJson {
            get {
                return this.CommandRequestJsonField;
            }
            set {
                if ((object.ReferenceEquals(this.CommandRequestJsonField, value) != true)) {
                    this.CommandRequestJsonField = value;
                    this.RaisePropertyChanged("CommandRequestJson");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public System.Guid MessageGuid {
            get {
                return this.MessageGuidField;
            }
            set {
                if ((this.MessageGuidField.Equals(value) != true)) {
                    this.MessageGuidField = value;
                    this.RaisePropertyChanged("MessageGuid");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public System.DateTime RequestDate {
            get {
                return this.RequestDateField;
            }
            set {
                if ((this.RequestDateField.Equals(value) != true)) {
                    this.RequestDateField = value;
                    this.RaisePropertyChanged("RequestDate");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="MessageResponse", Namespace="http://schemas.datacontract.org/2004/07/IC.Core")]
    [System.SerializableAttribute()]
    public partial class MessageResponse : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string CommandResponseJsonField;
        
        private System.Guid MessageGuidField;
        
        private System.DateTime ResponseDateField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string CommandResponseJson {
            get {
                return this.CommandResponseJsonField;
            }
            set {
                if ((object.ReferenceEquals(this.CommandResponseJsonField, value) != true)) {
                    this.CommandResponseJsonField = value;
                    this.RaisePropertyChanged("CommandResponseJson");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public System.Guid MessageGuid {
            get {
                return this.MessageGuidField;
            }
            set {
                if ((this.MessageGuidField.Equals(value) != true)) {
                    this.MessageGuidField = value;
                    this.RaisePropertyChanged("MessageGuid");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public System.DateTime ResponseDate {
            get {
                return this.ResponseDateField;
            }
            set {
                if ((this.ResponseDateField.Equals(value) != true)) {
                    this.ResponseDateField = value;
                    this.RaisePropertyChanged("ResponseDate");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ICWcfService._ICWcfService", CallbackContract=typeof(IC.WCF.ConsoleHost.PM.ICWcfService._ICWcfServiceCallback), SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface _ICWcfService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/_ICWcfService/SendMessage", ReplyAction="http://tempuri.org/_ICWcfService/SendMessageResponse")]
        IC.WCF.ConsoleHost.PM.ICWcfService.MessageResponse SendMessage(IC.WCF.ConsoleHost.PM.ICWcfService.MessageRequest messageRequest);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/_ICWcfService/SendMessage", ReplyAction="http://tempuri.org/_ICWcfService/SendMessageResponse")]
        System.Threading.Tasks.Task<IC.WCF.ConsoleHost.PM.ICWcfService.MessageResponse> SendMessageAsync(IC.WCF.ConsoleHost.PM.ICWcfService.MessageRequest messageRequest);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/_ICWcfService/RegisterClient", ReplyAction="http://tempuri.org/_ICWcfService/RegisterClientResponse")]
        void RegisterClient(string clientId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/_ICWcfService/RegisterClient", ReplyAction="http://tempuri.org/_ICWcfService/RegisterClientResponse")]
        System.Threading.Tasks.Task RegisterClientAsync(string clientId);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface _ICWcfServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/_ICWcfService/SendMessageToClient", ReplyAction="http://tempuri.org/_ICWcfService/SendMessageToClientResponse")]
        IC.WCF.ConsoleHost.PM.ICWcfService.MessageResponse SendMessageToClient(IC.WCF.ConsoleHost.PM.ICWcfService.MessageRequest messageRequest);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface _ICWcfServiceChannel : IC.WCF.ConsoleHost.PM.ICWcfService._ICWcfService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class _ICWcfServiceClient : System.ServiceModel.DuplexClientBase<IC.WCF.ConsoleHost.PM.ICWcfService._ICWcfService>, IC.WCF.ConsoleHost.PM.ICWcfService._ICWcfService {
        
        public _ICWcfServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public _ICWcfServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public _ICWcfServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public _ICWcfServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public _ICWcfServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public IC.WCF.ConsoleHost.PM.ICWcfService.MessageResponse SendMessage(IC.WCF.ConsoleHost.PM.ICWcfService.MessageRequest messageRequest) {
            return base.Channel.SendMessage(messageRequest);
        }
        
        public System.Threading.Tasks.Task<IC.WCF.ConsoleHost.PM.ICWcfService.MessageResponse> SendMessageAsync(IC.WCF.ConsoleHost.PM.ICWcfService.MessageRequest messageRequest) {
            return base.Channel.SendMessageAsync(messageRequest);
        }
        
        public void RegisterClient(string clientId) {
            base.Channel.RegisterClient(clientId);
        }
        
        public System.Threading.Tasks.Task RegisterClientAsync(string clientId) {
            return base.Channel.RegisterClientAsync(clientId);
        }
    }
}
