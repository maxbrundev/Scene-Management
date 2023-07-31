using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;

[CreateAssetMenu(fileName = "SceneEntryProjectSettingData", menuName = "Editor/Data/SceneEntry")]
public class SceneEntryProjectSettingData : ScriptableObject
{
	[SerializeField] public SceneReference data;

	void OnValidate()
	{
		UpdateEditorOnPlay();
	}

	public void UpdateEditorOnPlay()
	{
		if (data == null || data.BuildIndex > EditorBuildSettings.scenes.Length - 1)
		{
			EditorSceneManager.playModeStartScene = null;
			return;
		}

		EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(data.Scene.Path);
	}

	internal static SceneEntryProjectSettingData GetSettings()
	{
		var settings = AssetDatabase.LoadAssetAtPath<SceneEntryProjectSettingData>(EditorConstants.SCENE_ENTRY_SETTING_ASSET_PATH);

		if (settings == null)
		{
			AssetDatabase.CreateAsset(CreateInstance<SceneEntryProjectSettingData>(), EditorConstants.SCENE_ENTRY_SETTING_ASSET_PATH);

			settings = AssetDatabase.LoadAssetAtPath<SceneEntryProjectSettingData>(EditorConstants.SCENE_ENTRY_SETTING_ASSET_PATH);
		}

		return settings;
	}

	internal static SerializedObject GetSerializedSettings()
	{
		return new SerializedObject(GetSettings());
	}
}