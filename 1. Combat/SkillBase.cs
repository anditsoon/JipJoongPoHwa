using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public abstract class SkillBase : MonoBehaviour
{
    #region variables

    // 가져오기
    internal Animator anim;
    internal GameObject allyBody;
    private AttackManager attackMng;
    internal AIlyHPSystem hpSystem;
    internal H_PlayerManager playerManager;
    internal AllyNavMesh allyNavMesh;
    internal AllyAimUI aimUI;

    [Header("Attack 관련 변수")]
    internal int attackNum = 3;
    internal int attackDmg = 200;
    internal int AttRate;
    internal float skillTimeRate = 1.3f;

    [Header("Feather 관련 변수")]
    [SerializeField] private LayerMask featherLayer;
    [SerializeField] private float featherDist = 14;
    internal float featherEftTime = 0.2f;
    internal float featherTime = 10;
    private float particleEftTime = 0.2f;

    [Header("Scan 관련 변수")]
    private Collider[] targets;
    internal float scanRange = 100;
    [SerializeField] private LayerMask targetLayer;

    [Header("Timing 관련 변수")]
    protected float lastUsedTime;
    internal int BSkillTime = 4;
    internal int ESkillTime = int.MaxValue;
    internal int RSkillTime = int.MaxValue;
    internal int PSkillTime = 0;
    internal int EVSkillTime = int.MaxValue;

    [Header("Etc")]
    internal Transform nearestTarget;
    internal Vector3 dirB;
    internal bool pAble = false;

    public IObjectPool<GameObject> featherPool { get; set; }
    public IObjectPool<GameObject> attackParticlePool { get; set; }
    public IObjectPool<GameObject> dmgParticlePool { get; set; }
    public float Cooldown { get; set; }
    public int Priority { get; protected set; } // 스킬이 겹치면 어떤 스킬부터 실행할지 우선순위 부여

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        allyBody = GameObject.Find("AllyBody");
        attackMng = GetComponent<AttackManager>();
        hpSystem = GetComponent<AIlyHPSystem>();
        aimUI = GetComponentInChildren<AllyAimUI>();
        playerManager = GameObject.Find("H_PlayerManager").GetComponent<H_PlayerManager>();
        allyNavMesh = GetComponent<AllyNavMesh>();
        targetLayer = LayerMask.GetMask("Enemy");
        featherLayer = LayerMask.GetMask("Feather");

        attackMng.AddSkill(this);
    }

    public void Start()
    {
        attackParticlePool = ObjectPoolManager.instance.attackParticlePool;
        dmgParticlePool = ObjectPoolManager.instance.damageParticlePool;
        featherPool = ObjectPoolManager.instance.featherPool;
    }

    #endregion
    public virtual Transform GetNearestEnemy() 
    {
        Transform result = null;
        float dist = float.MaxValue;

        targets = Physics.OverlapSphere(allyBody.transform.position, scanRange, targetLayer);

        // 구에 오버랩된 게임오브젝트 중에 가장 가까운 적의 위치 찾기
        foreach (Collider target in targets)
            {
                float curDist = Vector3.Distance(allyBody.transform.position, target.transform.position);
                if (curDist < dist)
                {
                    dist = curDist;
                    result = target.transform;
                }
            }

        return result;
    }

    public void TryExecute()
    {
        if (IsReady())
        {
            Execute();
            lastUsedTime = Time.time;
        }
    }

    public bool IsReady()
    {
        return Time.time >= lastUsedTime + Cooldown;
    }

    public abstract void Execute();

    // 레이캐스트로 적 감지 후 데미지 주기
    public virtual void ApplyDamageToPiercedTargets(Vector3 myDir, Vector3 dir)
    {
        //관통된 적들 찾기
        RaycastHit[] hitInfos = Physics.RaycastAll(
            myDir, 
            dir, 
            featherDist, 
            targetLayer); 

        // 정렬
        System.Array.Sort(hitInfos, (x, y) => x.distance.CompareTo(y.distance));

        // 5명의 적들까지 데미지 줌. 관통당할 수록 데미지는 감소.
        int nth = 0;
        foreach (RaycastHit hitinfo in hitInfos)
        {
            if (nth < 5)
            {
                hitinfo.transform.GetComponent<EnemyHp>().UpdateHp(attackDmg * (1f - ((nth * 15f) / 100f)));
            }
            else
            {
                hitinfo.transform.GetComponent<EnemyHp>().UpdateHp(attackDmg * 0.4f);
            }
            nth++;

            DamageParticle(hitinfo.transform.position + Vector3.up);
        }
    }

    public virtual void PlaceFeather()
    {
        // 적절한 위치와 방향에 생성
        GameObject feather = ObjectPoolManager.instance.featherPool.Get();
        feather.transform.localEulerAngles = new Vector3(0, 0, 0);
        feather.transform.position = transform.position + dirB.normalized * featherDist;

        // 시간이 지나면 깃털이 사라지도록 코루틴을 걸어놓는다.
        Feather featherScript = feather.GetComponent<Feather>();
        if (featherScript == null) throw new Exception("featherScript 를 찾을 수 없습니다");
        featherScript.StopFeatherCoroutine();
        StartCoroutine(featherScript.AutoDestroy(featherTime));
    }

    public void RemoveFeather()
    {
        GameObject[] feathers = GameObject.FindGameObjectsWithTag("Feather");

        foreach (GameObject featherObj in feathers)
        {
            Feather feather = featherObj.GetComponent<Feather>();
            if (feather != null)
            {
                feather.StopFeatherCoroutine();
                StartCoroutine(feather.AutoDestroy(featherEftTime));
            }
        }
    }

    public virtual void PlaceAttackParticle(GameObject obj, Vector3 dir)
    {
        GameObject basicAttEff = ObjectPoolManager.instance.attackParticlePool.Get();

        basicAttEff.transform.position = transform.position;
        basicAttEff.transform.forward = dir;
        basicAttEff.transform.localScale = Vector3.one * 8.0f;
        basicAttEff.transform.position = obj.transform.position + 3 * Vector3.up;

        StartCoroutine(ReleaseAttParticle(basicAttEff, particleEftTime));
    }

    public virtual IEnumerator ReleaseAttParticle(GameObject AttEff, float time)
    {
        yield return new WaitForSeconds(time);
        attackParticlePool.Release(AttEff);
    }

    public void DamageParticle(Vector3 dir)
    {
        GameObject dp = ObjectPoolManager.instance.damageParticlePool.Get();

        dp.transform.localScale = Vector3.one * 5.0f;
        dp.transform.position = dir;

        StartCoroutine(ReleaseDmgParticle(dp, 1f));
    }

    IEnumerator ReleaseDmgParticle(GameObject particle, float time)
    {
        yield return new WaitForSeconds(time);
        dmgParticlePool.Release(particle);
    }

}
