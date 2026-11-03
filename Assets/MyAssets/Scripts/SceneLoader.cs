using UnityEngine;
using UnityEngine.SceneManagement;

namespace VibeCodeTest
{
    /// <summary>Loads scenes by Build Settings index, from code or from UI events.</summary>
    public sealed class SceneLoader : MonoBehaviour
    {
        public void LoadScene(int buildIndex)
        {
            SceneManager.LoadScene(buildIndex);
        }
    }
}
