# Unity 프로젝트 설정 가이드

## 📋 프로젝트 정보

- **Unity 버전**: 2022.3.62f2 LTS
- **템플릿**: Universal 2D (SRP)
- **플랫폼**: Android
- **스크립팅**: C# 9.0

---

## ⚙️ 필수 프로젝트 설정

### 1. Player Settings

Unity Editor에서 `Edit → Project Settings → Player`로 이동하여 다음 설정을 적용하세요.

#### Company & Product
```
Company Name: YourCompanyName
Product Name: Castle Lord Tycoon
```

#### Android Settings

**Identification**:
```
Package Name: com.yourcompany.castlelordtycoon
Version: 0.1.0
Bundle Version Code: 1
```

**Minimum API Level**:
- API Level 24 (Android 7.0 Nougat)

**Target API Level**:
- API Level 33+ (최신 Play Store 요구사항)

**Scripting Backend**:
- IL2CPP (성능 최적화)

**Target Architectures**:
- ✅ ARM64 (필수)
- ❌ ARMv7 (제거 - 64비트만)

**Configuration**:
```
Scripting Define Symbols: DEVELOPMENT (개발 빌드용)
```

---

### 2. Quality Settings

`Edit → Project Settings → Quality`

**Levels**:
- Default: Medium (모바일 균형)
- 추가 레벨 제거 (Low, High, Very High 삭제)

**Medium 설정**:
```yaml
Pixel Light Count: 1
Texture Quality: Full Res
Anisotropic Textures: Per Texture
Anti Aliasing: 2x Multi Sampling
Soft Particles: Off
Shadows: Disable (2D 게임)
Shadow Resolution: Low Resolution
Shadow Projection: Close Fit
Shadow Distance: 20
Shadow Cascades: No Cascades
Blend Weights: 2 Bones
VSync Count: Every V Blank
```

---

### 3. Graphics Settings

`Edit → Project Settings → Graphics`

**Scriptable Render Pipeline**:
- URP-HighQuality-Renderer (기본 생성됨)

**URP Asset 최적화**:
1. `Assets/Settings/URP-HighQuality-Renderer.asset` 선택
2. Inspector에서 다음 설정:

```yaml
Rendering:
  Depth Texture: Off (2D에서 불필요)
  Opaque Texture: Off
  Opaque Downsampling: None

Quality:
  HDR: Off (모바일 최적화)
  Anti Aliasing (MSAA): 2x
  Render Scale: 1.0

Lighting:
  Main Light: Off (2D 쿼터뷰에서 불필요)
  Additional Lights: Off

Shadows:
  Max Distance: 0
  Cascade Count: 0
  Depth Bias: 0
  Normal Bias: 0

Post-processing:
  Enabled: On (UI 이펙트용)
```

---

### 4. Time Settings

`Edit → Project Settings → Time`

```yaml
Fixed Timestep: 0.02 (50fps 물리 연산)
Maximum Allowed Timestep: 0.1
Time Scale: 1.0
```

---

### 5. Physics 2D Settings

`Edit → Project Settings → Physics 2D`

```yaml
Gravity: X=0, Y=-9.81
Default Material: None
Velocity Iterations: 8
Position Iterations: 3
Velocity Threshold: 1
Max Linear Correction: 0.2
Max Angular Correction: 8
Max Translation Speed: 100
Baumgarte Scale: 0.2
Baumgarte Time of Impact Scale: 0.75
Time to Sleep: 0.5
Linear Sleep Tolerance: 0.01
Angular Sleep Tolerance: 2.0

Layer Collision Matrix:
  - Player vs Enemy
  - Player vs Obstacles
  - Enemy vs Obstacles
```

---

### 6. Audio Settings

`Edit → Project Settings → Audio`

```yaml
DSP Buffer Size: Best Latency
Virtual Voice Count: 512
Real Voice Count: 32
```

---

## 📦 필수 패키지 설치

### Package Manager

`Window → Package Manager`에서 다음 패키지 설치:

#### Unity Registry

✅ **이미 설치됨** (2D URP 템플릿):
- 2D Sprite
- 2D Tilemap Editor
- Universal RP
- Cinemachine (카메라 제어용)

✅ **추가 설치 필요**:
- TextMeshPro (UI 텍스트용)
  - `Window → TextMeshPro → Import TMP Essential Resources`

---

### 외부 패키지 (NuGet for Unity 또는 수동)

#### 1. Newtonsoft.Json (JSON 처리)
- **방법 1**: Asset Store에서 "JSON .NET For Unity" 검색 후 설치
- **방법 2**: NuGet for Unity 사용
- **방법 3**: DLL 수동 추가 (Plugins 폴더)

#### 2. SignalR Unity Client (WebSocket)
**아직 설치 불필요** - Phase 2에서 추가 예정
- GitHub: https://github.com/Unity-Technologies/com.unity.transport
- 또는 커스텀 WebSocket 구현

---

## 🗂️ 프로젝트 구조

```
CastleLordTycoon/
├── Assets/
│   ├── Scenes/
│   │   ├── MainMenu.unity (메인 메뉴)
│   │   ├── GameWorld.unity (게임 월드)
│   │   └── Combat.unity (전투 씬)
│   ├── Scripts/
│   │   ├── Core/ (핵심 매니저)
│   │   ├── Network/ (API 통신)
│   │   ├── UI/ (UI 컨트롤러)
│   │   ├── Combat/ (전투 시스템)
│   │   ├── Character/ (영웅/캐릭터)
│   │   ├── World/ (월드 탐험)
│   │   └── Data/ (데이터 모델)
│   ├── Sprites/
│   │   ├── Characters/ (영웅 스프라이트)
│   │   ├── Monsters/ (몬스터)
│   │   ├── UI/ (UI 아이콘)
│   │   └── Tiles/ (타일맵)
│   ├── Prefabs/ (프리팹)
│   ├── Resources/ (동적 로딩용)
│   └── Settings/ (URP 설정)
├── Packages/ (패키지 매니페스트)
├── ProjectSettings/ (프로젝트 설정)
└── SETUP.md (이 파일)
```

---

## 🎮 빌드 설정

### Development Build

`File → Build Settings`

1. **Platform**: Android 선택 → Switch Platform
2. **Scenes In Build**:
   - MainMenu
   - GameWorld
   - Combat

3. **Build Settings**:
   ```
   Texture Compression: ASTC
   ETC2 fallback: 32-bit
   ```

4. **Development Build 체크박스**:
   - ✅ Development Build
   - ✅ Script Debugging
   - ✅ Autoconnect Profiler

---

## 🔧 IDE 설정 (Visual Studio Code)

### 권장 확장 프로그램

```
- C# (Microsoft)
- C# Dev Kit
- Unity Code Snippets
- Debugger for Unity
```

### Unity Preferences

`Edit → Preferences → External Tools`

```
External Script Editor: Visual Studio Code
```

---

## 🚀 다음 단계

### Phase 1: 네트워크 연결 테스트

1. **서버 실행**:
   ```bash
   cd CastleLordTycoon.Server
   dotnet run
   ```

2. **Unity 에디터 실행**:
   - Play 버튼 → 네트워크 연결 확인
   - Console에서 HTTP 요청 로그 확인

3. **API 테스트**:
   - 로그인 → 토큰 발급 확인
   - 플레이어 정보 조회

### Phase 2: MVP 구현

1. ✅ MainMenu 씬 생성 (로그인 UI)
2. ✅ GameWorld 씬 생성 (필드 이동)
3. ✅ Combat 씬 생성 (3×3 전투)
4. ✅ 기본 영웅 4종 구현
5. ✅ 몬스터 6종 구현

---

## 📚 참고 문서

- **기획 문서**: `docs/00-overview/game-concept.md`
- **아키텍처**: `docs/04-technical/architecture.md`
- **API 명세**: `docs/04-technical/client-server-contract.md`
- **데이터 구조**: `docs/04-technical/data-structures.md`

---

**최종 수정**: 2025-10-19
**상태**: 🟢 초기 설정 완료
