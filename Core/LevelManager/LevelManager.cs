using System;
using System.Collections.Generic;

using UnityEngine;

public class LevelManager : MonoBehaviour
{
	[Space, Header("LEVEL DATA")]
	[SerializeField] public List<LevelData> data;

	public event Action LevelLoadingEvent             = null;
	public event Action<float> LevelAsyncLoadingEvent = null;
	public event Action LevelLoadedEvent              = null;

	private LevelData currentLevelData = null;

	private int levelDataIndex = 0;

	public void Initialize()
	{
		if (data.Count <= 0)
			return;

		List<SceneData> list = new List<SceneData>();
		foreach (var levelData in data)
		{
			list.Add(levelData.SceneData);
		}

		SceneSystem.Instance.Initialize(list);

		SceneSystem.Instance.SceneLoadingEvent      += OnSceneLoading;
		SceneSystem.Instance.AsyncSceneLoadingEvent += OnAsyncSceneLoading;
		SceneSystem.Instance.SceneLoadedEvent       += OnSceneLoaded;

		currentLevelData = data[levelDataIndex];

		SceneSystem.Instance.LoadScene(currentLevelData.SceneData);
	}

	public void LoadNextLevel()
	{
		SceneSystem.Instance.LoadNextScene(true);
		levelDataIndex++;
		currentLevelData = data[levelDataIndex];
	}

	public void LoadLevel(LevelData data)
	{
		SceneSystem.Instance.UnloadScene(currentLevelData.SceneData);
		SceneSystem.Instance.LoadScene(data.SceneData);

		// - 1 because we work on the assumption that the index 0 is the Dependencies Scene and it's already loaded
		levelDataIndex = data.SceneData.Scene.BuildIndex - 1;
		currentLevelData = data;
	}

	public void ReloadLevel()
	{
		var asyncOp = SceneSystem.Instance.UnloadScene(currentLevelData.SceneData);
		asyncOp.completed += operation => Restart();
	}

	public void Restart()
	{
		LoadLevel(currentLevelData);
	}

	public LevelData GetCurrentLevelData()
	{
		return currentLevelData;
	}

	private void OnSceneLoading()
	{
		LevelLoadingEvent?.Invoke();
	}

	private void OnSceneLoaded()
	{
		LevelLoadedEvent?.Invoke();
	}

	private void OnAsyncSceneLoading(float progressValue)
	{
		LevelAsyncLoadingEvent?.Invoke(progressValue);
	}

	public List<LevelData> GetData()
	{
		return data;
	}
}
