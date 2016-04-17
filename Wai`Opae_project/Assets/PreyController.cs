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

		public float normalVelocity = 2;
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

		public float sprintVelocity = 4.0f;
		public override float SprintVelocity { get { return sprintVelocity; } }

		public float velocity = 0.0f;
		public override float Velocity
		{
			get { return velocity; }
			protected set { velocity = value; }
		}

		public override float UpdateDirection(float collisionAvoidanceDir)
		{
			return collisionAvoidanceDir;
		}

		public override void UpdateState()
		{
			if(SprintEnergy == MaxSprintEnergy && (int) (UnityEngine.Random.value * 100.0f / 2.0f) == 0)
			{
				SprintEnabled = true;
			}
		}
	}
}
