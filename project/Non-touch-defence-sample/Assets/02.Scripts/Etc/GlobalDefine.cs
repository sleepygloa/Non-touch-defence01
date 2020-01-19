using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

#region //Tool Defines 
public enum EventStartType { INTERACT, AUTOSTART, TRIGGER_ENTER, TRIGGER_EXIT, NONE, KEY_PRESS, DROP };
public enum AIConditionNeeded { ALL, ONE };
public enum ValueCheck { EQUALS, LESS, GREATER };
public enum SimpleOperator { ADD, SUB, SET };
public enum MouseButton { Left = 0, Right, Wheel };
#endregion




//애니메이션 데이터 animationListRootData를 가지고 가공해서 만든다. 
//tk2d애니메이션의 정보를 가지고 있다.
public class Tk2dAniData
{
    //애니아이디
    public int ani_ID { get; set; }

    //애니메이션 종류 - recharger,unit,building_normal,building_resource,building_defence
    public string type { get; set; }

    //동작 키값 - idle,walk,attack,heal,rs,working
    public string key { get; set; }

    //방향성체크 - 1이면 방향이 있음, 0이면 방향이 없음.
    public int directional { get; set; }

    //tk2dSpriteAnimation 오브젝트.
    public Animation animation { get; set; }
    //public tk2dSpriteAnimation animation { get; set; }
}

public static class Define
{
    //0
    public const string DataDirectory = "/Resources/Data/";

    //게임상 오브젝트들의 레이어 정의
    public static readonly int LAYERMASK_UI = 1 << LayerMask.NameToLayer("UI"); //ui

    public static readonly int LAYERMASK_BUILDING = 1 << LayerMask.NameToLayer("Building"); //설치된 건물

    public static readonly int LAYERMASK_GRID = 1 << LayerMask.NameToLayer("Grid");

    public static readonly int LAYERMASK_ALL_PICKLAYER = LAYERMASK_UI | LAYERMASK_BUILDING | LAYERMASK_GRID; //모든레이어를 선택가능 - 테스트씬에서 사용

    public static readonly int LAYERMASK_REPLAY_PICKLAYER = LAYERMASK_UI;//리플레이 피킹레이어 - 리플레이에서는 월드맵을 직접 찍을 일이 없다.

    public static readonly int LAYERMASK_BATTLE_PICKLAYER = LAYERMASK_UI | LAYERMASK_BUILDING; //배틀전용 피킹레이어

    public static readonly int LAYERMASK_PICKLAYER = LAYERMASK_UI | LAYERMASK_BUILDING;//베이스맵 피킹 레이어

    public static readonly Vector3 EXCEPT_POSITION = new Vector3(-10000.0f, -10000.0f, -10000.0f);

    public static float GridWidth = 128.0f;
    public static float GridWidthHalf = 64.0f;
    public static float GridHeight = 96.0f;
    public static float GridHeightHalf = 48.0f;
    public static float GridDiagonal = 80.0f;

}

public enum EntityList : int
{
    None = -1,
    Swordman = 100000,
    Slime = 100001,
    MAX,
}

public enum EntityCategory : int
{
    None = -1,
    Swordman = 10000,
    Slime = 20000,
    MAX
}

public enum EntityType : int
{
    None = -1,
    Normal,
    Unit,
    Defense,
    Resource,
    MAX
}

public class EntityModel
{
    
    public int ID;
    public EntityCategory entCategory;
    public EntityType enType;
    public int HP;
    public int Level;
    public int AttackPower;
    public float SearchRange;
    public string Prefab;
    public float AttackSpeed;

    public EntityModel(int id, string category, string eType, int hp, int level, int power, float range, float speed, string prefabName)
    {
        this.ID = id;
        this.entCategory = (EntityCategory)System.Enum.Parse(typeof(EntityCategory), category);
        this.enType = (EntityType)System.Enum.Parse(typeof(EntityType), eType);
        this.HP = hp;
        this.Level = level;
        this.AttackPower = power;
        this.SearchRange = range;
        this.AttackSpeed = speed;
        this.Prefab = prefabName;
    }


    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("ID:" + ID);
        builder.Append("/Category:" + entCategory.ToString());
        builder.Append("/Type:" + enType.ToString());
        builder.Append("/HP:" + HP.ToString());
        builder.Append("/Level:" + Level.ToString());
        builder.Append("/AttackPower:" + AttackPower.ToString());
        builder.Append("/SearchRange:" + SearchRange.ToString());
        builder.Append("/AttackSpeed:" + AttackSpeed.ToString());
        builder.Append("/Prefab:" + Prefab);
        return builder.ToString();
    }
}

public class StageModel
{

    public int StageID;
    public int Level;
    public string EntityId;
    public int EntityCnt;
    public string BossEntityId;
    public int BossCnt;


    public StageModel(int StageID, int level, string EntityId, int EntityCnt, string BossEntityId, int BossCnt)
    {

        this.StageID = StageID;
        this.Level = level;
        this.EntityId = EntityId;
        this.EntityCnt = EntityCnt;
        this.BossEntityId = BossEntityId;
        this.BossCnt = BossCnt;
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("StageID:" + StageID);
        builder.Append("/Level:" + Level);
        builder.Append("/EntityId:" + EntityId.ToString());
        builder.Append("/EntityCnt:" + EntityCnt.ToString());
        builder.Append("/BossEntityId:" + BossEntityId.ToString());
        builder.Append("/BossCnt:" + BossCnt.ToString());
        return builder.ToString();
    }
}


/// <summary>
/// 상태머신 아이디
/// </summary>
public enum StateID : int
{
    NULLSTATEID = -1,

    IDLE = 0,
    WALK = 1,
    ATTACK = 2,
    SEARCH = 3,
    DEATH = 4,
    ROTATE = 5,
    APPEAR = 6,
    STUN = 7,
    _MAX
}


/// <summary>
/// 상태머신 트랜지션
/// </summary>
public enum Transition : int
{
    NullTransition = 0,

    IdleToSearch,
    IdleToRotate,
    IdleToDie,

    SearchToIdle,
    SearchToWalk,
    SearchToAttack,
    SearchToRotate,
    SearchToDie,

    RotateToIdle,
    RotateToSearch,
    RotateToWalk,
    RotateToAttack,

    WalkToAttack,
    WalkToSearch,
    WalkToIdle,
    WalkToDeath,
    WalkToRotate,
    WalkToDie,

    AttackToDeath,
    AttackToSearch,
    AttackToIdle,
    AttackToRotate,
    AttackToDie,

    _Max
}

//애니메이션 종류
public enum AnimationType
{
    None = -1,
    Idle,
    Idle_bottom,
    Idle_head,
    Walk,
    Attack,
    Operation,
    Resource,

    Max
}

public enum Direction8Way : int
{
    n = 0,
    ne = 1,
    e = 2,
    se = 3,
    s = 4,
    sw = 5,
    w = 6,
    nw = 7,
}

/// <summary>
/// 16방향 표시 - 북쪽방향부터 시계방향으로 돌아간다. 
/// </summary>
public enum Direction16Way : int
{
    n = 0,
    nne = 1,
    ne = 2,
    nee = 3,
    e = 4,
    see = 5,
    se = 6,
    sse = 7,
    s = 8,
    ssw = 9,
    sw = 10,
    sww = 11,
    w = 12,
    nww = 13,
    nw = 14,
    nnw = 15,

}

public enum SoundPlayType
{
    None = -1,
    BGM,
    EFFECT,
    UI,
    
}

public enum MusicPlayType
{
    IDLE = -1,
    PLAY,
    PLAY_ONESHOT,
    STOP,
    FADE_IN,
    FADE_OUT,
    FADE_TO,
}

public enum EffectType
{
    NORMAL,
    MISSILE,
    PARABOLA,

}

public class Grid
{
    public bool isObstacle = false;
    public bool isPathObstacle = false;
    public GameObject gridObject = null;
    public Vector3 position = Vector3.zero;
    public int column, row = 0;

    public Grid(Vector3 pos)
    {
        this.position = pos;
    }
    public void SetColRow(int col, int ro)
    {
        this.column = col;
        this.row = ro;
    }
    public void Release()
    {
        this.isObstacle = false;
        this.isPathObstacle = false;

    }

}