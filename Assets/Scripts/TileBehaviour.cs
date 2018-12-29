using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TileBehaviour : MonoBehaviour {

    [Serializable]
    public struct GridPoint
    {
        public int x, y;
    }

    public enum TileType
    {
        Custom = 0,

        BumpersDefault = 1,
        Block = 2,
        Clear = 3,

        Vault = 4,

        FixedBumper = 5,
        FixedBumperClear = 6,

        BumperSingle = 7,

        LockRed = 10,
        LockBlue = 11,
    }

    public Transform meshTransform;
    public TileType tileType;
    public GridPoint gridPoint;

    [Header("Sub objects")]
    public BumperBehaviour bumperPrefab;
    public BlockBehaviour blockPrefab;
    public VaultBehaviour vaultPrefab;
    public PickupBehaviour pickupPrefab;
    public LockBehaviour lockRedPrefab;
    public LockBehaviour lockBluePrefab;
    public float subObjectAngle;

    [Header("Spawned Tracker")]
    public PickupBehaviour pickup;
    public List<BumperBehaviour> bumpers;
    public List<Transform> subObjects;

    [HideInInspector] [SerializeField] private bool isInitialized = false;

    void Start()
    {
        if (!isInitialized)
        {
            Initialize();
        }
    }


    // Use this for initialization
    public void Initialize () {
        isInitialized = true;

        if (meshTransform)
        {
            meshTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * GameplayConstants.tileEdgeSize;
        }

        bool gridRotate = false;

        switch (tileType)
        {
            case TileType.Custom:
                break;

            case TileType.Clear:
                break;

            case TileType.BumpersDefault:
                bumpers.Add(Instantiate(bumperPrefab, transform.position + Vector3.back * GameplayConstants.ballRadius / 10, Quaternion.identity, transform));
                bumpers.Add(Instantiate(bumperPrefab, transform.position + Vector3.back * GameplayConstants.ballRadius / 10, Quaternion.Euler(0.0f, 0.0f, 180.0f), transform));

                gridRotate = true;

                break;

            case TileType.Vault:
                Instantiate(vaultPrefab, transform.position + Vector3.back * GameplayConstants.ballRadius, Quaternion.Euler(0.0f, 0.0f, subObjectAngle), transform);
                break;

            case TileType.FixedBumper:
            case TileType.FixedBumperClear:
                BumperBehaviour fixedBumper = Instantiate(bumperPrefab, transform.position + Vector3.back * GameplayConstants.ballRadius / 10, Quaternion.Euler(0.0f, 0.0f, subObjectAngle), transform);
                fixedBumper.raiseBehaviour.isRaised = true;
                fixedBumper.isLockedByRule = true;
                bumpers.Add(fixedBumper);

                if (tileType == TileType.FixedBumper)
                {
                    bumpers.Add(Instantiate(bumperPrefab, transform.position + Vector3.back * GameplayConstants.ballRadius / 10, Quaternion.Euler(0.0f, 0.0f, subObjectAngle + 180.0f), transform));
                }
                gridRotate = true;

                break;


            case TileType.BumperSingle:
                bumpers.Add(Instantiate(bumperPrefab, transform.position + Vector3.back * GameplayConstants.ballRadius / 10, Quaternion.Euler(0.0f, 0.0f, subObjectAngle), transform));

                break;


            case TileType.Block:
                Instantiate(blockPrefab, transform.position + Vector3.back * GameplayConstants.ballRadius, Quaternion.identity, transform);
                break;


            case TileType.LockRed:
                subObjects.Add(Instantiate(lockRedPrefab, transform.position + Vector3.back * GameplayConstants.ballRadius / 10, Quaternion.identity, transform).transform);
                break;

            case TileType.LockBlue:
                subObjects.Add(Instantiate(lockBluePrefab, transform.position + Vector3.back * GameplayConstants.ballRadius / 10, Quaternion.identity, transform).transform);
                break;

            default:
                Debug.LogError("Unexpected TileType found in TileBehaviour.Start()");
                break;
        }

        if (gridRotate)
        {
            if ((gridPoint.x + gridPoint.y) % 2 == 1)
            {
                transform.Rotate(0.0f, 0.0f, 90.0f);
            }
        }

        if (pickupPrefab)
        {
            pickup = Instantiate(pickupPrefab, transform.position + Vector3.back * GameplayConstants.ballRadius, Quaternion.identity, transform);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UnBlock()
    {
        foreach (BumperBehaviour bumper in bumpers)
        {
            bumper.UnBlock();
        }
    }

    public void ConditionalBlock(Vector3 blockVector)
    {
        foreach (BumperBehaviour bumper in bumpers)
        {
            if (bumper) bumper.ConditionalBlock(blockVector);
        }
    }
}
