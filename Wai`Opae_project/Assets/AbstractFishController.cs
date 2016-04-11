using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets;

public abstract class AbstractFishController : MonoBehaviour {

	public GameObject FishObject { get; private set; }
	public string ScientificName { get { return GetScientificName(); }}
	public string CommonName { get { return GetCommonName(); }}
	public Texture2D Bitmap { get { return GetBitmap(); }}

	protected SphereCollider _collider;
	public SphereCollider Collider
	{
		get { return _collider; }

		private set { _collider = value; }
	}
	public Transform Transform
	{
		get { return FishObject.transform; }
	}
	public Vector3 Position
	{
		get { return FishObject.transform.position;	}
		set	{ FishObject.transform.position = value; }
	}
	public Quaternion Rotation
	{
		get { return FishObject.transform.rotation;	}
		set	{ FishObject.transform.rotation = value; }
	}
	public Vector3 Scale
	{
		get	{ return FishObject.transform.localScale; }
		set	{ FishObject.transform.localScale = value; }
	}

	private LinkedList<AbstractFishController> neighbors = new LinkedList<AbstractFishController>();

	public void BeforeUpdate()
	{
		// Do nothing.
	}

	public abstract void UpdateState();

	public abstract Quaternion UpdateRotation();

	public abstract Vector3 UpdatePosition(Quaternion rotation);

	public void AfterUpdate()
	{
		// Do nothing.
	}

	public abstract string GetScientificName();

	public abstract string GetCommonName();

	public abstract Texture2D GetBitmap();

	// Use this for initialization
	void Start () {
		FishObject = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		BeforeUpdate();
		UpdateState();
		Rotation = UpdateRotation();
		Position = UpdatePosition(Rotation);
		AfterUpdate();
	}

	void OnCollisionEnter(Collision collisionInfo)
	{
		GameObject collisionObject = collisionInfo.gameObject;
		AbstractFishController controller = collisionObject.GetComponent<AbstractFishController>();
		if(controller != null)
		{
			Debug.Log("Collision ENTER detected! <AbstractFishController.OnCollisionEnter()>");
			neighbors.AddLast(controller);
		}
	}

	void OnCollisionStay(Collision collisionInfo)
	{

	}

	void OnCollisionExit(Collision collisionInfo)
	{
		GameObject collisionObject = collisionInfo.gameObject;
		AbstractFishController controller = collisionObject.GetComponent<AbstractFishController>();
		if (controller != null)
		{
			Debug.Log("Collision EXIT detected! <AbstractFishController.OnCollisionExit()>");
			neighbors.Remove(controller);
		}
	}
}
