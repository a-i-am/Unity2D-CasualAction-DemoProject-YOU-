# [Unity 2D] CasualAction DemoProject : YOU

[플레이 시연 영상](https://youtu.be/DXausfETae0?si=pR9YFLdsrxy2ShNM) | [프로젝트 설명 문서 (PDF)](https://github.com/user-attachments/files/25697139/You_.pdf)

> 유니티 2D 엔진 메커니즘 학습 및 캐주얼 액션 아케이드 플랫폼 모작/개발 데모 프로젝트입니다.

---

### 프로젝트 정보

| 항목 | 내용 |
| --- | --- |
| 개발 기간 | 2024-03 - 2024-05 |
| 리팩터링 이력 | 1차: 2026-08-10 (C# 소스코드 컨벤션 & 캡슐화 완료) |
| 개발 인원 | 개인 프로젝트 (1인) |
| 엔진 및 버전 | Unity 2D (Unity 2022.3.x) |
| 장르 | 2D 캐주얼 액션 아케이드 |

### 기술 스택

<p>
  <img src="https://img.shields.io/badge/Unity 2D-000000?style=flat-square&logo=unity&logoColor=white"/>
  <img src="https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white"/>
  <img src="https://img.shields.io/badge/JSON Inventory-000000?style=flat-square"/>
  <img src="https://img.shields.io/badge/Object Pooling-000000?style=flat-square"/>
</p>

---

### 프로젝트 구조

<p>
  <img src="https://img.shields.io/badge/⭐_본인_담당_작업-FF5722?style=flat-square"/>
  <img src="https://img.shields.io/badge/기반_시스템-00599C?style=flat-square"/>
</p>

프로젝트의 핵심 C# 소스코드는 `Assets/` 아래에 기능별로 구조화되어 있습니다. 아래 파일 링크를 클릭하면 GitHub 소스코드 파일로 바로 이동합니다.

| 모듈 구분 | 파일 / 경로 | 담당 및 핵심 로직 설명 |
| :--- | :--- | :--- |
| <img src="https://img.shields.io/badge/⭐-FF5722?style=flat-square"/> **Player Core** | [`PlayerScr.cs`](Assets/Scripts/Objects/Characters/Players/PlayerScr.cs) | **플레이어 이동, 코요태 점프, 잔상 대시, 발사체 공격 및 피격/사망/리스폰 총괄** |
| <img src="https://img.shields.io/badge/⭐-FF5722?style=flat-square"/> **Player Anim** | [`PlayerAnimScr.cs`](Assets/Scripts/Objects/Characters/Players/PlayerAnimScr.cs) | **플레이어 이동, 공중 발사, 캐스팅 메카닉과 Animator 파라미터 연동** |
| <img src="https://img.shields.io/badge/⭐-FF5722?style=flat-square"/> **Camera** | [`PlayerFollowCamera.cs`](Assets/PlayerFollowCamera.cs) | **플레이어 Transform 추적 및 2D 맵 경계선(Clamp) 카메라 제어** |
| **Inventory** | [`ItemDatabase2.cs`](Assets/Scripts/Objects/Item/ItemDatabase2.cs) | JSON 데이터 기반 아이템 슬롯 및 인벤토리 로딩 |
| **UI System** | [`UI.cs`](Assets/Scripts/Objects/UI%20Control/UI.cs) | 플레이어 체력 HUD, 대화창 및 슬롯 UI 이벤트 동기화 |

#### 디렉토리 트리 구조

```text
Assets/
├── ⭐ PlayerFollowCamera.cs                       (2D 카메라 바운더리 추적)
└── 📂 Scripts/
    ├── 📂 Core/                                  (싱글톤 및 코어 인터페이스)
    └── 📂 Objects/
        ├── 📂 Characters/Players/
        │   ├── ⭐ PlayerScr.cs                    (플레이어 메인 조작 & 피격/점프/대시)
        │   └── ⭐ PlayerAnimScr.cs                (애니메이터 제어 & 파라미터 동기화)
        ├── 📂 Item/
        │   └── ItemDatabase2.cs                 (JSON 기반 아이템 데이터베이스)
        └── 📂 UI Control/
            └── UI.cs                            (HUD 및 UI 조작 컨트롤러)
```


---

### 플레이 및 조작 방법

Uni 2D Casual Action은 빠른 호흡의 액션과 타이밍 조작이 강조된 2D 플랫폼 액션 데모입니다.

#### 1. 기본 이동 및 조작키
- **좌우 이동 ([A] / [D] 또는 방향키)**: 캐릭터가 이동하는 방향에 맞춰 Sprite가 자동으로 Flip 처리됩니다.
- **점프 ([Space Bar])**: 지면 판정(`CheckGrounded`) 및 코요태 타임(Coyote Jump)이 적용되어 지면을 벗어난 직후에도 점프가 가능합니다.
- **잔상 대시 ([C])**: 대시 사용 시 잔상(Ghost Effect)과 함께 빠르게 돌진하며, 대시 도중 일시적으로 적 충돌을 회피합니다.

#### 2. 공격 및 기술 매카닉
- **원거리 투사체 발사 ([Z])**: 지상 및 공중에 맞춰 `LaunchAnimation`과 함께 투사체를 발사합니다.
- **마법 캐스팅 ([X])**: 지상 정지 상태에서 마법 캐스팅 파티클 이펙트를 재생하며 스펠을 준비합니다.

#### 3. 체력 및 리스폰
- 적 또는 공격에 충돌 시 무적 타임(3초)과 함께 넉백 피격 판정이 수행됩니다.
- 체력이 0이 되면 사망 모션과 함께 지정된 위치(`lastSafePosition`)로 리스폰됩니다.

---

### 담당 작업

- 플레이어 2D 릿지드바디 Physics 이동 및 코요태 타임 점프 로직 구현
- 잔상 이펙트(Ghost)가 적용된 대시 메커니즘 및 무적 피격 판정
- 2D 발사체 스폰 및 애니메이터 동기화
- 맵 경계 Clamping 처리된 PlayerFollowCamera 스크립트 작성
- JSON 인벤토리 데이터 파싱 및 HUD UI 연결

---

### 프로젝트 설명 자료 (슬라이드 갤러리)

<details>
<summary>📸 프로젝트 소개 PPT 슬라이드 펼치기</summary>

<img width="100%" alt="슬라이드1" src="https://github.com/user-attachments/assets/74731523-c89f-4a41-b0d1-33cb673ecadf" />
<img width="100%" alt="슬라이드2" src="https://github.com/user-attachments/assets/99d985fb-cc23-4fb1-beb9-aa0300cc8bad" />
<img width="100%" alt="슬라이드3" src="https://github.com/user-attachments/assets/6120db62-a651-44b2-9aff-7c107cf6a11f" />
<img width="100%" alt="슬라이드4" src="https://github.com/user-attachments/assets/03ac2e21-2837-4bac-92cb-7ec1c6827fed" />
<img width="100%" alt="슬라이드5" src="https://github.com/user-attachments/assets/0f726d0c-369f-4b83-a028-05738f5dcdba" />
<img width="100%" alt="슬라이드6" src="https://github.com/user-attachments/assets/8815a866-5693-4996-83a5-895a3c7ed8f0" />
<img width="100%" alt="슬라이드7" src="https://github.com/user-attachments/assets/61cee6cf-eb82-46c1-9ef6-2cd8cd55dd36" />
<img width="100%" alt="슬라이드8" src="https://github.com/user-attachments/assets/8eabe473-2f73-4fee-809b-fb267babaaca" />
<img width="100%" alt="슬라이드9" src="https://github.com/user-attachments/assets/775091e8-aeed-47eb-9fca-73b6462ed871" />
<img width="100%" alt="슬라이드10" src="https://github.com/user-attachments/assets/fcd976d2-c0b7-449e-97ce-38e1fd0f4b18" />

</details>


