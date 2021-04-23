# Scriptable Dictionary

Provides a base ScriptableObject class `ScriptableDictionary<TKey, TValue>` that implements `IDictionary<TKey, TValue>`. You can derive from that class to create custom dictionary asset.

## Examples
1. Simple types

```C#
[CreateAssetMenu(fileName = "new Simple Dictionary"]
public class MySimpleDictionary : ScriptableDictionary<string, float>
{
    //
}
```


2. Custom Types
```C#
public enum SpellType
{
    Air,
    Water,
    Fire,
    Earth
}

[System.Serializable]
public struct DamageInfo
{
    public float value;
    public string name;
    public float knockback;
}

[CreateAssetMenu(fileName = "new Spell Damage Map"]
public class SpellDamageMap : ScriptableDictionary<SpellType, DamageInfo>
{
    //
}
```