# 🔥 Whisper Flame

> **사이드뷰 플랫포머 ↔ 탑다운 슈팅이 실시간으로 전환되는 장르 융합형 보스전 게임**

https://youtu.be/-_z0IJ-bDIc

## 💡 주요 기능 (Key Features)

* **Dynamic Genre Switching:** 플레이 도중 중력과 시점이 실시간으로 변하는 독창적 시스템.
* **Wave-Based Boss FSM:** HP와 시간 흐름에 따라 변하는 보스 패턴 구현.
* **Custom Bullet Logic:** 삼각함수(Trigonometric)를 활용한 방사형/나선형 탄막 패턴.

## 💻 기술적 도전 (Technical Challenges)

### 💥 실시간 장르 전환 시스템 (Physics & Input Handling)
**Challenge:** 하나의 캐릭터 객체가 '중력이 있는 플랫포머'와 '무중력 비행 슈팅'을 오가야 하는 문제.
**Solution:**
1.  **Gravity Control:** `Rigidbody2D.gravityScale`을 런타임에 조절 (TopDown: 0 / SideView: 2).
2.  **Physics Reset:** 모드 전환 순간 `linearVelocity`를 초기화하여 이전 모드의 물리 관성이 남지 않도록 처리.
3.  **Hybrid Input Handler:** `MoveMode` Enum 상태에 따라 입력 처리 로직(점프 vs 8방향 이동)을 분기 처리하는 하이브리드 컨트롤러 구현.

### 🏛️ 구조적 설계 (Architecture)
* **FSM (Finite State Machine):** 보스의 상태(Start, Update) 로직을 분리하여 유지보수성 강화.
* **Data Persistence:** PlayerPrefs와 LINQ(OrderBy)를 활용한 로컬 랭킹 시스템 구축.
