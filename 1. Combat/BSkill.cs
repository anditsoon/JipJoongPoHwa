using System.Collections;
using UnityEngine;

public class BSkill : SkillBase
{
    internal bool isBSkill = false;
    
    private float animDelayTime = 0.3f;
    private float rotationTime = 0.4f;

    public BSkill()
    {
        Cooldown = BSkillTime;
        Priority = 5;
    }

    public override void Execute()
    {
        isBSkill = true;

        nearestTarget = GetNearestEnemy();
        if (nearestTarget == null) return;

        // 방향 설정 및 회전
        dirB = nearestTarget.position - transform.position;
        allyBody.transform.rotation = Quaternion.LookRotation(dirB, Vector3.up);
        aimUI.SetAimUIDir(nearestTarget);

        // 공격하기
        StartCoroutine(AttackCrt());
    }

    public virtual IEnumerator AttackCrt()
    {
        anim.SetTrigger("BASIC_ATTACK");
        yield return new WaitForSeconds(animDelayTime);

        allyBody.transform.forward = dirB;
        yield return new WaitForSeconds(rotationTime);

        for (int i = 0; i < attackNum; i++)
        {
            // 타겟이 이미 사망했으면 중단
            if (nearestTarget == null || nearestTarget.gameObject == null) break;

            // AimUI 조정
            aimUI.SetAimUIDir(nearestTarget);

            // 쏘아지는 이펙트 만들고 파괴
            PlaceAttackParticle(gameObject, dirB);
            SoundManager.instance.PlayZayahSound(0);

            // 데미지 주기
            ApplyDamageToPiercedTargets(transform.position + Vector3.up * 0.5f, dirB);

            // 깃털 목적지에 넣음
            PlaceFeather();

            yield return new WaitForSecondsRealtime(featherEftTime);
        }

        isBSkill = false;
        anim.SetTrigger("BLEND_TREE");
    }
}
