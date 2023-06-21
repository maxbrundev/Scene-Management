using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneData", menuName = "Editor/Data/Scene")]
public class SceneData : ScriptableObject
{
	[SerializeField] public SceneReference Scene;
	[SerializeField] public LoadSceneMode LoadSceneMode;
	[SerializeField] public bool IsAsync;
}