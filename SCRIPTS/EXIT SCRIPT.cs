using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void ExitGame()
    {
        // This method will be called when the exit button is clicked
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
