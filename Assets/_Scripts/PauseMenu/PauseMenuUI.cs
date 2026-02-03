using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{

    private PauseSystem _pauseSystem;

    public void Init(PauseSystem system)
    {
        _pauseSystem = system;
    }

    public void Resume()
    {
        _pauseSystem.Resume();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
