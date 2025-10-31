포트폴리오용으로 본인이 작성한 코드만 업로드한 레포지토리입니다.

# LOL 집중포화 모작
![Image](https://github.com/user-attachments/assets/0c313b28-1a03-429b-b14b-a59a6c1bcd69)
- 2024년 LOL 여름 이벤트 뱀서라이크 모드 집중포화의 모작
- 본래 멀티플레이 게임인 집중포화를 1인용 게임으로 변경
        → 플레이어를 보조하는 아군 AI를 추가하여 담당 및 구현
<br/>
시연 영상 보러가기 → [Click!](https://www.youtube.com/watch?v=qvJb0eChcuY)
<br/>
<br/>

## About Project
- **장르**: 뱀서라이크 / 멀티플레이
- **작업 기간**: 2024.7.18 - 2024.8.16
- **팀 구성**: 프로그래머 3인, 아트 2인, 기획 1인
- **담당 파트**: 아군AI / 캐릭터 UI
- **키워드**: 상태 머신 / 추상화를 활용한 효율적인 스킬 관리 / 오브젝트 풀링
<br/>

## 담당 구현 코드
AI 기능을 중심으로 구현하고, 동시에 PlayerManager 와 EnemyManager 와의 상호작용을 담당했습니다.
<img width="791" height="548" alt="Image" src="https://github.com/user-attachments/assets/76e7ad7c-9267-48ec-8bde-3b7a1319549a" />

- 아군 AI 기능을 중심으로 상태 머신을 활용한 빠른 상태 전환을 통해 플레이어와 에너미 캐릭터와 상호작용 <br/>
[Ally System](https://github.com/anditsoon/JipJoongPoHwa/tree/main/2.%20System)
- 추상화를 활용하여 총 5개의 아군 AI 스킬을 효율적으로 구현 <br/>
  [SkillBase](https://github.com/anditsoon/JipJoongPoHwa/blob/main/1.%20Combat/SkillBase.cs) &ensp; [BSkill](https://github.com/anditsoon/JipJoongPoHwa/blob/main/1.%20Combat/BSkill.cs) &ensp; [ESkill](https://github.com/anditsoon/JipJoongPoHwa/blob/main/1.%20Combat/ESkill.cs) &ensp; [RSkill](https://github.com/anditsoon/JipJoongPoHwa/blob/main/1.%20Combat/RSkill.cs) &ensp; [SkillEvolve](https://github.com/anditsoon/JipJoongPoHwa/blob/main/1.%20Combat/SkillEvolve.cs) &ensp; [SkillPassive](https://github.com/anditsoon/JipJoongPoHwa/blob/main/1.%20Combat/SkillPassive.cs)
- 플레이어의 레벨업과 연동된 AI 성장 시스템 <br/>
  [AttackManager](https://github.com/anditsoon/JipJoongPoHwa/blob/main/1.%20Combat/AttackManager.cs)
