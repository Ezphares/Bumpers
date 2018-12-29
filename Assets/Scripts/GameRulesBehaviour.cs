using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRulesBehaviour : MonoBehaviour {

    [SerializeField]
    public struct BallMovement
    {
        public BallBehaviour target;
        public Vector3 start, end;
    }


    [Header("Data References")]
    public BoardBehaviour board;

    [Header("Level Settings")]
    public float halfTurnTime;

    [Header("State")]
    public GameStateBehaviour gameState;

    [Header("UI Elements")]
    public Transform victoryUI;
    public Transform defeatUI;
    public Transform pauseUI;
    public UnityEngine.UI.Button resumeButton;
    public UnityEngine.UI.Text countDownText;

    [SerializeField] [HideInInspector] private float halfTurnDeltaT;
    [SerializeField] [HideInInspector] private List<BallMovement> ballMovement;
    [SerializeField] [HideInInspector] private bool isOffTurn;
    [SerializeField] /*[HideInInspector]*/ private GameObject[] goalPickups;
    [SerializeField] [HideInInspector] private int countdown;

    // Use this for initialization
    void Start () {
        ballMovement = new List<BallMovement>();
        isOffTurn = false;
        gameState.isPaused = false;
        halfTurnDeltaT = 2.0f;

        gameState.gameProgress = GameStateBehaviour.GameProgress.Pregame;

        countdown = 4;

        foreach (SpecialRuleBehaviour rule in board.activeRules)
        {
            rule.tickTime = halfTurnTime;
        }

        resumeButton.onClick.AddListener(() =>
        {
            pauseUI.gameObject.SetActive(false);
            gameState.isPaused = false;
        });

        CountDown();
	}
	
	// Update is called once per frame
	void Update ()
    { 
        if (IsRunning())
        {
            if (Input.GetAxis("Pause") > 0.5f)
            {
                Pause();
                return;
            }



            halfTurnDeltaT += Time.deltaTime / halfTurnTime;

            foreach (BallMovement movement in ballMovement)
            {
                if (!movement.target)
                {
                    continue;
                }

                movement.target.transform.position = Vector3.Lerp(movement.start, movement.end, halfTurnDeltaT);
            }

            if (halfTurnDeltaT >= 1.0f)
            {
                Tick();
            }
        }

	}

    void Tick()
    {
        if (!CheckEndConditions())
        {
            ExecuteTickLogic();
            StartTickMovement();
        }
    }

    bool CheckEndConditions()
    {
        // Victory check
        bool hasWon = true;
        foreach (GameObject goal in goalPickups)
        {
            if (goal)
            {
                hasWon = false;
            }
        }

        if (hasWon)
        {
            Win();
            return true;
        }

        // Defeat check
        if (board.activeBalls.Count == 0)
        {
            Lose();
            return true;
        }

        foreach (SpecialRuleBehaviour rule in board.activeRules)
        {
            if (rule.HasCausedDefeat())
            {
                Lose();
                return true;
            }
        }

        return false;
    }

    void ExecuteTickLogic()
    {
        foreach (SpecialRuleBehaviour rule in board.activeRules)
        {
            rule.OnTick();
        }

        foreach (TileBehaviour tile in board.activeTiles)
        {
            tile.UnBlock();
        }

        // Run different logic based on whether the balls are in the middle of a tile or on the edge
        if (isOffTurn)
        {
            ExecuteTickOffTurn();
        }
        else
        {
            ExecuteTickMainTurn();
        }

        isOffTurn = !isOffTurn;
    }

    // To be called on ticks where the balls are in the middle of a tile
    void ExecuteTickMainTurn()
    {
        foreach (BallBehaviour ball in board.activeBalls)
        {
            TileBehaviour tile = board.GetTileInside(ball);
            if (!tile)
            {
                Debug.LogError("Ball outside board");
                gameState.isPaused = true;
                continue;
            }

            switch (tile.tileType)
            {
                case TileBehaviour.TileType.Vault:
                    ball.forward *= -1.0f;
                    break;

                default:
                    BounceBall(ball);
                    break;
            }

            tile.ConditionalBlock(ball.forward);

            if (tile.pickup)
            {
                tile.pickup.PickUp(ball);
            }
        }
    }

    // To be called on ticks where the balls are on the edge between two tiles
    void ExecuteTickOffTurn()
    {
        foreach (BallBehaviour ball in board.activeBalls)
        {
            TileBehaviour tile = board.GetTileEntered(ball);
            if (!tile)
            {
                Debug.LogError("Ball leaving board");
                gameState.isPaused = true;
                continue;
            }

            if (tile.tileType == TileBehaviour.TileType.Block)
            {
                ball.forward *= -1.0f;
            }
            else if (tile.tileType == TileBehaviour.TileType.Vault)
            {
                if (!tile.GetComponentInChildren<VaultBehaviour>().CanEnter(ball.forward))
                {
                    ball.forward *= -1.0f;
                }
            }
            else if (tile.tileType == TileBehaviour.TileType.LockRed || tile.tileType == TileBehaviour.TileType.LockBlue)
            {
                if (tile.GetComponentInChildren<RaiseBehaviour>().isRaised)
                {
                    ball.forward *= -1.0f;
                }
            }
            else
            {
                BounceBall(ball);
            }

            tile.ConditionalBlock(ball.forward * -1.0f);
        }
    }

    void BounceBall(BallBehaviour ball)
    {
        Ray bumperRay = new Ray(ball.transform.position + Vector3.forward * GameplayConstants.ballRadius * 0.8f, ball.forward);
        RaycastHit hit;

        if (Physics.Raycast(bumperRay, out hit, GameplayConstants.tileEdgeSize / 2.1f))
        {
            BumperBehaviour bumper = hit.collider.GetComponent<BumperBehaviour>();
            if (bumper)
            {
                ball.forward = bumper.GetBounceDirection(ball.forward);

                if (!bumper.isLockedByRule)
                {
                    bumper.raiseBehaviour.Lower();
                }
            }
        }
    }

    void StartTickMovement()
    {
        halfTurnDeltaT = 0;
        ballMovement.Clear();

        foreach (BallBehaviour ball in board.activeBalls)
        {
            ballMovement.Add(new BallMovement { target = ball, start = ball.transform.position, end = ball.transform.position + ball.forward.normalized * GameplayConstants.tileEdgeSize / 2 });
            ball.forwardIndicatorTransform.gameObject.SetActive(false);
        }
    }

    void CountDown()
    {
        countdown -= 1;
        if (countdown <= 0)
        {
            countDownText.gameObject.SetActive(false);
            StartGame();
        }
        else
        {
            countDownText.text = countdown.ToString();
            countDownText.fontSize = 96 - countdown * 8;
            countDownText.gameObject.SetActive(true);

            Invoke("CountDown", 0.8f);
        }
    }

    void StartGame()
    {
        gameState.gameProgress = GameStateBehaviour.GameProgress.Running;
        goalPickups = GameObject.FindGameObjectsWithTag("Goal");
    }

    void UnPause()
    {
        gameState.isPaused = false;
    }

    void Pause()
    {
        pauseUI.gameObject.SetActive(true);
        gameState.isPaused = true;
    }

    bool IsRunning()
    {
        return gameState.IsRunning();
    }

    void Win()
    {
        gameState.gameProgress = GameStateBehaviour.GameProgress.Ended;
        victoryUI.gameObject.SetActive(true);
    }

    void Lose()
    {
        gameState.gameProgress = GameStateBehaviour.GameProgress.Ended;
        defeatUI.gameObject.SetActive(true);
    }
}
