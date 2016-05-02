using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
	public class PreyController : AbstractFishController
	{
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
			get { return velocity; }
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

		public float collisionAvoidanceBias = 0.25f;
		public override float CollisionAvoidanceBias { get { return collisionAvoidanceBias; } }

		public float neighborEvalBias = 0.5f;
		public override float NeighborEvalBias { get { return neighborEvalBias; } }

		public float hitDespawnDelay = 6.0f;
		public override float HitDespawnDelay { get { return hitDespawnDelay; } }

		public float skittishness = 0.5f;
		public float Skittishness {
			get { return skittishness; }
			set { skittishness = value; }
		}

		protected bool attacked = false;

		public override void InitController()
		{
			AnimController.HeightScale = 1.6f;
			Skittishness = UnityEngine.Random.value;
		}

		public override CourseDeviationInfo EvaluateNeighbor(
			AbstractFishController neighbor, CourseDeviationInfo collisionInfo)
		{
			CourseDeviationInfo neighborInfo = new CourseDeviationInfo();
			if(neighbor is RoiController)
			{
				neighborInfo.weight = 1.0f - (Mathf.Clamp(
					Vector3.Distance(Position, neighbor.Position), 0.0f, DetectionRadius) / DetectionRadius);
				neighborInfo.deviation = 
					-Mathf.Sign(Transform.InverseTransformPoint(neighbor.Position).x);
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
			if(SprintEnergy > (MaxSprintEnergy * (1.0f - Skittishness)))
			{
				foreach (AbstractFishController neighbor in neighborCollector.Neighbors)
				{
					if (neighbor.Alive && Vector3.Distance(Position, neighbor.Position) < detectionRadius * Skittishness)
					{
						SprintEnabled = true;
						break;
					}
				}
			}
		}

		public bool OnAttacked(RoiController attacker, float attackAnimDuration)
		{
			if(!attacked)
			{
				attacked = true;
				return false;
			}
			return attacked;
		}
	}
}
