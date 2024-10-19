using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    public static GameMenuManager instance;
    [Header("Debug")]
    [SerializeField] bool check;
    [Header("References - Objects")]
    [Header("References - UI")]
    [SerializeField] Canvas pauseCanvas;
    [SerializeField] Canvas controlsCanvas;
    [SerializeField] Canvas endWin;
    [SerializeField] Canvas endLose;
    [SerializeField] CanvasGroup loadingGroup;

    [Header("First Selections")]
    [SerializeField] GameObject winFirst;
    [SerializeField] GameObject loseFirst;

    private void Start()
    {
        check = CursorManager.Instance.usingGamepad;
    }
    private void FixedUpdate()
    {
        if (check != CursorManager.Instance.usingGamepad)
        {
            CursorManager.Instance.InputDeviceUIAssign();
            check = CursorManager.Instance.usingGamepad;
            if (check)
            {
                if (endLose.enabled == true)
                {
                    EventSystem.current.SetSelectedGameObject(loseFirst);
                }
                if (endWin.enabled == true)
                {
                    EventSystem.current.SetSelectedGameObject(winFirst);
                }
            }
        }
        if (endLose.enabled == false && endWin.enabled == false && pauseCanvas.enabled == false && controlsCanvas.enabled == false)
        {
            CursorManager.Instance.CursorOff();
        }
    }
    /*public void GameWin()//Logic for winning the game - Triggered after each AI death
    {
        if ( <= 0)//Logic only completes if there are no more AIs
        {
            PauseScript.Instance.gameOver = true;
            endWin.gameObject.SetActive(true);
            endWin.enabled = true;
            PlayerController.Instance.OnDisable();
            CursorManager.Instance.InputDeviceUIAssign();
            EventSystem.current.SetSelectedGameObject(winFirst);
            aiParent.gameObject.SetActive(false);
        }
    }
    public void GameOver()//Logic for losing the game
    {
        endFail.gameObject.SetActive(true);
        PauseScript.Instance.gameOver = true;
        camera2.gameObject.SetActive(true);
        PlayerController.Instance.OnDisable();
        CursorManager.Instance.InputDeviceUIAssign();
        endFail.enabled = true;
        EventSystem.current.SetSelectedGameObject(lossFirst);
        aiParent.gameObject.SetActive(false);
    }*/

    public void GoToMain()//Change Scenes to the Main Menu
    {
        Debug.Log("Return to main menu");
        pauseCanvas.enabled = false;
        loadingGroup.alpha = 1.0f;
        Time.timeScale = 1;
        SceneLoaderManager.Instance.LoadScene(0);
    }
    public void Reload()//Reload the Game Scene to start over
    {
        Time.timeScale = 1;
        SceneLoaderManager.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Quit()//Quit Application
    {
        Application.Quit();
    }
}
