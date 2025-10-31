using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
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

    private Queue<SkillBase> skillQueue = new Queue<SkillBase>();
    private float delayBtwSkills = 0.2f;
    public bool IsOnSkill = false;

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

        SkillBase[] skills = { skillB, skillE, skillR, skillEv, skillP };
        foreach (var s in skills)
        {
            s.OnReady += EnqueueSkill;
        }

        StartQueueLoop();
    }

    private void ApplyLevelSettings(int level)
    {
        switch (level)
        {
            case 1:
                skillB.BCoolTime = 5;
                skillB.attackNum = 3;
                break;
            case 2:
                skillB.BCoolTime = 4;
                break;
            case 3:
                skillB.BCoolTime = 3;
                skillE.IsReady = true;
                skillE.Cooldown = 9;
                break;
            case 4:
                skillB.BCoolTime = 2;
                skillE.Cooldown = 7;
                break;
            case 5:
                skillE.Cooldown = 5;
                skillB.attackNum = 5;
                skillR.IsReady = true;
                skillR.Cooldown = 20;
                break;
            case 7:
                skillEv.IsReady = true;
                skillEv.Cooldown = 10;
                break;
        }
    }

    public void HandleStop(bool stop)
    {
        enabled = !stop;
    }

    private void EnqueueSkill(SkillBase skill)
    {
        skillQueue.Enqueue(skill);
    }

    private async UniTaskVoid StartQueueLoop()
    {
        while (!GameManager.instance.IsStop)
        {
            if(skillQueue.Count > 0)
            {
                await UniTask.WaitUntil(() => !IsOnSkill);
                var skill = skillQueue.Dequeue();

                skill.TryExecute();

                // 각 스킬 실행 간 간격 (살짝 텀을 주기)
                await UniTask.Delay(TimeSpan.FromSeconds(delayBtwSkills));
            }
            else
            {
                // 큐가 비어 있으면 잠깐 대기
                await UniTask.Yield();
            }
        }
    }
}
