using System.Collections;
using UnityEngine;

public class AllyFSM : MonoBehaviour
{
    enum AllyState
    {
        Move,
        Damaged,
        Die,
        ReturnToPlayer,
        Reborn
    }

    AllyState a_State;

    // Move
    internal Vector3 moveDir = new Vector3(0, 0, 0);

    #region 가져오기

    private AttackManager attackManager;

    private AIlyHPSystem hp;
    private DamageUI damageUI;
    private Animator anim;
    private GameObject player;
    private BSkill bSkill;
    private ESkill eSkill;
    private RSkill rSkill;

    #endregion

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Collider[] targets;

    internal Coroutine stateRoutine;
    internal Coroutine chooseDir;

    void Start()
    {
        attackManager = GetComponent<AttackManager>();
        hp = GetComponent<AIlyHPSystem>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player");
        damageUI = gameObject.GetComponentInChildren<DamageUI>();

        bSkill = GetComponent<BSkill>();
        eSkill = GetComponent<ESkill>();
        rSkill = GetComponent<RSkill>();
        
        moveDir = Vector3.right;

        a_State = AllyState.Move;

        GameManager.instance.IsStopped += _ => Die(false);

        stateRoutine = StartCoroutine(StateRoutine());
        chooseDir = StartCoroutine(ChooseDir());
    }

    public IEnumerator StateRoutine()
    {
        while (true)
        {
            switch (a_State)
            {
                case AllyState.Move:
                    Move();
                    break;
                case AllyState.Damaged:
                    Damaged();
                    break;
                case AllyState.Die:
                    Die();
                    break;
                case AllyState.ReturnToPlayer:
                    ReturnToPlayer();
                    break;
                case AllyState.Reborn:
                    Reborn();
                    break;
            }

            anim.SetFloat("MOVE", moveDir.magnitude);
            yield return null;
        }
    }


    private void Move()
    {
        transform.Translate(moveDir * Time.deltaTime * 3f, Space.World);

        if (Input.GetKeyDown(KeyCode.Alpha1))
            a_State = AllyState.ReturnToPlayer;
    }

    public IEnumerator ChooseDir()
    {
        while (!hp.IsDead)
        {
            SetRandomDirection();
            yield return new WaitForSeconds(3f);
        }
    }

    private void SetRandomDirection()
    {
        int randInt = Random.Range(1, 9);
        switch (randInt)
        {
            case 1:
                moveDir = new Vector3(1, 0, 0);
                break;
            case 2:
                moveDir = new Vector3(1, 0, 1).normalized;
                break;
            case 3:
                moveDir = new Vector3(0, 0, 1);
                break;
            case 4:
                moveDir = new Vector3(-1, 0, 1).normalized;
                break;
            case 5:
                moveDir = new Vector3(-1, 0, 0);
                break;
            case 6:
                moveDir = new Vector3(-1, 0, -1).normalized;
                break;
            case 7:
                moveDir = new Vector3(0, 0, -1);
                break;
            case 8:
                moveDir = new Vector3(1, 0, -1).normalized;
                break;
        }
    }

    void Damaged()
    {
        StartCoroutine(damageUI.ChangeColorTemporarily());
        a_State = AllyState.Move;
    }

    void Die(bool isReborn = true)
    {
        bSkill.RemoveFeather();
        StopCoroutine(chooseDir);

        hp.Die();
        attackManager.HandleStop(true);

        if(isReborn)
        {
            a_State = AllyState.Reborn;
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    void ReturnToPlayer()
    {
        if (!bSkill.isBSkill && !eSkill.IsESkill && !rSkill.isRSkill)
        {
            transform.position = player.transform.position + new Vector3(2, 0, 0);
            a_State = AllyState.Move;
        }
    }

    void Reborn()
    {
        hp.Reborn();
        a_State = AllyState.Move;
    }

    public void HitAlly(float hitPower)
    {
        if (hp.CurrHealth > 0)
        {
            a_State = AllyState.Damaged;
        }
        else
        {
            a_State = AllyState.Die;
        }
    }
}
