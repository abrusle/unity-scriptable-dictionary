using UnityEngine;

namespace ScriptableDictionaries.Example
{
    [CreateAssetMenu(fileName = "new Spell Damage Map", menuName = "Packages/Scriptable Dictionary/Spell Damage Map", order = 1000)]
    internal class SpellDamageMap : ScriptableDictionary<SpellType, DamageInfo>
    {
        
    }

    internal enum SpellType
    {
        Water, Fire, Air, Earth
    }

    [System.Serializable]
    internal struct DamageInfo
    {
        public string name;
        public float damageAmount;
        public float multiplier;
        public ScriptableObject settings;
    }
}