using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.Scriptables
{
    [CreateAssetMenu(fileName = "_Parameters", menuName = "Configs/Unit/Parameters", order = 0)]
    public class UnitParameters : ScriptableObject
    {
        [SerializeField] private UnitConfig[] configs = Array.Empty<UnitConfig>();

        private readonly Dictionary<Type, UnitConfig> _cache = new();

        public T GetConfig<T>() where T : UnitConfig
        {
            var type = typeof(T);

            if (_cache.TryGetValue(type, out var cached))
                return cached as T;

            var found = Array.Find(configs, config => config is T);
            if (found) _cache[type] = found;

            return found as T;
        }
    }

}