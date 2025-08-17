using UnityEngine;

public class AllyBodyRot : MonoBehaviour
{
    private GameObject allyBody;
    private AllyNavMesh allyNaveMesh;

    private BSkill bSkill;
    private ESkill eSkill;
    private RSkill rSkill;

    void Start()
    {
        allyBody = GameObject.Find("AllyBody");
        allyNaveMesh = GetComponent<AllyNavMesh>();
        bSkill = GetComponent<BSkill>();
        eSkill = GetComponent<ESkill>();
        rSkill = GetComponent<RSkill>();
    }

    public void RotateAllyBody()
    {
        if (!bSkill.isBSkill && !eSkill.IsESkill && !rSkill.isRSkill)
            allyBody.transform.forward = allyNaveMesh.agent.destination - transform.position;
    }

}
