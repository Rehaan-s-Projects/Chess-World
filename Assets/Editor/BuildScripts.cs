using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ChessWorld.Editor
{
    public static class BuildScripts
    {
        public static void BuildAndroid()
        {
            const string outDir = "build/android";
            const string outPath = outDir + "/ChessWorld.apk";
            Directory.CreateDirectory(outDir);

            ConfigureAndroidToolchain();

            var opts = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Main.unity" },
                locationPathName = outPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            PlayerSettings.SetApplicationIdentifier(
                NamedBuildTarget.Android, "com.chessworld.app");
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            EditorUserBuildSettings.buildAppBundle = false;

            var report = BuildPipeline.BuildPlayer(opts);
            if (report.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError($"Build failed: {report.summary.result}");
                EditorApplication.Exit(1);
            }
            Debug.Log($"Build succeeded: {outPath}");
            EditorApplication.Exit(0);
        }

        private static void ConfigureAndroidToolchain()
        {
            // Read external SDK/NDK/JDK paths from env (set by user's Android Studio install).
            // Falls back to whatever Unity has configured if env vars are unset.
            var sdk = Environment.GetEnvironmentVariable("ANDROID_HOME");
            var ndk = Environment.GetEnvironmentVariable("ANDROID_NDK_HOME");
            var jdk = Environment.GetEnvironmentVariable("JAVA_HOME");

            if (!string.IsNullOrEmpty(sdk))
                EditorPrefs.SetString("AndroidSdkRoot", sdk);
            if (!string.IsNullOrEmpty(ndk))
                EditorPrefs.SetString("AndroidNdkRootR23D", ndk);
            if (!string.IsNullOrEmpty(jdk))
                EditorPrefs.SetString("JdkPath", jdk);
        }
    }
}
