using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {

    public void loadScene(string sceneName) {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    public void EndGame() {
        Application.Quit();
    }
    public void PAUSE(GameObject PAUSE) {
        if (Time.timeScale == 0) return;
        PAUSE.SetActive(true);
        Time.timeScale = 0;
    }
    public void RESUME(GameObject PAUSE) {
        PAUSE.SetActive(false);
        Time.timeScale = 1;
    }
}
