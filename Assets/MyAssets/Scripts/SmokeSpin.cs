using UnityEngine;

namespace VibeCodeTest
{
    /// <summary>Deployment check only: proves the player loop runs in the browser. Delete with the smoke objects.</summary>
    public sealed class SmokeSpin : MonoBehaviour
    {
        private const float DegreesPerSecond = 90f;

        private void Update()
        {
            transform.Rotate(Vector3.up, DegreesPerSecond * Time.deltaTime, Space.World);
        }
    }
}
