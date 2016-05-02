using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
	public class RoiController : AbstractFishController
	{
		public event EventHandler<GameModel.PreyConsumedEventArgs> PreyConsumed;

		public float acceleration = 10;
		public override float Acceleration { get { return acceleration; } }

		public Texture2D bitmap;
		public override Texture2D Bitmap { get { return bitmap; } }

		public string commonName = "";
		public override string CommonName { get { return commonName; } }

		public bool alive = true;
		public override bool Alive
		{
			get { return alive; }
			set { alive = value; }
		}

		public float maxCollisionAvoidanceDistance = 7.0f;
		public override float CollisionAvoidanceDist { get { return maxCollisionAvoidanceDistance; } }

		public float maxSprintEnergy = 5.0f;
		public override float MaxSprintEnergy { get { return maxSprintEnergy; } }

		public float normalTurnRate = 180.0f;
		public override float NormalTurnRate { get { return normalTurnRate; } }

		public float turnRate = 180.0f;
		public override float TurnRate
		{
			get { return turnRate; }
			protected set { turnRate = value; }
		}

		public float sprintTurnRate = 315.0f;
		public override float SprintTurnRate { get { return sprintTurnRate; } }

		public float normalVelocity = 1.6f;
		public override float NormalVelocity { get { return normalVelocity; } }

		public string scientificName = "";
		public override string ScientificName { get { return scientificName; } }

		public bool sprintEnabled = false;
		public override bool SprintEnabled
		{
			get { return sprintEnabled; }
			protected set { sprintEnabled = value; }
		}

		public float sprintEnergy = 0.0f;
		public override float SprintEnergy
		{
			get { return sprintEnergy; }
			protected set { sprintEnergy = value; }
		}

		public float sprintVelocity = 2.9f;
		public override float SprintVelocity { get { return sprintVelocity; } }

		public float velocity = 0.0f;
		public override float Velocity
		{
			get	{ return velocity; }
			protected set { velocity = value; }
		}

		public float normalDetectionRadius = 2.5f;
		public override float NormalDetectionRadius { get { return normalDetectionRadius; } }

		public float detectionRadius = 2.5f;
		public override float DetectionRadius
		{
			get { return detectionRadius; }
			protected set { detectionRadius = value; }
		}

		public float collisionAvoidanceBias = 0.15f;
		public override float CollisionAvoidanceBias { get { return collisionAvoidanceBias; } }

		public float neighborEvalBias = -0.15f;
		public override float NeighborEvalBias { get { return neighborEvalBias; } }

		public float hitDespawnDelay = 8.0f;
		public override float HitDespawnDelay { get { return hitDespawnDelay; } }

		public float agressiveness = 0.5f;
		public float Agressiveness
		{
			get { return agressiveness; }
			set { agressiveness = value; }
		}

		public float maxAttackRange = 1.0f;
		public float MaxAttackRange
		{
			get { return maxAttackRange; }
			set { maxAttackRange = value; }
		}

		public float attackDuration = 5.0f;
		public float AttackDuration
		{
			get { return attackDuration; }
			set { attackDuration = value; }
		}

		public float attackInterstitialDelay = 4.0f;
		public float AttackInterstitialDelay
		{
			get { return attackInterstitialDelay; }
			set { attackInterstitialDelay = value; }
		}

		protected float lastAttackTime = 0.0f;
		protected List<PreyController> attackableNeighbors = new List<PreyController>();

		public override void InitController()
		{
			AnimController.HeightScale = 1.75f;
			Agressiveness = UnityEngine.Random.value;
		}

		public override CourseDeviationInfo EvaluateNeighbor(
			AbstractFishController neighbor, CourseDeviationInfo collisionInfo)
		{
			CourseDeviationInfo neighborInfo = new CourseDeviationInfo();
			if (neighbor is PreyController)
			{
				Vector3 neighborRelativePos = Transform.InverseTransformPoint(neighbor.Position);
				if (neighborRelativePos.z >= 0.0f)
				{
					neighborInfo.weight = (1.0f - (Mathf.Clamp(
					Vector3.Distance(Position, neighbor.Position), 0.0f, DetectionRadius) / DetectionRadius));
				}
				else
				{
					neighborInfo.weight = (1.0f - (Mathf.Clamp(
					Vector3.Distance(Position, neighbor.Position), 0.0f, DetectionRadius / 2.0f) / DetectionRadius / 2.0f));
				}
				
                neighborInfo.deviation = (neighborRelativePos.x == 0.0f ? 0.0f : Mathf.Sign(neighborRelativePos.x));

				if (!GameModel.Model.GameSuspended && Vector3.Distance(Position, neighbor.Position) <= maxAttackRange)
				{
					attackableNeighbors.Add(neighbor as PreyController);
				}
			}
			else
			{
				neighborInfo.deviation = collisionInfo.deviation;
				neighborInfo.weight = 0.0f;
			}
			return neighborInfo;
		}

		public override void UpdateState()
		{
			if (SprintEnergy > (MaxSprintEnergy * (1.0f - Agressiveness)))
			{
				foreach(AbstractFishController neighbor in neighborCollector.Neighbors)
				{
					if(neighbor.Alive && Vector3.Distance(Position, neighbor.Position) < detectionRadius * Agressiveness)
					{
						SprintEnabled = true;
						break;
					}
				}
			}
		}

		public override void AfterUpdate()
		{
			float elapsedInterstitialTime = Time.time - lastAttackTime;
			if (elapsedInterstitialTime > AttackInterstitialDelay && elapsedInterstitialTime > AttackDuration )
			{
				float minDist = float.MaxValue;
				PreyController nearestNeighbor = null;
				foreach (PreyController attackable in attackableNeighbors)
				{
					if (!object.Equals(attackable, null) && attackable.Alive
						&& Vector3.Distance(Position, attackable.Position) < minDist
						&& Transform.InverseTransformPoint(attackable.Position).z > 1.0f)
					{
						nearestNeighbor = attackable;
					}
				}
				if (nearestNeighbor != null)
				{
					AttackPrey(nearestNeighbor);
				}
			}
			attackableNeighbors.Clear();
		}

		protected void AttackPrey(PreyController target)
		{
			bool alreadyAttacked = target.OnAttacked(this, AttackDuration);
			if (!alreadyAttacked)
			{
				lastAttackTime = Time.time;
				updatesSuspended = true;
				StartCoroutine(AttackAnimation(target));
			}
		}

		protected void FirePreyConsumed(PreyController consumed)
		{
			if(PreyConsumed != null)
			{
				PreyConsumed.Invoke(this, new GameModel.PreyConsumedEventArgs(consumed, this));
			}
		}

		private IEnumerator AttackAnimation(PreyController target)
		{
			float animStartTime = Time.time;
			Vector3 startingPosition = Position;
			float originalAnimSpeed = AnimController.SpeedScale;
			AnimController.SpeedScale *= 1.5f;
			// 1/5 of animation to catch up to target.
			while (target.Alive && Time.time - animStartTime < (AttackDuration / 5.0f) * 2.0f)
			{
				Vector3 newPos = Vector3.Lerp(startingPosition, target.Position, (Time.time - animStartTime) / (AttackDuration / 3.0f));
				newPos.y = startingPosition.y;
				Position = newPos;
				Transform.LookAt(target.Position);
				Transform.eulerAngles = new Vector3(0.0f, Transform.eulerAngles.y, 0.0f);
				yield return new WaitForFixedUpdate();
			}
			if(target.Alive)
			{
				target.Alive = false;
				// 1/5 of animation thrashing fish around.
				target.transform.SetParent(Transform);
				target.transform.localPosition = new Vector3(0.0f, 0.0f, 0.4f);
				target.AnimController.SpeedScale *= 2.5f;
				AnimController.SpeedScale = originalAnimSpeed * 2.5f;
				updatesSuspended = false;
				yield return new WaitForSeconds(AttackDuration / 5.0f);

				// 1/5 of animation fish free-floating.
				target.Transform.SetParent(null);
				target.GetComponent<Rigidbody>().isKinematic = false;
				Vector3 startingRoiRotation = Rotation.eulerAngles;
				float roiStartingDepth = Position.y;
				float roiStartingAnimSpeed = AnimController.SpeedScale;
				float roiResetStartTime = Time.time;
				float preyAnimStartSpeed = target.AnimController.SpeedScale;
				while (Time.time - animStartTime < (AttackDuration / 5.0f) * 3.0f)
				{
					float roiResetProgress = (Time.time - roiResetStartTime) / (AttackDuration / 5.0f);
					Transform.eulerAngles = new Vector3(Mathf.Lerp(Transform.eulerAngles.x, 0.0f, roiResetProgress),
						transform.eulerAngles.y, Mathf.Lerp(Transform.eulerAngles.z, 0.0f, roiResetProgress));
					Position = new Vector3(Position.x, 
						Mathf.Lerp(roiStartingDepth, startingPosition.y, roiResetProgress));
					AnimController.SpeedScale = Mathf.Lerp(roiStartingAnimSpeed, originalAnimSpeed, roiResetProgress);
					target.AnimController.SpeedScale = Mathf.Lerp(preyAnimStartSpeed, 0.0f, roiResetProgress);
					yield return new WaitForFixedUpdate();
                }
				Transform.eulerAngles = new Vector3(0.0f, Transform.eulerAngles.y, 0.0f);
				Position = new Vector3(Position.x, startingPosition.y, Position.z);
				AnimController.SpeedScale = originalAnimSpeed;

				// 2/5 of animation floating fish to surface.
				Rigidbody preyRigidBody = target.GetComponent<Rigidbody>();
				preyRigidBody.velocity = preyRigidBody.velocity.normalized * 0.25f;
				float preyStartingDepth = target.Position.y;
				float floatStartTime = Time.time;
				while (Time.time - animStartTime < AttackDuration)
				{
					Vector3 newPreyPos = target.Position;
					newPreyPos.y = Mathf.Lerp(preyStartingDepth, 3.5f, (Time.time - floatStartTime) / (AttackDuration / 2.0f));
					target.Position = newPreyPos;
					yield return new WaitForFixedUpdate();
				}
			}
			FirePreyConsumed(target);
			GameObject.Destroy(target.gameObject);

		}
	}
}
