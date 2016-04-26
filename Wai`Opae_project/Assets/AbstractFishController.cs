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

	public const float RAYCAST_REDRAW_DELAY = 3.0f;
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
	public abstract float NormalDetectionRadius { get; }
	public abstract float DetectionRadius { get; protected set; }
	public abstract float CollisionAvoidanceBias { get; }
	public abstract float NeighborEvalBias { get; }

	public FishAnimationController AnimController { get; private set; }

	protected ProjectileController hitProjectile = null;
	protected float hitDespawnDelay = 3.0f;
	protected float hitTime = 0.0f;

	protected SphereCollider detectionTrigger;
	public SphereCollider DetectionTrigger
	{
		get { return detectionTrigger; }

		private set { detectionTrigger = value; }
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

	protected LinkedList<AbstractFishController> neighbors = new LinkedList<AbstractFishController>();
	private const float BaseBiasIntensity = 0.95f;

	public abstract void InitController();

	public void BeforeUpdate()
	{
		// Do nothing.
	}

	public abstract void UpdateState();

	public void UpdateRotation()
	{
		CourseDeviationInfo collisionInfo = GetCollisionAvoidanceInfo();
		float updatedDirection = Mathf.Clamp(UpdateDirection(collisionInfo), -1.0f, 1.0f);
		float maxRotationDelta = (SprintEnabled ? SprintTurnRate : NormalTurnRate) * Time.deltaTime;
		Transform.Rotate(new Vector3(0.0f, maxRotationDelta * updatedDirection, 0.0f));
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

	public float UpdateDirection(CourseDeviationInfo collisionInfo)
	{
		if(neighbors.Count == 0)
		{
			return collisionInfo.deviation;
		}

		float weightedAvgNum = 0.0f;
		float weightedAvgDenom = 0.0f;
		foreach(AbstractFishController neighborController in neighbors)
		{
			CourseDeviationInfo neighborInfo = EvaluateNeighbor(neighborController, collisionInfo);
			neighborInfo.weight = ApplyWeightBias(neighborInfo.weight, NeighborEvalBias);
			// Interpolates between neighbor deviation and collision avoidance deviation based on 
			// collision avoidance weight.
			neighborInfo.deviation = (neighborInfo.weight * neighborInfo.deviation) 
				+ ((1.0f - neighborInfo.weight) * ((collisionInfo.deviation * collisionInfo.weight) 
				+ ((1.0f - collisionInfo.weight) * neighborInfo.deviation)));
			weightedAvgNum += neighborInfo.weight * neighborInfo.deviation;
			weightedAvgDenom += neighborInfo.weight;
		}
		if (weightedAvgDenom == 0.0f)
		{
			return collisionInfo.deviation;
		}
		else
		{
			// Add in collision deviation and weight.
			weightedAvgNum += (collisionInfo.weight * weightedAvgDenom) * collisionInfo.deviation;
			weightedAvgDenom += collisionInfo.weight * weightedAvgDenom;
			// Interpolate between collision and ai deviations according to collision avoidance weight.
			float avgCourseDeviation = Mathf.Clamp(weightedAvgNum / weightedAvgDenom, -1.0f, 1.0f);
			float courseDeviation = (collisionInfo.deviation * collisionInfo.weight)
				+ (avgCourseDeviation * (1.0f - collisionInfo.weight));
			return Mathf.Clamp(avgCourseDeviation, -1.0f, 1.0f);
		}
	}

	public abstract CourseDeviationInfo EvaluateNeighbor(
		AbstractFishController neighbor, CourseDeviationInfo collisionAvoidanceInfo);

	public void Hit(ProjectileController projectile)
	{
		hitProjectile = projectile;
		hitTime = Time.time;
		Velocity = 0.0f;
	}

	protected float ApplyWeightBias(float percentValue, float biasIntensity)
	{
		float bias = 0.0f;
		if (Mathf.Abs(percentValue) > 1.0f)
		{
			bias = Mathf.Sign(percentValue) * 1.0f;
		}
		else
		{   // Emphasizes highly-weighted values and suppresses low-weighted values.
			// Allows stronger response to more important course deviations.
			float biasCeiling = (BaseBiasIntensity * (1.0f / (1.0f + biasIntensity))) * Mathf.PI;
			bias = Mathf.Clamp(Arctanh(percentValue), -biasCeiling, biasCeiling) / biasCeiling;
		}
		return bias;
	}

	protected float Arctanh(float value)
	{	
		return (Mathf.Log(1.0f + value) - Mathf.Log(1.0f - value)) / 2.0f;
	}

	protected CourseDeviationInfo GetCollisionAvoidanceInfo()
	{
		CourseDeviationInfo collisionInfo = new CourseDeviationInfo();
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
			collisionInfo.deviation = DIRECTION_FORWARD;
			collisionInfo.weight = 0.0f;
		}
		else if (forwardDist <= leftDist && forwardDist <= rightDist)
		{   // Obstacle straight ahead.
			collisionInfo.weight = (CollisionAvoidanceDist - forwardDist) / CollisionAvoidanceDist;
			collisionInfo.deviation = 
				(leftDist > rightDist ? -1.0f : 1.0f) * collisionInfo.weight;
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
			collisionInfo.weight = 
				(CollisionAvoidanceDist - Mathf.Min(leftDist, rightDist)) / CollisionAvoidanceDist;
			collisionInfo.deviation = 
				(leftRearDist > rightRearDist ? -1.0f : 1.0f) * collisionInfo.weight;
		} else if (rightDist <= forwardDist && rightDist <= leftDist)
		{   // Obstacle on the right.
			collisionInfo.weight = (CollisionAvoidanceDist - rightDist) / CollisionAvoidanceDist;
			collisionInfo.deviation = -(1.0f * collisionInfo.weight); // Negate to turn left.
		} else if (leftDist <= forwardDist && leftDist <= rightDist) 
		{   // Obstacle on the left.
			collisionInfo.weight = (CollisionAvoidanceDist - leftDist) / CollisionAvoidanceDist;
			collisionInfo.deviation = 1.0f * collisionInfo.weight;
		} else
		{
			throw new System.Exception(string.Format("Unhandled distance value set [L,C,R]: [{0},{1},{2}]", 
				leftDist, forwardDist, rightDist));
		}
		//collisionInfo.weight = ApplyWeightBias(collisionInfo.weight, CollisionAvoidanceBias);
		collisionInfo.weight = Mathf.Clamp(collisionInfo.weight * (1.0f + CollisionAvoidanceBias), -1.0f, 1.0f);
		return collisionInfo;
	}

	protected float GetRaycastDistance(float direction, float maxDistance, bool debugDrawRay)
	{
		Vector3 directionVector = DirectionToVector(direction);
		maxDistance *= Velocity / NormalVelocity;
		RaycastHit raycastHit;
		Physics.Raycast(Position, directionVector, out raycastHit, maxDistance);
		float distance = (raycastHit.distance > 0 ? raycastHit.distance : maxDistance);
		if (debugDrawRay && distance != maxDistance)
		{
			Debug.DrawRay(Position, Vector3.Scale(Vector3.Normalize(directionVector),
			new Vector3(maxDistance, maxDistance, maxDistance)),
			Color.Lerp(Color.green, Color.red, distance / maxDistance), Time.deltaTime * 2.0f, true);
		}
		return distance;
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
		// Update sprintEnabled.
		SprintEnergy = Mathf.Clamp(
			(SprintEnergy + (SprintEnabled ? -1.0f : 1.0f) * Time.deltaTime),
			0.0f, MaxSprintEnergy);
		SprintEnabled = (SprintEnergy > 0.0f ? SprintEnabled : false);
		TurnRate = (SprintEnabled ? SprintTurnRate : NormalTurnRate);
		DetectionTrigger.radius = NormalDetectionRadius * (Velocity / NormalVelocity);
		if(hitProjectile != null)
		{
			if(Time.time - hitTime > hitDespawnDelay)
			{
				GameObject.Destroy(FishObject);
				GameObject.Destroy(hitProjectile);
				hitProjectile = null;
			}
			else
			{
				hitProjectile.transform.position = Position;
				hitProjectile.transform.rotation = Rotation;
			}
		}
	}

	// Use this for initialization
	void Start () {
		FishObject = this.gameObject;
		AnimController = FishObject.GetComponent<FishAnimationController>();
		DetectionTrigger = FishObject.GetComponent<SphereCollider>();
		DetectionTrigger.radius = NormalDetectionRadius;
		InitController();
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
		AbstractFishController controller = Utils.GetAbstractFishController(collisionInfo.gameObject);
		if (controller != null && !neighbors.Contains(controller))
		{
			neighbors.AddLast(controller);
		}
	}

	void OnTriggerExit(Collider collisionInfo)
	{
		AbstractFishController controller = Utils.GetAbstractFishController(collisionInfo.gameObject);
		if (controller != null)
		{
			neighbors.Remove(controller);
		}
	}

	public struct CourseDeviationInfo
	{
		public float deviation;
		public float weight;
	}
}
