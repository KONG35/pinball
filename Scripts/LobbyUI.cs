using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LobbyUI : MonoBehaviour
{
    [SerializeField]
    private Button startButton;

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(PressStartButton);
    }

    private void PressStartButton()
    {
        SceneHandler.Instance.LoadScene_Async(SceneName.Ingame, LoadSceneMode.Additive);
        UIManager.Instance.lobbyUI.gameObject.SetActive(false);
    }
}
