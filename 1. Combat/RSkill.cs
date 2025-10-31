using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RSkill : SkillBase
{
    private int RSkillTime = 0;
    [SerializeField] private float featherRNo = 24;
    private float animDelayTime = 0.8f;

    private bool unbeatable = false;
    public bool Unbeatable => unbeatable;

    private void Awake()
    {
        IsReady = false;
        Cooldown = RSkillTime;
    }

    private void OnDisable()
    {
        IsReady = false;
    }

    public override async UniTask Execute()
    {
        attackManager.IsOnSkill = true;

        var unbeatableTask = SetUnbeatableAsync();
        await AttackRSkillAsync();

        // 무적 상태가 남아있으면 완료될 때까지 기다림
        await unbeatableTask;

        attackManager.IsOnSkill = false;
        anim.SetTrigger("BLEND_TREE");
    }

    // R 스킬 때 무적 상태, 속도 빨라짐
    private async UniTask SetUnbeatableAsync()
    {
        unbeatable = true;
        allyNavMesh.moveSpeed *= 2;
        await UniTask.Delay(TimeSpan.FromSeconds(2f));
        unbeatable = false;
        allyNavMesh.moveSpeed /= 2;
    }

    private async UniTask AttackRSkillAsync()
    {
        anim.SetTrigger("RSKILL");
        await UniTask.Delay(TimeSpan.FromSeconds(animDelayTime));

        for (int i = 1; i <= featherRNo; i++)
        {
            // 깃털 360도로 퍼지게
            GameObject feather = ObjectPoolManager.instance.featherPool.Get();
            feather.layer = LayerMask.NameToLayer("Feather");
            feather.transform.localEulerAngles = new Vector3(0, (360 / featherRNo) * i, 0);
            feather.transform.position = transform.position + featherDist * feather.transform.forward;

            // 파티클 재생
            PlaceAttackParticle(gameObject, feather.transform.forward);

            // Enemy 에게 데미지 주기
            ApplyDamageToPiercedTargets(transform.position + Vector3.up * 0.5f, feather.transform.forward);

            PlaceFeather();
        }
    }
}
