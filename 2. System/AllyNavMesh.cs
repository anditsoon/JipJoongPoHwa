using UnityEngine;
using UnityEngine.AI;

public class AllyNavMesh : MonoBehaviour
{
    internal NavMeshAgent agent;
    private GameObject player;
    private AllyFSM allyFSM;
    private AllyBodyRot allyBodyRot;
    private AttackManager attackManager;
    private BSkill bSkill;
    private Boss boss;

    [SerializeField] internal float moveSpeed = 25f;
    private float currTime = 0;
    private float changeTime = 3;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        
        player = GameObject.Find("Player");
        allyFSM = GetComponent<AllyFSM>();
        allyBodyRot = GetComponent<AllyBodyRot>();
        attackManager = GetComponent<AttackManager>();
        bSkill = GetComponent<BSkill>();
    }
    void Start()
    {
        agent.speed = moveSpeed;
        agent.destination = transform.position + new Vector3(1, 0, 1).normalized * 10; 
    }

    void Update()
    {
        if (GameManager.instance.isStop) return;
        currTime += Time.deltaTime;

        if (currTime > changeTime && attackManager.IsOnSkill)
        {
            if (GameManager.instance.bossSpawn)
            {
                if (boss == null)
                {
                    boss = GameObject.FindWithTag("Boss").GetComponent<Boss>();
                }
                else if (boss.stone.activeSelf)
                {
                    int i = Random.Range(1, 4);
                    if (i == 1) agent.destination = boss.stone.transform.position - (bSkill.featherDist - 3) * Vector3.forward;
                    if (i == 2) agent.destination = boss.stone.transform.position - (bSkill.featherDist - 1) * Vector3.right;
                    if (i == 3) agent.destination = boss.stone.transform.position - (bSkill.featherDist - 2) * new Vector3(1, 0, 1).normalized;
                }
                else if (!boss.stone.activeSelf)
                {
                    agent.destination = boss.transform.position - (bSkill.featherDist - 5) * Vector3.forward;
                }
            }
            else 
            {

                agent.destination = player.transform.position + 5 * allyFSM.moveDir; 
            }

            currTime = 0;
        }

        allyBodyRot.RotateAllyBody();
    }
}
