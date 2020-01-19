using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : SingletonMonobehaviour<EntityManager>
{
    public List<Entity> entities = new List<Entity>();

    public void SpawnEntity(int ID)
    {
        EntityModel data = DataManager.Instance.GetEntityData(ID);
        //710.5, -252.27

        if (data != null)
        {
            /*
            Vector3 pos = Vector3.zero;
            if(InputManager.Instance.GetCurrentInput() != null)
            {
                //InGame
                pos = InputManager.Instance.GetCurrentInput().currentRayHitPosition;
                //pos = new Vector3(0, 0, 1);
            }
            else
            {
                //Editor
                InputManager.FingerInput mouseInput = new InputManager.FingerInput((int)MouseButton.Left);
                mouseInput.SetCurrentPoint(TouchPhase.Began, Input.mousePosition);
                pos = mouseInput.currentRayHitPosition;

                //pos = InputManager.Instance.GetCurrentInput().currentRayHitPosition;
            }
            */
            Vector3 pos = new Vector3(710f, -250f, 1f);
            pos.z = -1.0f;
            Object source = null;
            source = ResourceManager.Load(data.Prefab);

            //GameObject)Instantiate(Enemy, new Vector3(data.Prefab, pos, Quaternion.identity);
            //GameObject spawnObject = ResourceManager.Instance.Instantiate(data.Prefab, new Vector3(710f, -250f, 1f));
            GameObject spawnObject = (GameObject)Instantiate(source, pos, Quaternion.identity);
            //spawnObject.transform.position = pos;
            spawnObject.transform.localPosition = Vector3.zero;
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
