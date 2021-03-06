﻿using UnityEngine;
using System.Collections;

public class MenuButtonFunctionality : ButtonFunctionality {
	public ButtonTypes buttonType;
	public enum ButtonTypes
	{
		Start,
		Options,
		Credits,
		Quit
	}

	protected override void PressButton ()
	{
		switch (buttonType)
		{
		case ButtonTypes.Start:
			_asyncOp = Application.LoadLevelAsync ("ModeSelect");
			break;
		case ButtonTypes.Options:
			_asyncOp = Application.LoadLevelAsync ("Options");
			break;
		case ButtonTypes.Credits:
			_asyncOp = Application.LoadLevelAsync ("Credits");
			break;
		case ButtonTypes.Quit:
			Application.Quit ();
			break;
		}
		base.PressButton ();
	}
}
