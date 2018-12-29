using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class BoardBehaviour : MonoBehaviour {

    [Serializable]
    public struct BoardSize
    {
        public int x, y;
    }

    public BoardSize boardSize;
    public Camera mainCamera;

    public bool isPregenerated;

    [Header("Generation Info")]
    public MapBehaviour map;

    [Header("Prefabs")]
    public TileBehaviour tilePrefab;
    public BallBehaviour ballPrefab;

    [Header("Tracked Objects")]
    public List<BallBehaviour> activeBalls;
    public List<TileBehaviour> activeTiles;
    public List<SpecialRuleBehaviour> activeRules;

    // Build the board on awake, to make sure the gamerulesbehaviour can access board elements
    private void Awake()
    {
        if (!isPregenerated)
        {
            Generate();
        }
    }

    public void Clear()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (!child.GetComponent<Camera>())
            {
                children.Add(child.gameObject);
            }
        }

        foreach (GameObject child in children)
        {
            DestroyImmediate(child);
        }

        activeBalls = new List<BallBehaviour>();
        activeTiles = new List<TileBehaviour>();
        activeRules = new List<SpecialRuleBehaviour>();
    }

    public void Generate()
    {
        boardSize = new BoardSize { x = map.width, y = map.height };

        mainCamera.orthographicSize = Mathf.Max(boardSize.x, boardSize.y) / 2.0f;
        GameObject[] ruleObjects = GameObject.FindGameObjectsWithTag("SpecialRule");
        foreach (GameObject obj in ruleObjects)
        {
            activeRules.Add(obj.GetComponent<SpecialRuleBehaviour>());
        }

        for (int x = 0; x < boardSize.x; ++x)
        {
            for (int y = 0; y < boardSize.y; ++y)
            {
                Vector3 delta = GetTileCenterRelative(x, y);

                MapBehaviour.TileInfo tileInfo = map.tiles[x * boardSize.y + y];

                TileBehaviour tile = Instantiate(tilePrefab, transform.position + delta, Quaternion.identity, transform);
                tile.gridPoint.x = x;
                tile.gridPoint.y = y;
                tile.tileType = tileInfo.tileType;
                tile.subObjectAngle = tileInfo.tileAngle;

                if (tileInfo.pickupPrefab)
                {
                    tile.pickupPrefab = tileInfo.pickupPrefab;
                }

                if (tileInfo.hasBall)
                {
                    BallBehaviour ball = Instantiate(ballPrefab, tile.transform.position + Vector3.back * GameplayConstants.ballRadius, Quaternion.identity, transform);
                    ball.forward = Quaternion.Euler(0.0f, 0.0f, tileInfo.ballForward) * Vector3.right;
                    activeBalls.Add(ball);
                }

                foreach (SpecialRuleBehaviour rule in activeRules)
                {
                    if (x == rule.boardInfo.x && y == rule.boardInfo.y)
                    {
                        float deltaZ = GameplayConstants.ballRadius / 10.0f;
                        if (tile.tileType == TileBehaviour.TileType.Block)
                        {
                            deltaZ *= 21.0f;
                        }

                        rule.transform.position = tile.transform.position + Vector3.back * deltaZ;
                    }
                }

                tile.Initialize();
                activeTiles.Add(tile);
            }
        }
        foreach (SpecialRuleBehaviour rule in activeRules)
        {
            rule.Initialize(this);
        }
    }

    // Use this for initialization
    void Start () {

    }

    public Vector3 GetTileCenterRelative(int x, int y)
    {
        Vector2 fSize = new Vector2((float)boardSize.x - 1, (float)boardSize.y - 1);
        Vector2 start = fSize / -2 * GameplayConstants.tileEdgeSize;
        Vector3 delta = new Vector3(start.x + (GameplayConstants.tileEdgeSize * x), start.y + (GameplayConstants.tileEdgeSize * y), 0.0f);
        return delta;
    }

    public TileBehaviour GetTile(int x, int y)
    {
        return activeTiles[x * boardSize.y + y];
    }

    public TileBehaviour GetTileEntered(BallBehaviour ball)
    {

        Vector2 target = ball.transform.position + ball.forward.normalized * GameplayConstants.tileEdgeSize / 2;
        foreach (TileBehaviour tile in activeTiles)
        {
            Vector2 tileXY = tile.transform.position;
            if (Vector2.Distance(target, tileXY) < GameplayConstants.tileEdgeSize / 2)
            {
                return tile;
            }
        }

        return null;
    }

    public TileBehaviour GetTileInside(BallBehaviour ball)
    {

        Vector2 target = ball.transform.position;
        foreach (TileBehaviour tile in activeTiles)
        {
            Vector2 tileXY = tile.transform.position;
            if (Vector2.Distance(target, tileXY) < GameplayConstants.tileEdgeSize / 2)
            {
                return tile;
            }
        }

        return null;
    }

    // Update is called once per frame
    void Update () {
        activeBalls.RemoveAll((BallBehaviour b) => { return !b; });
	}

    public List<BumperBehaviour> GetBumpers()
    {
        List<BumperBehaviour> result = new List<BumperBehaviour>();
        foreach (TileBehaviour tile in activeTiles)
        {
            foreach (BumperBehaviour bumper in tile.bumpers)
            {
                if (bumper)
                {
                    result.Add(bumper);
                }
            }
        }
        return result;
    }
}
