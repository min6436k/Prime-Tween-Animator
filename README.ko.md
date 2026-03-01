<div align="center">

# 🌌 PrimeTween Animator

**Effortless PrimeTween management via the Unity Inspector.**

[![license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/min6436k/Prime-Tween-Animator?tab=MIT-1-ov-file)
[![Unity](https://img.shields.io/badge/Unity-2018.4%2B-111111?style=flat&logo=unity&logoColor=white)](https://unity.com/)
[![version](https://img.shields.io/github/package-json/v/min6436k/Prime-Tween-Animator?filename=Assets%2FPrimeTweenAnimator%2Fpackage.json&color=lightgray)](CHANGELOG.md)
[![dependency](https://img.shields.io/badge/dependency-PrimeTween-ab19cc.svg)](https://github.com/KyryloKuzyk/PrimeTween)

🌐 : [English](README.md) | [한국어](README.ko.md)
<br>
📜 : [Changelog](CHANGELOG.md)

</div>


<br>


## 🌿 About this Project
PrimeTween Animator는 뛰어난 Tweening 에셋인 PrimeTween을 코드 작성 없이 유니티 인스펙터에서 편하게 사용할 수 있도록 확장한 프로젝트입니다.

개발 초기 단계이며 저는 오픈소스 배포 경험이 적기 때문에 버그가 있을 수 있습니다.

또한 원작자인 **KyryloKuzyk**이 PrimeTween의 Pro 버전을 제작중에 있으니 만약 출시한다면 꼭 사용해보시기 바랍니다!


<br>


## 📦 Installation

> [!IMPORTANT]
> PrimeTween Animator는 PrimeTween을 기반으로 작동합니다. 먼저 [여기](https://github.com/KyryloKuzyk/PrimeTween?tab=readme-ov-file#install-via-unity-package-manager-upm)를 참고해 UPM으로 설치를 완료해 주세요.

이 패키지는 Unity Package Manager(UPM)를 통해 설치할 수 있습니다.

1. Unity 에디터에서 `Package Manager`를 엽니다.
2. 좌측 상단의 `+` 버튼을 클릭하고 `Add package from git URL...` 을 선택합니다.
3. 아래의 URL을 입력하고 `Add`를 누릅니다:
   ```
   https://github.com/min6436k/Prime-Tween-Animator.git?path=Assets/PrimeTweenAnimator
   ```
   ※ 유니티 버전에 따라 `Add` 대신 `Install`로 표시될 수 있습니다.


<br>


## ✨ Additive Feature (Optional)

`Additive` 트윈 기능을 활성화하려면 프로젝트에 전용 심볼을 등록해야 합니다.

1. **Project Settings** > **Player** > **Other Settings**로 이동합니다.
2. **Scripting Define Symbols** 항목에 아래 문구를 추가합니다:
   ```
   PRIME_TWEEN_EXPERIMENTAL
   ```
   
3. 이제 Transform의 일부 Tween에서 Additive버튼이 활성화되어 기존 값에 더해지는 애니메이션을 사용할 수 있습니다! 

> [!WARNING]
> Additive를 사용하게 되면 매우 작은 부동소숫점 오차가 생기게 됩니다. 수천 회 수준에서는 문제 없지만, 수십만 회 이상 누적된다면 어느정도 영향을 끼칠 수 있습니다.
> 자세한 내용은 이 [논의](https://github.com/KyryloKuzyk/PrimeTween/discussions/55#discussioncomment-8844367)
> 를 참고하세요.