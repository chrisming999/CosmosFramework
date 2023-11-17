﻿using UnityEngine;
using Cosmos;
public class EntityLauncher : MonoBehaviour
{
    void Start()
    {
        Debug.Log("start");
        var launcherState = new EntityLauncherState();
        var gameState = new EntityGameState();
        CosmosEntry.ProcedureManager.AddProcedureNodes(gameState, launcherState);
        CosmosEntry.ProcedureManager.RunProcedureNode<EntityLauncherState>();
        CosmosEntry.InputManager.SetInputHelper(new StandardInputHelper());
    }
}
