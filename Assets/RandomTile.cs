using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RandomTile : Tile {
    // Basic struct to store a sprite info with its weight info
    [System.Serializable]
    public struct SpriteWithProbability {
        public Sprite sprite;
        public int weight;
    }

    // List of sprites
    [SerializeField] public SpriteWithProbability[] sprites;

    // Set tile for a position
    public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData) {
        base.GetTileData(location, tileMap, ref tileData);

        // Don't do anything if we got no sprites
        if (sprites == null || sprites.Length <= 0f) return;

        // Seed the random generator based on the tile so our preview sticks as is
        long hash = location.x;
        hash = hash + 0xabcd1234 + (hash << 15);
        hash = hash + 0x0987efab ^ (hash >> 11);
        hash ^= location.y;
        hash = hash + 0x46ac12fd + (hash << 7);
        hash = hash + 0xbe9730af ^ (hash << 11);
        Random.InitState((int) hash);

        // Get the cumulative weight of the sprites
        var cumulativeWeight = 0;
        foreach (var spriteInfo in sprites) cumulativeWeight += spriteInfo.weight;
        // NOTE: Could just do this: sprites.Sum(s => s.weight);

        // Pick a random weight and choose a sprite depending on it
        var randomWeight = Random.Range(0, cumulativeWeight);
        foreach (var spriteInfo in sprites) {
            randomWeight -= spriteInfo.weight;
            if (randomWeight <= 0) {
                tileData.sprite = spriteInfo.sprite;    
                break;
            }
        }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Tiles/Random Tile")]
    public static void CreateRandomTile() {
        var path = EditorUtility.SaveFilePanelInProject("Save Random Tile", "New Random Tile", "asset",
            "Save Random tile", "Assets");
        if (path == "") return;
        AssetDatabase.CreateAsset(CreateInstance<RandomTile>(), path);
    }
#endif
}
