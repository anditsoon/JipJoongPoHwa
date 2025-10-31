using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;

public class BSkill : SkillBase
{
    internal int BCoolTime = 4;

    private float animDelayTime = 0.3f;
    private float rotationTime = 0.4f;

    private void Awake()
    {
        Cooldown = BCoolTime;
    }

    public override async UniTask Execute()
    {
        attackManager.IsOnSkill = true;

        nearestTarget = GetNearestEnemy();
        if (nearestTarget == null)
        {
            attackManager.IsOnSkill = false;
            return;
        }

        // 방향 설정 및 회전
        attackDir = nearestTarget.position - transform.position;
        allyBody.transform.rotation = Quaternion.LookRotation(attackDir, Vector3.up);
        aimUI.SetAimUIDir(nearestTarget);

        // 공격하기
        await AttackAsync();

        attackManager.IsOnSkill = false;
    }

    private async UniTask AttackAsync()
    {
        anim.SetTrigger("BASIC_ATTACK");
        await UniTask.Delay(TimeSpan.FromSeconds(animDelayTime));

        allyBody.transform.forward = attackDir;
        await UniTask.Delay(TimeSpan.FromSeconds(rotationTime));

        for (int i = 0; i < attackNum; i++)
        {
            // 타겟이 이미 사망했으면 중단
            if (nearestTarget == null || nearestTarget.gameObject == null) break;

            // AimUI 조정
            aimUI.SetAimUIDir(nearestTarget);

            // 쏘아지는 이펙트 만들고 파괴
            PlaceAttackParticle(gameObject, attackDir);
            SoundManager.instance.PlayZayahSound(0);

            // 데미지 주기
            ApplyDamageToPiercedTargets(transform.position + Vector3.up * 0.5f, attackDir);

            // 깃털 목적지에 넣음
            PlaceFeather();

            await UniTask.Delay(TimeSpan.FromSeconds(featherEftTime));
        }

        anim.SetTrigger("BLEND_TREE");
    }
}
