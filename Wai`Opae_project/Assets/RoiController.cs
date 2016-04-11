using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
	public class RoiController : AbstractFishController
	{
		public string commonName;
		public string scientificName;
		public Texture2D bitmap;

		public override Texture2D GetBitmap()
		{
			return bitmap;
		}

		public override string GetCommonName()
		{
			return commonName;
		}

		public override string GetScientificName()
		{
			return scientificName;
		}

		public override Vector3 UpdatePosition(Quaternion rotation)
		{
			return Position;
		}

		public override Quaternion UpdateRotation()
		{
			return Rotation;
		}

		public override void UpdateState()
		{
			// Do nothing.
		}
	}
}
