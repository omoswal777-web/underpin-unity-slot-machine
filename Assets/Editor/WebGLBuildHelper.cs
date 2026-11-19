#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class WebGLBuildHelper
{
    [MenuItem("Underpin/Build WebGL")]
    public static void BuildWebGL()
    {
        const string output = "Build/WebGL";
        string[] scenes = { "Assets/Scenes/SlotMachine.unity" };
        if (!System.IO.File.Exists(scenes[0]))
        {
            EditorUtility.DisplayDialog("Scene Missing", "First run Underpin > Create Slot Machine Demo Scene.", "OK");
            return;
        }

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = output,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result == BuildResult.Succeeded)
            EditorUtility.DisplayDialog("WebGL Build Complete", "Build created at " + output, "OK");
        else
            EditorUtility.DisplayDialog("WebGL Build Failed", "Check the Console and Build report for details.", "OK");
    }
}
#endif
