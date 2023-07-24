using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
static class SceneEntryProjectSetting
{
	[SettingsProvider]
	public static SettingsProvider CreateMyCustomSettingsProvider()
	{
		var provider = new SettingsProvider("Project/SceneEntryProjectSetting", SettingsScope.Project)
		{
			label = EditorConstants.SCENE_ENTRY_LABEL_NAME,

			guiHandler = (searchContext) =>
			{
				var settings = SceneEntryProjectSettingData.GetSerializedSettings();
				EditorGUILayout.PropertyField(settings.FindProperty("data"), new GUIContent("Scene Entry Point"));
				settings.ApplyModifiedPropertiesWithoutUndo();
			},

			keywords = new HashSet<string>(new[] { "Scene Entry Point" })
		};

		return provider;
	}
}