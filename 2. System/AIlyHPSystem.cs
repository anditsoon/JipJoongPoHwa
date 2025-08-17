using System.Collections;
using UnityEngine;

public class AIlyHPSystem : MonoBehaviour
{
    private float maxHealth = 500;
    public float MaxHealth
    {
        get { return maxHealth; }
        private set { maxHealth = value; }
    }
    private float currHealth;
    public float CurrHealth
    {
        get => currHealth;
        private set
        {
            if (currHealth != value)
            {
                currHealth = Mathf.Clamp(value, 0, MaxHealth);
                UpdateHPBar();
            }
        }
    }
    private float timeTillReborn;
    public bool IsDead;
    private bool rebornable;

    private GameObject allyBody;
    private GameObject playerBody;
    private AllyFSM allyFSM;
    private RSkill rSkill;

    [SerializeField] private UnityEngine.UI.Image hpBar;
    [SerializeField] private GameObject aimUI;
    [SerializeField] private GameObject hpUI;

    void Start()
    {
        currHealth = maxHealth;
        IsDead = false;
        rebornable = true;

        timeTillReborn = 7f;
        allyBody = GameObject.Find("AllyBody");
        playerBody = GameObject.Find("Player");
        allyFSM = allyBody.GetComponentInParent<AllyFSM>();
        rSkill = GetComponent<RSkill>();
        GameManager.instance.IsStopped += _ => Die();
    }

    public void UpdateHPBar()
    {
        hpBar.fillAmount = currHealth / maxHealth;
    }

    public void UpdateHp(float amount)
    {
        if (rSkill.Unbeatable || IsDead) return;

        CurrHealth += amount; // amount 가 음수면 공격

        if (amount < 0)
        {
            allyFSM.HitAlly(amount);
        }
    }

    public void Die()
    {
        if (IsDead) return;

        IsDead = true;
        hpUI.SetActive(false);
        aimUI.SetActive(false);

        GameObject.Find("AllyBody").SetActive(false);

    }

    public void Reborn()
    {
        if(rebornable)
        {
            rebornable = false;
            StartCoroutine(RebornCrt());
        }
    }

    private IEnumerator RebornCrt()
    {
        yield return new WaitForSeconds(timeTillReborn);

        gameObject.transform.position = playerBody.transform.position + 3 * transform.right;
        allyBody.SetActive(true);

        gameObject.GetComponent<AttackManager>().HandleStop(false);

        currHealth = maxHealth;
        IsDead = false;

        aimUI.SetActive(true);
        hpUI.SetActive(true);

        allyFSM.chooseDir = StartCoroutine(allyFSM.ChooseDir());

        rebornable = true;
    }

}
