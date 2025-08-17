using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private H_PlayerManager pm;
    internal SkillBase[] Skills;
    [Header("하단의 스크립트들이 비어 있다면 채워주세요")]
    [SerializeField] private BSkill skillB;
    [SerializeField] private ESkill skillE;
    [SerializeField] private RSkill skillR;
    [SerializeField] private SkillEvolve skillEv;
    [SerializeField] private SkillPassive skillP;

    private List<SkillBase> activeSkills = new List<SkillBase>();
    private Queue<SkillBase> executionQueue = new Queue<SkillBase>();

    private void Awake()
    {
        Skills = GetComponentsInChildren<SkillBase>();
        if (Skills.Length != 5) throw new Exception("스킬 스크립트들이 하나 이상 비어 있습니다");
    }

    private void Start()
    {
        pm = GameObject.Find("H_PlayerManager").GetComponent<H_PlayerManager>();
        if (pm == null) throw new Exception("PlayerManager를 찾을 수 없습니다");

        pm.OnLevelChanged += ApplyLevelSettings;
        GameManager.instance.IsStopped += HandleStop;
    }

    private void Update()
    {
        // 쿨타임 끝난 스킬들 큐에 넣기
        foreach (var skill in activeSkills)
        {
            if (skill.IsReady())
            {
                executionQueue.Enqueue(skill);
            }
        }

        // 큐에서 하나씩 실행
        int count = executionQueue.Count;
        for (int i = 0; i < count; i++)
        {
            SkillBase skill = executionQueue.Dequeue();
            skill.TryExecute();
        }
    }

    private void ApplyLevelSettings(int level)
    {
        switch (level)
        {
            case 1:
                skillB.BSkillTime = 5;
                skillB.attackNum = 3;
                break;
            case 2:
                skillB.BSkillTime = 4;
                break;
            case 3:
                skillB.BSkillTime = 3;
                skillE.Cooldown = 9;
                skillB.BSkillTime = 4;
                break;
            case 4:
                skillB.BSkillTime = 2;
                skillE.Cooldown = 7;
                break;
            case 5:
                skillE.Cooldown = 5;
                skillB.attackNum = 5;
                skillR.Cooldown = 20;
                break;
            case 7:
                skillEv.Cooldown = 10;
                break;
        }
    }

    public void HandleStop(bool stop)
    {
        enabled = !stop;
    }

    public void AddSkill(SkillBase skill)
    {
        if (!activeSkills.Contains(skill))
            activeSkills.Add(skill);
    }
}
