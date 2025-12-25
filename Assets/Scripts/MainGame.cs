using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MainGame : MonoBehaviour
{
    private void Start()
    {
        GameResourceManager.Instance.onAllDataLoaded += StartGame;
    }
    private void StartGame()
    {
        Debug.Log("Game da bat dau!");
    }
    private void OnDestroy()
    {
        GameResourceManager.Instance.onAllDataLoaded -= StartGame;
    }
}
