using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine.SceneManagement;

using AsyncOperation = UnityEngine.AsyncOperation;

public class SceneSystem
{
	public event Action SceneLoadingEvent       = null;
	public event Action SceneLoadedEvent        = null;
	public event Action AsyncSceneLoadedEvent   = null;
	public event Action AsyncSceneUnloadedEvent = null;

	public event Action<float> AsyncSceneLoadingEvent = null;

	private List<SceneData> data = null;

	private int sceneDataIndex = 0;

	private static SceneSystem instance;
	public static SceneSystem Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new SceneSystem();
			}

			return instance;
		}
	}

	public void Initialize(List<SceneData> sceneData)
	{
		data = sceneData;

		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public async void LoadScene(SceneData data)
	{
		OnSceneLoading();

		if (data.IsAsync)
		{
			var asyncOperation = SceneManager.LoadSceneAsync(data.Scene, data.LoadSceneMode);

			asyncOperation.completed += OnAsyncSceneLoaded;

			while (!asyncOperation.isDone)
			{
				if (asyncOperation.progress >= 0.9f)
				{
					OnAsyncSceneLoading(1.0f);
				}
				else
				{
					OnAsyncSceneLoading(asyncOperation.progress);
				}

				await Task.Yield();
			}
		}
		else
		{
			SceneManager.LoadScene(data.Scene, data.LoadSceneMode);
		}

		// - 1 because we work on the assumption that the index 0 is the Dependencies Scene and it's already loaded
		sceneDataIndex = data.Scene.BuildIndex - 1;
	}

	public void LoadNextScene(bool isUnloadingCurrentActiveScene)
	{
		if (isUnloadingCurrentActiveScene)
		{
			var asyncOperation = UnloadScene(data[sceneDataIndex]);

			asyncOperation.completed += OnAsyncSceneUnloaded;
		}

		sceneDataIndex++;

		SceneData currentSceneData = data[sceneDataIndex];

		LoadScene(currentSceneData);
	}

	public AsyncOperation UnloadScene(SceneData data)
	{
		return SceneManager.UnloadSceneAsync(data.Scene.BuildIndex);
	}

	private void OnSceneLoading()
	{
		SceneLoadingEvent?.Invoke();
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		SceneManager.SetActiveScene(scene);

		SceneLoadedEvent?.Invoke();
	}

	private void OnAsyncSceneLoaded(AsyncOperation operation)
	{
		if (operation.isDone)
		{
			operation.completed -= OnAsyncSceneLoaded;

			AsyncSceneLoadedEvent?.Invoke();
		}
	}

	private void OnAsyncSceneUnloaded(AsyncOperation operation)
	{
		if (operation.isDone)
		{
			operation.completed -= OnAsyncSceneUnloaded;

			AsyncSceneUnloadedEvent?.Invoke();
		}
	}

	private void OnAsyncSceneLoading(float progressValue)
	{
		AsyncSceneLoadingEvent?.Invoke(progressValue);
	}
}
