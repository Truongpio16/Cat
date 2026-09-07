using UnityEngine;

[CreateAssetMenu(fileName = "LevelCatalog", menuName = "Cat/Level Catalog")]
public class LevelCatalog : ScriptableObject
{
    public LevelData[] levels;
}
