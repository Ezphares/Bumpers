using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialRuleBehaviour : MonoBehaviour {

    [System.Serializable]
    public struct BoardInfo
    {
        public int x, y;
    }

    public Transform graphicsTransform;
    public BoardBehaviour boardAffected;
    public float tickTime;

    [Header("Visuals")]
    public BoardInfo boardInfo;
    
    public void Initialize(BoardBehaviour board)
    {
        boardAffected = board;
        OnInitialize(board);
    }

    public virtual void OnInitialize(BoardBehaviour board)
    {

    }

    public virtual bool HasCausedDefeat()
    {
        return false;
    }

    public virtual void OnTick()
    {

    }
}
