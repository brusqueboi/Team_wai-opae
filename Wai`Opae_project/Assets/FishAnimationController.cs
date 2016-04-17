using UnityEngine;
using System.Collections;

public class FishAnimationController : MonoBehaviour {

	protected float baseSpeed = 9.75f;
	public float BaseSpeed
	{
		get { return baseSpeed; }
		set { baseSpeed = value; }
	}

	protected float speedScale = 1.0f;
	public float SpeedScale
	{
		get { return speedScale; }
		set { speedScale = value; }
	}

	protected float baseHeight = 0.05f;
	public float BaseHeight
	{
		get { return baseHeight; }
		set { baseHeight = value; }
	}

	protected float heightScale = 1.0f;
	public float HeightScale
	{
		get { return heightScale; }
		set { heightScale = value; }
	}

	protected float wavelengthScale = 1.0f;
	public float WavelengthScale
	{
		get { return wavelengthScale; }
		set { wavelengthScale = value; }
	}

	private float[] vertOffsets;
	private MeshFilter frontSprite;
	private MeshFilter backSprite;
	private MeshFilter centerSprite;
	private float minZ;
	private float maxZ;
	float lateralLength;
	private float randomOffset;

	// Use this for initialization
	void Start () {
		BaseSpeed = 9.75f;
		randomOffset = Mathf.PI * Random.value;
		frontSprite = GameObject.Find(gameObject.name + "/Fish_Sprite_Front").GetComponent<MeshFilter>();
		centerSprite = GameObject.Find(gameObject.name + "/Fish_Sprite_Center").GetComponent<MeshFilter>();
		backSprite = GameObject.Find(gameObject.name + "/Fish_Sprite_Back").GetComponent<MeshFilter>();
		Vector3[] centerVerts = centerSprite.mesh.vertices;
		vertOffsets = new float[centerVerts.Length];
		// Establish min and max local z.
		minZ = float.MaxValue;
		maxZ = float.MinValue;
		for(int i = 0; i < centerVerts.Length; i++)
		{
			if(centerVerts[i].y < minZ)
			{
				minZ = centerVerts[i].y;
			}
			if (centerVerts[i].y > maxZ)
			{
				maxZ = centerVerts[i].y;
			}
		}
		lateralLength = maxZ - minZ;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3[] centerVertices = centerSprite.mesh.vertices;
		
		for (int i = 0; i < vertOffsets.Length; i++)
		{
			float offset = GetVertexOffset(GetLateralPositionPercent(centerVertices[i]));
			centerVertices[i] = OffsetVertex(centerVertices[i], offset);
		}
		frontSprite.mesh.vertices = centerVertices;
		backSprite.mesh.vertices = centerVertices;
	}

	private float GetLateralPositionPercent(Vector3 vertexPosition)
	{
		return (vertexPosition.y - minZ) / lateralLength;
	}

	private float GetVertexOffset(float lateralPositionPercent)
	{
		return -Mathf.Sin(randomOffset + (Time.time * (BaseSpeed * SpeedScale)) 
			+ (Mathf.PI * lateralPositionPercent) * WavelengthScale) * (BaseHeight * HeightScale);
	}

	private Vector3 OffsetVertex(Vector3 vertex, float offset)
	{
		return vertex + new Vector3(0.0f, 0.0f, offset);
		
	}

	private Vector3 RotateVertex(Vector3 vertex, Vector3 pivot, Vector3 eulerRotation)
	{
		Vector3 rotationDirection = vertex - pivot;
		rotationDirection = Quaternion.Euler(eulerRotation) * rotationDirection;
		vertex = rotationDirection + pivot;
		return vertex;
	}
}
