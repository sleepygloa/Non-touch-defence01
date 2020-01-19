using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : SingletonMonobehaviour<DataManager>
{
    public EntityTable entityData = null;

    public StageTable stageData = null;
    //key, data model..
    public Dictionary<int, EntityModel> entityTable = new Dictionary<int, EntityModel>();
    public Dictionary<int, StageModel> stageTable = new Dictionary<int, StageModel>();
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

        this.stageData = (StageTable)Resources.Load("Data/StageTable");
        Debug.Log("StageTable import Start......");
        foreach (StageTable.Sheet sheet in stageData.sheets)
        {
            foreach (StageTable.Param param in sheet.list)
            {
                stageTable.Add(param.ID, new StageModel(param.ID, param.Level, param.EntityId, param.EntityCnt, param.BossEntityId, param.BossCnt));

            }
        }

        Debug.Log("StageTable import end......");
        if (this.soundData == null)
        {
            this.soundData = ScriptableObject.CreateInstance<SoundData>();
            this.soundData.LoadData();
        }
        

    }

    public EntityModel GetEntityData(string ID) {
        int r = int.Parse(ID);
        return GetEntityData(r);
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

    public StageModel GetStageData(int ID)
    {
        if (this.stageTable.Count > 0)
        {
            if (this.stageTable.ContainsKey(ID) == true)
            {
                return this.stageTable[ID];
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
