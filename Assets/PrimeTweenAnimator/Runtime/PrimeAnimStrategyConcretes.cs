using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ODY.PrimeTweenAnimation
{
    public partial class PrimeAnimContainer
    {
        #region Base

        public interface IPrimeAnimStrategy
        {
            Tween Play();

            bool RefreshTarget(out Object castedTarget)
            {
                castedTarget = null;
                return false;
            }
        }

        [Serializable, PrimeAnimStrategy(TweenType.None)]
        public class EmptyAnimStrategy : IPrimeAnimStrategy
        {
            public Tween Play()
            {
                Debug.LogWarning("Null 트윈이 실행되었습니다.");
                return default;
            }
        }

        [Serializable]
        public abstract class DirectionAnimBase<T> : IPrimeAnimStrategy where T : struct
        {
            [SerializeField] protected TweenSettings<T> tweenSettings;

            [SerializeField] protected bool reverse;
            [SerializeField] protected bool startFromCurrent;

            protected DirectionAnimBase()
            {
                tweenSettings.settings.cycles = 1;
                tweenSettings.settings.duration = 1;
            }

            protected TweenSettings<T> ApplyDirection
            {
                get
                {
                    TweenSettings<T> result = tweenSettings;
                    result.startFromCurrent = startFromCurrent;

                    if (!reverse) return result;

                    (result.startValue, result.endValue) = (result.endValue, result.startValue);
                    return result;
                }
            }

            protected T EndValue => reverse ? tweenSettings.startValue : tweenSettings.endValue;
            protected T StartValue => reverse ? tweenSettings.endValue : tweenSettings.startValue;
            public abstract Tween Play();

            public virtual bool RefreshTarget(out Object castedTarget)
            {
                castedTarget = null;
                return false;
            }
        }

        [Serializable]
        public abstract class AdditiveAnimBase<T> : DirectionAnimBase<T> where T : struct
        {
            #if PRIME_TWEEN_EXPERIMENTAL
            [SerializeField] protected bool additive;
            #endif
        }

        [Serializable]
        public abstract class TransformAnimBase : AdditiveAnimBase<Vector3>
        {
            [SerializeField] protected Transform target;
            [SerializeField] protected bool self = true;
        }

        [Serializable]
        public abstract class ShakeAnimBase : IPrimeAnimStrategy
        {
            [SerializeField] protected ShakeSettings shakeSettings;
            [SerializeField] protected Transform target;
            [SerializeField] protected bool self = true;

            protected ShakeAnimBase()
            {
                shakeSettings.cycles = 1;
                shakeSettings.duration = 1;
            }

            public abstract Tween Play();
        }

        [Serializable]
        public abstract class ColorBase<T> : DirectionAnimBase<T> where T : struct
        {
            protected readonly HashSet<Type> AllowTypes = new();
            [SerializeField] protected Object target;
            [SerializeField] protected bool self = true;

            public override bool RefreshTarget(out Object castedTarget)
            {
                Type type = null;
                castedTarget = null;

                if (target == null || AllowTypes.Any(i => i.IsAssignableFrom(type ??= target.GetType()))) return false;

                if (target is GameObject go)
                {
                    //컴포넌트이고, 허용되는 타입 중에서
                    foreach (var i in AllowTypes.Where(i => typeof(Component).IsAssignableFrom(i)))
                    {
                        //target에서 GetComponent 한 결과가 null이 아닐 때 Refresh
                        if ((castedTarget = go.GetComponent(i)) != null) return true;
                    }
                }

                var log = new StringBuilder();
                log.AppendFormat("[PrimeAnimator] 해당 Tween에 사용 가능한 사용 가능한 컴포넌트를 찾지 못했습니다. 다음 타입을 사용해주세요.↴\n" +
                                 "---------------------------\n");
                log.AppendJoin(", ", AllowTypes.Select(t => t.Name));
                log.Append("\n---------------------------");
                Debug.LogError(log.ToString());
                return true;
            }

            protected Material RendererToMaterial(Renderer ren) =>
                Application.isPlaying ? ren.material : ren.sharedMaterial;
        }

        [Serializable]
        public abstract class CustomBase<T> : DirectionAnimBase<T> where T : struct
        {
            //TODO CustomAdditive 대응?
            [SerializeField] private UnityEvent<T> _callBack;

            protected void OnValueChange(T value) => _callBack.Invoke(value);
        }

        #endregion

        #region Transform

        [Serializable, PrimeAnimStrategy(TweenType.Pos)]
        public sealed class Position : TransformAnimBase
        {
            public override Tween Play()
            {
                #if PRIME_TWEEN_EXPERIMENTAL
                if (additive) return Tween.PositionAdditive(target, EndValue, ApplyDirection.settings);
                #endif
                return Tween.Position(target, ApplyDirection);
            }
        }

        [Serializable, PrimeAnimStrategy(TweenType.Rotate)]
        public sealed class Rotation : TransformAnimBase
        {
            public override Tween Play()
            {
                #if PRIME_TWEEN_EXPERIMENTAL
                if (additive) return Tween.RotationAdditive(target, EndValue, ApplyDirection.settings);
                #endif
                return Tween.Rotation(target, ApplyDirection);
            }
        }

        [Serializable, PrimeAnimStrategy(TweenType.Scale)]
        public sealed class Scale : TransformAnimBase
        {
            public override Tween Play()
            {
                #if PRIME_TWEEN_EXPERIMENTAL
                if (additive) return Tween.ScaleAdditive(target, EndValue, ApplyDirection.settings);
                #endif
                return Tween.Scale(target, ApplyDirection);
            }
        }

        #endregion

        #region LocalTransform

        [Serializable, PrimeAnimStrategy(TweenType.LocalPos)]
        public sealed class LocalPosition : TransformAnimBase
        {
            public override Tween Play()
            {
                #if PRIME_TWEEN_EXPERIMENTAL
                if (additive) return Tween.LocalPositionAdditive(target, EndValue, ApplyDirection.settings);
                #endif
                return Tween.LocalPosition(target, ApplyDirection);
            }
        }

        [Serializable, PrimeAnimStrategy(TweenType.LocalRotate)]
        public sealed class LocalRotation : TransformAnimBase
        {
            public override Tween Play()
            {
                #if PRIME_TWEEN_EXPERIMENTAL
                if (additive) return Tween.LocalRotationAdditive(target, EndValue, ApplyDirection.settings);
                #endif
                return Tween.LocalRotation(target, ApplyDirection);
            }
        }


        #endregion

        #region Punch

        [Serializable, PrimeAnimStrategy(TweenType.PunchPosition)]
        public sealed class PunchPosition : ShakeAnimBase
        {
            public override Tween Play()
                => Tween.PunchLocalPosition(target, shakeSettings);
        }

        [Serializable, PrimeAnimStrategy(TweenType.PunchRotation)]
        public sealed class PunchRotation : ShakeAnimBase
        {
            public override Tween Play()
                => Tween.PunchLocalRotation(target, shakeSettings);
        }

        [Serializable, PrimeAnimStrategy(TweenType.PunchScale)]
        public sealed class PunchScale : ShakeAnimBase
        {
            public override Tween Play()
                => Tween.PunchScale(target, shakeSettings);
        }

        #endregion

        #region Shake

        [Serializable, PrimeAnimStrategy(TweenType.ShakePosition)]
        public sealed class ShakePosition : ShakeAnimBase
        {
            public override Tween Play()
                => Tween.ShakeLocalPosition(target, shakeSettings);
        }

        [Serializable, PrimeAnimStrategy(TweenType.ShakeRotation)]
        public sealed class ShakeRotation : ShakeAnimBase
        {
            public override Tween Play()
                => Tween.ShakeLocalRotation(target, shakeSettings);
        }

        [Serializable, PrimeAnimStrategy(TweenType.ShakeScale)]
        public sealed class ShakeScale : ShakeAnimBase
        {
            public override Tween Play()
                => Tween.ShakeScale(target, shakeSettings);
        }

        #endregion

        #region Image/Sprite

        [Serializable, PrimeAnimStrategy(TweenType.Fade)]
        public sealed class SpriteFade : ColorBase<float>
        {
            public SpriteFade()
            {
                AllowTypes.Add(typeof(SpriteRenderer));
                AllowTypes.Add(typeof(Graphic));
                AllowTypes.Add(typeof(Material));
                AllowTypes.Add(typeof(Renderer));
                AllowTypes.Add(typeof(CanvasGroup));
            }

            public override Tween Play()
            {
                return target switch
                {
                    SpriteRenderer sr => Tween.Alpha(sr, ApplyDirection),
                    Graphic img => Tween.Alpha(img, ApplyDirection),
                    Material mat => Tween.MaterialAlpha(mat, ApplyDirection),
                    Renderer ren => Tween.MaterialAlpha(RendererToMaterial(ren), ApplyDirection),
                    CanvasGroup cng => Tween.Alpha(cng, ApplyDirection),
                    _ => default
                };
            }
        }

        [Serializable, PrimeAnimStrategy(TweenType.Color)]
        public sealed class ColorAnim : ColorBase<Color>
        {
            public ColorAnim()
            {
                AllowTypes.Add(typeof(SpriteRenderer));
                AllowTypes.Add(typeof(Graphic));
                AllowTypes.Add(typeof(Material));
                AllowTypes.Add(typeof(Renderer));
                AllowTypes.Add(typeof(Light));
                AllowTypes.Add(typeof(Camera));
            }

            public override Tween Play()
            {
                return target switch
                {
                    SpriteRenderer sr => Tween.Color(sr, ApplyDirection),
                    Graphic img => Tween.Color(img, ApplyDirection),
                    Material mat => Tween.MaterialColor(mat, ApplyDirection),
                    Renderer ren => Tween.MaterialColor(RendererToMaterial(ren), ApplyDirection),
                    Light light => Tween.LightColor(light, ApplyDirection),
                    Camera cam => Tween.CameraBackgroundColor(cam, ApplyDirection),
                    _ => default
                };
            }
        }

        #endregion

        #region Custom

        [Serializable, PrimeAnimStrategy(TweenType.CustomFloat)]
        public sealed class CustomFloat : CustomBase<float>
        {
            public override Tween Play()
            {
                return Tween.Custom(ApplyDirection, OnValueChange);
            }
        }

        [Serializable, PrimeAnimStrategy(TweenType.CustomVector2)]
        public sealed class CustomVector2 : CustomBase<Vector2>
        {
            public override Tween Play() => Tween.Custom(ApplyDirection, OnValueChange);
        }

        [Serializable, PrimeAnimStrategy(TweenType.CustomVector3)]
        public sealed class CustomVector3 : CustomBase<Vector3>
        {
            public override Tween Play() => Tween.Custom(ApplyDirection, OnValueChange);
        }

        [Serializable, PrimeAnimStrategy(TweenType.CustomQuaternion)]
        public sealed class CustomQuaternion : CustomBase<Quaternion>
        {
            public override Tween Play() => Tween.Custom(ApplyDirection, OnValueChange);
        }

        [Serializable, PrimeAnimStrategy(TweenType.CustomColor)]
        public sealed class CustomColor : CustomBase<Color>
        {
            public override Tween Play() => Tween.Custom(ApplyDirection, OnValueChange);
        }

        [Serializable, PrimeAnimStrategy(TweenType.CustomRect)]
        public sealed class CustomRect : CustomBase<Rect>
        {
            public override Tween Play() => Tween.Custom(ApplyDirection, OnValueChange);
        }

        #endregion

        #region Other

        [Serializable, PrimeAnimStrategy(TweenType.Delay)]
        public sealed class Delay : IPrimeAnimStrategy
        {
            [SerializeField] private float _delay;
            [SerializeField] private bool _useUnscaledTime;
            [SerializeField] private UnityEvent _callBack;

            public Tween Play() => Tween.Delay(_delay, OnComplete, _useUnscaledTime);
            private void OnComplete() => _callBack.Invoke();
        }

        [Serializable, PrimeAnimStrategy(TweenType.ShakeCamera)]
        public sealed class ShakeCamera : IPrimeAnimStrategy
        {
            [SerializeField] private Camera _camera;
            [SerializeField] private float _strength;
            [SerializeField] private float _duration;
            [SerializeField] private float _frequency;
            [SerializeField] private float _startDelay;
            [SerializeField] private float _endDelay;

            //Tween 제어 불가
            public Tween Play()
            {
                Tween.ShakeCamera(_camera, _strength, _duration, _frequency, _startDelay, _endDelay);
                return default;
            }
        }

        #endregion
    }
}