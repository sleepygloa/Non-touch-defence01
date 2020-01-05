using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : SingletonMonobehaviour<DataManager>
{
    public EntityTable entityData = null;
    //key, data model..
    public Dictionary<int, EntityModel> entityTable = new Dictionary<int, EntityModel>();
    //soundData
    public SoundData soundData = null;
    private void Start()
    {
        this.entityData = (EntityTable)Resources.Load("Data/EntityTable");

        foreach (EntityTable.Sheet sheet in entityData.sheets)
        {
            foreach (EntityTable.Param param in sheet.list)
            {
                Debug.Log(param.ToString());
                entityTable.Add(param.ID, new EntityModel(param.ID, param.EntityCategory, param.EntityType, param.HP,
                    param.Level, param.AttackPower, param.SearchRange, param.AttackSpeed, param.Prefab));

            }
        }

        if(this.soundData == null)
        {
            this.soundData = ScriptableObject.CreateInstance<SoundData>();
            this.soundData.LoadData();
        }

    }

    public EntityModel GetEntityData(int ID)
    {
        if (this.entityTable.Count > 0)
        {
            if (this.entityTable.ContainsKey(ID) == true)
            {
                return this.entityTable[ID];
            }
        }
        return null;
    }

    public static SoundData Sound()
    {
        if(DataManager.Instance.soundData == null)
        {
            DataManager.Instance.soundData = ScriptableObject.CreateInstance<SoundData>();
            DataManager.Instance.soundData.LoadData();
        }
        return DataManager.Instance.soundData;
    }


}
