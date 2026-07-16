#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class BootstrapPlayModeRedirect
{
    private const string BOOTSTRAP_SCENE_NAME = "Bootstrap";
    private const string BOOTSTRAP_SCENE_PATH = "Assets/_Root/Scenes/Bootstrap.unity";
    
    private const string MENU_SCENE_NAME = "MainMenu";
    private const string GAMEPLAY_SCENE_NAME = "Gameplay";

    static BootstrapPlayModeRedirect()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode)
        {
            return;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name is MENU_SCENE_NAME or GAMEPLAY_SCENE_NAME)
        {
            SceneAsset bootstrapScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(BOOTSTRAP_SCENE_PATH);
            if (bootstrapScene == null)
            {
                Debug.LogError($"Bootstrap scene not found at path: {BOOTSTRAP_SCENE_PATH}");
                EditorSceneManager.playModeStartScene = null;
                return;
            }

            EditorSceneManager.playModeStartScene = bootstrapScene;
            return;
        }

        EditorSceneManager.playModeStartScene = null;
    }
}
#endif
