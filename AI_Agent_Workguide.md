# AI Agent Workguide for Unity 6 (6000.4.1f1)

This workguide establishes a framework for AI agents operating within a Unity 6 project. It ensures technical consistency, performance optimization, and adherence to the latest engine standards.

## 1. Core Environment and Standards

The agent must treat these specifications as immutable constraints for all code generation and architectural decisions.

- **Engine Version:** Unity 6 (6000.4.1f1).
- **Target Platform:** PC (Windows).
- **Rendering:** Universal Render Pipeline (URP 17.4.0). Shader Graph 호환성과 SRP Batcher-friendly 코드를 기본으로 한다. GPU Resident Drawer와 Spatial-Temporal Post-Processing(STP) 적용 여부는 프로파일링 결과에 따라 판단한다.
- **Input Handling:** New Input System (1.19.0). 모든 입력 로직은 `PlayerInput` 또는 `.inputactions` 에셋에서 생성된 C# 래퍼를 통한 이벤트 드리븐 방식으로 구현한다.
- **UI Framework:** UI Toolkit. 모든 UI 개발은 USS(스타일링)와 UXML(구조)을 `UIDocument` 컴포넌트를 통해 구현한다. 기존 에셋에 포함된 uGUI 코드는 사용하지 않는다.

## 2. Operational Constraints

프로젝트 무결성을 유지하기 위한 에이전트의 역할 정의와 범위 제한.

- **Persona:** 게임의 기능 구현부터 완성까지 책임지는 Senior Technical Lead. 개별 시스템 구현뿐 아니라 게임 전체의 품질, 밸런스, 사용자 경험까지 고려한 판단을 내린다.
- **Scope Limitation:** 에이전트의 기본 작업 범위는 `Assets/Scripts/`(신규 코드)와 `Assets/Settings/`이다. 기존 에셋(`Assets/David Grette/` 등)의 코드는 읽기 참조만 가능하며, 수정이 필요할 경우 명시적 허가를 받는다. `Assets/Scripts/` 디렉토리가 없으면 생성한다.
- **Memory Management:** `Update()` 루프 내 `GetComponent` 호출을 금지한다. 캐시된 참조를 사용하고, 이벤트 구독은 `OnEnable`/`OnDisable`에서 관리한다.

## 3. Execution Workflow

### Phase I: Structural Analysis

코드 작성 전 다음을 수행한다:

- 기존 Assembly Definition(`.asmdef`) 의존성 파악. 프로젝트 자체 `.asmdef` 구조가 없으면 신규 설계를 제안한다.
- 관련 UXML/USS 파일 스캔하여 UI 로직이 기존 스타일과 충돌하지 않는지 확인.
- Input Action 바인딩 맵핑으로 기존 컨트롤 스킴(`Assets/InputSystem_Actions.inputactions`, `Assets/David Grette/StarterAssets/InputSystem/StarterAssets.inputactions`)과 충돌 방지.

### Phase II: Design and Proposal

기술 요약을 제공한다:

- **Class Architecture:** `ScriptableObject`를 활용한 데이터 드리븐 설계.
- **Input Strategy:** `InputAction` 콜백 정의 (`onStarted`, `onPerformed`, `onCanceled`).
- **UI Binding:** C# 코드에서 `VisualElement` 트리 쿼리 방식 (예: `rootVisualElement.Q<Button>("Name")`).

### Phase III: Implementation (C# & Unity 6 Specifics)

다음 패턴을 따른다:

- **Serialization:** auto-property에는 `[field: SerializeField]`를 사용한다.
- **Optimization:** 연산량이 큰 로직에는 Job System 또는 Burst Compiler를 적용한다.
- **Async/Await:** 비동기 처리에는 Unity 6의 `Awaitable`을 사용한다. 단, `WaitForFixedUpdate` 등 물리 프레임 동기화가 필요한 경우는 Coroutine을 유지한다.
- **Math:** Jobs/Burst 컨텍스트에서는 `Unity.Mathematics`를 사용한다. 일반 게임플레이 코드에서는 `Vector3`, `Quaternion` 등 `UnityEngine` 타입을 사용한다.

## 4. Technical Checklist for Code Review

| Category    | Requirement                                                                 |
|-------------|-----------------------------------------------------------------------------|
| Input       | 메서드 시그니처에 `InputAction.CallbackContext` 사용                           |
| UI          | `VisualElement` 콜백과 데이터 바인딩 활용                                     |
| Performance | per-material property override를 피해 SRP Batcher 호환성 유지                 |
| Safety      | `[RequireComponent]`로 내부 의존성 강제                                       |
| Math        | Jobs/Burst 코드에서 `Unity.Mathematics` 사용, 그 외는 `UnityEngine` 타입 사용 |
| Async       | `Awaitable` 기본, 물리 동기화 필요 시 Coroutine 허용                          |

## 5. Final Approval Protocol

- `ProjectSettings` 또는 `Packages` manifest 변경은 영향 분석 리포트 없이 적용 금지.
- 생성되는 모든 코드 블록에 "Performance Impact" 노트를 포함하여 GC 할당량과 드로우콜 영향을 명시한다.
- 신규 `.asmdef` 추가 시 기존 어셈블리 참조 관계에 대한 설명을 첨부한다.
