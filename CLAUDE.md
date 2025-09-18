# CLAUDE.md

이 파일은 Claude Code (claude.ai/code)가 이 저장소에서 코드 작업을 할 때 가이드를 제공합니다.

## 프로젝트 개요

이것은 2D 게임 개발에 중점을 둔 Unity 2D 프로젝트(Unity 6000.0.41f1)입니다. 프로젝트 이름은 "2d Claude MCP"이며 다음을 포함합니다:
- `com.coplaydev.unity-mcp` 패키지를 통한 Unity MCP (Model Context Protocol) 연동
- Universal Render Pipeline (URP)을 사용한 2D 게임 개발 기능
- 현대적인 입력 처리를 위한 Input System
- Undead Survivor 게임 에셋 (교육/데모 콘텐츠로 보임)

## Unity MCP 연동

이 프로젝트는 Claude Code가 MCP 도구를 통해 Unity와 직접 상호작용할 수 있게 하는 Unity MCP 서버 연동을 포함합니다. 이를 통해 다음이 가능합니다:
- 스크립트 관리 및 편집
- 에셋 조작
- 씬 관리
- GameObject 작업
- 실시간 Unity 에디터 제어

## 개발 환경

### Unity 구성
- **Unity 버전**: 6000.0.41f1
- **렌더 파이프라인**: Universal Render Pipeline (URP) 17.0.4
- **입력 시스템**: New Input System 1.13.1
- **대상 플랫폼**: 주로 2D 게임

### 주요 프로젝트 구조
- `Assets/Scripts/` - 현재 비어있음, C# 스크립트 준비됨
- `Assets/Scenes/` - SampleScene.unity 및 기타 씬 템플릿 포함
- `Assets/Sprites/` - 2D 스프라이트 에셋
- `Assets/Settings/` - URP 및 프로젝트 설정
- `Assets/Undead Survivor/` - 데모 게임 에셋 및 씬
- `Assets/InputSystem_Actions.inputactions` - 입력 액션 정의

### 패키지 의존성
- **Core 2D**: 2D 게임 개발을 위한 `com.unity.feature.2d`
- **Input System**: 현대적인 입력 처리를 위한 `com.unity.inputsystem`
- **URP**: 최적화된 렌더링을 위한 `com.unity.render-pipelines.universal`
- **Unity MCP**: Claude Code 연동을 위한 `com.coplaydev.unity-mcp`
- **Test Framework**: 단위 테스트를 위한 `com.unity.test-framework`

## MCP를 통한 Unity 작업

이 프로젝트는 MCP 도구를 통해 Unity와 작업하도록 설정되어 있습니다. 다음이 가능합니다:
- Unity에서 직접 C# 스크립트 생성 및 편집
- GameObject 및 컴포넌트 관리
- 씬 및 에셋 작업
- 디버깅을 위한 Unity 콘솔 액세스
- Unity 메뉴 명령 실행

## Input System 설정

프로젝트는 다음과 같은 미리 정의된 액션과 함께 Unity의 새로운 Input System을 사용합니다:
- 플레이어 이동 (Vector2)
- 룩/카메라 제어
- 표준 게임 입력 패턴

입력 액션은 `Assets/InputSystem_Actions.inputactions`에 구성되어 있습니다.

## 개발 참고사항

- 프로젝트는 현재 최소한의 커스텀 코드를 가지고 있어 빠른 프로토타이핑에 이상적입니다
- URP는 2D 최적화로 구성되어 있습니다
- 2D 기능 패키지를 통해 Pixel Perfect Camera 지원이 가능합니다
- Undead Survivor 데모는 2D 게임 구현 패턴의 예제를 제공합니다