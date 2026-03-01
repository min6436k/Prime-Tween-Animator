using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace ODY.PrimeTweenAnimation
{
    public class PrimeTweenAnimator : MonoBehaviour
    {
        #if UNITY_EDITOR
        [SerializeField] private string _testPlayID;
        public int TestPlayIDHash => GetCustomHash(_testPlayID);
        
        [SerializeField] private bool _playOnStart;
        #endif
        
        public List<PrimeAnimContainer> animList = new();
        private readonly Dictionary<int, List<PrimeAnimContainer>> _animListCache = new();
        private static readonly Dictionary<string, int> IDHashCache = new();
        private bool _cacheDirty;

        private void Awake() => UpdateCache();

        public void Start()
        {
            if (_playOnStart) PlayAll();
        }

        #region Play
        
        public void PlayAll()
        {
            foreach (var t in animList) t.anim.Play();
        }
        
        public Sequence PlayAllAsSequence()
        {
            var sequence = Sequence.Create();
            foreach (var t in animList) sequence.Group(t.anim.Play());
            return sequence;
        }

        public void PlayByID(int hashID)
        {
            if(!GetListByID(hashID, out var list)) return;
            foreach (var i in list) i.anim.Play();
        }
        public void PlayByID(string id) => PlayByID(GetCustomHash(id));
        
        public Sequence PlayByIDAsSequence(int hashID)
        {
            var sequence = Sequence.Create();
            if(!GetListByID(hashID, out var list)) return sequence;
            foreach (var t in list) sequence.Group(t.anim.Play());
            return sequence;
        }
        public Sequence PlayByIDAsSequence(string id) => PlayByIDAsSequence(GetCustomHash(id));
        
        #endregion
        
        public void SetCacheDirty() => _cacheDirty = true;
        

        private bool GetListByID(int hashID, out List<PrimeAnimContainer> list)
        {
            if (_cacheDirty) UpdateCache();
            return _animListCache.TryGetValue(hashID, out list);
        }
        private void UpdateCache()
        {
            _cacheDirty = false;
            
            foreach (var list in _animListCache.Values) list.Clear();

            foreach (var i in animList)
            {
                if (!IDHashCache.TryGetValue(i.PlayID, out var hashId))
                    hashId = IDHashCache[i.PlayID] = GetCustomHash(i.PlayID);

                if (!_animListCache.TryGetValue(hashId, out var list))
                    list = _animListCache[hashId] = new List<PrimeAnimContainer>();

                list.Add(i);
            }
        }

        public static int GetCustomHash(string str)
        {
            unchecked
            {
                uint hash = 2166136261;
            
                const uint prime = 16777619;

                for (int i = 0; i < str.Length; i++)
                    hash = (hash ^ str[i]) * prime;
            
                return (int)hash;
            }
        }
    }
}


