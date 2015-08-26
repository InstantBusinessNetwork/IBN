﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.17929.
// 
#pragma warning disable 1591

namespace OutlookAddin.SyncService {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="SynchronizationServiceSoap", Namespace="http://tempuri.org/")]
    public partial class SynchronizationService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback SetCredentialsOperationCompleted;
        
        private System.Threading.SendOrPostCallback SetProviderTypeForSyncSessionOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetIdFormatsOperationCompleted;
        
        private System.Threading.SendOrPostCallback BeginSessionOperationCompleted;
        
        private System.Threading.SendOrPostCallback EndSessionOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetChangeBatchOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetFullEnumerationChangeBatchOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetSyncBatchParametersOperationCompleted;
        
        private System.Threading.SendOrPostCallback ProcessChangeBatchOperationCompleted;
        
        private System.Threading.SendOrPostCallback ProcessFullEnumerationChangeBatchOperationCompleted;
        
        private System.Threading.SendOrPostCallback CleanupTombstonesOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public SynchronizationService() {
            this.Url = global::OutlookAddin.Properties.Settings.Default.OutlookAddin_SyncService_SynchronizationService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event SetCredentialsCompletedEventHandler SetCredentialsCompleted;
        
        /// <remarks/>
        public event SetProviderTypeForSyncSessionCompletedEventHandler SetProviderTypeForSyncSessionCompleted;
        
        /// <remarks/>
        public event GetIdFormatsCompletedEventHandler GetIdFormatsCompleted;
        
        /// <remarks/>
        public event BeginSessionCompletedEventHandler BeginSessionCompleted;
        
        /// <remarks/>
        public event EndSessionCompletedEventHandler EndSessionCompleted;
        
        /// <remarks/>
        public event GetChangeBatchCompletedEventHandler GetChangeBatchCompleted;
        
        /// <remarks/>
        public event GetFullEnumerationChangeBatchCompletedEventHandler GetFullEnumerationChangeBatchCompleted;
        
        /// <remarks/>
        public event GetSyncBatchParametersCompletedEventHandler GetSyncBatchParametersCompleted;
        
        /// <remarks/>
        public event ProcessChangeBatchCompletedEventHandler ProcessChangeBatchCompleted;
        
        /// <remarks/>
        public event ProcessFullEnumerationChangeBatchCompletedEventHandler ProcessFullEnumerationChangeBatchCompleted;
        
        /// <remarks/>
        public event CleanupTombstonesCompletedEventHandler CleanupTombstonesCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SetCredentials", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void SetCredentials(NetworkCredential credentials) {
            this.Invoke("SetCredentials", new object[] {
                        credentials});
        }
        
        /// <remarks/>
        public void SetCredentialsAsync(NetworkCredential credentials) {
            this.SetCredentialsAsync(credentials, null);
        }
        
        /// <remarks/>
        public void SetCredentialsAsync(NetworkCredential credentials, object userState) {
            if ((this.SetCredentialsOperationCompleted == null)) {
                this.SetCredentialsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSetCredentialsOperationCompleted);
            }
            this.InvokeAsync("SetCredentials", new object[] {
                        credentials}, this.SetCredentialsOperationCompleted, userState);
        }
        
        private void OnSetCredentialsOperationCompleted(object arg) {
            if ((this.SetCredentialsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SetCredentialsCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SetProviderTypeForSyncSession", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void SetProviderTypeForSyncSession(eSyncProviderType providerType) {
            this.Invoke("SetProviderTypeForSyncSession", new object[] {
                        providerType});
        }
        
        /// <remarks/>
        public void SetProviderTypeForSyncSessionAsync(eSyncProviderType providerType) {
            this.SetProviderTypeForSyncSessionAsync(providerType, null);
        }
        
        /// <remarks/>
        public void SetProviderTypeForSyncSessionAsync(eSyncProviderType providerType, object userState) {
            if ((this.SetProviderTypeForSyncSessionOperationCompleted == null)) {
                this.SetProviderTypeForSyncSessionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSetProviderTypeForSyncSessionOperationCompleted);
            }
            this.InvokeAsync("SetProviderTypeForSyncSession", new object[] {
                        providerType}, this.SetProviderTypeForSyncSessionOperationCompleted, userState);
        }
        
        private void OnSetProviderTypeForSyncSessionOperationCompleted(object arg) {
            if ((this.SetProviderTypeForSyncSessionCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SetProviderTypeForSyncSessionCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetIdFormats", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] GetIdFormats() {
            object[] results = this.Invoke("GetIdFormats", new object[0]);
            return ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public void GetIdFormatsAsync() {
            this.GetIdFormatsAsync(null);
        }
        
        /// <remarks/>
        public void GetIdFormatsAsync(object userState) {
            if ((this.GetIdFormatsOperationCompleted == null)) {
                this.GetIdFormatsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetIdFormatsOperationCompleted);
            }
            this.InvokeAsync("GetIdFormats", new object[0], this.GetIdFormatsOperationCompleted, userState);
        }
        
        private void OnGetIdFormatsOperationCompleted(object arg) {
            if ((this.GetIdFormatsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetIdFormatsCompleted(this, new GetIdFormatsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/BeginSession", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void BeginSession() {
            this.Invoke("BeginSession", new object[0]);
        }
        
        /// <remarks/>
        public void BeginSessionAsync() {
            this.BeginSessionAsync(null);
        }
        
        /// <remarks/>
        public void BeginSessionAsync(object userState) {
            if ((this.BeginSessionOperationCompleted == null)) {
                this.BeginSessionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnBeginSessionOperationCompleted);
            }
            this.InvokeAsync("BeginSession", new object[0], this.BeginSessionOperationCompleted, userState);
        }
        
        private void OnBeginSessionOperationCompleted(object arg) {
            if ((this.BeginSessionCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.BeginSessionCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/EndSession", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void EndSession() {
            this.Invoke("EndSession", new object[0]);
        }
        
        /// <remarks/>
        public void EndSessionAsync() {
            this.EndSessionAsync(null);
        }
        
        /// <remarks/>
        public void EndSessionAsync(object userState) {
            if ((this.EndSessionOperationCompleted == null)) {
                this.EndSessionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnEndSessionOperationCompleted);
            }
            this.InvokeAsync("EndSession", new object[0], this.EndSessionOperationCompleted, userState);
        }
        
        private void OnEndSessionOperationCompleted(object arg) {
            if ((this.EndSessionCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.EndSessionCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetChangeBatch", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] GetChangeBatch(uint batchSize, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] rawDestinationKnowledge, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] out byte[] changeDataRetriever) {
            object[] results = this.Invoke("GetChangeBatch", new object[] {
                        batchSize,
                        rawDestinationKnowledge});
            changeDataRetriever = ((byte[])(results[1]));
            return ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public void GetChangeBatchAsync(uint batchSize, byte[] rawDestinationKnowledge) {
            this.GetChangeBatchAsync(batchSize, rawDestinationKnowledge, null);
        }
        
        /// <remarks/>
        public void GetChangeBatchAsync(uint batchSize, byte[] rawDestinationKnowledge, object userState) {
            if ((this.GetChangeBatchOperationCompleted == null)) {
                this.GetChangeBatchOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetChangeBatchOperationCompleted);
            }
            this.InvokeAsync("GetChangeBatch", new object[] {
                        batchSize,
                        rawDestinationKnowledge}, this.GetChangeBatchOperationCompleted, userState);
        }
        
        private void OnGetChangeBatchOperationCompleted(object arg) {
            if ((this.GetChangeBatchCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetChangeBatchCompleted(this, new GetChangeBatchCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetFullEnumerationChangeBatch", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] GetFullEnumerationChangeBatch(uint batchSize, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] lowerEnumerationBoundRaw, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] rawKnowledgeForDataRetrieval, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] out byte[] changeDataRetriever) {
            object[] results = this.Invoke("GetFullEnumerationChangeBatch", new object[] {
                        batchSize,
                        lowerEnumerationBoundRaw,
                        rawKnowledgeForDataRetrieval});
            changeDataRetriever = ((byte[])(results[1]));
            return ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public void GetFullEnumerationChangeBatchAsync(uint batchSize, byte[] lowerEnumerationBoundRaw, byte[] rawKnowledgeForDataRetrieval) {
            this.GetFullEnumerationChangeBatchAsync(batchSize, lowerEnumerationBoundRaw, rawKnowledgeForDataRetrieval, null);
        }
        
        /// <remarks/>
        public void GetFullEnumerationChangeBatchAsync(uint batchSize, byte[] lowerEnumerationBoundRaw, byte[] rawKnowledgeForDataRetrieval, object userState) {
            if ((this.GetFullEnumerationChangeBatchOperationCompleted == null)) {
                this.GetFullEnumerationChangeBatchOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetFullEnumerationChangeBatchOperationCompleted);
            }
            this.InvokeAsync("GetFullEnumerationChangeBatch", new object[] {
                        batchSize,
                        lowerEnumerationBoundRaw,
                        rawKnowledgeForDataRetrieval}, this.GetFullEnumerationChangeBatchOperationCompleted, userState);
        }
        
        private void OnGetFullEnumerationChangeBatchOperationCompleted(object arg) {
            if ((this.GetFullEnumerationChangeBatchCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetFullEnumerationChangeBatchCompleted(this, new GetFullEnumerationChangeBatchCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetSyncBatchParameters", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("batchSize")]
        public uint GetSyncBatchParameters([System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] out byte[] rawKnowledge) {
            object[] results = this.Invoke("GetSyncBatchParameters", new object[0]);
            rawKnowledge = ((byte[])(results[1]));
            return ((uint)(results[0]));
        }
        
        /// <remarks/>
        public void GetSyncBatchParametersAsync() {
            this.GetSyncBatchParametersAsync(null);
        }
        
        /// <remarks/>
        public void GetSyncBatchParametersAsync(object userState) {
            if ((this.GetSyncBatchParametersOperationCompleted == null)) {
                this.GetSyncBatchParametersOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetSyncBatchParametersOperationCompleted);
            }
            this.InvokeAsync("GetSyncBatchParameters", new object[0], this.GetSyncBatchParametersOperationCompleted, userState);
        }
        
        private void OnGetSyncBatchParametersOperationCompleted(object arg) {
            if ((this.GetSyncBatchParametersCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetSyncBatchParametersCompleted(this, new GetSyncBatchParametersCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ProcessChangeBatch", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] ProcessChangeBatch(int resolutionPolicy, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] sourceChanges, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] rawChangeDataRetriever, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] changeApplierInfo) {
            object[] results = this.Invoke("ProcessChangeBatch", new object[] {
                        resolutionPolicy,
                        sourceChanges,
                        rawChangeDataRetriever,
                        changeApplierInfo});
            return ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public void ProcessChangeBatchAsync(int resolutionPolicy, byte[] sourceChanges, byte[] rawChangeDataRetriever, byte[] changeApplierInfo) {
            this.ProcessChangeBatchAsync(resolutionPolicy, sourceChanges, rawChangeDataRetriever, changeApplierInfo, null);
        }
        
        /// <remarks/>
        public void ProcessChangeBatchAsync(int resolutionPolicy, byte[] sourceChanges, byte[] rawChangeDataRetriever, byte[] changeApplierInfo, object userState) {
            if ((this.ProcessChangeBatchOperationCompleted == null)) {
                this.ProcessChangeBatchOperationCompleted = new System.Threading.SendOrPostCallback(this.OnProcessChangeBatchOperationCompleted);
            }
            this.InvokeAsync("ProcessChangeBatch", new object[] {
                        resolutionPolicy,
                        sourceChanges,
                        rawChangeDataRetriever,
                        changeApplierInfo}, this.ProcessChangeBatchOperationCompleted, userState);
        }
        
        private void OnProcessChangeBatchOperationCompleted(object arg) {
            if ((this.ProcessChangeBatchCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ProcessChangeBatchCompleted(this, new ProcessChangeBatchCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ProcessFullEnumerationChangeBatch", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] ProcessFullEnumerationChangeBatch(int resolutionPolicy, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] sourceChanges, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] rawChangeDataRetriever, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] changeApplierInfo) {
            object[] results = this.Invoke("ProcessFullEnumerationChangeBatch", new object[] {
                        resolutionPolicy,
                        sourceChanges,
                        rawChangeDataRetriever,
                        changeApplierInfo});
            return ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public void ProcessFullEnumerationChangeBatchAsync(int resolutionPolicy, byte[] sourceChanges, byte[] rawChangeDataRetriever, byte[] changeApplierInfo) {
            this.ProcessFullEnumerationChangeBatchAsync(resolutionPolicy, sourceChanges, rawChangeDataRetriever, changeApplierInfo, null);
        }
        
        /// <remarks/>
        public void ProcessFullEnumerationChangeBatchAsync(int resolutionPolicy, byte[] sourceChanges, byte[] rawChangeDataRetriever, byte[] changeApplierInfo, object userState) {
            if ((this.ProcessFullEnumerationChangeBatchOperationCompleted == null)) {
                this.ProcessFullEnumerationChangeBatchOperationCompleted = new System.Threading.SendOrPostCallback(this.OnProcessFullEnumerationChangeBatchOperationCompleted);
            }
            this.InvokeAsync("ProcessFullEnumerationChangeBatch", new object[] {
                        resolutionPolicy,
                        sourceChanges,
                        rawChangeDataRetriever,
                        changeApplierInfo}, this.ProcessFullEnumerationChangeBatchOperationCompleted, userState);
        }
        
        private void OnProcessFullEnumerationChangeBatchOperationCompleted(object arg) {
            if ((this.ProcessFullEnumerationChangeBatchCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ProcessFullEnumerationChangeBatchCompleted(this, new ProcessFullEnumerationChangeBatchCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/CleanupTombstones", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void CleanupTombstones(TimeSpan timespan) {
            this.Invoke("CleanupTombstones", new object[] {
                        timespan});
        }
        
        /// <remarks/>
        public void CleanupTombstonesAsync(TimeSpan timespan) {
            this.CleanupTombstonesAsync(timespan, null);
        }
        
        /// <remarks/>
        public void CleanupTombstonesAsync(TimeSpan timespan, object userState) {
            if ((this.CleanupTombstonesOperationCompleted == null)) {
                this.CleanupTombstonesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCleanupTombstonesOperationCompleted);
            }
            this.InvokeAsync("CleanupTombstones", new object[] {
                        timespan}, this.CleanupTombstonesOperationCompleted, userState);
        }
        
        private void OnCleanupTombstonesOperationCompleted(object arg) {
            if ((this.CleanupTombstonesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CleanupTombstonesCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class NetworkCredential {
        
        private string userNameField;
        
        private string passwordField;
        
        private string domainField;
        
        /// <remarks/>
        public string UserName {
            get {
                return this.userNameField;
            }
            set {
                this.userNameField = value;
            }
        }
        
        /// <remarks/>
        public string Password {
            get {
                return this.passwordField;
            }
            set {
                this.passwordField = value;
            }
        }
        
        /// <remarks/>
        public string Domain {
            get {
                return this.domainField;
            }
            set {
                this.domainField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class TimeSpan {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public enum eSyncProviderType {
        
        /// <remarks/>
        Mail,
        
        /// <remarks/>
        Appointment,
        
        /// <remarks/>
        Contact,
        
        /// <remarks/>
        Task,
        
        /// <remarks/>
        Journal,
        
        /// <remarks/>
        Note,
        
        /// <remarks/>
        Post,
        
        /// <remarks/>
        Distribution,
        
        /// <remarks/>
        Undef,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void SetCredentialsCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void SetProviderTypeForSyncSessionCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetIdFormatsCompletedEventHandler(object sender, GetIdFormatsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetIdFormatsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetIdFormatsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public byte[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void BeginSessionCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void EndSessionCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetChangeBatchCompletedEventHandler(object sender, GetChangeBatchCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetChangeBatchCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetChangeBatchCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public byte[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
        
        /// <remarks/>
        public byte[] changeDataRetriever {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[1]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetFullEnumerationChangeBatchCompletedEventHandler(object sender, GetFullEnumerationChangeBatchCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetFullEnumerationChangeBatchCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetFullEnumerationChangeBatchCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public byte[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
        
        /// <remarks/>
        public byte[] changeDataRetriever {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[1]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetSyncBatchParametersCompletedEventHandler(object sender, GetSyncBatchParametersCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetSyncBatchParametersCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetSyncBatchParametersCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public uint Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((uint)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public byte[] rawKnowledge {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[1]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void ProcessChangeBatchCompletedEventHandler(object sender, ProcessChangeBatchCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ProcessChangeBatchCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ProcessChangeBatchCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public byte[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void ProcessFullEnumerationChangeBatchCompletedEventHandler(object sender, ProcessFullEnumerationChangeBatchCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ProcessFullEnumerationChangeBatchCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ProcessFullEnumerationChangeBatchCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public byte[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void CleanupTombstonesCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
}

#pragma warning restore 1591