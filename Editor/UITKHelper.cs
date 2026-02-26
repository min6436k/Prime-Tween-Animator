using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace ODY.Editor.Helper
{
    public static class UITKHelper
    {
        #region FindObjectFromPropertyPath

        /// <summary>
        /// Extracts the actual object instance via reflection based on <see cref="SerializedProperty.propertyPath"/>.
        /// </summary>
        public static object GetCurrentObject(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');

            foreach (var element in elements)
            {
                obj = GetTargetObject(obj, element);
                if (obj == null) return null;
            }

            return obj;
        }

        /// <summary>
        /// Gets the child object by parsing the member name or array indexer from the source.
        /// </summary>
        /// <param name="source">The target object to explore.</param>
        /// <param name="element">The path segment containing the member name or indexer (e.g., "fieldName", "listName[0]").</param>
        private static object GetTargetObject(object source, string element)
        {
            if (source == null) return null;

            int i = element.IndexOf('[');
            if (i >= 0)
            {
                var enumerable = GetMember(source, element[..i]) as IEnumerable;
                if (enumerable == null) return null;

                int index = int.Parse(element[(i + 1)..^1]);
                var enm = enumerable.GetEnumerator();

                using (enm as IDisposable)
                {
                    for (int j = 0; j <= index; j++)
                        if (!enm.MoveNext())
                            return null;
                    return enm.Current;
                }
            }

            return GetMember(source, element);
        }

        /// <summary>
        /// Gets the value of a field or property of an object using reflection. 
        /// </summary>
        /// <param name="source">The object that owns the member.</param>
        /// <param name="name">The name of the target to find.</param>
        private static object GetMember(object source, string name)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            var type = source.GetType();
            while (type != null)
            {
                if (type.GetField(name, flags) is { } f) return f.GetValue(source);
                if (type.GetProperty(name, flags | BindingFlags.IgnoreCase) is { } p) return p.GetValue(source);

                // Handling private members of inherited base classes
                type = type.BaseType;
            }

            return null;
        }

        #endregion

        #region ElementView

        /// <summary>
        /// Changes the display state of the target.
        /// </summary>
        /// <param name="target">The target to change.</param>
        /// <param name="value">The display state value.</param>
        public static void ChangeDisplay(this VisualElement target, bool value)
        {
            var display = value ? DisplayStyle.Flex : DisplayStyle.None;
            if (target.style.display == display) return;
            target.style.display = display;
        }

        /// <summary>
        /// Changes the enable state of the target.
        /// </summary>
        /// <param name="target">The target to change.</param>
        /// <param name="value">The enable state value.</param>
        public static void ChangeEnable(this VisualElement target, bool value)
        {
            if (target.enabledSelf == value) return;
            target.SetEnabled(value);
        }

        /// <summary>
        /// Subscribes to changes in a specific <see cref="INotifyValueChanged{T}"/> to change the ViewStyle of the target.
        /// </summary>
        /// <param name="valueElement">The element to subscribe to for changes.</param>
        /// <param name="target">The target to apply the action to.</param>
        /// <param name="predicate">A function that evaluates the value of valueElement and returns a bool.</param>
        /// <param name="action">The function that actually changes the state of the target. The default is <see cref="UITKHelper.ChangeDisplay"/>, and you can use <see cref="UITKHelper.ChangeEnable"/> or any other Action{VisualElement, bool}.</param>
        /// <typeparam name="T">The value type of the valueElement.</typeparam>
        /// <example>testButton.BindViewStyle(targetElement, x => !x, action: UITKHelper.ChangeEnable);</example>
        public static void BindViewStyle<T>(this INotifyValueChanged<T> valueElement, VisualElement target,
            Func<T, bool> predicate, Action<VisualElement, bool> action = null)
        {
            action ??= ChangeDisplay;
            
            valueElement.RegisterValueChangedCallback(evt => action(target, predicate(evt.newValue)));

            action(target, predicate(valueElement.value));
        }

        /// <summary>
        /// An overload that can be used when T is of type bool in <see cref="BindViewStyle{T}"/>.
        /// </summary>
        /// <example>testButton.BindViewStyle(targetElement);</example>
        public static void BindViewStyle(this INotifyValueChanged<bool> valueElement, VisualElement target,
            Action<VisualElement, bool> action = null)
            => valueElement.BindViewStyle(target, x => x, action);

        #endregion
    }
}