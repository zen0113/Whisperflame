<div align="center">

![header](https://capsule-render.vercel.app/api?type=waving&color=gradient&customColorList=6,11,20&height=200&section=header&text=Whisper%20Flame&fontSize=70&animation=twinkling)

[![YouTube](https://img.shields.io/badge/YouTube-FF0000?style=for-the-badge&logo=youtube&logoColor=white)](YOUR_YOUTUBE_LINK)
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white)](YOUR_GITHUB_REPO_LINK)

</div>

## 📌 Overview

**장르 융합형 보스전 게임** - 사이드뷰 플랫포머와 탑다운 슈팅의 실시간 전환

동적 물리 제어 시스템을 통해 장르와 조작 방식이 실시간으로 전환되는 독창적인 액션 게임

<div align="center">

| | |
|:---:|:---:|
| **개발 기간** | 2025.05.24 ~ 2025.06.17 (3주) |
| **플랫폼** | Unity 2D |
| **개발 형태** | Solo Project |
| **역할** | Programmer, Game Designer |

</div>

---

## 🎮 Core Features

### 🔄 Dynamic Genre Switching System
**핵심 문제**: 하나의 캐릭터가 중력이 있는 '플랫포머'와 중력이 없는 '탑다운' 방식을 오가야 함

**구현 방식**:
- `SetMoveMode` 함수를 통해 Rigidbody2D 설정을 동적으로 재할당
- **Gravity Control**: 탑다운 모드 진입 시 `gravityScale = 0`으로 설정하여 자유 비행 구현, 사이드뷰 복귀 시 중력 복원
- **Physics Reset**: 모드 전환 순간 이전 물리 관성(`linearVelocity`)을 초기화하여 조작감의 이질감 제거
```csharp
// 핵심 코드 예시
void SetMoveMode(MoveMode mode) {
    currentMode = mode;
    if (mode == MoveMode.TopDown) {
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.None;
    } else {
        rb.gravityScale = originalGravity;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    rb.linearVelocity = Vector2.zero;
}
```

### 🎯 물리 & 시점 제어
- **Genre Switching System**: Rigidbody2D의 `gravityScale`과 `Constraints`를 런타임에 제어하여 사이드뷰↔탑다운 물리 설정 실시간 전환
- **Hybrid Input Handler**: 이동 모드(Enum)에 따라 점프(Y축)와 상하좌우(XY축) 입력을 분기 처리하는 하이브리드 컨트롤러 구현
- **Seamless Transition**: 장르 변화 시 카메라 오프셋과 추적 속도를 Lerp로 보정하여 시각적 위화감 최소화

### 💫 탄막 알고리즘
- **Trigonometric Logic**: 삼각함수를 활용해 360도 방사형, 나선형 등 수학적 원리를 적용한 탄막 패턴 설계
- **Async Pattern Logic**: Coroutine을 활용한 비동기 시퀀스 제어로 [경고 → 딜레이 → 발사]의 타이밍을 정교하게 관리하여 리듬감 있는 공격 구현

### 🤖 Wave-Based Boss FSM
**FSM 상태 제어**: Enum과 Update 루프로 'HP(1페이즈) → 시간(2페이즈)' 등 상이한 전환 조건을 정밀하게 관리

**구현 특징**:
- **라이프사이클 분리**: 상태 진입(Start)과 갱신(Update) 로직을 분리하여 코드 결합도를 낮추고 유지보수성 강화
- **Hybrid Flow Control**: 보스 이동 및 컷신 연출 등 비동기 작업에는 Coroutine을, 실시간 로직 판정에는 Update를 적재적소에 활용하여 최적화된 흐름 구현

---

## 🎯 전투 시스템 기획

### Phase-Based Design
`BattleWaveManager`를 통해 전투를 3단계로 구조화하여 기승전결 구현:

1. **1단계**: HP 기반 패턴 공격
2. **2단계**: 시간 버티기 미션
3. **3단계**: 최종 격파

### Mechanic Design
- **사이드뷰 단계**: '점프' 기반 회피 메커닉
- **탑다운 단계**: '정밀 회피' 요구하는 페이즈별 기믹(Gimmick) 차별화

---

## 💾 Data Structure & System

### Data Persistence & Sorting
- **PlayerPrefs**로 데이터를 영속화
- **LINQ(OrderBy)**를 활용한 정렬 로직을 통해 즉각적인 랭킹 시스템 구축

---

## 🛠 Technical Stack

<div align="center">

![Unity](https://img.shields.io/badge/Unity-000000?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)

</div>

**주요 기술**:
- Rigidbody2D Physics Control
- Coroutine-based Async Pattern
- FSM (Finite State Machine)
- LINQ Query
- PlayerPrefs

---

## 📊 Development Management

### 1인 개발 관리

**Scope Management**
- 3주라는 제한된 기간 내에 '장르 전환'이라는 핵심 코어(Core)에 집중
- 플레이 가능한 빌드 완성을 최우선 목표로 설정

**Polishing**
- 무료 에셋의 스프라이트 색상 변조 및 파티클 커스텀을 통해 일관된 아트 톤앤매너 유지

---

## 🎓 What I Learned

- 물리 엔진의 동적 제어를 통한 게임플레이 다양화
- FSM 패턴을 활용한 복잡한 보스 AI 구현
- 제한된 시간 내에서의 스코프 관리와 우선순위 설정
- 비동기 로직과 동기 로직의 적절한 분리 및 활용

---

## 📹 Demo

<div align="center">

[![YouTube Video](https://img.shields.io/badge/▶️_Watch_Demo-FF0000?style=for-the-badge&logo=youtube&logoColor=white)](YOUR_YOUTUBE_LINK)

</div>

---


<br>

<div align="center">

![footer](https://capsule-render.vercel.app/api?type=waving&color=gradient&customColorList=6,11,20&height=120&section=footer)

</div>
