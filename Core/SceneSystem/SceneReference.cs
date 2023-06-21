using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
#endif

[Serializable]
public class SceneReference : ISerializationCallbackReceiver
{
#if UNITY_EDITOR
	[SerializeField] private UnityEngine.Object asset;

	private bool IsSceneAsset
	{
		get
		{
			if (asset == null)
				return false;

			return asset is SceneAsset;
		}
	}
#endif

	[SerializeField] private string path = string.Empty;

	public string Path
	{
		get
		{
#if UNITY_EDITOR
			return GetScenePathFromAsset();
#else
			return path;
#endif
		}
		set
		{
			path = value;
#if UNITY_EDITOR
			asset = GetSceneAssetFromPath();
#endif
		}
	}

	[SerializeField] private int buildIndex;
	public int BuildIndex => buildIndex;

	public static implicit operator string(SceneReference sceneReference)
	{
		return sceneReference.Path;
	}

	public void OnBeforeSerialize()
	{
#if UNITY_EDITOR
		if (!IsSceneAsset && !string.IsNullOrEmpty(path))
		{
			asset = GetSceneAssetFromPath();

			if (asset == null)
			{
				path = string.Empty;
			}
		}
		else
		{
			path = GetScenePathFromAsset();
		}

		if(path == string.Empty)
			return;

		var currentBuildIndex = SceneUtility.GetBuildIndexByScenePath(path);

		if (currentBuildIndex < 0 && buildIndex != currentBuildIndex)
		{
			Debug.LogError($"Scene at path {path} is not present in the Build.");
		}

		buildIndex = currentBuildIndex;
#endif
	}

	public void OnAfterDeserialize()
	{
#if UNITY_EDITOR
		EditorApplication.update += HandleAfterDeserialize;
#endif
	}

#if UNITY_EDITOR
	private SceneAsset GetSceneAssetFromPath()
	{
		return string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
	}

	private string GetScenePathFromAsset()
	{
		return asset == null ? string.Empty : AssetDatabase.GetAssetPath(asset);
	}

	private void HandleAfterDeserialize()
	{
		EditorApplication.update -= HandleAfterDeserialize;

		if (IsSceneAsset || string.IsNullOrEmpty(path))
			return;

		asset = GetSceneAssetFromPath();

		if (asset == null)
			path = string.Empty;
	}
#endif
}