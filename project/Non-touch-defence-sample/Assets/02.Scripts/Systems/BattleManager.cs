using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : SingletonMonobehaviour<BattleManager>
{
    public void AttackEntity(Entity attacker, Entity target, int damage, Vector3 hittingPosition)
    {
        if(attacker == null || target == null || target.IsDead() == true)
        {
            return;
        }
        target.HP -= damage;
        target.OnDamaged(damage);

        if(target.IsDead() == true)
        {
            target.DestroyEntity();
            attacker.OnTargetDestroy();
            Debug.Log(attacker.name + "이(가) " + target.name + " 을(를) 공격하여 " + damage.ToString() + "의 피해를 입히고 파괴하였습니다.");
        }else
        {
            Debug.Log(attacker.name + "이(가) " + target.name + " 을(를) 공격하여 " + damage.ToString() + "의 피해를 입혔습니다.");
        }
    }
	
}
