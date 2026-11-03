using UnityEngine;
using UnityEngine.SceneManagement;

namespace VibeCodeTest
{
    /// <summary>Entry point of 00 - Initialize: runs startup work, then opens the main menu.</summary>
    public sealed class Bootstrap : MonoBehaviour
    {
        private const int MainMenuBuildIndex = 1;

        private void Start()
        {
            SceneManager.LoadScene(MainMenuBuildIndex);
        }
    }
}
