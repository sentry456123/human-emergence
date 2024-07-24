using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button button;

    private bool isGamePaused = false;
    private float cachedFixedDeltaTime;

    void Start()
    {
        cachedFixedDeltaTime = Time.fixedDeltaTime;
		button.enabled = false;
        button.gameObject.SetActive(false);
        button.onClick.AddListener(OnQuitButtonClick);
	}

	void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isGamePaused = !isGamePaused;
            if (isGamePaused)
            {
                Time.timeScale = 0.0f;
                cachedFixedDeltaTime = Time.fixedDeltaTime;
                Time.fixedDeltaTime = 0.0f;
                button.enabled = true;
				button.gameObject.SetActive(true);
			}
            else
            {
                Time.timeScale = 1.0f;
				Time.fixedDeltaTime = cachedFixedDeltaTime;
				button.enabled = false;
				button.gameObject.SetActive(false);
			}
        }
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }
}
