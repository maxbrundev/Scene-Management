using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

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

	public void LoadNextLevel(bool isUnloadingCurrentLevel)
	{
		var nextLevelData = data[levelDataIndex + 1];
		bool isSingle = nextLevelData.SceneData.LoadSceneMode == LoadSceneMode.Single;

		SceneSystem.Instance.LoadNextScene(!isSingle && isUnloadingCurrentLevel && SceneManager.sceneCount > 1);
		levelDataIndex++;
		currentLevelData = data[levelDataIndex];
	}

	public void LoadPreviousLevel(bool isUnloadingCurrentLevel)
	{
		var previousLevelData = data[levelDataIndex - 1];
		bool isSingle = previousLevelData.SceneData.LoadSceneMode == LoadSceneMode.Single;

		SceneSystem.Instance.LoadNextScene(!isSingle && isUnloadingCurrentLevel && SceneManager.sceneCount > 1);
		levelDataIndex--;
		currentLevelData = data[levelDataIndex];
	}

	public void LoadLevel(LevelData data, bool isUnloadingCurrentLevel)
	{
		if(data.SceneData.LoadSceneMode != LoadSceneMode.Single && isUnloadingCurrentLevel && SceneManager.sceneCount > 1)
			SceneSystem.Instance.UnloadScene(currentLevelData.SceneData);

		SceneSystem.Instance.LoadScene(data.SceneData);

		// - 1 because we work on the assumption that the index 0 is the Dependencies Scene and it's already loaded
		levelDataIndex = data.SceneData.Scene.BuildIndex - 1;
		currentLevelData = data;
	}

	public void ReloadLevel()
	{
		if(SceneManager.sceneCount > 1)
		{
			var asyncOp = SceneSystem.Instance.UnloadScene(currentLevelData.SceneData);
			asyncOp.completed += operation => Restart(isUnloadingCurrentLevel: false);
		}
		else
		{
			Restart(isUnloadingCurrentLevel: false);
		}
	}

	public void Restart(bool isUnloadingCurrentLevel)
	{
		LoadLevel(currentLevelData, isUnloadingCurrentLevel);
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
