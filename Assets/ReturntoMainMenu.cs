using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturntoMainMenu : MonoBehaviour
{
    public void ReturntoMenu()
    {
        GameplayManager.instance.VisitMainMenuScene();
    }
}
