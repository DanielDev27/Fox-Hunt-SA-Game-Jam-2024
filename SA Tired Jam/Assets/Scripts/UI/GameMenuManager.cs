using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour
{
    public static GameMenuManager instance;
    [Header("Debug")]
    public bool gameActive = true;
    [SerializeField] bool check;
    [SerializeField] float currentGameTime;
    [SerializeField] float totalGameTime;

    [Header("References - Objects")]
    [Header("References - UI")]
    [SerializeField] Image timerImage;
    [SerializeField] Canvas pauseCanvas;
    [SerializeField] Canvas controlsCanvas;
    [SerializeField] Canvas endWin;
    [SerializeField] Canvas endLose;
    [SerializeField] CanvasGroup loadingGroup;

    [Header("First Selections")]
    [SerializeField] GameObject winFirst;
    [SerializeField] GameObject loseFirst;

    private void Awake()
    {
        instance = this;
        gameActive = true;
    }
    private void Start()
    {
        currentGameTime = 0;
        check = CursorManager.Instance.usingGamepad;
    }
    private void Update()
    {
        timerImage.fillAmount = 1 - currentGameTime / totalGameTime;
        if (gameActive && currentGameTime < totalGameTime && !PauseScript.Instance.pause)
        {
            currentGameTime += Time.deltaTime;
        }
        if (gameActive && currentGameTime >= totalGameTime)
        {
            gameActive = false;
            if (HungerTracker.instance.hunger > 0)
            {
                GameWin();
            }
        }
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
    public void GameWin()//Logic for winning the game - Triggered after each AI death
    {

        PauseScript.Instance.gameOver = true;
        endWin.gameObject.SetActive(true);
        endWin.enabled = true;
        CharacterController.instance.OnDisable();
        CursorManager.Instance.InputDeviceUIAssign();
        EventSystem.current.SetSelectedGameObject(winFirst);
        //aiParent.gameObject.SetActive(false);

    }
    public void GameOver()//Logic for losing the game
    {
        endLose.gameObject.SetActive(true);
        PauseScript.Instance.gameOver = true;
        CharacterController.instance.OnDisable();
        CursorManager.Instance.InputDeviceUIAssign();
        endLose.enabled = true;
        EventSystem.current.SetSelectedGameObject(loseFirst);
        //aiParent.gameObject.SetActive(false);
    }

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
