using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
	class Utils
	{
		private const float BaseBiasIntensity = 0.95f;

		public static float Arctanh(float value)
		{
			return (Mathf.Log(1.0f + value) - Mathf.Log(1.0f - value)) / 2.0f;
		}

		public static float ArctanFilterValue(float percentValue, float biasIntensity)
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

		public static bool AnyButtonPressed(int playerId)
		{
			if(!GameModel.Model.GetPlayer(playerId).Enabled)
			{
				return false;
			}
			ControllerModel playerController = GameModel.Model.GetPlayer(playerId).Controller;
			return GameModel.Model.GetPlayer(playerId).Enabled && (playerController.AButton || playerController.BButton
				|| playerController.XButton || playerController.YButton || playerController.LeftBumper
				|| playerController.RightBumper || playerController.LeftTrigger > playerController.DeadZoneRadius 
				|| playerController.RightTrigger > playerController.DeadZoneRadius);
		}

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
