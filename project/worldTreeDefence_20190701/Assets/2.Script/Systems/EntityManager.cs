using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : SingletonMonobehaviour<EntityManager>
{
    public List<Entity> entities = new List<Entity>();

    public void SpawnEntity(int ID)
    {
        EntityModel data = DataManager.Instance.GetEntityData(ID);
        if(data != null)
        {
            Vector3 pos = Vector3.zero;
            if(InputManager.Instance.GetCurrentInput() != null)
            {
                //InGame
                pos = InputManager.Instance.GetCurrentInput().currentRayHitPosition;
            }else
            {
                //Editor
                InputManager.FingerInput mouseInput = new InputManager.FingerInput((int)MouseButton.Left);
                mouseInput.SetCurrentPoint(TouchPhase.Began, Input.mousePosition);
                pos = mouseInput.currentRayHitPosition;
            }
            pos.z = -1.0f;
            GameObject spawnObject = ResourceManager.Instance.Instantiate(data.Prefab);
            spawnObject.transform.position = pos;
            Entity newEntity = spawnObject.GetComponent<Entity>();
            newEntity.CashingObject();
            newEntity.InitEntity(data);
            if(this.entities.Contains(newEntity) == false)
            {
                entities.Add(newEntity);
            }
        }
    }

    public void UpdateEntities()
    {
        if(entities.Count == 0)
        {
            return;
        }

        for(int i = 0; i < entities.Count; i++)
        {
            if(entities[i] != null)
            {
                entities[i].UpdateEntity();
            }
        }
    }

    public Transform GetCloseEntity(Vector3 searchPoint, float Range, EntityType wantedType)
    {
        if(this.entities.Count == 0)
        {
            return null;
        }
        Transform target = null;

        for(int i = 0; i < this.entities.Count; i++)
        {
            if(entities[i] == null || entities[i].IsDead() == true)
            {
                continue;
            }
            if(this.entities[i].entityType == wantedType)
            {
                float distance = (this.entities[i].myTransform.position - searchPoint).magnitude;
                if(distance <= Range)
                {
                    Range = distance;
                    target = entities[i].myTransform;
                }
            }
        }
        
        return target;
    }
	
}
