using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerHolder : SingletonMonobehaviour<ManagerHolder>
{
    DataManager dataManager;
    SoundManager soundManager;

    GridManager gridManager;

    InputManager inputManager;
    CameraManager cameraManager;

    TimeManager timeManager;
    EntityManager entityManager;
    BattleManager battleManager;

    UnityEngine.Component AddManager<T>()
    {
        GameObject newGameObject = new GameObject(typeof(T).ToString());
        newGameObject.transform.SetParent(transform);
        return newGameObject.AddComponent(typeof(T));
    }

    // Use this for initialization
    void Start()
    {
        //30프레임으로 고정.
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        Application.runInBackground = true;

        this.dataManager = (DataManager)AddManager<DataManager>();
        this.soundManager = (SoundManager)AddManager<SoundManager>();

        /*
        this.gridManager = (GridManager)AddManager<GridManager>();
        this.gridManager.Init();

        */
        //this.inputManager = (InputManager)AddManager<InputManager>();
        //this.cameraManager = (CameraManager)AddManager<CameraManager>();

        
        this.timeManager = (TimeManager)AddManager<TimeManager>();
        this.entityManager = (EntityManager)AddManager<EntityManager>();
        this.battleManager = (BattleManager)AddManager<BattleManager>();
        

        SceneManager.LoadScene("PlayScene");
    }

    // Update is called once per frame
    void Update()
    {
        
        if (this.timeManager != null)
        {
            this.timeManager.UpdateTime();
        }
     
        /*
        if (this.inputManager != null)
        {
            this.inputManager.TouchUpdate();

            this.inputManager.UpdateGameInput();

        }
        */
        
        if (this.entityManager != null)
        {
            this.entityManager.UpdateEntities();
        }
        
    }

    private void LateUpdate()
    {
        /*
        if (this.cameraManager != null)
        {
            this.cameraManager.CameraUpdate();
        }
        */
    }

}
