using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
	class Utils
	{

		public static T RequireNonNull<T>(T obj)
		{
			return RequireNonNull(obj, "null reference: " + obj.ToString());
		}

		public static T RequireNonNull<T>(T obj, string message)
		{
			if(obj == null)
			{
				throw new NullReferenceException(message);
			}
			return obj;
		}

		public static float RangeCheck(float value, float min, float max)
		{
			return RangeCheck(value, min, max, String.Format("out of range[{0}-{1}]: {2}", min, max, value));
		}

		public static float RangeCheck(float value, float min, float max, string message)
		{
			if (value < min || value > max)
			{
				throw new ArgumentOutOfRangeException(message);
			}
			return value;
		}

		public static int RangeCheck(int value, int min, int max)
		{
			return RangeCheck(value, min, max, String.Format("out of range[{0}-{1}]: {2}", min, max, value));
		}

		public static int RangeCheck(int value, int min, int max, string message)
		{
			if(value < min || value > max)
			{
				throw new ArgumentOutOfRangeException(message);
			}
			return value;
		}

		public static PreyController GetPreyController(GameObject gameObject)
		{
			return gameObject.GetComponent<PreyController>();
		}

		public static RoiController GetRoiController(GameObject gameObject)
		{
			return gameObject.GetComponent<RoiController>();
		}

		public static AbstractFishController GetAbstractFishController(GameObject gameObject)
		{
			return gameObject.GetComponent<AbstractFishController>();
		}
	}
}
