using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ImageUIController : MonoBehaviour
{
    public Image displayImage;
    public Sprite[] images;
    public Button nextButton;
    public Button backButton;
    public Button exitButton;

    private int currentImageIndex = 0;
    private string sceneKey;

    void Start()
    {
        sceneKey = "Scene_" + SceneManager.GetActiveScene().name + "_HasSeenUI";

        exitButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);

        displayImage.sprite = images[currentImageIndex];

        nextButton.onClick.AddListener(NextImage);
        backButton.onClick.AddListener(BackImage);
        exitButton.onClick.AddListener(ExitUI);

        if (PlayerPrefs.GetInt(sceneKey, 0) == 1)
        {
            gameObject.SetActive(false);
            HideCursor();
        }
        else
        {
            ShowCursor();
        }
    }

    void NextImage()
    {
        currentImageIndex++;

        if (currentImageIndex > 0)
        {
            backButton.gameObject.SetActive(true);
        }

        if (currentImageIndex >= images.Length - 1)
        {
            exitButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(false);
        }

        displayImage.sprite = images[currentImageIndex];
    }

    void BackImage()
    {
        currentImageIndex--;

        if (currentImageIndex < images.Length - 1)
        {
            nextButton.gameObject.SetActive(true);
            exitButton.gameObject.SetActive(false);
        }

        if (currentImageIndex == 0)
        {
            backButton.gameObject.SetActive(false);
        }

        displayImage.sprite = images[currentImageIndex];
    }

    void ExitUI()
    {
        PlayerPrefs.SetInt(sceneKey, 1);
        gameObject.SetActive(false);
        HideCursor();
    }

    void OnDestroy()
    {
        PlayerPrefs.SetInt(sceneKey, 0);
    }

    void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
    }

    void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center
    }
}