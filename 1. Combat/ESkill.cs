using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ESkill : SkillBase
{
    #region variables
    private int ESkillTime = 0;
    private float animDelayTime = 0.6f;

    [SerializeField] private float eAttRate = 1.2f;
    [SerializeField] private float featherSpeedE = 100;
    [SerializeField] private float enmStopTime = 3f;

    [SerializeField] private SkillPassive passive;

    private void Awake()
    {
        IsReady = false;
        Cooldown = ESkillTime;
    }

    private void OnDisable()
    {
        IsReady = false;
    }

    #endregion

    // E 스킬 : 깃털 회수
    public override async UniTask Execute()
    {
        await ExecuteESkill();
    }

    private async UniTask ExecuteESkill()
    {
        attackManager.IsOnSkill = true;
        anim.SetTrigger("ESKILL");
        await UniTask.Delay(TimeSpan.FromSeconds(animDelayTime));

        // 주변의 깃털 스캔
        Collider[] feathers = Physics.OverlapSphere(transform.position, scanRange, featherLayer);

        // 깃털 회수 방향에 맞추어 데미지 주고 파티클 광선 생성
        foreach (Collider feather in feathers)
        {
            feather.gameObject.name = "feather";

            Vector3 dirFrFthToAlly = transform.position - feather.gameObject.transform.position;
            dirFrFthToAlly.y = 0;
            Vector3 dirFrFthToAllyNor = dirFrFthToAlly.normalized;

            PlaceAttackParticle(feather.gameObject, dirFrFthToAlly);
            SoundManager.instance.PlayZayahSound(1);

            feather.transform.position = transform.position;

            if (feather != null)
            {
                Feather featherScript = feather.GetComponent<Feather>();
                featherScript.StopFeatherCoroutine();
                StartCoroutine(featherScript.AutoDestroy(featherTime));
            }

            ApplyDamageToPiercedTargets(feather.transform.position + Vector3.up * 0.5f, dirFrFthToAlly);
        }

        // 회수한 깃털이 30개 이상이면 패시브 스킬을 사용할 수 있다.
        if (feathers.Length >= 30 && !passive.isPAttack)
        {
            passive.IsReady = true;
           // passive.pAble = true;
        }

        attackManager.IsOnSkill = false;
        anim.SetTrigger("BLEND_TREE");
    }

    public override void ApplyDamageToPiercedTargets(Vector3 myDir, Vector3 dir)
    {
        // 레이캐스트로 관통한 적들에게 데미지 부여
        RaycastHit[] hitInfos = Physics.RaycastAll(
            myDir,
            dir,
            dir.magnitude,
            targetLayer);

        foreach (RaycastHit hitinfo in hitInfos)
        {
            hitinfo.transform.GetComponent<EnemyHp>().UpdateHp(attackDmg * eAttRate);
            DamageParticle(hitinfo.transform.position + Vector3.up);
            _ = StopEnemyAsync(hitinfo);
        }
    }

    // E 스킬 때 에너미 멈추게 함
    private async UniTaskVoid StopEnemyAsync(RaycastHit hitinfo)
    {
        NavMeshAgent agent = hitinfo.transform.GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;
        await UniTask.Delay(TimeSpan.FromSeconds(enmStopTime));
        if (hitinfo.transform != null) hitinfo.transform.GetComponent<NavMeshAgent>().enabled = true;
    }
}
