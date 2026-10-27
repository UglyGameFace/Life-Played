using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace LifePlayed.Client.Editor
{
    public static class LifePlayedAndroidBuild
    {
        public static void BuildAndroid()
        {
            LifePlayedProjectConfigurator.EnsureConfigured();

            var outputDirectory = "Builds/Android";
            Directory.CreateDirectory(outputDirectory);

            EditorUserBuildSettings.buildAppBundle = false;

            var options = new BuildPlayerOptions
            {
                scenes = Array.ConvertAll(
                    EditorBuildSettings.scenes,
                    static scene => scene.path),
                locationPathName = outputDirectory + "/LifePlayed.apk",
                target = BuildTarget.Android,
                options = BuildOptions.Development,
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Life Played Android build failed: " +
                    report.summary.result);
            }
        }
    }
}
