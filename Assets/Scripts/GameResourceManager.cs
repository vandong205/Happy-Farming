using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UIElements;
using Unity.VisualScripting;
public class GameResourceManager : SingletonPattern<GameResourceManager>
{
    public event Action onAllDataLoaded;
    private int totalSteps = 1;
    private int currentStep = 1;
    private void Start()
    {
        StartCoroutine(LoadMapData());
    }
    IEnumerator LoadMapData()
    {
        yield return WorldManager.Instance.LoadMapWalkableData(Consts.DataPaths.map_walkabledata);
        Debug.Log($"Dang tai tai nguyen, da hoan thanh {currentStep}/{totalSteps}");
        UpdateCurrentStep();
        yield return WorldManager.Instance.LoadWorldMatrix(Consts.DataPaths.world_matrix);
        Debug.Log($"Dang tai tai nguyen, da hoan thanh {currentStep}/{totalSteps}");
        UpdateCurrentStep();
    }
    private void UpdateCurrentStep()
    {
        currentStep++;
        if (currentStep > totalSteps) {
            onAllDataLoaded?.Invoke();
        }
    }
}
