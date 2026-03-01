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
PrimeTween Animator is an extension project designed to make PrimeTween—an exceptional tweening asset—even more convenient to use within the Unity Inspector without modifying any code.

As this is in the early stages of development and I have limited experience with open-source distribution, there may be bugs.

Additionally, the original creator, **KyryloKuzyk**, is currently developing a Pro version of PrimeTween. Once it's released, I highly recommend checking it out!


<br>


## 📦 Installation

> [!IMPORTANT]
> This extension requires **PrimeTween**. Make sure to [install PrimeTween via UPM](https://github.com/KyryloKuzyk/PrimeTween?tab=readme-ov-file#install-via-unity-package-manager-upm) before proceeding.

You can install this package via the Unity Package Manager (UPM).

1. Open the `Package Manager` in the Unity Editor.
2. Click the `+` button in the top-left corner and select `Add package from git URL...`.
3. Enter the following URL and click `Add`:
   ```
   https://github.com/min6436k/Prime-Tween-Animator.git?path=Assets/PrimeTweenAnimator
   ```
   Note: Depending on your Unity version, the button may say `Install` instead of `Add`.


<br>


## ✨ Additive Feature (Optional)

To enable the `Additive` tween feature, you need to register a specific scripting symbol in your project.

1. Navigate to **Project Settings** > **Player** > **Other Settings**.
2. Add the following text to the **Scripting Define Symbols** field:
   ```
   PRIME_TWEEN_EXPERIMENTAL
   ```

3. Once added, the **Additive** button will be enabled for specific Transform tweens, allowing you to create animations that stack on top of current values!

> [!WARNING]
> Using Additive tweens may introduce very small floating-point errors. While negligible for thousands of iterations, it may have a noticeable impact if accumulated hundreds of thousands of times or more.
> For more details, please refer to this [Discussion](https://github.com/KyryloKuzyk/PrimeTween/discussions/55#discussioncomment-8844367).