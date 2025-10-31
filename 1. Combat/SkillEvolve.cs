using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillEvolve : SkillBase
{
    internal int EVSkillTime = 0;
    [SerializeField] private GameObject evFeatherPrefab;
    private float featherSpeed = 10;
    private int bounceCount = 3;
    [SerializeField] private GameObject shield;
    private float shieldDuration = 5f;
    private float healPerBounceRate = 0.1f;

    private void Awake()
    {
        IsReady = false;
        Cooldown = EVSkillTime;
    }

    private void OnDisable()
    {
        IsReady = false;
    }

    public override async UniTask Execute()
    {
        await EvolveAsync();
    }

    // 기본무기 강화 (연인의 도탄)
    private async UniTask EvolveAsync()
    {
        // Feather 생성
        GameObject feather = Instantiate(evFeatherPrefab, transform.position, Quaternion.identity);
        feather.layer = LayerMask.NameToLayer("Feather");

        List<(Vector3 dir, Collider collider)> enemies = GetEnemies();
        if (enemies.Count == 0)
        {
            Destroy(feather);
            return;
        }

        // 튕기게 하기
        for (int i = 0; i <= bounceCount; i++)
        {
            // 마지막 점프는 자기 자신(귀환)
            var targetPos = (i < enemies.Count && enemies[i].collider != null)
                ? enemies[i].collider.transform.position
                : transform.position;

            // 목적지까지 이동
            while (Vector3.Distance(feather.transform.position, targetPos) > 0.05f)
            {
                feather.transform.position = Vector3.MoveTowards(
                    feather.transform.position,
                    targetPos,
                    featherSpeed * Time.deltaTime
                );
                await UniTask.Yield();
            }

            // 공격 처리
            if (i < enemies.Count && enemies[i].collider != null)
            {
                var enemyHp = enemies[i].collider.GetComponent<EnemyHp>();
                if (enemyHp != null)
                {
                    enemyHp.UpdateHp(attackDmg);
                    DamageParticle(enemies[i].collider.transform.position + Vector3.up);
                }
            }

            // 마지막 처리
            if (i == bounceCount)
            {
                Destroy(feather);
                await ActivateShieldAsync();
                hpSystem.UpdateHp(hpSystem.MaxHealth * healPerBounceRate * i);
            }
        }
    }

    private async UniTask ActivateShieldAsync()
    {
        shield.transform.position = transform.position;
        shield.SetActive(true);

        await UniTask.Delay(TimeSpan.FromSeconds(shieldDuration));

        shield.SetActive(false);
    }

    public List<(Vector3 dir, Collider collider)> GetEnemies()
    {
        // 리스트 만들어줌
        List<(Vector3 dir, Collider collider)> targets = new List<(Vector3 dir, Collider collider)>();

        // OverlapSphere 로 근처에 있는 에너미들 정보 가져오고
        Collider[] hitInfos = Physics.OverlapSphere(transform.position, scanRange, targetLayer);
        // 리스트에 추가해줌
        foreach (Collider hitInfo in hitInfos)
        {
            Vector3 dirToTarget = hitInfo.gameObject.transform.position - transform.position;
            targets.Add((dirToTarget, hitInfo));
        }

        // 리스트 거리 순으로 소팅
        return targets.OrderBy(target => target.dir.magnitude).ToList();
    }
}
