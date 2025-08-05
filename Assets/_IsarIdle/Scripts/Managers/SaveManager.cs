using System;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private void OnApplicationQuit()
    {
		GameData.Instance.Save();
    }
}