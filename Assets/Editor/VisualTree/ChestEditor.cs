using SomeGame.Configuration;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SomeGame.Editor
{
    [CustomEditor(typeof(TreeChestConfig))]
    public class ChestEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            return root;
        }
    }
}
