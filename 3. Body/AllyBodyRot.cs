using UnityEngine;

public class AllyBodyRot : MonoBehaviour
{
    private GameObject allyBody;
    private AllyNavMesh allyNaveMesh;

    private AttackManager attackManager;

    void Start()
    {
        allyBody = GameObject.Find("AllyBody");
        allyNaveMesh = GetComponent<AllyNavMesh>();

        attackManager = GetComponent<AttackManager>();
    }

    public void RotateAllyBody()
    {
        if (!attackManager.IsOnSkill)
            allyBody.transform.forward = allyNaveMesh.agent.destination - transform.position;
    }

}
