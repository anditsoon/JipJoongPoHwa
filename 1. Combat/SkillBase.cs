using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public abstract class SkillBase : MonoBehaviour
{
    #region variables

    // 가져오기
    internal Animator anim;
    internal GameObject allyBody;
    internal AIlyHPSystem hpSystem;
    internal H_PlayerManager playerManager;
    internal AttackManager attackManager;
    internal AllyNavMesh allyNavMesh;
    internal AllyAimUI aimUI;

    [Header("Attack 관련 변수")]
    internal int attackNum = 3;
    internal int attackDmg = 200;
    internal int AttRate;
    internal float skillTimeRate = 1.3f;
    public bool IsReady = true;

    [Header("Feather 관련 변수")]
    [SerializeField] internal LayerMask featherLayer;
    [SerializeField] internal float featherDist = 14;
    internal float featherEftTime = 0.2f;
    internal float featherTime = 10;
    private float particleEftTime = 0.2f;

    [Header("Scan 관련 변수")]
    private Collider[] targets;
    internal float scanRange = 100;
    [SerializeField] internal LayerMask targetLayer;

    [Header("Timing 관련 변수")]
    protected float lastUsedTime;
    public event Action<SkillBase> OnReady;
    public float Cooldown { get; set; }
    public float SkillTime;

    [Header("Etc")]
    internal Transform nearestTarget;
    internal Vector3 attackDir;

    public IObjectPool<GameObject> featherPool { get; set; }
    public IObjectPool<GameObject> attackParticlePool { get; set; }
    public IObjectPool<GameObject> dmgParticlePool { get; set; }

    public void Start()
    {
        anim = GetComponentInChildren<Animator>();
        allyBody = GameObject.Find("AllyBody");
        hpSystem = GetComponent<AIlyHPSystem>();
        aimUI = GetComponentInChildren<AllyAimUI>();
        playerManager = GameObject.Find("H_PlayerManager").GetComponent<H_PlayerManager>();
        attackManager = GetComponent<AttackManager>();
        allyNavMesh = GetComponent<AllyNavMesh>();
        targetLayer = LayerMask.GetMask("Enemy");
        featherLayer = LayerMask.GetMask("Feather");

        attackParticlePool = ObjectPoolManager.instance.attackParticlePool;
        dmgParticlePool = ObjectPoolManager.instance.damageParticlePool;
        featherPool = ObjectPoolManager.instance.featherPool;

        StartSkillLoop().Forget();
    }

    #endregion

    private async UniTaskVoid StartSkillLoop()
    {
        while (gameObject.activeSelf)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Cooldown));
            await UniTask.WaitUntil(() => IsReady);

            // 쿨타임이 끝나면 매니저에게 요청, 실행 Queue 에 추가
            OnReady?.Invoke(this);
            await UniTask.Yield();
        }
    }

    public void TryExecute()
    {
        Execute();
    }

    public virtual async UniTask Execute()
    {
        Debug.Log("Execute 실행");
    }

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
        feather.transform.position = transform.position + attackDir.normalized * featherDist;

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
