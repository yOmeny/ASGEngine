using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private PauseSystem pauseSystem;

    public void Init(PauseSystem system)
    {
        pauseSystem = system;
    }

    public void Resume()
    {
        pauseSystem.Resume();
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
