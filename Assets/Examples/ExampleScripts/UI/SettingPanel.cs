﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
[PrefabAsset("UI/SettingPanel",PrefabName = "SettingPanel")]
public class SettingPanel : UILogicResident
{
    protected override void OnInitialization()
    {
        GetUIForm<Button>("BtnQuit").onClick.AddListener(QuitClick);
    }
    protected override void OnTermination()
    {
        GetUIForm<Button>("BtnQuit").onClick.RemoveAllListeners();
    }
    void QuitClick()
    {
        HidePanel();
    }
}
