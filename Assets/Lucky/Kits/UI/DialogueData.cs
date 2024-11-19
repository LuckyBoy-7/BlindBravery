using System.Collections.Generic;
using UnityEngine;

namespace Lucky.Kits.UI
{
    [CreateAssetMenu(menuName = "DialogueData")]
    public class DialogueData : ScriptableObject
    {
        [TextArea(3, 10)] public List<string> contents;
    }
}