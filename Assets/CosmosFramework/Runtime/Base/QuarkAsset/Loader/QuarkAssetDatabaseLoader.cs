﻿using Quark.Asset;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cosmos;
namespace Quark.Loader
{
    public class QuarkAssetDatabaseLoader : IQuarkAssetLoader
    {
        /// <summary>
        /// AssetDataBase模式下资源的映射字典；
        /// Key : AssetName---Value :  Lnk [QuarkAssetObject]
        /// </summary>
        Dictionary<string, LinkedList<QuarkAssetDatabaseObject>> assetDatabaseMap
            = new Dictionary<string, LinkedList<QuarkAssetDatabaseObject>>();

        /// <summary>
        /// Hash===QuarkObjectInfo
        /// </summary>
        Dictionary<int, QuarkObjectInfo> hashQuarkObjectInfoDict = new Dictionary<int, QuarkObjectInfo>();

        public void SetLoaderData(object customeData)
        {
            SetAssetDatabaseModeData(customeData as QuarkAssetDataset);
        }
        public T LoadAsset<T>(string assetName, string assetExtension, bool instantiate = false) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset name is invalid!");
            QuarkAssetDatabaseObject quarkAssetDatabaseObject = null;
            if (assetDatabaseMap == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            if (assetDatabaseMap.TryGetValue(assetName, out var lnk))
                quarkAssetDatabaseObject = GetAssetDatabaseObject<T>(lnk, assetExtension);
            if (quarkAssetDatabaseObject!=null)
            {
                var guid2path = UnityEditor.AssetDatabase.GUIDToAssetPath(quarkAssetDatabaseObject.AssetGuid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(guid2path);
                if (instantiate)
                    return GameObject.Instantiate(asset);
                return asset;
            }
            else
                return null;
#else
                return null;
#endif
        }
        public Coroutine LoadScenetAsync(string sceneName, Action<float> progress, Action callback, bool additive = false)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadSceneAsync(sceneName, progress, callback, additive));
        }
        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, bool instantiate = false) where T : UnityEngine.Object
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAsssetAsync(assetName, string.Empty, callback, instantiate));
        }
        public Coroutine LoadAssetAsync<T>(string assetName, string assetExtension, Action<T> callback, bool instantiate = false) where T : UnityEngine.Object
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAsssetAsync(assetName, assetExtension, callback, instantiate));
        }
        public void UnLoadAllAssetBundle(bool unloadAllLoadedObjects = false)
        {
            Utility.Debug.LogInfo("AssetDatabase Mode UnLoadAllAsset");
        }
        public void UnLoadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            Utility.Debug.LogInfo("AssetDatabase Mode UnLoadAllAsset");
        }
        public QuarkObjectInfo GetInfo<T>(string assetName, string assetExtension) where T : UnityEngine.Object
        {
            QuarkAssetDatabaseObject abObject = null;
            if (assetDatabaseMap.TryGetValue(assetName, out var abLnk))
            {
                if (string.IsNullOrEmpty(assetExtension))
                {
                    abObject = abLnk.First.Value;
                }
                else
                {
                    foreach (var ab in abLnk)
                    {
                        if (ab.AssetExtension == assetExtension)
                        {
                            abObject = ab;
                            break;
                        }
                    }
                }
                return hashQuarkObjectInfoDict[abObject.GetHashCode()];
            }
            return QuarkObjectInfo.None;
        }
        public QuarkObjectInfo[] GetAllInfos()
        {
            return hashQuarkObjectInfoDict.Values.ToArray();
        }
        IEnumerator EnumLoadAsssetAsync<T>(string assetName, string assetExtension, Action<T> callback, bool instantiate = false) where T : UnityEngine.Object
        {
            var asset = LoadAsset<T>(assetName, assetExtension, instantiate);
            yield return null;
            callback?.Invoke(asset);
        }
        IEnumerator EnumLoadSceneAsync(string sceneName, Action<float> progress, Action callback, bool additive)
        {
            LoadSceneMode loadSceneMode = additive == true ? LoadSceneMode.Additive : LoadSceneMode.Single;
            var ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            while (!ao.isDone)
            {
                progress.Invoke(ao.progress);
                yield return null;
            }
            progress.Invoke(1);
            callback.Invoke();
        }
        QuarkAssetDatabaseObject GetAssetDatabaseObject<T>(LinkedList<QuarkAssetDatabaseObject> lnk, string assetExtension = null)
where T : UnityEngine.Object
        {
            QuarkAssetDatabaseObject quarkAssetObject = null;
            if (!string.IsNullOrEmpty(assetExtension))
            {
                foreach (var adObject in lnk)
                {
                    if (adObject.AssetExtension == assetExtension)
                    {
                        quarkAssetObject = adObject;
                        break;
                    }
                }
            }
            else
            {
                quarkAssetObject = lnk.First.Value;
            }
            return quarkAssetObject;
        }
        /// <summary>
        /// 对QuarkAssetDataset进行编码
        /// </summary>
        /// <param name="assetData">QuarkAssetDataset对象</param>
        void SetAssetDatabaseModeData(QuarkAssetDataset assetData)
        {
            var length = assetData.QuarkAssetObjectList.Count;
            for (int i = 0; i < length; i++)
            {
                var adObject = assetData.QuarkAssetObjectList[i];
                if (!assetDatabaseMap.TryGetValue(adObject.AssetName, out var lnkList))
                {
                    var lnk = new LinkedList<QuarkAssetDatabaseObject>();
                    lnk.AddLast(assetData.QuarkAssetObjectList[i]);
                    assetDatabaseMap.Add(adObject.AssetName, lnk);
                }
                else
                {
                    lnkList.AddLast(assetData.QuarkAssetObjectList[i]);
                }
                var info = QuarkObjectInfo.Create(adObject.AssetName, null, adObject.AssetExtension, 0);
                info.ABObjectHash = adObject.GetHashCode();
                hashQuarkObjectInfoDict.Add(info.ABObjectHash, info);
            }
        }
        /// <summary>
        /// 增加一个引用计数；
        /// </summary>
        void IncrementQuarkObjectInfo(QuarkAssetDatabaseObject abObject)
        {
            var hashCode = abObject.GetHashCode();
            var info = hashQuarkObjectInfoDict[hashCode];
            hashQuarkObjectInfoDict[hashCode] = info++;
        }

        /// <summary>
        /// 减少一个引用计数；
        /// </summary>
        void DecrementQuarkObjectInfo(QuarkAssetDatabaseObject abObject)
        {
            var hashCode = abObject.GetHashCode();
            var info = hashQuarkObjectInfoDict[hashCode];
            hashQuarkObjectInfoDict[hashCode] = info--;
        }
    }
}
