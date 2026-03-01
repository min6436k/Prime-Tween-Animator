using System.Collections.Generic;
using System.Text.RegularExpressions;
using ODY.Editor.Helper;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace ODY.PrimeTweenAnimation.Editor
{
    [CustomEditor(typeof(PrimeTweenAnimator))]
    public class PrimeAnimatorEditor : UnityEditor.Editor
    {
        private static VisualTreeAsset _uxml;

        // Regular expression to find the target to delete
        private static readonly Regex ArrayElementRegex = new(@"(.+)\.Array\.data\[(\d+)\]");

        public override VisualElement CreateInspectorGUI()
        {
            _uxml ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UITKHelper.UxmlPath+"PrimeAnimatorEditor.uxml");
            
            var root = _uxml.CloneTree();

            var instance = (PrimeTweenAnimator)target;
            root.Q<Button>("PlayAllButton").clicked += () => instance.PlayAll();
            root.Q<Button>("PlayByStringButton").clicked += () => instance.PlayByID(instance.TestPlayIDHash);

            var animList = root.Q<ListView>("AnimList");
            animList.itemsAdded += OnAddedItem;

            root.RegisterCallback<KeyDownEvent>(evt =>
            {
                // Return if the key is not Delete or no Element is selected
                if (evt.keyCode != KeyCode.Delete ||
                    root.Q(className: "unity-collection-view__item--selected") is not { } selected) return;

                // Extract the array path from within the selected item
                foreach (var i in selected.Query<PropertyField>().Build())
                {
                    if (string.IsNullOrEmpty(i.bindingPath)) continue;

                    Match match = ArrayElementRegex.Match(i.bindingPath);
                    if (!match.Success) continue;

                    var arrayProp = serializedObject.FindProperty(match.Groups[1].Value);
                    int index = int.Parse(match.Groups[2].Value);

                    if (arrayProp is { isArray: true } && index < arrayProp.arraySize)
                    {
                        arrayProp.DeleteArrayElementAtIndex(index);
                        serializedObject.ApplyModifiedProperties();
                        evt.StopPropagation();
                    }

                    return;
                }
            }, TrickleDown.TrickleDown);

            return root;
        }

        /// <summary>
        /// Function to prevent reference copying of the last value when adding items to the list.
        /// </summary>
        private void OnAddedItem(IEnumerable<int> arg)
        {
            Undo.RecordObject(target, "Add Anim Item");
            foreach (var i in arg)
            {
                ((PrimeTweenAnimator)target).animList[i] = new PrimeAnimContainer();
            }
            EditorUtility.SetDirty(target);
        }
    }
}