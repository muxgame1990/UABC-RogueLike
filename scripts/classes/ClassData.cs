using System;

public enum PassiveType
{
	None, Caballero, Arquero, Asesino, Berserker,
	Catalizador, Ogro, MagoMaestro, CaballeroCorcel
}

public partial class ClassData
{
	public string      Name;
	public string      Description;
	public string      PassiveName;
	public string      PassiveDescription;
	public string      StartingWeaponName; // debe existir en WeaponLibrary
	public PassiveType Passive;
}
