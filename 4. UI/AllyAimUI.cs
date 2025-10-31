using UnityEngine;

public class AllyAimUI : MonoBehaviour
{
    private GameObject ally;
    private BSkill bSkill;
    [SerializeField] private GameObject aimUI;

    void Start()
    {
        ally = GameObject.Find("Ally");
        bSkill = ally.GetComponent<BSkill>();

        GameManager.instance.IsStopped += _ => DeactivateAimUI();
    }

    public void DeactivateAimUI()
    {
        aimUI.SetActive(false);
    }

    public void SetAimUIDir(Transform nearestTarget)
    {
        if(nearestTarget != null)
        {
            transform.rotation = Quaternion.LookRotation(-transform.forward, bSkill.attackDir);
        }
    }
}
