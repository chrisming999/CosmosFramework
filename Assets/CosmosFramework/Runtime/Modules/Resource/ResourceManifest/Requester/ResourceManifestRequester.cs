﻿using Cosmos.WebRequest;
using System;
using System.Collections.Generic;

namespace Cosmos.Resource
{
    internal class ResourceManifestRequester
    {
        readonly IWebRequestManager webRequestManager;
        readonly Action<string, ResourceManifest> onSuccess;
        readonly Action<string, string> onFailure;
        /// <summary>
        /// taskId===manifestEncryptionKey
        /// </summary>
        Dictionary<long, string> taskIdKeyDict;
        public ResourceManifestRequester(IWebRequestManager webRequestManager, Action<string, ResourceManifest> onSuccess, Action<string, string> onFailure)
        {
            taskIdKeyDict = new Dictionary<long, string>();
            this.webRequestManager = webRequestManager;
            this.onSuccess = onSuccess;
            this.onFailure = onFailure;
        }
        public void OnInitialize()
        {
            webRequestManager.OnSuccessCallback += OnSuccessCallback;
            webRequestManager.OnFailureCallback += OnFailureCallback;
        }
        public void OnTerminate()
        {
            webRequestManager.OnSuccessCallback -= OnSuccessCallback;
            webRequestManager.OnFailureCallback -= OnFailureCallback;
        }
        public void StartRequestManifest(string url, string manifestEncryptionKey)
        {
            var taskId = webRequestManager.AddDownloadTextTask(url);
            taskIdKeyDict.TryAdd(taskId, manifestEncryptionKey);
        }
        public void StopRequestManifest()
        {
            foreach (var tk in taskIdKeyDict)
            {
                webRequestManager.RemoveTask(tk.Key);
            }
            taskIdKeyDict.Clear();
        }
        void OnSuccessCallback(WebRequestSuccessEventArgs eventArgs)
        {
            var manifestContext = eventArgs.GetText();
            try
            {
                taskIdKeyDict.Remove(eventArgs.TaskId, out var key);
                var resourceManifest = ResourceUtility.Manifest.DeserializeManifest(manifestContext, key);
                onSuccess?.Invoke(eventArgs.URL, resourceManifest);
            }
            catch (Exception e)
            {
                onFailure?.Invoke(eventArgs.URL, e.ToString());
            }
        }
        void OnFailureCallback(WebRequestFailureEventArgs eventArgs)
        {
            taskIdKeyDict.Remove(eventArgs.TaskId);
            onFailure?.Invoke(eventArgs.URL, eventArgs.ErrorMessage);
        }
    }
}
