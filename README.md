# ModuleAndPlugin

Unreal Engine 5 C++ 프로젝트에서 **새 C++ 모듈을 직접 등록하고**, 메인 게임 모듈과 상호작용시키며, 별도의 **Runtime 플러그인**을 구성하는 과제입니다.

## 개발 환경

- Unreal Engine 5.7
- C++ Third Person Template
- Target Platform: Desktop
- Quality Preset: Scalable
- 프로젝트 이름: `ModuleAndPlugin`

## 학습 목표

- Unreal Engine의 모듈 및 플러그인 구조 이해
- `Build.cs`, `Target.cs`, `.uproject`, `.uplugin`의 역할 이해
- 서로 다른 모듈 사이의 의존성 설정
- 게임 모듈에서 별도 모듈의 C++ 클래스 사용
- 재사용 가능한 Runtime 플러그인 구성
- `UObject` 기반 데이터 클래스 작성 및 사용

## 프로젝트 구조

```text
ModuleAndPlugin/
├─ ModuleAndPlugin.uproject
├─ Source/
│  ├─ ModuleAndPlugin/                 # Primary Game Module
│  ├─ Test/                            # 직접 추가한 Runtime Module
│  │  ├─ Test.Build.cs
│  │  ├─ Test.h
│  │  ├─ Test.cpp
│  │  ├─ TestActor.h
│  │  └─ TestActor.cpp
│  ├─ ModuleAndPlugin.Target.cs
│  └─ ModuleAndPluginEditor.Target.cs
└─ Plugins/
   └─ Temporary/                       # 직접 구성한 Runtime Plugin
      ├─ Content/
      ├─ Temporary.uplugin
      └─ Source/Temporary/
         ├─ Temporary.Build.cs
         ├─ Temporary.h
         └─ Temporary.cpp
```

## 1. C++ 프로젝트 생성

Unreal Project Browser에서 다음 설정으로 프로젝트를 생성했습니다.

- Games → Third Person
- Implementation: C++
- Target Platform: Desktop
- Quality Preset: Scalable
- Project Name: `ModuleAndPlugin`

Blueprint 프로젝트도 Unreal Engine의 기본 모듈을 사용하지만, C++ 프로젝트에서는 개발자가 직접 C++ 모듈을 추가하고 다른 모듈과의 의존성을 설정할 수 있습니다.

## 2. `Test` 모듈 생성

`Source/Test` 경로에 새 Runtime 모듈을 만들고 다음 필수 파일을 추가했습니다.

- `Test.Build.cs`: 모듈의 빌드 규칙과 의존성 정의
- `Test.h`: 모듈 공용 헤더
- `Test.cpp`: 모듈 구현 및 등록

### 모듈 의존성

`Test.Build.cs`에는 기본적인 Unreal C++ 클래스와 Actor를 사용할 수 있도록 다음 모듈을 등록했습니다.

```csharp
PublicDependencyModuleNames.AddRange(new string[]
{
    "Core",
    "CoreUObject",
    "Engine",
    "InputCore"
});
```

- **Public Dependency**: 해당 모듈의 헤더와 소스에서 모두 사용하는 의존성
- **Private Dependency**: 구현 소스에서만 사용하는 의존성

`Test.cpp`에서는 다음 매크로로 일반 모듈을 등록했습니다.

```cpp
IMPLEMENT_MODULE(FDefaultModuleImpl, Test);
```

두 번째 인자인 `Test`는 `Build.cs`, `.uproject`, `Target.cs`에서 사용하는 모듈 이름과 정확히 일치해야 합니다.

## 3. 모듈을 빌드 대상에 등록

새로 만든 모듈이 실제 컴파일 및 로딩 대상에 포함되도록 다음 파일을 수정했습니다.

### Target 설정

게임과 에디터 빌드에서 `Test` 모듈을 사용할 수 있도록 두 Target 파일의 `ExtraModuleNames`에 등록했습니다.

```csharp
ExtraModuleNames.Add("ModuleAndPlugin");
ExtraModuleNames.Add("Test");
```

적용 대상:

- `Source/ModuleAndPlugin.Target.cs`
- `Source/ModuleAndPluginEditor.Target.cs`

### `.uproject` 설정

프로젝트의 `Modules` 배열에 `Test` 모듈을 추가했습니다.

```json
{
  "Name": "Test",
  "Type": "Runtime",
  "LoadingPhase": "PreDefault"
}
```

`PreDefault`로 설정하여 Primary Game Module의 기본 로딩 단계보다 먼저 로드되도록 구성했습니다. 설정 변경 후 `.uproject` 파일을 우클릭하여 Visual Studio 프로젝트 파일을 다시 생성하고 프로젝트를 빌드했습니다.

## 4. 모듈 간 상호작용

`Test` 모듈에 `AActor`를 상속받는 `ATestActor`를 작성했습니다. `BeginPlay()`에서 로그와 화면 메시지를 출력하여 모듈이 정상적으로 동작하는지 확인했습니다.

```cpp
void ATestActor::BeginPlay()
{
    Super::BeginPlay();

    UE_LOG(LogTemp, Warning, TEXT("TestActor BeginPlay"));

    if (GEngine)
    {
        GEngine->AddOnScreenDebugMessage(
            -1, 5.0f, FColor::Green, TEXT("TestActor Spawned!"));
    }
}
```

Primary Game Module에서 `Test` 모듈의 클래스를 사용하므로 `ModuleAndPlugin.Build.cs`에 다음 Private Dependency를 추가했습니다.

```csharp
PrivateDependencyModuleNames.AddRange(new string[]
{
    "Test"
});
```

이후 `AModuleAndPluginCharacter::BeginPlay()`에서 `ATestActor`를 생성했습니다.

```cpp
#include "Test/TestActor.h"

void AModuleAndPluginCharacter::BeginPlay()
{
    Super::BeginPlay();
    GetWorld()->SpawnActor<ATestActor>();
}
```

플레이 시 화면에 `TestActor Spawned!`가 표시되고 Output Log에 `TestActor BeginPlay`가 출력되는 것으로 모듈 연결을 검증할 수 있습니다.

## 5. `Temporary` 플러그인 구성

`Plugins/Temporary` 경로에 프로젝트 전용 Runtime 플러그인을 수동으로 구성했습니다.

### 플러그인 디스크립터

`Temporary.uplugin`에는 플러그인의 메타데이터와 포함 모듈을 JSON 형식으로 정의했습니다.

```json
{
  "FileVersion": 3,
  "Version": 1,
  "VersionName": "1.0",
  "FriendlyName": "Temporary",
  "Description": "Reusable temporary plugin.",
  "Category": "Project",
  "CanContainContent": true,
  "Modules": [
    {
      "Name": "Temporary",
      "Type": "Runtime",
      "LoadingPhase": "PreDefault"
    }
  ]
}
```

### 플러그인 모듈

`Temporary.Build.cs`에서 `Core`, `CoreUObject`, `Engine`, `InputCore` 의존성을 설정했습니다. 모듈 클래스는 `IModuleInterface`를 상속하고 시작 및 종료 시점을 구현했습니다.

```cpp
class FTemporaryModule : public IModuleInterface
{
public:
    virtual void StartupModule() override;
    virtual void ShutdownModule() override;
};
```

```cpp
void FTemporaryModule::StartupModule()
{
    UE_LOG(LogTemp, Warning, TEXT("Temporary plugin module started."));
}

void FTemporaryModule::ShutdownModule()
{
    UE_LOG(LogTemp, Warning, TEXT("Temporary plugin module shut down."));
}

IMPLEMENT_MODULE(FTemporaryModule, Temporary);
```

이 구조는 Primary Game Module과 분리되어 있으므로 다른 Unreal 프로젝트의 `Plugins` 폴더로 옮겨 재사용할 수 있습니다.

## 6. 플러그인 활성화 및 검증

`ModuleAndPlugin.uproject`의 `Plugins` 배열에 `Temporary` 플러그인을 활성화했습니다.

```json
{
  "Name": "Temporary",
  "Enabled": true
}
```

프로젝트 빌드 후 다음 항목을 확인합니다.

1. Unreal Editor가 오류 없이 실행되는지 확인
2. Plugins 창에서 `Temporary`가 활성화되어 있는지 확인
3. Output Log에서 `Temporary plugin module started.` 확인
4. C++ Classes 또는 IDE에서 플러그인의 C++ 모듈 확인

## 7. 도전 구현: `UObject` 기반 캐릭터 데이터

플러그인이 실제 게임 코드에 데이터를 제공하도록 `Temporary` 플러그인에 `UCharacterData` 클래스를 추가할 수 있습니다.

```text
Plugins/Temporary/Source/Temporary/
├─ Public/CharacterData.h
└─ Private/CharacterData.cpp
```

다른 모듈에서 사용할 헤더는 `Public` 폴더에 배치합니다.

### `CharacterData.h`

```cpp
#pragma once

#include "CoreMinimal.h"
#include "UObject/Object.h"
#include "CharacterData.generated.h"

UCLASS(BlueprintType)
class TEMPORARY_API UCharacterData : public UObject
{
    GENERATED_BODY()

public:
    UCharacterData();

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Character Data")
    FString CharacterName;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Character Data")
    float MaxHealth;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Character Data")
    float MoveSpeed;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Character Data")
    int32 Age;
};
```

`TEMPORARY_API`는 클래스를 플러그인 모듈 외부에서도 사용할 수 있도록 내보내는 매크로입니다.

### `CharacterData.cpp`

```cpp
#include "CharacterData.h"

UCharacterData::UCharacterData()
{
    CharacterName = TEXT("Mart Raider");
    MaxHealth = 100.0f;
    MoveSpeed = 500.0f;
    Age = 20;
}
```

`UObject`는 위치, 충돌, 렌더링이 필요 없는 데이터 객체에 적합합니다. 캐릭터의 기본 설정값만 담으므로 불필요한 `AActor`를 생성할 필요가 없습니다.

### Primary Game Module에서 플러그인 참조

`ModuleAndPlugin.Build.cs`에 플러그인 모듈 의존성을 추가합니다.

```csharp
PrivateDependencyModuleNames.AddRange(new string[]
{
    "Test",
    "Temporary"
});
```

캐릭터 헤더에서는 전방 선언 후 `UPROPERTY`로 객체를 보관합니다.

```cpp
class UCharacterData;

UPROPERTY(VisibleInstanceOnly, Category = "Character Data")
TObjectPtr<UCharacterData> CharacterData;
```

`UPROPERTY`로 참조를 보관해야 Unreal Engine의 가비지 컬렉터가 사용 중인 데이터 객체를 추적할 수 있습니다.

캐릭터의 `BeginPlay()`에서 데이터를 생성하고 값을 출력합니다.

```cpp
#include "CharacterData.h"
#include "Engine/Engine.h"

void AModuleAndPluginCharacter::BeginPlay()
{
    Super::BeginPlay();

    CharacterData = NewObject<UCharacterData>(this);

    if (IsValid(CharacterData) && GEngine)
    {
        const FString Message = FString::Printf(
            TEXT("Name: %s\nHealth: %.1f\nMove Speed: %.1f\nAge: %d"),
            *CharacterData->CharacterName,
            CharacterData->MaxHealth,
            CharacterData->MoveSpeed,
            CharacterData->Age
        );

        GEngine->AddOnScreenDebugMessage(
            -1, 10.0f, FColor::Green, Message);
    }
}
```

`NewObject<UCharacterData>(this)`는 캐릭터를 데이터 객체의 `Outer`로 지정합니다. 화면 출력은 과제 결과 확인을 위한 디버그 용도입니다.

## 실행 결과

정상 실행 시 다음 결과를 확인할 수 있습니다.

- `TestActor Spawned!` 화면 출력
- `TestActor BeginPlay` 로그 출력
- `Temporary plugin module started.` 로그 출력
- 도전 구현 적용 시 캐릭터 이름, 체력, 이동 속도 및 나이 출력


## 트러블슈팅

### 모듈을 찾을 수 없는 경우

- `Build.cs`, `Target.cs`, `.uproject`의 모듈 이름 철자를 확인합니다.
- 프로젝트 파일을 다시 생성한 후 전체 빌드합니다.
- Primary Game Module의 `Build.cs`에 사용하는 모듈이 Dependency로 등록되어 있는지 확인합니다.

### `CharacterData.h`를 찾지 못하는 경우

- 헤더가 플러그인 모듈의 `Public` 폴더에 있는지 확인합니다.
- `ModuleAndPlugin.Build.cs`에 `Temporary`가 추가되어 있는지 확인합니다.
- `.uproject`에서 `Temporary` 플러그인이 활성화되어 있는지 확인합니다.

### `generated.h` 관련 오류가 발생하는 경우

- `CharacterData.generated.h`가 헤더의 마지막 `#include`인지 확인합니다.
- 에디터를 종료하고 프로젝트 파일 생성 및 전체 빌드를 다시 수행합니다.

## 정리

이번 과제를 통해 Unreal Engine의 기능은 명시적인 의존성을 가진 여러 모듈과 플러그인으로 분리할 수 있다는 점을 확인했습니다. `Test` 모듈을 통해 모듈 등록 및 참조 과정을 실습했고, `Temporary` 플러그인을 통해 프로젝트와 분리된 재사용 단위를 구성했습니다. 또한 `UObject` 기반 데이터 클래스를 사용하면 Actor를 생성하지 않고도 캐릭터 설정값을 가볍게 관리할 수 있습니다.

과제 구현 과정은 티스토리 블로그에 작성해두었습니다.
https://think05994.tistory.com/94
