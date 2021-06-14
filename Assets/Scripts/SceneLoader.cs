using UnityEngine.SceneManagement;

public static class SceneLoader
{ 
    public static void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }
}
