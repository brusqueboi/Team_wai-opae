using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets;

public abstract class AbstractFishController : MonoBehaviour {

	public const float DIRECTION_LEFT = -45.0f;
	public const float DIRECTION_FORWARD = 0.0f;
	public const float DIRECTION_RIGHT = 45.0f;
	public const float DIRECTION_LEFT_REAR = -135.0f;
	public const float DIRECTION_RIGHT_REAR = 135.0f;

	public const float RAYCAST_REDRAW_DELAY = 0.25f;
	private float lastFrontRaycastRedraw = 0.0f;
	private float lastRearRaycastRedraw = 0.0f;

	public GameObject FishObject { get; private set; }
	public abstract string ScientificName { get; }
	public abstract string CommonName { get; }
	public abstract Texture2D Bitmap { get; }
	public abstract float CollisionAvoidanceDist { get; }

	public abstract float Velocity { get; protected set; }
	public abstract float SprintVelocity { get; }
	public abstract float NormalVelocity { get; }
	public abstract float Acceleration { get; }
	public abstract bool SprintEnabled { get; protected set; }
	public abstract float MaxSprintEnergy { get; }
	public abstract float SprintEnergy { get; protected set; } // Usage rate: 1 unit per second.
	public abstract float TurnRate { get; protected set; }
	public abstract float NormalTurnRate { get; }
	public abstract float SprintTurnRate { get; }

	public FishAnimationController AnimController { get; private set; }

	protected float maxRotation;

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

	public void UpdateRotation()
	{
		float direction = GetCollisionAvoidanceDir();
		direction = Mathf.Clamp(UpdateDirection(direction), -maxRotation, maxRotation);
		Transform.Rotate(new Vector3(0.0f, direction, 0.0f));
	}

	public void UpdatePosition()
	{
		SprintEnabled = (SprintEnergy > 0.0f ? SprintEnabled : false);
		// Update velocity.
		if (Velocity != (SprintEnabled ? SprintVelocity : NormalVelocity))
		{
			float velDiff = (SprintEnabled ? SprintVelocity : NormalVelocity) - Velocity;
			Velocity = Mathf.Clamp(
				(Velocity + (Mathf.Sign(velDiff) * (Acceleration * Time.deltaTime))), 
				NormalVelocity, SprintVelocity);
		}

		if(AnimController != null)
		{
			AnimController.SpeedScale = Velocity / NormalVelocity;
		}

		// Update position.
		Position += Transform.forward * Time.deltaTime * Velocity;
	}

	public void AfterUpdate()
	{
		// Do nothing.
	}

	public abstract float UpdateDirection(float collisionAvoidanceDir);

	protected float GetCollisionAvoidanceDir()
	{
		// Cast a ray forward, left, and right.
		bool drawRaycast = Time.time - lastFrontRaycastRedraw > RAYCAST_REDRAW_DELAY;
		if(drawRaycast)
		{
			lastFrontRaycastRedraw = Time.time;
		}
		float forwardDist = GetRaycastDistance(DIRECTION_FORWARD, CollisionAvoidanceDist, drawRaycast);
		float leftDist = GetRaycastDistance(DIRECTION_LEFT, CollisionAvoidanceDist, drawRaycast);
		float rightDist = GetRaycastDistance(DIRECTION_RIGHT, CollisionAvoidanceDist, drawRaycast);
		
		// Choose collision avoidance direction.
		if (forwardDist == leftDist && leftDist == rightDist)
		{	// No obstacles ahead.
			return DIRECTION_FORWARD;
		}
		else if (forwardDist <= leftDist && forwardDist <= rightDist)
		{   // Obstacle straight ahead.
			float distancePercent = (CollisionAvoidanceDist - forwardDist) / CollisionAvoidanceDist;
			float rotation = (leftDist > rightDist ? -maxRotation : maxRotation) * distancePercent;
			return rotation;
		}
		else if (forwardDist >= leftDist && forwardDist >= rightDist)
		{   // Obstacles on both left and right.
			bool drawRearRaycast = Time.time - lastRearRaycastRedraw > RAYCAST_REDRAW_DELAY;
			if (drawRearRaycast)
			{
				lastRearRaycastRedraw = Time.time;
			}
			float leftRearDist = GetRaycastDistance(DIRECTION_LEFT_REAR, CollisionAvoidanceDist, drawRearRaycast);
			float rightRearDist = GetRaycastDistance(DIRECTION_RIGHT_REAR, CollisionAvoidanceDist, drawRearRaycast);
			float distancePercent = 
				(CollisionAvoidanceDist - Mathf.Min(leftDist, rightDist)) / CollisionAvoidanceDist;
			return (leftRearDist > rightRearDist ? -maxRotation : maxRotation) * distancePercent;
		} else if (rightDist <= forwardDist && rightDist <= leftDist)
		{   // Obstacle on the right.
			float distancePercent = (CollisionAvoidanceDist - rightDist) / CollisionAvoidanceDist;
			float rotation = maxRotation * distancePercent;
			rotation = -rotation; // Negate to turn left.
			return rotation;
		} else if (leftDist <= forwardDist && leftDist <= rightDist) 
		{   // Obstacle on the left.
			float distancePercent = (CollisionAvoidanceDist - leftDist) / CollisionAvoidanceDist;
			float rotation = maxRotation * distancePercent;
			return rotation;
		} else
		{
			throw new System.Exception(string.Format("Unhandled distance value set [L,C,R]: [{0},{1},{2}]", 
				leftDist, forwardDist, rightDist));
		}
	}

	protected float GetRaycastDistance(float direction, float maxDistance, bool debugDrawRay)
	{
		Vector3 directionVector = DirectionToVector(direction);
		maxDistance *= Velocity / NormalVelocity;
		if(debugDrawRay)
		{
			Debug.DrawRay(Position, Vector3.Scale(Vector3.Normalize(directionVector),
			new Vector3(maxDistance, maxDistance, maxDistance)),
			Color.green, Time.deltaTime * 2.0f, true);
		}
		RaycastHit raycastHit;
		Physics.Raycast(Position, directionVector, out raycastHit, maxDistance);
		return (raycastHit.distance > 0 ? 
			raycastHit.distance : maxDistance );
	}

	protected Vector3 DirectionToVector(float direction)
	{
		Vector3 directionVector = Transform.TransformDirection(Vector3.forward);
		if (direction != 0.0f)
		{
			directionVector = Quaternion.AngleAxis(direction, Vector3.up) * directionVector;
		}
		return directionVector;
	}

	private void UpdateInternalState()
	{
        maxRotation = (SprintEnabled ? SprintTurnRate : NormalTurnRate) * Time.deltaTime;
		// Update sprintEnabled.
		SprintEnergy = Mathf.Clamp(
			(SprintEnergy + (SprintEnabled ? -1.0f : 1.0f) * Time.deltaTime),
			0.0f, MaxSprintEnergy);
		SprintEnabled = (SprintEnergy > 0.0f ? SprintEnabled : false);
		TurnRate = (SprintEnabled ? SprintTurnRate : NormalTurnRate);
	}

	// Use this for initialization
	void Start () {
		FishObject = this.gameObject;
		AnimController = FishObject.GetComponent<FishAnimationController>();
	}
	
	// Update is called once per frame
	void Update () {
		BeforeUpdate();
		UpdateInternalState();
		UpdateState();
		UpdateRotation();
		UpdatePosition();
		AfterUpdate();
	}

	void OnTriggerEnter(Collider collisionInfo)
	{
		Debug.Log(string.Format("Trigger ENTER detected: {0} <AbstractFishController.OnTriggerEnter()>", collisionInfo.name));
		if (collisionInfo.name == "Respawn Trigger")
		{
			Debug.Log("Respawn: gameobject");
			//SpawnController.Controller.Respawn(FishObject);
		}
		else
		{
			AbstractFishController controller = Utils.GetAbstractFishController(collisionInfo.gameObject);
			if (controller != null)
			{
				neighbors.AddLast(controller);
			}
		}	
	}

	void OnTriggerExit(Collider collisionInfo)
	{
		Debug.Log("Trigger EXIT detected! <AbstractFishController.OnTriggerEnter()>");
		AbstractFishController controller = Utils.GetAbstractFishController(collisionInfo.gameObject);
		if (controller != null)
		{
			neighbors.Remove(controller);
		}
	}

	void OnCollisionEnter(Collision collisionInfo)
	{
		Debug.Log("Collision ENTER detected! <AbstractFishController.OnCollisionEnter()>");
	}

	void OnCollisionExit(Collision collisionInfo)
	{
		Debug.Log("Collision EXIT detected! <AbstractFishController.OnCollisionEnter()>");
	}
}
