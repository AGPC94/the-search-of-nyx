using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string music;
    [SerializeField] string gameScene;

    void Start()
    {
        if (music != string.Empty)
            AudioManager.instance.PlayMusic(music);
    }

    public void Play()
    {
        Debug.Log("Play()");
        SceneManager.LoadScene(gameScene);
    }

    public void Quit()
    {
        Debug.Log("Quit()");
        Application.Quit();
    }

}
