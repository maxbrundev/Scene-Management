using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Data/Level")]
public class LevelData : ScriptableObject
{
	public SceneData SceneData;
	public string Name;
}