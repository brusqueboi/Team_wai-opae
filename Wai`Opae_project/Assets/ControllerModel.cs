using UnityEngine;
using System.Collections;

public class ControllerModel {

	protected static readonly ButtonUIDs BaseButtonIds = 
		new ButtonUIDs("LeftJoystickHorizontal", "LeftJoystickVertical", "RightJoystickHorizontal", 
			"RightJoystickVertical", "LeftTrigger", "RightTrigger", "LeftBumper", "RightBumper", "StartButton", 
			"BackButton", "AButton", "BButton", "XButton", "YButton");

	public static readonly ButtonUIDs Player1ButtonUIDs = GetButtonUIDs(1);
	public static readonly ButtonUIDs Player2ButtonUIDs = GetButtonUIDs(2);
	public static readonly ButtonUIDs Player3ButtonUIDs = GetButtonUIDs(3);
	public static readonly ButtonUIDs Player4ButtonUIDs = GetButtonUIDs(4);

	// Button model.
	public ButtonUIDs Ids { get; protected set; }

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
		return Input.GetButtonDown(buttonId);
	}

	protected float GetAxis(string axisId)
	{
		return Input.GetAxis(axisId);
	}

	protected Vector2 GetAnalog(string xAxisId, string yAxisId)
	{
		return new Vector2(GetAxis(xAxisId), GetAxis(yAxisId));
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

		public ButtonUIDs(string leftAnalogX, string leftAnalogY, string rightAnalogX, string rightAnalogY,
			string leftTrigger, string rightTrigger, string leftBumper, string rightBumper, string startBtn,
			string backBtn, string aBtn, string bBtn, string xBtn, string yBtn)
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
			return uidsCopy;
		}

	}
}
