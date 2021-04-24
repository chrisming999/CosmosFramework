﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
[UIAsset(nameof(StorePanel), "UI/StorePanel" )]
public class StorePanel : UIResidentForm
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
        HideUIForm();
    }
}
