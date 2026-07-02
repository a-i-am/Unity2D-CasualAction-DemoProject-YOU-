# YOU

[플레이 영상](https://youtu.be/DXausfETae0) | [프로젝트 PDF](Docs/You_.pdf)

적을 기절시켜 수집한 뒤, 아군 팔로워(Follower)로 소환하여 전투에 활용하는 2D 캐주얼 액션 데모입니다.

README 업데이트: 2026-07-01



### 프로젝트 정보

| 항목 | 내용 |
| --- | --- |
| 개발 기간 | 2024-08 - 2024-10 |
| 리팩터링 이력 | 1차: 2026-05-22 - 2026-06-01 |
| 인원 | 1인 |
| 엔진 | Unity 2022.3.28f1 |
| 플랫폼 | Windows |

### 기술 스택
<p>
  <img src="https://img.shields.io/badge/Unity-000000?style=flat-square&logo=unity&logoColor=white"/>
  <img src="https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white"/>
  <img src="https://img.shields.io/badge/Cinemachine-000000?style=flat-square"/>
  <img src="https://img.shields.io/badge/DOTween-000000?style=flat-square"/>
  <img src="https://img.shields.io/badge/URP-000000?style=flat-square"/>
</p>

### 서드파티 플러그인 연동
- DOTween 플러그인을 활용한 UI 및 컷신 트윈 연출 구현

### 프로젝트 구조
```text
(프로젝트 구조도 추가 예정)
```

### 플레이 및 조작 방법
*(캐릭터 이동 및 공격, 적 포획 및 팔로워 소환/해제 버튼 안내 작성 예정)*

### 핵심 구현

- 기절한 적 수집, 인벤토리 등록, 팔로워 소환으로 이어지는 게임 흐름
- 다중 팔로워 간 거리 기반 고유 타겟팅 (중복 선택 방지)
- 팔로워 대시 공격 및 기존 위치 복귀 로직
- 아이템 원본 데이터와 슬롯별 생성 인스턴스를 분리한 인벤토리 리팩터링
- Cinemachine 기반 카메라 워크 및 보스 패턴 연출

### 업데이트 계획

- 팔로워 AI 또는 컷신 캡처 추가 검토
- 사용한 에셋 출처 표기 예정
