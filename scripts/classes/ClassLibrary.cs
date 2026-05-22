using Godot;
using System.Collections.Generic;

public static class ClassLibrary
{
	public static List<ClassData> BuildAll()
	{
		return new List<ClassData>
		{
			new ClassData {
				Name               = "Caballero",
				Description        = "Un guerrero blindado experto en combate cuerpo a cuerpo.",
				PassiveName        = "Maestria Melee",
				PassiveDescription = "Inflige un 10% mas de daño con armas cuerpo a cuerpo.",
				StartingWeaponName = "Espada Larga",
				Passive            = PassiveType.Caballero
			},
			new ClassData {
				Name               = "Arquero",
				Description        = "Un tirador experto letal a distancia.",
				PassiveName        = "Ojo de Aguila",
				PassiveDescription = "Inflige un 10% mas de daño con armas a distancia.",
				StartingWeaponName = "Arco",
				Passive            = PassiveType.Arquero
			},
			new ClassData {
				Name               = "Asesino",
				Description        = "Rapido y letal, golpea antes de que te vean.",
				PassiveName        = "Sombra Veloz",
				PassiveDescription = "Velocidad base +10% y ataca mas rapido con armas ligeras.",
				StartingWeaponName = "Dagas",
				Passive            = PassiveType.Asesino
			},
			new ClassData {
				Name               = "Berserker",
				Description        = "Cuantos mas enemigos rodean al berserker, mas peligroso se vuelve.",
				PassiveName        = "Furia de Masa",
				PassiveDescription = "Ganas mas daño cuantos mas enemigos esten cerca.",
				StartingWeaponName = "Mazo",
				Passive            = PassiveType.Berserker
			},
			new ClassData {
				Name               = "Catalizador",
				Description        = "Un ser de energia pura que escala su poder con cada kill.",
				PassiveName        = "Cascada de Poder",
				PassiveDescription = "Cada kill acumula un multiplicador de daño (max x8). Recibes 15% mas de daño.",
				StartingWeaponName = "Catalizador de Energia",
				Passive            = PassiveType.Catalizador
			},
			new ClassData {
				Name               = "Ogro",
				Description        = "Un coloso que se vuelve mas peligroso conforme recibe daño.",
				PassiveName        = "Marca de Sangre",
				PassiveDescription = "Mas daño segun la vida que falte. Si caes debajo del 50% HP de un golpe, el enemigo recibe Marca de Sangre.",
				StartingWeaponName = "Hacha de Acero",
				Passive            = PassiveType.Ogro
			},
			new ClassData {
				Name               = "Mago Maestro",
				Description        = "Un maestro de las artes arcanas.",
				PassiveName        = "Dominio Arcano",
				PassiveDescription = "Inflige un 10% mas de daño con armas magicas.",
				StartingWeaponName = "Libro Magico",
				Passive            = PassiveType.MagoMaestro
			},
			new ClassData {
				Name               = "Caballero con Corcel",
				Description        = "Un jinete que gana poder al moverse sin detenerse.",
				PassiveName        = "Carga de Caballeria",
				PassiveDescription = "Caminar 10s sin detenerse activa una carga: +15% velocidad y +30% daño al primer golpe. Cooldown 25s.",
				StartingWeaponName = "Lanza de Acero",
				Passive            = PassiveType.CaballeroCorcel
			},
		};
	}

	public static ClassData Get(int index)
	{
		var all = BuildAll();
		return index >= 0 && index < all.Count ? all[index] : null;
	}

	// Aplica los stats base de la pasiva al inicio de la partida
	public static void ApplyPassiveStats(PassiveType passive, PlayerStats stats)
	{
		switch (passive)
		{
			case PassiveType.Caballero:
				stats.MeleeDamageBonus  += 0.10f;
				break;
			case PassiveType.Arquero:
				stats.RangedDamageBonus += 0.10f;
				break;
			case PassiveType.Asesino:
				stats.MovementSpeedMult += 0.10f;
				stats.AttackSpeedMultiplier += 0.10f;
				break;
			case PassiveType.MagoMaestro:
				stats.MagicDamageBonus  += 0.10f;
				break;
			case PassiveType.Catalizador:
				// Empieza normal, escala en runtime en Player.cs
				break;
			case PassiveType.Ogro:
			case PassiveType.Berserker:
			case PassiveType.CaballeroCorcel:
				// Lógica en Player._Process
				break;
		}
	}
}
