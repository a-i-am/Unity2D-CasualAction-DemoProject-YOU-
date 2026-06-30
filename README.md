# YOU

[한국어](#한국어) | [English](#english)

[플레이 영상](https://youtu.be/DXausfETae0) | [프로젝트 PDF](https://github.com/user-attachments/files/25697139/You_.pdf)

## 한국어

기절시킨 적을 수집하고 아군 팔로워로 다시 소환하는 2D 캐주얼 액션 게임입니다.

### 프로젝트 정보

| 항목 | 내용 |
| --- | --- |
| 개발 기간 | 2025-09 - 2025-12 |
| 리팩터링 기간 | 2026-05-22 - 2026-06-01 |
| 인원 | 1인 |
| 엔진 | Unity 2022.3.28f1 |
| 플랫폼 | Windows |

### 핵심 구현

- 기절한 적을 수집, 인벤토리 등록, 팔로워 소환으로 연결
- 여러 팔로워가 같은 적을 중복 선택하지 않는 거리 기반 타깃 배정
- 팔로워의 대시 공격과 원래 편대 위치 복귀
- 아이템 원본 데이터와 슬롯별 인스턴스를 분리한 인벤토리 리팩터링
- Cinemachine 기반 컷신과 보스 패턴 연출

### 구현 이유

- 팔로워 생성 순서를 유지하기 위해 등록 순서가 보장되는 큐를 사용했습니다.
- 탐지 후보 중복을 막고 제거 비용을 낮추기 위해 집합 기반 후보 목록을 사용했습니다.
- 대시 시작 시 복귀 좌표를 고정해 이동 중 편대 위치가 변해도 안정적으로 돌아오게 했습니다.
- 슬롯마다 새 아이템 인스턴스를 생성해 같은 원본 데이터를 참조하던 삭제 버그를 해결했습니다.

### 기술 스택

`Unity 2D` `C#` `JSON` `DOTween` `Cinemachine` `URP`

### 리팩터링

- 아이템 정의와 소유 인스턴스 분리
- 인벤토리 슬롯 삭제 대상 식별 수정
- 팔로워 타깃 탐색과 컴포넌트 참조 캐싱 정리

### 에셋 출처

- DOTween: Demigiant
- Seasonal Tilesets: GrafxKid, CC0 1.0
- Stylized 2D Alpine Nature Pack: Enxemac
- 포함된 외부 패키지와 에셋의 원본 라이선스 파일 적용

### 배운 점

공유 원본 데이터와 실행 중 생성되는 상태를 분리해야 인벤토리, 팔로워, 타깃 시스템을 독립적으로 수정할 수 있다는 점을 배웠습니다.

## English

YOU is a 2D casual action game where stunned enemies can be collected and redeployed as allied followers.

### Project

- Development: 2025-09 - 2025-12
- Refactoring: 2026-05-22 - 2026-06-01
- Team: Solo
- Engine: Unity 2022.3.28f1

### Highlights

- Enemy capture, inventory registration, and follower deployment form one gameplay loop.
- Followers reserve nearby targets without selecting the same enemy.
- Dash attacks return to a position captured at attack start.
- Inventory definitions and owned item instances are separated to fix slot deletion errors.

### Implementation Decisions

- A queue preserves follower deployment order.
- A set prevents duplicate detection candidates and supports fast removal.
- The return position is captured when a dash begins, so formation movement does not change the destination mid-attack.
- Each slot owns a new item instance instead of sharing one mutable definition reference.

### Refactoring

- Separated item definitions from owned runtime instances.
- Corrected inventory-slot deletion identity.
- Reorganized follower targeting and cached component references.

### Stack and Assets

`Unity 2D` `C#` `JSON` `DOTween` `Cinemachine` `URP`

- DOTween: Demigiant
- Seasonal Tilesets: GrafxKid, CC0 1.0
- Stylized 2D Alpine Nature Pack: Enxemac
- Included third-party packages remain subject to their original licenses.

### Lessons

Separating immutable definitions from runtime ownership made inventory, targeting, and follower behavior easier to reason about and refactor.
