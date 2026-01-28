using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PauseSystem : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPrefab;
    [SerializeField] private Canvas uiCanvas;

    private GameObject currentPauseMenu;
    private bool isPaused;

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }


    private void Pause()
    {


        if (currentPauseMenu == null)
        {
            currentPauseMenu = Instantiate(pauseMenuPrefab, uiCanvas.transform);


            var ui = currentPauseMenu.GetComponent<PauseMenuUI>();
            ui.Init(this);
        }

        Time.timeScale = 0f;// pauses game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;//shows cursor
        GamePause.SetPaused(true);
        isPaused = true;
    }

    public void Resume()
    {
        if (currentPauseMenu != null)
        {
            Destroy(currentPauseMenu);
            currentPauseMenu = null;
        }

        Time.timeScale = 1f;//resume game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;//hide cursor
        GamePause.SetPaused(false);
        isPaused = false;
    }
}