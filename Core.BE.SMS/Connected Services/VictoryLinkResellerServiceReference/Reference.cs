﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VictoryLinkResellerServiceReference
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="VictoryLinkResellerServiceReference.SMSSenderSoap")]
    public interface SMSSenderSoap
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SendSMS", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<int> SendSMSAsync(string UserName, string Password, string SMSText, string SMSLang, string SMSSender, string SMSReceiver, string SMSID, string CampaignID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SendSMSWithDLR", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<int> SendSMSWithDLRAsync(string UserName, string Password, string SMSText, string SMSLang, string SMSSender, string SMSReceiver, string SMSID, string CampaignID, string DLRURL);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SendSMSWithTemplate", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<int> SendSMSWithTemplateAsync(string UserName, string Password, string SMSLang, string SMSSender, string SMSReceiver, string SMSID, string CampaignID, string Template, VictoryLinkResellerServiceReference.Item[] Data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SendSMSWithValidity", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<int> SendSMSWithValidityAsync(string UserName, string Password, string SMSText, string SMSLang, string SMSSender, string SMSReceiver, string SMSID, string CampaignID, string Validity, string StartTime);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SendToMany", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<int> SendToManyAsync(string[] Anis, string UserName, string Password, string SMSText, string SMSLang, string SMSSender, string CampaignID, string WithDLR, string Validty);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SendOfflineSMS", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<int> SendOfflineSMSAsync(string UserName, int AccountID, string Password, string SMSText, string SMSLang, string SMSSender, string SMSReceiver, string SMSID, string CampaignID, string WithDLR, string Validty);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckCredit", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<int> CheckCreditAsync(string userName, string password);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class Item
    {
        
        private string valueField;
        
        private string keyField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string key
        {
            get
            {
                return this.keyField;
            }
            set
            {
                this.keyField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    public interface SMSSenderSoapChannel : VictoryLinkResellerServiceReference.SMSSenderSoap, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    public partial class SMSSenderSoapClient : System.ServiceModel.ClientBase<VictoryLinkResellerServiceReference.SMSSenderSoap>, VictoryLinkResellerServiceReference.SMSSenderSoap
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public SMSSenderSoapClient(EndpointConfiguration endpointConfiguration) : 
                base(SMSSenderSoapClient.GetBindingForEndpoint(endpointConfiguration), SMSSenderSoapClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public SMSSenderSoapClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(SMSSenderSoapClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public SMSSenderSoapClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(SMSSenderSoapClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public SMSSenderSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.Threading.Tasks.Task<int> SendSMSAsync(string UserName, string Password, string SMSText, string SMSLang, string SMSSender, string SMSReceiver, string SMSID, string CampaignID)
        {
            return base.Channel.SendSMSAsync(UserName, Password, SMSText, SMSLang, SMSSender, SMSReceiver, SMSID, CampaignID);
        }
        
        public System.Threading.Tasks.Task<int> SendSMSWithDLRAsync(string UserName, string Password, string SMSText, string SMSLang, string SMSSender, string SMSReceiver, string SMSID, string CampaignID, string DLRURL)
        {
            return base.Channel.SendSMSWithDLRAsync(UserName, Password, SMSText, SMSLang, SMSSender, SMSReceiver, SMSID, CampaignID, DLRURL);
        }
        
        public System.Threading.Tasks.Task<int> SendSMSWithTemplateAsync(string UserName, string Password, string SMSLang, string SMSSender, string SMSReceiver, string SMSID, string CampaignID, string Template, VictoryLinkResellerServiceReference.Item[] Data)
        {
            return base.Channel.SendSMSWithTemplateAsync(UserName, Password, SMSLang, SMSSender, SMSReceiver, SMSID, CampaignID, Template, Data);
        }
        
        public System.Threading.Tasks.Task<int> SendSMSWithValidityAsync(string UserName, string Password, string SMSText, string SMSLang, string SMSSender, string SMSReceiver, string SMSID, string CampaignID, string Validity, string StartTime)
        {
            return base.Channel.SendSMSWithValidityAsync(UserName, Password, SMSText, SMSLang, SMSSender, SMSReceiver, SMSID, CampaignID, Validity, StartTime);
        }
        
        public System.Threading.Tasks.Task<int> SendToManyAsync(string[] Anis, string UserName, string Password, string SMSText, string SMSLang, string SMSSender, string CampaignID, string WithDLR, string Validty)
        {
            return base.Channel.SendToManyAsync(Anis, UserName, Password, SMSText, SMSLang, SMSSender, CampaignID, WithDLR, Validty);
        }
        
        public System.Threading.Tasks.Task<int> SendOfflineSMSAsync(string UserName, int AccountID, string Password, string SMSText, string SMSLang, string SMSSender, string SMSReceiver, string SMSID, string CampaignID, string WithDLR, string Validty)
        {
            return base.Channel.SendOfflineSMSAsync(UserName, AccountID, Password, SMSText, SMSLang, SMSSender, SMSReceiver, SMSID, CampaignID, WithDLR, Validty);
        }
        
        public System.Threading.Tasks.Task<int> CheckCreditAsync(string userName, string password)
        {
            return base.Channel.CheckCreditAsync(userName, password);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.SMSSenderSoap))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                result.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.SMSSenderSoap12))
            {
                System.ServiceModel.Channels.CustomBinding result = new System.ServiceModel.Channels.CustomBinding();
                System.ServiceModel.Channels.TextMessageEncodingBindingElement textBindingElement = new System.ServiceModel.Channels.TextMessageEncodingBindingElement();
                textBindingElement.MessageVersion = System.ServiceModel.Channels.MessageVersion.CreateVersion(System.ServiceModel.EnvelopeVersion.Soap12, System.ServiceModel.Channels.AddressingVersion.None);
                result.Elements.Add(textBindingElement);
                System.ServiceModel.Channels.HttpsTransportBindingElement httpsBindingElement = new System.ServiceModel.Channels.HttpsTransportBindingElement();
                httpsBindingElement.AllowCookies = true;
                httpsBindingElement.MaxBufferSize = int.MaxValue;
                httpsBindingElement.MaxReceivedMessageSize = int.MaxValue;
                result.Elements.Add(httpsBindingElement);
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.SMSSenderSoap))
            {
                return new System.ServiceModel.EndpointAddress("https://smsvas.vlserv.com/VLSMSPlatformResellerAPI/SendSMSService/SMSSender.asmx");
            }
            if ((endpointConfiguration == EndpointConfiguration.SMSSenderSoap12))
            {
                return new System.ServiceModel.EndpointAddress("https://smsvas.vlserv.com/VLSMSPlatformResellerAPI/SendSMSService/SMSSender.asmx");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        public enum EndpointConfiguration
        {
            
            SMSSenderSoap,
            
            SMSSenderSoap12,
        }
    }
}
