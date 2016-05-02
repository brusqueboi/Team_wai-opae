using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerModel {

	protected static readonly ButtonUIDs BaseButtonIds = 
		new ButtonUIDs("LeftJoystickHorizontal", "LeftJoystickVertical", "RightJoystickHorizontal", 
			"RightJoystickVertical", "LeftTrigger", "RightTrigger", "LeftBumper", "RightBumper", "StartButton", 
			"BackButton", "AButton", "BButton", "XButton", "YButton", "HorizontalDPAD", "VerticalDPAD");

	public static readonly ButtonUIDs Player1ButtonUIDs = GetButtonUIDs(1);
	public static readonly ButtonUIDs Player2ButtonUIDs = GetButtonUIDs(2);
	public static readonly ButtonUIDs Player3ButtonUIDs = GetButtonUIDs(3);
	public static readonly ButtonUIDs Player4ButtonUIDs = GetButtonUIDs(4);

	// Button model.
	public ButtonUIDs Ids { get; protected set; }

	protected float buttonRepeatDelay = 0.5f;
	public float ButtonRepeatDelay
	{
		get { return buttonRepeatDelay; }
		set { buttonRepeatDelay = value; }
	}

	protected float deadZoneRadius = 0.0f;
	public float DeadZoneRadius
	{
		get { return deadZoneRadius; }
		set { deadZoneRadius = value; }
	}

	// Controller buttons.
	public Vector2 LeftAnalog { get	{ return GetAnalog(Ids.leftAnalogX, Ids.leftAnalogY); }}
	public Vector2 RightAnalog { get { return GetAnalog(Ids.rightAnalogX, Ids.rightAnalogY); }}
	public float LeftTrigger { get { return 0.0f; /* TODO: Implement trigger detection.*/ }}
	public float RightTrigger {	get	{ return 0.0f; /* TODO: Implement trigger detection.*/ }}
	public bool LeftBumper { get { return GetButtonDown(Ids.leftBumper); }}
	public bool RightBumper { get { return GetButtonDown(Ids.rightBumper); }}
	public bool AButton { get { return GetButtonDown(Ids.aBtn); }}
	public bool BButton { get { return GetButtonDown(Ids.bBtn);	}}
	public bool XButton { get { return GetButtonDown(Ids.xBtn); }}
	public bool YButton { get { return GetButtonDown(Ids.yBtn);	}}
	public bool StartButton { get { return GetButtonDown(Ids.startBtn); }}
	public bool BackButton { get { return GetButtonDown(Ids.backBtn); }}
	public bool DPadLeft { get { return GetAxis(Ids.horizontalDPad) < 0.0f; } }
	public bool DPadRight { get { return GetAxis(Ids.horizontalDPad) > 0.0f; } }
	public bool DPadUp { get { return GetAxis(Ids.verticalDPad) > 0.0f; } }
	public bool DPadDown { get { return GetAxis(Ids.verticalDPad) < 0.0f; } }

	protected Dictionary<string, float> repeatDelays = new Dictionary<string, float>();

	public ControllerModel(ButtonUIDs controllerIds)
	{
		Ids = controllerIds;
	}

	public static ButtonUIDs GetButtonUIDs(int playerIdx)
	{
		return BaseButtonIds.AppendIdSuffix(string.Format(PlayerModel.UnformattedPlayerIdSuffix, playerIdx));
	}

	protected bool GetButtonDown(string buttonId)
	{
		return ApplyRepeatDelay(Input.GetButtonDown(buttonId), buttonId);
	}

	protected float GetAxis(string axisId)
	{
		return Input.GetAxis(axisId);
	}

	protected Vector2 GetAnalog(string xAxisId, string yAxisId)
	{
		return new Vector2(GetAxis(xAxisId), GetAxis(yAxisId));
	}

	protected bool ApplyRepeatDelay(bool originalButtonState, string buttonId)
	{
		if(!originalButtonState)
		{	// If button is not down, no repeat delay to apply.
			return originalButtonState;
		}
		else if (repeatDelays.ContainsKey(buttonId))
		{	// If button has been pressed before, look up the time in the dictionary. 
			float lastPressTime = repeatDelays[buttonId];
			originalButtonState = Time.time - lastPressTime > ButtonRepeatDelay;
			if(originalButtonState)
			{	// If the repeat delay has elapsed, update the last press time and return true.
				repeatDelays.Remove(buttonId);
				repeatDelays.Add(buttonId, Time.time);
			}
		}
		else
		{	// If key has not been added, add it.
			repeatDelays.Add(buttonId, Time.time);
		}
		return originalButtonState;
	}

	public struct ButtonUIDs
	{
		public string leftAnalogX;
		public string leftAnalogY;
		public string rightAnalogX;
		public string rightAnalogY;
		public string leftTrigger;
		public string rightTrigger;
		public string leftBumper;
		public string rightBumper;
		public string startBtn;
		public string backBtn;
		public string aBtn;
		public string bBtn;
		public string xBtn;
		public string yBtn;
		public string horizontalDPad;
		public string verticalDPad;

		public ButtonUIDs(string leftAnalogX, string leftAnalogY, string rightAnalogX, string rightAnalogY,
			string leftTrigger, string rightTrigger, string leftBumper, string rightBumper, string startBtn,
			string backBtn, string aBtn, string bBtn, string xBtn, string yBtn, string horizontalDPad, 
			string verticalDPad)
		{
			this.leftAnalogX = leftAnalogX;
			this.leftAnalogY = leftAnalogY;
			this.rightAnalogX = rightAnalogX;
			this.rightAnalogY = rightAnalogY;
			this.leftTrigger = leftTrigger;
			this.rightTrigger = rightTrigger;
			this.leftBumper = leftBumper;
			this.rightBumper = rightBumper;
			this.startBtn = startBtn;
			this.backBtn = backBtn;
			this.aBtn = aBtn;
			this.bBtn = bBtn;
			this.xBtn = xBtn;
			this.yBtn = yBtn;
			this.horizontalDPad = horizontalDPad;
			this.verticalDPad = verticalDPad;
		}

		public ButtonUIDs AppendIdSuffix(string suffix)
		{
			ButtonUIDs uidsCopy = this;
			uidsCopy.leftAnalogX += suffix;
			uidsCopy.leftAnalogY += suffix;
			uidsCopy.rightAnalogX += suffix;
			uidsCopy.rightAnalogY += suffix;
			uidsCopy.leftTrigger += suffix;
			uidsCopy.rightTrigger += suffix;
			uidsCopy.leftBumper += suffix;
			uidsCopy.rightBumper += suffix;
			uidsCopy.startBtn += suffix;
			uidsCopy.backBtn += suffix;
			uidsCopy.aBtn += suffix;
			uidsCopy.bBtn += suffix;
			uidsCopy.xBtn += suffix;
			uidsCopy.yBtn += suffix;
			uidsCopy.horizontalDPad += suffix;
			uidsCopy.verticalDPad += suffix;
			return uidsCopy;
		}

	}
}
