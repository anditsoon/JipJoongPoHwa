using System.Collections;
using UnityEngine;

public class RSkill : SkillBase
{
    internal bool isRSkill = false;
    [SerializeField] private float featherRNo = 24;
    private float animDelayTime = 0.8f;

    private bool unbeatable = false;
    public bool Unbeatable => unbeatable;

    public RSkill()
    {
        Cooldown = RSkillTime;
        Priority = 3;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Execute();
        }
    }

    public override void Execute()
    {
        isRSkill = true;

        StartCoroutine(SetUnbeatable());
        StartCoroutine(AttackRSkill());
    }

    // R 스킬 때 무적 상태, 속도 빨라짐
    private IEnumerator SetUnbeatable()
    {
        unbeatable = true;
        allyNavMesh.moveSpeed *= 2;
        yield return new WaitForSeconds(2f);
        unbeatable = false;
        allyNavMesh.moveSpeed /= 2;
    }

    public IEnumerator AttackRSkill()
    {
        anim.SetTrigger("RSKILL");
        yield return new WaitForSeconds(animDelayTime);

        for (int i = 1; i <= featherRNo; i++)
        {
            // 깃털 360도로 퍼지게
            GameObject feather = ObjectPoolManager.instance.featherPool.Get();
            feather.layer = LayerMask.NameToLayer("Feather");
            feather.transform.localEulerAngles = new Vector3(0, (360 / featherRNo) * i, 0);
            feather.transform.position = transform.position + featherDist * feather.transform.forward;

            // 파티클 재생
            PlaceAttackParticle(gameObject, feather.transform.forward);

            // Enemy 에게 데미지 주기
            ApplyDamageToPiercedTargets(transform.position + Vector3.up * 0.5f, feather.transform.forward);

            PlaceFeather();
        }

        isRSkill = false;
        anim.SetTrigger("BLEND_TREE");
    }
}
