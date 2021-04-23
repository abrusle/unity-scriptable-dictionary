using UnityEngine;

namespace ScriptableDictionaries.Example
{
    [CreateAssetMenu(fileName = "new Example Dictionary", menuName = "Packages/Scriptable Dictionary/Example Dictionary", order = 1000)]
    internal class ExampleDictionary : ScriptableDictionary<string, float>
    {
        
    }
}