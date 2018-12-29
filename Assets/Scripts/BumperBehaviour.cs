using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(RaiseBehaviour))]
public class BumperBehaviour : MonoBehaviour {

    [System.Serializable] public class BumperEvent : UnityEngine.Events.UnityEvent<BumperBehaviour> { }
    
    [Header("State")]
    public GameStateBehaviour gameState;
    public bool isHighlighted;
    public bool isBlocked;
    public bool isLockedByRule;

    [Header("Components")]
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    public MeshRenderer meshRenderer;
    public Transform cornerIndicator;
    public RaiseBehaviour raiseBehaviour;
    public ParticleSystem destructionParticlePrefab;

    [Header("Materials")]
    public Material defaultMaterial;
    public Material highlightMaterial;
    public Material raisedMaterial;
    public Material ruleLockedMaterial;
    public Material ruleLockedMaterialRaised;


    // Use this for initialization
    void Start () {
        gameState = GameObject.FindGameObjectWithTag("GameState").GetComponent<GameStateBehaviour>();
        Mesh mesh = ConvertToFlat( GenerateMesh() );
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        raiseBehaviour = GetComponent<RaiseBehaviour>();
	}
	
	// Update is called once per frame
	void Update () {

        // Update material
        Material targetMaterial = defaultMaterial;
        if (isLockedByRule)
        {
            targetMaterial = raiseBehaviour.isRaised ? ruleLockedMaterialRaised : ruleLockedMaterial;
        }
        else if (raiseBehaviour.isRaised)
        {
            targetMaterial = raisedMaterial;
        }
        else if (isHighlighted && CanBeRaised())
        {
            targetMaterial = highlightMaterial;
        }
        if (meshRenderer.material != targetMaterial)
        {
            meshRenderer.material = targetMaterial;
        }
    }

    void OnMouseEnter()
    {
        isHighlighted = true;
    }

    void OnMouseExit()
    {
        isHighlighted = false;
    }

    private void OnMouseDown()
    {
        if (!raiseBehaviour.isRaised && CanBeRaised())
        {
            raiseBehaviour.Raise();
        }
    }

    Mesh GenerateMesh()
    {
        Mesh result = new Mesh();
        result.Clear();
        result.name = name + " Mesh";

        float edgeOffset = (GameplayConstants.tileEdgeSize / 2) - GameplayConstants.ballRadius;
        Vector3 C = new Vector3(-edgeOffset, -edgeOffset);

        Vector3 diagonalTangentPoint = C.normalized * GameplayConstants.ballRadius;
        Vector3 diagonal = Quaternion.Euler(0.0f, 0.0f, -90.0f) * diagonalTangentPoint;

        float deltaY = C.y - diagonalTangentPoint.y;
        float scaleFactor = deltaY / diagonalTangentPoint.y;

        Vector3 A = diagonalTangentPoint + diagonal * scaleFactor;
        Vector3 B = diagonalTangentPoint - diagonal * scaleFactor;

        Vector3 bottomOffset = Vector3.forward * GameplayConstants.ballRadius * 2;

        result.vertices = new Vector3[] { A, B, C, A + bottomOffset, B + bottomOffset, C + bottomOffset };
        result.triangles = new int[] {
            0, 1, 2, // Top
            0, 4, 1,   4, 0, 3, // A-B
            1, 5, 2,   5, 1, 4, // B-C
            2, 3, 0,   3, 2, 5, // A-C
            5, 4, 3 // Bottom
        };


        cornerIndicator.transform.localPosition = C;
        result.RecalculateNormals();
        return result;
    }

    Mesh ConvertToFlat(Mesh mesh)
    {
        Mesh result = new Mesh();
        result.Clear();
        result.name = mesh.name + " Flat";

        int triangles = mesh.triangles.Length;

        Vector3[] newVertices = new Vector3[triangles];
        int[] newTriangles = new int[triangles];

        for (int i = 0; i < triangles; ++i)
        {
            newVertices[i] = mesh.vertices[mesh.triangles[i]];
            newTriangles[i] = i;
        }

        result.vertices = newVertices;
        result.triangles = newTriangles;
        result.RecalculateNormals();
        return result;
    }

    bool CanBeRaised()
    {
        return !isBlocked && gameState.IsRunning();
    }

    public Vector3 GetBounceDirection(Vector3 currentDirection)
    {
        Vector3 cornerNormal = (cornerIndicator.position - transform.position).normalized;
        Vector3 currentNormal = currentDirection.normalized;

        // Corner is 45 degrees, so x and y will be either ~ 0.7f or ~ -0.7

        // Check if outside is hit
        if (Mathf.Abs(-cornerNormal.x - currentNormal.x) < 0.5f)
        {
            return currentDirection * -1.0f;
        }

        if (Mathf.Abs(-cornerNormal.y - currentNormal.y) < 0.5f)
        {
            return currentDirection * -1.0f;
        }

        if (Mathf.Abs(cornerNormal.x - currentNormal.x) < 0.5f)
        {
            return new Vector3(0.0f, -cornerNormal.y, 0.0f).normalized * currentDirection.magnitude;
        }

        if (Mathf.Abs(cornerNormal.y - currentNormal.y) < 0.5f)
        {
            return new Vector3(-cornerNormal.x, 0.0f, 0.0f).normalized * currentDirection.magnitude;
        }

        Debug.LogError("Failed to execute a bounce on bumper");
        return currentDirection;
    }

    public void UnBlock()
    {
        isBlocked = false;
    }

    public void ConditionalBlock(Vector3 blockVector)
    {
        Vector3 cornerNormal = (cornerIndicator.position - transform.position).normalized;
        Vector3 blockNormal = blockVector.normalized;

        if (Mathf.Abs(cornerNormal.x - blockNormal.x) < 0.5f)
        {
            isBlocked = true;
        }

        if (Mathf.Abs(cornerNormal.y - blockNormal.y) < 0.5f)
        {
            isBlocked = true;
        }

    }

    public void Kill()
    {
        ParticleSystem destructionParticles = Instantiate(destructionParticlePrefab, transform.position, transform.rotation);
        ParticleSystem.ShapeModule shape = destructionParticles.shape;
        shape.mesh = meshFilter.mesh;
        destructionParticles.Play();
        Destroy(destructionParticles.gameObject, destructionParticles.main.startLifetime.constant);
        Destroy(gameObject);

    }
}
