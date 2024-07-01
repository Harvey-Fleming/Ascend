using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL
        Destroy(GameObject.Find("Quit"));
#endif
    }

    public void ResumeSaveGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void NewSaveGame()
    {
        GameManager.instance.NewGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LevelSelect()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
}
