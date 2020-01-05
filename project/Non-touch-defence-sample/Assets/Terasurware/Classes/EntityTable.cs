using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntityTable : ScriptableObject
{	
	public List<Sheet> sheets = new List<Sheet> ();

	[System.SerializableAttribute]
	public class Sheet
	{
		public string name = string.Empty;
		public List<Param> list = new List<Param>();
	}

	[System.SerializableAttribute]
	public class Param
	{
		
		public int ID;
		public string EntityCategory;
		public string EntityType;
		public int HP;
		public int Level;
		public string Prefab;
		public float SearchRange;
		public int AttackPower;
		public float AttackSpeed;
	}
}

