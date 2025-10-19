# Play Games Plugin for Unity 통합 가이드

## 📋 개요

Google Play Games Services를 Unity에 통합하여 Google 계정 로그인을 구현합니다.

**핵심 특징**:
- ✅ 완전 무료 (사용량 제한 없음)
- ✅ Google Play Games 자동 로그인
- ✅ 업적, 리더보드 기본 제공
- ✅ Android 전용

---

## 🎯 전체 흐름

```
[Unity Client]
    ↓
Play Games Plugin 초기화
    ↓
자동 로그인 시도 (Google Play Games)
    ↓
Server Auth Code 발급
    ↓
POST /api/auth/google-login
    ↓
[ASP.NET Core Server]
    ↓
Google API로 Server Auth Code 검증
    ↓
사용자 DB 조회/생성
    ↓
자체 JWT 발급
    ↓
[Unity Client]
    ↓
SecureStorage에 암호화 저장
    ↓
게임 플레이
```

---

## 📝 사전 준비사항 (완료됨)

### ✅ Google Cloud Console 설정
- Google Cloud 프로젝트 생성
- OAuth 동의 화면 구성
- Android OAuth Client ID 생성

### ✅ Keystore 생성
- SHA-1 지문: `E8:20:85:F9:C1:B5:74:05:A1:68:84:20:98:47:CD:00:6A:47:04:38`
- Package Name: `com.madalang.CastleLordTycoon`
- Client ID: `209932296601-sridvo9fbk8h4esk29f9sc02hsh3c2ep.apps.googleusercontent.com`

---

## 🔧 1단계: Play Games Plugin 다운로드 및 설치

### 1.1 Plugin 다운로드

**GitHub 릴리즈 페이지**:
```
https://github.com/playgameservices/play-games-plugin-for-unity/releases/tag/v2.1.0
```

**다운로드 파일**:
```
GooglePlayGamesPlugin-0.11.01.unitypackage
```

---

### 1.2 Unity에 Import

```
Unity Editor 열기
↓
Assets → Import Package → Custom Package...
↓
다운로드한 .unitypackage 선택
↓
모든 항목 선택 (Import All)
↓
Import 버튼 클릭
```

**Import 후 생성되는 폴더**:
```
Assets/GooglePlayGames/
Assets/Plugins/Android/
Assets/PlayServicesResolver/
```

---

## ⚙️ 2단계: Play Games 설정

### 2.1 Setup Android 실행

```
Window → Google Play Games → Setup → Android setup...
```

**설정 입력**:
```yaml
Client ID:
  209932296601-sridvo9fbk8h4esk29f9sc02hsh3c2ep.apps.googleusercontent.com

⚠️ 주의: Web Application Client ID가 아닌 Android Client ID 입력!
```

**Setup 버튼 클릭**

---

### 2.2 자동 생성되는 파일

```
Assets/GooglePlayGames/GameInfo.cs
Assets/Plugins/Android/GooglePlayGamesManifest.plugin/AndroidManifest.xml
```

---

## 📱 3단계: Unity Player Settings 확인

### 3.1 Package Name 확인

```
Edit → Project Settings → Player → Android
Other Settings → Identification
```

**확인사항**:
```yaml
Package Name: com.madalang.CastleLordTycoon
Minimum API Level: API Level 24 (Android 7.0)
Target API Level: Automatic (highest installed)
```

---

### 3.2 Keystore 설정

```
Publishing Settings 섹션
```

**설정**:
```yaml
✅ Custom Keystore

Keystore Path: keystore/castle-lord-tycoon.keystore
Keystore Password: CastleLord2025!
Alias: castle_key
Key Password: CastleLord2025!
```

---

## 💻 4단계: Unity 클라이언트 코드 작성

### 4.1 PlayGamesManager.cs 생성

**파일**: `Assets/Scripts/Network/PlayGamesManager.cs`

```csharp
using System;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace CastleLordTycoon.Network
{
    /// <summary>
    /// Google Play Games Services 관리
    /// </summary>
    public class PlayGamesManager : MonoBehaviour
    {
        private static PlayGamesManager _instance;
        public static PlayGamesManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("PlayGamesManager");
                    _instance = go.AddComponent<PlayGamesManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        // 이벤트
        public event Action<string> OnAuthSuccess;      // Server Auth Code
        public event Action<string> OnAuthFailed;

        public bool IsAuthenticated { get; private set; }

        private void Awake()
        {
            InitializePlayGames();
        }

        /// <summary>
        /// Play Games 초기화
        /// </summary>
        private void InitializePlayGames()
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                .RequestServerAuthCode(false)  // Server Auth Code 요청
                .RequestIdToken()              // ID Token 요청
                .Build();

            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();

            Debug.Log("Play Games 초기화 완료");
        }

        /// <summary>
        /// 로그인 (자동 또는 수동)
        /// </summary>
        public void Authenticate()
        {
            Debug.Log("Play Games 로그인 시작...");

            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    IsAuthenticated = true;
                    string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();

                    Debug.Log($"Play Games 로그인 성공!");
                    Debug.Log($"  사용자: {Social.localUser.userName}");
                    Debug.Log($"  ID: {Social.localUser.id}");
                    Debug.Log($"  ID Token 길이: {idToken?.Length ?? 0}");

                    if (!string.IsNullOrEmpty(idToken))
                    {
                        OnAuthSuccess?.Invoke(idToken);
                    }
                    else
                    {
                        Debug.LogError("ID Token을 받지 못했습니다.");
                        OnAuthFailed?.Invoke("ID Token을 받지 못했습니다.");
                    }
                }
                else
                {
                    IsAuthenticated = false;
                    Debug.LogWarning("Play Games 로그인 실패");
                    OnAuthFailed?.Invoke("사용자가 로그인을 취소하거나 실패했습니다.");
                }
            });
        }

        /// <summary>
        /// 로그아웃
        /// </summary>
        public void SignOut()
        {
            PlayGamesPlatform.Instance.SignOut();
            IsAuthenticated = false;
            Debug.Log("Play Games 로그아웃 완료");
        }
    }
}
```

---

### 4.2 AuthService 통합

기존 `AuthService.cs`의 `LoginWithGoogle` 메서드는 그대로 사용합니다.

**호출 예시**:
```csharp
// PlayGamesManager 이벤트 리스너
PlayGamesManager.Instance.OnAuthSuccess += (idToken) =>
{
    AuthService.Instance.LoginWithGoogle(idToken, (success, message) =>
    {
        if (success)
        {
            Debug.Log("서버 인증 성공!");
            // 게임 시작
        }
        else
        {
            Debug.LogError($"서버 인증 실패: {message}");
        }
    });
};

// 로그인 시작
PlayGamesManager.Instance.Authenticate();
```

---

## 🧪 5단계: 테스트

### 5.1 Editor에서 테스트 (제한적)

Play Games는 Editor에서 제한적으로 동작합니다.

**예상 결과**:
```
Play Games 초기화 완료
Play Games 로그인 실패
(Android 빌드 필요)
```

---

### 5.2 Android 빌드 및 테스트

#### 빌드 준비

```
File → Build Settings
Platform: Android
Switch Platform
```

**빌드 설정**:
```
✅ Development Build (테스트용)
Compression Method: LZ4
```

#### APK 빌드

```
Build 버튼 클릭
저장 위치 선택
빌드 완료 대기 (5-10분)
```

#### 실제 기기 테스트

1. **APK 설치**
   - Android 기기에 APK 전송 및 설치

2. **앱 실행**
   - 앱 실행 시 자동으로 Play Games 로그인 시도

3. **Google 계정 선택**
   - Google 계정 목록 표시
   - 계정 선택

4. **권한 동의**
   - Play Games 권한 요청
   - 허용 클릭

5. **서버 인증 확인**
   - 서버로 ID Token 전송
   - JWT 발급 확인

---

## 🔍 문제 해결

### 문제 1: "Developer Error"

**원인**: SHA-1 지문 또는 Package Name 불일치

**해결**:
```
1. Unity Player Settings에서 Package Name 확인
2. Build Settings → Player Settings → Publishing Settings
   → Use Custom Keystore 확인
3. Google Cloud Console에서 SHA-1 재확인
```

---

### 문제 2: "Sign-in failed"

**원인**: OAuth 동의 화면 설정 문제

**해결**:
```
Google Cloud Console
→ APIs & Services → OAuth consent screen
→ Test users에 본인 계정 추가
→ 또는 "PUBLISH APP" (배포 모드 전환)
```

---

### 문제 3: Plugin Import 실패

**원인**: Unity 버전 불일치

**해결**:
```
1. Unity 2022.3 LTS 사용 확인
2. Plugin 재다운로드
3. Clean Import:
   Assets/GooglePlayGames 폴더 삭제 후 재Import
```

---

### 문제 4: "ID Token is null"

**원인**: Play Games 설정에서 ID Token 요청 누락

**해결**:
```csharp
// PlayGamesManager.cs InitializePlayGames() 메서드 확인
PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
    .RequestIdToken()  // ← 이 줄 확인
    .Build();
```

---

## 📊 보안 고려사항

### 1. ID Token 검증 필수

```
✅ 서버에서 Google API로 반드시 검증
❌ 클라이언트가 보낸 토큰을 무조건 신뢰 금지
```

### 2. SecureStorage 사용

```
✅ RefreshToken을 SecureStorage에 암호화 저장
❌ PlayerPrefs에 평문 저장 금지
```

### 3. HTTPS 사용

```
개발: HTTP (localhost)
배포: HTTPS 필수
```

---

## 🎯 다음 단계

### 서버 구현

1. **Google.Apis.Auth NuGet 패키지 설치**
2. **GoogleAuthService.cs 작성**
3. **AuthController google-login 엔드포인트 구현**
4. **User 모델에 GoogleId 필드 추가**

### 통합 테스트

1. **서버 실행**
2. **Android 빌드**
3. **Play Games 로그인 테스트**
4. **서버 인증 확인**

---

## 📚 참고 자료

- **Play Games Plugin**: https://github.com/playgameservices/play-games-plugin-for-unity
- **Google Play Console**: https://play.google.com/console
- **Google Cloud Console**: https://console.cloud.google.com

---

**설정 날짜**: 2025-10-19
**Plugin 버전**: v2.1.0 (0.11.01)
**Package Name**: com.madalang.CastleLordTycoon
**Client ID**: 209932296601-sridvo9fbk8h4esk29f9sc02hsh3c2ep.apps.googleusercontent.com
