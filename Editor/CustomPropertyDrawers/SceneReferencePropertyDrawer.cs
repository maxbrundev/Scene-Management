using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferencePropertyDrawer : PropertyDrawer
{
	private const string sceneAssetProperty      = "asset";
	private const string scenePathProperty       = "path";

	private static readonly float lineHeight = EditorGUIUtility.singleLineHeight;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.serializedObject.isEditingMultipleObjects)
		{
			GUI.Label(position, "Scene multiediting not supported");
			return;
		}

		var sceneAssetProperty = GetSceneAssetProperty(property);

		EditorGUI.BeginChangeCheck();

		sceneAssetProperty.objectReferenceValue = EditorGUI.ObjectField(position, label, sceneAssetProperty.objectReferenceValue, typeof(SceneAsset), false);

		if (EditorGUI.EndChangeCheck())
		{
			if (sceneAssetProperty.objectReferenceValue == null)
			{
				GetScenePathProperty(property).stringValue = string.Empty;
			}
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return lineHeight;
	}

	private static SerializedProperty GetSceneAssetProperty(SerializedProperty property)
	{
		return property.FindPropertyRelative(sceneAssetProperty);
	}

	private static SerializedProperty GetScenePathProperty(SerializedProperty property)
	{
		return property.FindPropertyRelative(scenePathProperty);
	}
}
#endif