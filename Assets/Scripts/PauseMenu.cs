using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;


    private void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1);
    }

    public void OnResume()
    {
        Time.timeScale = 1;
        FindObjectOfType<PlayerManager>().Resume();
    }

    public void OnRestart()
    {
        Time.timeScale = 1;
        PlayerPrefs.SetInt("Menu", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnQuit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void OnVolumeChange()
    {
        GameObject.FindWithTag("MusicPlayer").GetComponent<AudioSource>().volume = volumeSlider.value;
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.Save();
    }
}
