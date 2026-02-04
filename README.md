<!-- ===================== -->

<!--  GitHub Project README -->

<!-- ===================== -->

<div align="center">

![header](https://capsule-render.vercel.app/api?type=waving\&color=gradient\&customColorList=6,11,20\&height=180\&section=header\&text=Whisper%20Flame\&fontSize=64\&animation=twinkling)

</div>

<br>

## Overview

사이드뷰 플랫포머와 탑다운 슈팅을 **실시간으로 전환하는 장르 융합형 보스전 게임**입니다.

물리 시스템, 카메라, 입력 구조를 하나의 제어 흐름으로 통합하여
플레이 도중에도 조작 방식과 시점이 자연스럽게 변화하도록 설계했습니다.

<br><br>

### Project Information

<div align="center">

| 항목    | 내용                           |
| :---- | :--------------------------- |
| 개발 기간 | 2025.05.24 ~ 2025.06.17 (3주) |
| 플랫폼   | Unity 2D                     |
| 개발 형태 | 1인 개발                        |
| 역할    | 프로그래밍 / 게임 기획                |

</div>

<br><br><br>

## Demo

<div align="center">

|                                                 플레이 화면                                                 | 설명                                     |
| :----------------------------------------------------------------------------------------------------: | :------------------------------------- |
| <img src="https://github.com/zen0113/Whisperflame/blob/main/whisperflame_1.gif?raw=true" width="320"/> <img src="https://github.com/zen0113/Whisperflame/blob/main/whisperflame_2.gif?raw=true" width="320"/> | 중력이 적용되는 사이드뷰 구간으로 점프 기반 회피 중심 전투      |
| <img src="https://github.com/zen0113/Whisperflame/blob/main/whisperflame_5.gif?raw=true" width="320"/> | 중력이 제거된 탑다운 구간에서 자유 이동과 정밀 회피를 요구하는 전투 |
| <img src="https://github.com/zen0113/Whisperflame/blob/main/whisperflame_3.gif?raw=true" width="320"/> | 게임 클리어 화면     |

</div>

<br><br><br>

## Core Systems

### Dynamic Genre Switching

하나의 캐릭터가
중력이 존재하는 **사이드뷰**와
중력이 없는 **탑다운** 방식을 오가며
이질감 없는 조작감을 유지해야 했습니다.

이를 위해 이동 모드를 단일 진입점으로 관리하고,
물리 속성을 런타임에 재구성하는 방식을 선택했습니다.

* `SetMoveMode` 함수를 중심으로 이동 모드 제어
* 모드에 따라 Rigidbody2D의 중력 및 제약 조건을 동적 변경
* 전환 시 이전 물리 관성을 초기화하여 조작 붕괴 방지

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

<br><br>

### Physics & Camera Control

이동 모드에 따라
물리 계산 방식과 시점 제어가 동시에 변경되도록 설계했습니다.

* `gravityScale`, `constraints`를 런타임에 전환
* Enum 기반 입력 분기로 점프 중심 / 자유 이동 로직 분리
* 장르 전환 시 카메라 오프셋과 추적 속도를 Lerp로 보정

<br><br>

### Bullet Pattern System

보스 공격은
단순 랜덤이 아닌
수학적 규칙을 가진 패턴으로 설계했습니다.

* 삼각함수를 활용한 방사형, 나선형 탄막 패턴
* Coroutine 기반 비동기 흐름으로
  경고 → 딜레이 → 발사 타이밍 제어

<br><br>

### Boss FSM Architecture

보스의 행동은
Enum 기반 FSM으로 관리했습니다.

* 체력 기반 페이즈와 시간 기반 페이즈 혼합 설계
* 상태 진입 로직과 갱신 로직을 분리하여 구조 명확화
* 패턴 추가 시 기존 코드 수정 없이 확장 가능하도록 구성

<br><br><br>

## Combat Design

### Phase Structure

`BattleWaveManager`를 통해
전투 흐름을 명확한 단계로 분리했습니다.

1. 체력 기반 패턴 공격 구간
2. 제한 시간 생존 미션 구간
3. 최종 격파 구간

<br>

### Mechanic Differentiation

장르에 따라
요구되는 플레이 감각이 달라지도록 설계했습니다.

* 사이드뷰 구간: 점프를 활용한 수직 회피 중심
* 탑다운 구간: 정밀한 위치 조정과 공간 인지 중심

<br><br><br>

## Data & Persistence

<img src="https://github.com/zen0113/Whisperflame/blob/main/whisperflame_4.png?raw=true" width="320"/>
플레이 데이터는
간단하지만 확장 가능한 구조로 관리했습니다.

* PlayerPrefs를 사용한 데이터 영속화
* LINQ(`OrderBy`) 기반 정렬을 통한 랭킹 시스템 구현

<br><br><br>

## Technical Stack

<div align="center">

![Unity](https://img.shields.io/badge/Unity-000000?style=for-the-badge\&logo=unity\&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge\&logo=csharp\&logoColor=white)

</div>

<br>

**주요 기술 요소**

* Rigidbody2D 물리 제어
* Coroutine 기반 비동기 처리
* FSM(Finite State Machine)
* LINQ
* PlayerPrefs

<br><br><br>

## Development Notes

3주라는 제한된 일정 안에서
모든 기능을 넣기보다는
핵심 메커닉 완성에 집중했습니다.

* 장르 전환 시스템을 가장 우선순위로 설정
* 기능 확장보다 플레이 가능한 빌드 완성을 목표로 진행
* 무료 에셋을 기반으로 색상 및 파티클을 수정하여 톤앤매너 유지

<br><br><br>

## Lessons Learned

이번 프로젝트를 통해
다음과 같은 점을 학습했습니다.

* 물리 엔진의 동적 제어를 통한 게임플레이 확장 가능성
* FSM 구조가 복잡한 보스 패턴 관리에 효과적임을 체감
* 제한된 일정에서의 스코프 관리와 우선순위 설정의 중요성
* 동기 로직과 비동기 로직을 분리한 설계의 안정성

<br><br><br>

<div align="center">

![footer](https://capsule-render.vercel.app/api?type=waving\&color=gradient\&customColorList=6,11,20\&height=120\&section=footer)

</div>
