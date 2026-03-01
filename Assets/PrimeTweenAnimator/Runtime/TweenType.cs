using UnityEngine;

namespace ODY.PrimeTweenAnimation
{
    public enum TweenType
    {
        None,
        Pos,
        LocalPos,
        Rotate,
        LocalRotate,
        Scale,
        Color,
        Fade,
        [InspectorName("Punch/Position")] PunchPosition,
        [InspectorName("Punch/Rotation")] PunchRotation,
        [InspectorName("Punch/Scale")] PunchScale,
        [InspectorName("Shake/Position")] ShakePosition,
        [InspectorName("Shake/Rotation")] ShakeRotation,
        [InspectorName("Shake/Scale")] ShakeScale,
        [InspectorName("Shake/Camera")] ShakeCamera,
        [InspectorName("Custom/Float")] CustomFloat,
        [InspectorName("Custom/Vector2")] CustomVector2,
        [InspectorName("Custom/Vector3")] CustomVector3,
        [InspectorName("Custom/Quaternion")] CustomQuaternion,
        [InspectorName("Custom/Color")] CustomColor,
        [InspectorName("Custom/Rect")] CustomRect,
        // [InspectorName("MaterialProperty/Vector4")] MaterialVector4,
        // [InspectorName("MaterialProperty/float")] MaterialFloat,
        Delay,
    }
}