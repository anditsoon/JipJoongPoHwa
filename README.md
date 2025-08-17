# LOL 집중포화 모작
![Image](https://github.com/user-attachments/assets/4c523900-8622-44d7-938c-ae968095c61b)
- 2024년 LOL 여름 이벤트 뱀서라이크 모드 집중포화의 모작
- 본래 멀티플레이 게임인 집중포화를 1인용 게임으로 변경
        → 플레이어를 보조하는 아군 AI를 추가하여 담당 및 구현

## About Project
- **장르**: 뱀서라이크 / 멀티플레이
- **작업 기간**: 2024.7.18 - 2024.8.16
- **팀 구성**: 프로그래머 3인, 아트 2인, 기획 1인
- **담당 파트**: 아군AI / 캐릭터 UI
- **키워드**: 상태 머신 / 추상화를 활용한 효율적인 스킬 관리 / 오브젝트 풀링

## 담당 구현 코드
AI 기능을 중심으로 구현하고, 동시에 PlayerManager 와 EnemyManager 와의 상호작용을 담당했습니다.
<img width="791" height="548" alt="Image" src="https://github.com/user-attachments/assets/e20f5cc4-cd7b-4b4a-bbf3-f65e7303ae5c" />

- 아군 AI 기능을 중심으로 상태 머신을 활용한 빠른 상태 전환을 통해 플레이어와 에너미 캐릭터와 상호작용 <br/>
  [AIFSM](https://github.com/anditsoon/JipJoongPoHwa/blob/Develop_Beta/Assets/YDW/Scripts/Y_AllyFSM.cs) &ensp; [AIHPSystem](https://github.com/anditsoon/JipJoongPoHwa/blob/Develop_Beta/Assets/YDW/Scripts/AIHPSystem.cs) 
- 추상화를 활용하여 총 5개의 아군 AI 스킬을 효율적으로 구현 <br/>
  [SkillBase](https://github.com/anditsoon/JipJoongPoHwa/blob/Develop_Beta/Assets/YDW/Scripts/NewScripts/SkillBase.cs) &ensp; [BSkill](https://github.com/anditsoon/JipJoongPoHwa/blob/Develop_Beta/Assets/YDW/Scripts/NewScripts/BSkill.cs) &ensp; [ESkill](https://github.com/anditsoon/JipJoongPoHwa/blob/Develop_Beta/Assets/YDW/Scripts/NewScripts/ESkill.cs) &ensp; [RSkill](https://github.com/anditsoon/JipJoongPoHwa/blob/Develop_Beta/Assets/YDW/Scripts/NewScripts/RSkill.cs) &ensp; [SkillEvolve](https://github.com/anditsoon/JipJoongPoHwa/blob/Develop_Beta/Assets/YDW/Scripts/NewScripts/SkillEvolve.cs) &ensp; [SkillPassive](https://github.com/anditsoon/JipJoongPoHwa/blob/Develop_Beta/Assets/YDW/Scripts/NewScripts/SkillPassive.cs)
- 플레이어의 레벨업과 연동된 AI 성장 시스템 <br/>
  [AttackManager](https://github.com/anditsoon/JipJoongPoHwa/blob/Develop_Beta/Assets/YDW/Scripts/NewScripts/AttackManager.cs)
