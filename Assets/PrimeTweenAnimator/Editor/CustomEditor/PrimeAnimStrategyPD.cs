using PrimeTween;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ODY.Editor.Helper;
using static ODY.PrimeTweenAnimation.PrimeAnimContainer;

namespace ODY.PrimeTweenAnimation.Editor
{
    [CustomPropertyDrawer(typeof(IPrimeAnimStrategy))]
    public class PrimeAnimStrategyPD : PropertyDrawer
    {
        //For debugging purposes
        private const bool ShowRawField = false;

        private static VisualTreeAsset _tweenSettingsUXML;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _tweenSettingsUXML ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UITKHelper.UxmlPath+"TweenSettings.uxml");

            var root = new VisualElement();

            BuildGUI(property.Copy(), root);

            return root;
        }

        private static void BuildGUI(SerializedProperty property, VisualElement container)
        {
            var iter = property.Copy();
            var end = iter.GetEndProperty();

            // Skip the root property itself
            if (!iter.NextVisible(true) || SerializedProperty.EqualContents(iter, end)) return;

            // Foldout for Tweens that do not have Settings or for debugging
            var rawField = new Foldout { text = "RawField", name = "RawField", value = false };
            rawField.ChangeDisplay(ShowRawField);
            container.Add(rawField);

            bool openRawFieldFlag = true;
            do
            {
                if (ApplyTweenSettingsType(iter, container, out var settingsRoot))
                {
                    openRawFieldFlag = false;
                    InitDisplayState(settingsRoot);
                }
                else rawField.Add(new PropertyField(iter.Copy()));
            } while (iter.NextVisible(false) && !SerializedProperty.EqualContents(iter, end));

            // Open RawField if no element with Settings exists
            if (openRawFieldFlag)
            {
                rawField.value = true;
                rawField.ChangeDisplay(true);
                rawField.AddToClassList("ODY-FoldOut-NoHeader");
            }

            if(container.Q<VisualElement>(className: "ODY-HideableSettings") is {} hideableSettings)
                InitHideable(hideableSettings, property);

            rawField.BringToFront();
        }

        /// <summary>
        /// Distinguishes between TweenSettings and ShakeSettings according to the current Strategy, and returns whether the Strategy has Settings.
        /// </summary>
        /// <returns>Returns true if the strategy contains TweenSettings or ShakeSettings; otherwise, false.</returns>
        private static bool ApplyTweenSettingsType(SerializedProperty iter, VisualElement container,
            out VisualElement settingsRoot)
        {
            settingsRoot = null;
            bool isTween = iter.type.StartsWith(nameof(TweenSettings));
            bool isShake = !isTween && iter.type.Equals(nameof(ShakeSettings));

            if (!isTween && !isShake) return false;

            var root = _tweenSettingsUXML.CloneTree();

            var (show, remove) = isTween
                ? ("TweenSettingsRoot", "ShakeSettingsRoot")
                : ("ShakeSettingsRoot", "TweenSettingsRoot");

            settingsRoot = root.Q(show);
            settingsRoot.ChangeDisplay(true);
            root.Q(remove).RemoveFromHierarchy();

            container.Add(root);

            if (container.Q<Foldout>("RawField") is { } rawField)
                rawField.Add(new PropertyField(iter));

            return true;
        }

        /// <summary>
        /// Registers callbacks to control the View of mutually dependent fields or corrects the values of the fields.
        /// </summary>
        private static void InitDisplayState(VisualElement container)
        {
            //Cycles => CycleMode Display
            if (container.Q<IntegerField>("Cycles") is { } cyclesField &&
                container.Q<EnumField>("CycleMode") is { } cycleModeField)
            {
                cyclesField.RegisterValueChangedCallback(evt =>
                {
                    if (evt.newValue == 0) cyclesField.value = 1;
                });

                cyclesField.BindViewStyle(cycleModeField, val => val != 1);
            }

            //Ease => CustomEase Display
            if (container.Q<EnumField>("Ease") is { } easeField)
            {
                if (container.Q<CurveField>("CustomEase") is { } customEaseField)
                    easeField.BindViewStyle(customEaseField, x => x is Ease.Custom);

                if (container.name.StartsWith(nameof(ShakeSettings)) &&
                    container.Q<ToggleableButton>("EnableFalloff") is { } falloffField)
                    falloffField.BindViewStyle(easeField, action: UITKHelper.ChangeEnable);
            }

            //EaseBetweenShakes = !Ease.Custom
            if (container.Q<EnumField>("EaseBetweenShakes") is { } shakeEaseField)
            {
                shakeEaseField.RegisterValueChangedCallback(evt
                    =>
                {
                    if (evt.newValue is not Ease.Custom) return;
                    Debug.LogWarning(
                        "[PrimeAnimator] ShakeSettings-EaseBetweenShakes in PrimeTween does not support Custom Easing.");
                    shakeEaseField.value = Ease.Default;
                });
            }
        }

        /// <summary>
        /// Initializes the Display of fields visible only in specific Strategies.
        /// </summary>
        private static void InitHideable(VisualElement container, SerializedProperty property)
        {
            SetHideable(container);
            
            // This query exists in PrimeAnimContainerPD as well, but it's used twice due to execution order issues.
            if (container.Q<ToggleableButton>("Self") is { } selfButton &&
                container.Q<ObjectField>("Target") is { } targetField &&
                property.serializedObject.targetObject is Component component)
            {
                selfButton.BindViewStyle(targetField, x => !x, action: UITKHelper.ChangeEnable);
            }
            
            return;

            void SetHideable(VisualElement ve)
            {
                foreach (var i in ve.Children())
                {
                    if (i.ClassListContains("ODY-HideableSettings"))
                    {
                        SetHideable(i);
                        continue;
                    }

                    if (i is not IBindable bindable || string.IsNullOrEmpty(bindable.bindingPath)) continue;

                    var currentProp = property.FindPropertyRelative(bindable.bindingPath);
                    i.ChangeDisplay(currentProp is not null);
                }
            }
        }
    }
}