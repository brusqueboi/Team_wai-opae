using UnityEngine;
using System.Collections;

public class FishAnimationController : MonoBehaviour {

	protected float baseSpeed = 9.75f;
	public float BaseSpeed
	{
		get { return baseSpeed; }
		set { baseSpeed = value; }
	}

	public float speedScale = 1.0f;
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

	public float heightScale = 1.0f;
	public float HeightScale
	{
		get { return heightScale; }
		set { heightScale = value; }
	}

	public float wavelengthScale = 1.0f;
	public float WavelengthScale
	{
		get { return wavelengthScale; }
		set { wavelengthScale = value; }
	}

	private MeshFilter frontSprite;
	private MeshFilter backSprite;
	private Vector3[] centerVertices;
	private float minZ;
	private float maxZ;
	float lateralLength;
	private float randomOffset;

	// Use this for initialization
	void Start () {
		BaseSpeed = 9.75f;
		randomOffset = Mathf.PI * Random.value;
		frontSprite = GameObject.Find(gameObject.name + "/Fish_Sprite_Front").GetComponent<MeshFilter>();
		backSprite = GameObject.Find(gameObject.name + "/Fish_Sprite_Back").GetComponent<MeshFilter>();
		// Establish min and max local z.
		centerVertices = frontSprite.mesh.vertices;
		minZ = float.MaxValue;
		maxZ = float.MinValue;
		for(int i = 0; i < centerVertices.Length; i++)
		{
			if(centerVertices[i].y < minZ)
			{
				minZ = centerVertices[i].y;
			}
			if (centerVertices[i].y > maxZ)
			{
				maxZ = centerVertices[i].y;
			}
		}
		lateralLength = maxZ - minZ;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3[] frontVertices = frontSprite.mesh.vertices;
		Vector3[] backVertices = backSprite.mesh.vertices;
		if(frontVertices.Length != backVertices.Length)
		{
			Debug.Log("Different Front and Back sprite meshes");
		}
		for (int i = 0; i < frontVertices.Length; i++)
		{
			float frontOffset = GetVertexOffset(GetLateralPositionPercent(frontVertices[i]));
			float backOffset = GetVertexOffset(GetLateralPositionPercent(backVertices[i]));
			frontVertices[i] = OffsetVertex(
				new Vector3(frontVertices[i].x, frontVertices[i].y, centerVertices[i].z), frontOffset);
			backVertices[i] = OffsetVertex(new Vector3(backVertices[i].x, backVertices[i].y, centerVertices[i].z), frontOffset);
		}
		frontSprite.mesh.vertices = frontVertices;
		backSprite.mesh.vertices = backVertices;
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
