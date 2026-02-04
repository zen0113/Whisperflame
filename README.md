<!-- ===================== -->

<!--  GitHub Project README -->

<!-- ===================== -->

<div align="center">

![header](https://capsule-render.vercel.app/api?type=waving\&color=gradient\&customColorList=6,11,20\&height=180\&section=header\&text=Whisper%20Flame\&fontSize=64\&animation=twinkling)

</div>

---

## Overview

**사이드뷰 플랫포머와 탑다운 슈팅을 실시간으로 전환하는 장르 융합형 보스전 게임**

물리 시스템, 카메라, 입력 구조를 하나의 제어 흐름으로 통합하여
플레이 도중 장르와 조작 방식이 자연스럽게 전환되는 액션 게임입니다.

---

### Project Information

<div align="center">

| 항목    | 내용                           |
| :---- | :--------------------------- |
| 개발 기간 | 2025.05.24 ~ 2025.06.17 (3주) |
| 플랫폼   | Unity 2D                     |
| 개발 형태 | 1인 개발                        |
| 역할    | 프로그래밍 / 게임 기획                |

</div>

---

## Demo

<div align="center">

|                                                 플레이 화면                                                 | 설명                                     |
| :----------------------------------------------------------------------------------------------------: | :------------------------------------- |
| <img src="https://github.com/zen0113/Whisperflame/blob/main/whisperflame_1.gif?raw=true" width="320"/> | 중력이 적용되는 사이드뷰 구간으로 점프 기반 회피 중심 전투      |
| <img src="https://github.com/zen0113/Whisperflame/blob/main/whisperflame_2.gif?raw=true" width="320"/> | 중력이 제거된 탑다운 구간에서 자유 이동과 정밀 회피를 요구하는 전투 |
| <img src="https://github.com/zen0113/Whisperflame/blob/main/whisperflame_3.gif?raw=true" width="320"/> | 전투 도중 장르가 전환되며 보스 패턴이 변화하는 핵심 시스템      |

</div>

---

## Core Systems

### Dynamic Genre Switching

**문제 상황**
하나의 캐릭터가 중력이 존재하는 사이드뷰와 중력이 없는 탑다운 방식을 오가며
이질감 없는 조작감을 유지해야 했습니다.

**해결 방식**

* `SetMoveMode` 함수를 중심으로 이동 모드를 단일 진입점에서 제어
* 모드에 따라 Rigidbody2D의 중력 및 제약 조건을 동적으로 재설정
* 모드 전환 시 이전 물리 관성 값을 초기화하여 조작감 붕괴 방지

```csharp
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

---

### Physics & Camera Control

* 이동 모드에 따라 `gravityScale`, `constraints`를 런타임에 전환
* Enum 기반 입력 분기를 통해 점프 중심 / 자유 이동 입력을 구분 처리
* 장르 전환 시 카메라 오프셋과 추적 속도를 Lerp로 보정하여 시각적 위화감 최소화

---

### Bullet Pattern System

* 삼각함수 기반 로직으로 방사형, 나선형 등 수학적 규칙을 가진 탄막 패턴 설계
* Coroutine을 활용해 경고 → 딜레이 → 발사의 흐름을 비동기적으로 제어

---

### Boss FSM Architecture

* Enum 기반 FSM으로 보스 상태를 관리
* 체력 기반 페이즈와 시간 기반 페이즈를 혼합한 전환 조건 설계
* 상태 진입 로직과 갱신 로직을 분리하여 유지보수성과 확장성 확보

---

## Combat Design

### Phase Structure

`BattleWaveManager`를 통해 전투 흐름을 3단계로 구조화했습니다.

1. 체력 기반 패턴 공격 구간
2. 제한 시간 동안 생존하는 미션 구간
3. 최종 격파 구간

### Mechanic Differentiation

* 사이드뷰 구간: 점프를 활용한 수직 회피 중심 설계
* 탑다운 구간: 정밀한 위치 조정과 공간 인지를 요구하는 회피 메커닉

---

## Data & Persistence

<img src="https://github.com/zen0113/Whisperflame/blob/main/whisperflame_4.png?raw=true" width="320"/>
* PlayerPrefs를 사용해 플레이 데이터 영속화
* LINQ(`OrderBy`) 기반 정렬을 통해 즉각적인 랭킹 시스템 구현

---

## Technical Stack

<div align="center">

![Unity](https://img.shields.io/badge/Unity-000000?style=for-the-badge\&logo=unity\&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge\&logo=csharp\&logoColor=white)

</div>

**주요 기술 요소**

* Rigidbody2D 물리 제어
* Coroutine 기반 비동기 처리
* FSM(Finite State Machine)
* LINQ
* PlayerPrefs

---

## Development Notes

* 3주라는 제한된 기간 내에서 장르 전환이라는 핵심 메커닉에 집중
* 기능 확장보다 플레이 가능한 완성 빌드를 우선 목표로 설정
* 무료 에셋을 기반으로 색상 및 파티클을 수정하여 시각적 통일성 확보

---

## Lessons Learned

* 물리 엔진의 동적 제어를 통해 게임 플레이 경험을 다양화할 수 있음을 체감
* FSM 구조가 복잡한 보스 패턴 관리에 효과적임을 학습
* 제한된 일정에서의 스코프 관리와 우선순위 설정의 중요성 인식
* 동기 로직과 비동기 로직을 명확히 분리하는 설계의 필요성

---

<div align="center">

![footer](https://capsule-render.vercel.app/api?type=waving\&color=gradient\&customColorList=6,11,20\&height=120\&section=footer)

</div>
