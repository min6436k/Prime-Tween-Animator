using System;
using UnityEngine;

namespace ODY.PrimeTweenAnimation
{
    /// <summary>
    /// A wrapper class that manages IPrimeAnimStrategy and links it to PrimeTweenAnimator.
    /// </summary>
    [Serializable]
    public partial class PrimeAnimContainer
    {
        [SerializeField] private string _playID;
        public string PlayID => _playID;

        [SerializeReference] public IPrimeAnimStrategy anim;
        
        #if UNITY_EDITOR
        [SerializeField] private TweenType _tweenType;
        [SerializeField] private bool _foldOutViewData;
        #endif

        public void OnTweenTypeChanged(TweenType newValue)
        {
            if (!PrimeAnimStrategyAttribute.GetClassType(newValue, out var targetType))
            {
                anim = new EmptyAnimStrategy();
                return;
            }

            anim = (IPrimeAnimStrategy)Activator.CreateInstance(targetType);
        }
    }
}