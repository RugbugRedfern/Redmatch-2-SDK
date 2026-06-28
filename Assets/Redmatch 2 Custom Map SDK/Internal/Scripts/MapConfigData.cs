
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum CustomMapMatchSetting
{
	Time = 8,
	TeamSpawns = 10,
	GrappleGear = 11,
	Night = 15,
	DamageMultiplier = 18,
	SpeedMultiplier = 19,
	JumpMultiplier = 20,
	StartingAmmoMultiplier = 21,
	MagazineSizeMultiplier = 22,
	UpgradeCabinets = 30,
	RifleDamage = 31,
	ShotgunDamage = 32,
	SniperDamage = 33,
	PlayerSize = 34,
	Gravity = 36,
	MeleeDamage = 42,
	TeamCount = 46,
	RevolverDamage = 47,
	Infection_SurvivorGrappleGear = 51,
	Infection_InfectedBonusHealth = 52,
	AllowSpectating = 54,
	SMGDamage = 65,
	DoubleBarrelDamage = 66,
	KnockbackMultiplier = 67
}

[System.Serializable]
public class UnsanitizedMapConfigData
{
	public Dictionary<string, string> MatchSettingOverrides = new Dictionary<string, string>();
	public string SDKVersion;
}


// Some code here has been excluded from the SDK.
