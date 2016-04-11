using UnityEngine;
using System.Collections;

public class FishAnimationController : MonoBehaviour {

	private float[] vertOffsets;
	private MeshFilter frontSprite;
	private MeshFilter backSprite;
	private MeshFilter centerSprite;

	// Use this for initialization
	void Start () {
		Debug.Log("MeshFilter: " + gameObject.GetComponent<MeshFilter>());
		//vertOffsets = new float[gameObject.GetComponent<MeshFilter>().mesh.vertices.Length];
	}
	
	// Update is called once per frame
	void Update () {
		float heightMidpoint = gameObject.transform.position.y;
		//for(int i = 0; i < vertOffsets.Length; i++)
		//{
			
		//}
	}
}
