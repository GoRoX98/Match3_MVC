using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void StartGame() => SceneManager.LoadScene("Game");
    public void ExitGame() => Application.Quit();
}
