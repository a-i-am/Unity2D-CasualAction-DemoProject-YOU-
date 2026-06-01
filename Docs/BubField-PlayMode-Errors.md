# Bub Field 플레이모드 에러 정리

## 에러 1 — Missing Script / NullReferenceException

**로그**
```
The referenced script on this Behaviour (Game Object 'Player') is missing!
NullReferenceException at UI.cs:86 → player.projectilePrefab
```

**원인**
- `PlayerScr.cs` 삭제됨 → Player 오브젝트에 Missing Script 잔존
- `UI.cs` Awake: `GetComponent<Player>()` → null → 86번 라인 크래시

**해결**
1. Player 오브젝트 Inspector → Missing Script 제거
2. 새 `Player.cs` (Refactoring/Player/) 컴포넌트 추가
3. 시리얼라이즈 필드 재연결 (`projectilePrefab`, `launchOffsetL/R`, `ghost` 등)

---

## 에러 2 — Addressables 카탈로그 미빌드

**로그**
```
Invalid path: 'Library/com.unity.addressables/aa/Windows/settings.json'
No Location found for Key=ItemTable
Failed to load ItemTable via Addressables.
```

**원인**
- `InventoryDatabase.cs`가 `Addressables.LoadAssetAsync<TextAsset>("ItemTable")` 호출
- Addressables 한 번도 빌드 안 함 → catalog 없음, `"ItemTable"` 주소 미등록

**해결 A — 개발 중 빠른 방법**
```
Window → Asset Management → Addressables → Groups
→ Play Mode Script → "Use Asset Database (faster)"
```

**해결 B — 정식 등록**
1. Addressables Groups에서 그룹 생성
2. `ItemTable`, `CharacterTable` TextAsset을 그룹에 추가, 주소 동일하게 설정
3. `Build → New Build → Default Build Script` 실행

---

## 우선순위

1. Player Missing Script 제거 + `Player.cs` 재연결 → UI.cs NullRef 해소
2. Addressables Play Mode Script 변경 → Inventory 에러 해소
