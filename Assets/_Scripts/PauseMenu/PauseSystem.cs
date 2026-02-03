using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public sealed class PauseSystem : MonoBehaviour
{
    [FormerlySerializedAs("pauseMenuPrefab")] [SerializeField] private GameObject _pauseMenuPrefab;
    [FormerlySerializedAs("uiCanvas")] [SerializeField] private Canvas _uiCanvas;

    private GameObject _currentPauseMenu;
    private bool _isPaused;

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (_isPaused)
                Resume();
            else
                Pause();
        }
    }


    private void Pause()
    {


        if (_currentPauseMenu == null)
        {
            _currentPauseMenu = Instantiate(_pauseMenuPrefab, _uiCanvas.transform);


            var ui = _currentPauseMenu.GetComponent<PauseMenuUI>();
            ui.Init(this);
        }

        Time.timeScale = 0f;// pauses game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;//shows cursor
        GamePause.SetPaused(true);
        _isPaused = true;
    }

    public void Resume()
    {
        if (_currentPauseMenu != null)
        {
            Destroy(_currentPauseMenu);
            _currentPauseMenu = null;
        }

        Time.timeScale = 1f;//resume game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;//hide cursor
        GamePause.SetPaused(false);
        _isPaused = false;
    }
}