using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBehaviour : MonoBehaviour {

    [System.Serializable]
    public struct TileInfo
    {
        public TileBehaviour.TileType tileType;
        public float tileAngle;

        public bool hasBall;
        public float ballForward;

        public PickupBehaviour pickupPrefab;
    }

    public int width, height;
    public TileInfo[] tiles;

    public bool IsValid()
    {
        return (width > 0 && height > 0 && tiles.Length == width * height);
    }

    public void SetDefaultTiles()
    {
        tiles = new TileInfo[width * height];

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                TileBehaviour.TileType desiredType = TileBehaviour.TileType.BumpersDefault;

                if (x == 0 || x + 1 == width || y == 0 || y + 1 == height)
                {
                    desiredType = TileBehaviour.TileType.Block;
                }

                tiles[x * height + y] = new TileInfo {
                    tileType = desiredType,
                    tileAngle = 0.0f,
                    hasBall = false,
                    ballForward = 0.0f,
                    pickupPrefab = null
                };
            }
        }
    }



}
