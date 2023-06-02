using System;
using Editor.Tree.ChestEditor;
using SomeGame.Configuration;
using UnityEditor;
using UnityEngine.UIElements;

namespace SomeGame.Editor.RewardConfiguration
{
    [CustomPropertyDrawer(typeof(TreeChestConfig.RewardInfo))]
    public class RewardPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var rewardView = new ItemVisualElementView();
            var itemChangesManipulator = new ItemChangesManipulator();
            itemChangesManipulator.ValueChanged += (sender, eventArgs) => UpdatePropertyVisualElement(property, rewardView);
            rewardView.AddManipulator(itemChangesManipulator);

            UpdatePropertyVisualElement(property, rewardView);
            return rewardView;
        }

        private void UpdatePropertyVisualElement(SerializedProperty property, ItemVisualElementView rewardView)
        {
            var typeProperty = property.FindPropertyRelative("type");
            var type = (TreeChestConfig.RewardType)Enum.GetValues(typeof(TreeChestConfig.RewardType)).GetValue(typeProperty.enumValueIndex);
            var rewardVisualElement = rewardView.RewardContainerVisualElement;
            if (rewardVisualElement.RewardTypeKey != type)
            {
                rewardVisualElement.SetActiveType(type);
            }
        }
    }
}
