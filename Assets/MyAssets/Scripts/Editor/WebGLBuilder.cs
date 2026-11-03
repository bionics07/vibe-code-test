using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace VibeCodeTest.EditorTools
{
    /// <summary>Builds the WebGL player into docs/, the folder GitHub Pages serves.</summary>
    public static class WebGLBuilder
    {
        private const string OutputFolder = "docs";

        [MenuItem("Tools/Build WebGL to docs")]
        public static void Build()
        {
            string output = Path.GetFullPath(Path.Combine(Application.dataPath, "..", OutputFolder));
            if (Directory.Exists(output))
            {
                Directory.Delete(output, true);
            }

            BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray(),
                locationPathName = output,
                target = BuildTarget.WebGL,
                options = BuildOptions.None,
            });

            BuildSummary summary = report.summary;
            Debug.Log($"[WebGLBuilder] {summary.result} in {summary.totalTime.TotalSeconds:F0}s, {summary.totalSize / (1024f * 1024f):F1} MB");

            if (summary.result != BuildResult.Succeeded)
            {
                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(1);
                }

                return;
            }

            // Stops GitHub Pages from running the output through Jekyll.
            File.WriteAllText(Path.Combine(output, ".nojekyll"), string.Empty);
        }
    }
}
