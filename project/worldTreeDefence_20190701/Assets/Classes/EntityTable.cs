using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
		public int SearchRange;
		public int AttackPower;
		public float AttackSpeed;

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("Param ID : " + ID);
            builder.Append("/ EntityCategory : " + EntityCategory);
            builder.Append("/ EntityType : " + EntityType);
            builder.Append("/ HP : " + HP);
            builder.Append("/ Level : " + Level);
            builder.Append("/ Prefab : " + Prefab);
            builder.Append("/ SearchRange : " + SearchRange);
            builder.Append("/ AttackPower : " + AttackPower);
            builder.Append("/ AttackSpeed : " + AttackSpeed);
        
            return builder.ToString();
        }

    }
}

