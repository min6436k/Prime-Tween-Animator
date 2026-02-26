using System;
using System.Linq;
using ODY.Editor.Helper;
using PrimeTween;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static ODY.Editor.Helper.UITKHelper;

namespace ODY.PrimeTweenAnimation.Editor
{
    [CustomPropertyDrawer(typeof(PrimeAnimContainer))]
    public class PrimeAnimContainerPD : PropertyDrawer
    {
        private static VisualTreeAsset _uxml;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _uxml ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.ody.primetween-animator/Editor/UXML/PrimeAnimContainerPD.uxml");

            VisualElement root = _uxml.CloneTree();

            if (GetCurrentObject(property) is not PrimeAnimContainer instance)
            {
                Debug.LogError("Failed to find the PrimeAnimContainer instance.");
                return root;
            }

            var data = new CustomData
            {
                Instance = instance,
                Property = property.Copy(),

                RootFoldout = root.Q<Foldout>("RootFoldout"),
                TweenType = root.Q<EnumField>("TweenType"),
                PlayButton = root.Q<Button>("PlayButton"),
            };
            
            data.RootFoldout.RegisterValueChangedCallback(evt =>
            {
                if (evt.target != data.RootFoldout) return;
                OnFoldoutValueChanged(evt.newValue, data);
            });
            
            data.TweenType.RegisterValueChangedCallback(evt => OnChangedTweenType(evt, data));
            data.PlayButton.clicked += () => OnClickPlayButton(instance);

            var originInstance = (PrimeTweenAnimator)property.serializedObject.targetObject;
            root.Q<TextField>("PlayIDField").RegisterValueChangedCallback(_ => originInstance.SetCacheDirty());
            
            
            return root;
        }


        /// <summary>
        /// A data structure for caching to ensure operations are within the same PrimeAnimContainer instance.
        /// </summary>
        private class CustomData
        {
            public PrimeAnimContainer Instance;
            public SerializedProperty Property;

            public PropertyField AnimField;
            public Foldout RootFoldout;
            public EnumField TweenType;
            public Button PlayButton;
        }

        /// <summary>
        /// Calls the change function of the PrimeAnimContainer instance and updates the UI when the TweenType is changed.
        /// </summary>
        private static void OnChangedTweenType(ChangeEvent<Enum> evt, CustomData data)
        {
            var newValue = (TweenType)evt.newValue;
            data.RootFoldout.ChangeDisplay(newValue != TweenType.None);

            if (Equals(evt.newValue, evt.previousValue)) return;
            
            data.Instance.OnTweenTypeChanged(newValue); 
            OnFoldoutValueChanged(data.RootFoldout.value, data);
        }
        
        private static void OnFoldoutValueChanged(bool value, CustomData data)
        {
            if (value) RefreshAnimField(data);
            else if(data.AnimField != null)
            {
                data.AnimField.RemoveFromHierarchy();
                data.AnimField = null;
            }
        }

        /// <summary>
        /// Refresh the AnimField with the updated value.
        /// </summary>
        private static void RefreshAnimField(CustomData data)
        {
            data.AnimField?.RemoveFromHierarchy();

            const string bindingPath = nameof(PrimeAnimContainer.anim);
            
            var property = data.Property;
            property.serializedObject.Update();

            var animField = data.AnimField = new PropertyField {name = "AnimField"};
            data.RootFoldout.Add(data.AnimField);
            animField.BindProperty(property.FindPropertyRelative(bindingPath));
            property.serializedObject.ApplyModifiedProperties();

            animField.schedule.Execute(_ => BindAnimFieldChildren(data));
        }

        /// <summary>
        /// Handles the necessary bindings within the internal fields of the AnimField.
        /// </summary>
        private static void BindAnimFieldChildren(CustomData data)
        {
            if (data.AnimField.Q<ToggleableButton>("Self") is { } selfButton &&
                data.AnimField.Q<ObjectField>("Target") is { } targetField &&
                data.Property.serializedObject.targetObject is Component component)
            {
                targetField.RegisterValueChangedCallback(_ =>
                {
                    if (data.Instance.anim.RefreshTarget(out var target)) targetField.value = target;
                });
                
                selfButton.RegisterValueChangedCallback(evt =>
                {
                    if (Equals(evt.previousValue, evt.newValue)) return;
                    targetField.value = selfButton.value ? component.gameObject : null;
                });
                
                // I didn't want to do it this way either.
                if (!selfButton.value) return;
                targetField.value = null;
                targetField.value = component.gameObject;
            }
        }

        private static void OnClickPlayButton(PrimeAnimContainer instance) => instance.anim.Play();
    }
}