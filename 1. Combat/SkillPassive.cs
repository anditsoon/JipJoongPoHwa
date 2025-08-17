using System.Collections;
using UnityEngine;

public class SkillPassive : SkillBase
{
    [Header("Passive Attack 관련 변수")]
    private float curPAttTime = 0;
    private const float P_SKILL_DURATION = 15;
    internal bool isPAttack = false;
    [SerializeField] private GameObject featherAFactory;
    internal float batRate = 1.05f;
    private float batRateIncreaseRatio = 0.1f;

    [Header("베지어 곡선을 위한 벡터들")]
    Vector3 p1, p2, p3, p4, p5, p6, p7, p8;
    Vector3 dirP;

    public SkillPassive()
    {
        Cooldown = PSkillTime;
        Priority = 2;
    }

    private void Update()
    {
        if (!hpSystem.IsDead)
        {
            batRate = 1.05f + batRateIncreaseRatio * playerManager.indexLev;
        }
    }

    public override void Execute()
    {
        if (pAble)
        {
            StartCoroutine(SkillTimeFast());
            StartCoroutine(PassiveAttack());
        }
        pAble = false;
    }

    public IEnumerator SkillTimeFast()
    {
        skillTimeRate = 1.1f;
        yield return new WaitForSeconds(P_SKILL_DURATION);
        skillTimeRate = 1f;
    }

    public IEnumerator PassiveAttack()
    {
        curPAttTime = 0;
        isPAttack = true;

        while (!hpSystem.IsDead && curPAttTime < P_SKILL_DURATION)
        {
            Transform targetP = GetNearestEnemy();
            if (targetP == null)
            {
                yield return null;
                continue;
            }
            p4 = targetP.position;

            for (int i = 0; i < attackNum; i++)
            {
                float time = 0f;

                GameObject feather1 = Instantiate(featherAFactory);
                feather1.layer = LayerMask.NameToLayer("PassiveFeather");
                GameObject feather2 = Instantiate(featherAFactory);
                feather2.layer = LayerMask.NameToLayer("PassiveFeather");

                dirP = p4 - transform.position;

                if (i == 0)
                {
                    // 몸통 회전시키기
                    allyBody.transform.forward = dirP;
                }

                while (time < 1f)
                {
                    p1 = transform.position;
                    p2 = transform.position + 5f * transform.right;
                    p3 = transform.position - 5f * transform.right;

                    dirP = p4 - transform.position;

                    // 투사체 이동 애니메이션 진행률 보정
                    time += Time.deltaTime * 3f;
                    time = Mathf.Clamp01(time);

                    p5 = Vector3.Lerp(p1, p2, time);
                    p6 = Vector3.Lerp(p2, p4, time);
                    p7 = Vector3.Lerp(p1, p3, time);
                    p8 = Vector3.Lerp(p3, p4, time);

                    feather1.transform.position = Vector3.Lerp(p5, p6, time);
                    feather2.transform.position = Vector3.Lerp(p7, p8, time);

                    yield return null;
                }

                Destroy(feather1);
                Destroy(feather2);

                yield return null;
            }

            if (targetP != null && targetP.GetComponent<EnemyMove>() != null)
            {
                targetP.GetComponent<EnemyHp>().UpdateHp(attackDmg * batRate * attackNum);
                DamageParticle(targetP.transform.position + Vector3.up);
            }
            yield return new WaitForSeconds(1f);
            curPAttTime += 1;
        }

        isPAttack = false;
    }
}
