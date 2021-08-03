﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
namespace Cosmos.CosmosEditor
{
    internal enum AssetBundleHashType : byte
    {
        DefaultName = 0,
        AppendHash = 1,
        HashInstead = 2
    }
    [Serializable]
    internal class AssetBundleBuildTabData
    {
        public BuildTarget BuildTarget { get; set; }
        public string OutputPath { get; set; }
        public bool UseDefaultPath { get; set; }
        public bool ClearOutputFolders { get; set; }
        public bool CopyToStreamingAssets { get; set; }
        public string StreamingAssetsPath { get; set; }
        public bool WithoutManifest { get; set; }
        public AssetBundleHashType NameHashType { get; set; }
        public bool UseAESEncryption { get; set; }
        public string AESEncryptionKey { get; set; }
        public BuildAssetBundleOptions BuildAssetBundleOptions { get; set; }

        public AssetBundleBuildTabData()
        {
            BuildTarget = BuildTarget.StandaloneWindows;
            OutputPath = "AssetBundles/StandaloneWindows";
            UseDefaultPath = true;
            ClearOutputFolders = true;
            CopyToStreamingAssets = false;
            StreamingAssetsPath = "Assets/StreamingAssets";
            WithoutManifest = true;
            NameHashType = AssetBundleHashType.DefaultName;
            UseAESEncryption = false;
            AESEncryptionKey = "QuarkAssetBundle";
            BuildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
        }
    }
}