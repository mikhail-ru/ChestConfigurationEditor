using System;
using SomeGame.Configuration;
using SomeGame.Editor.RewardConfiguration;
using SomeGame.Editor.VisualElements;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.Tree.ChestEditor
{
    public class ItemChangesManipulator : Manipulator, IItemChangesManipulator
    {
        public event EventHandler ValueChanged;

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<ChangeEvent<int>>(ValueChangedHandler);
            target.RegisterCallback<ChangeEvent<float>>(ValueChangedHandler);
            target.RegisterCallback<ChangeEvent<string>>(ValueChangedHandler);
            target.RegisterCallback<ChangeEvent<TreeChestConfig.RewardType>>(ValueChangedHandler);
            target.RegisterCallback<ChangeEvent<TreeChestConfig.RewardInfo>>(ValueChangedHandler);
            target.RegisterCallback<ChangeEvent<TreeChestConfig>>(ValueChangedHandler);
            target.RegisterCallback<ChangeEvent<object>>(ValueChangedHandler);
            target.RegisterCallback<SerializedPropertyChangeEvent>(ValueChangedHandler, TrickleDown.TrickleDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<ChangeEvent<int>>(ValueChangedHandler);
            target.UnregisterCallback<ChangeEvent<float>>(ValueChangedHandler);
            target.UnregisterCallback<ChangeEvent<string>>(ValueChangedHandler);
            target.UnregisterCallback<ChangeEvent<TreeChestConfig.RewardType>>(ValueChangedHandler);
            target.UnregisterCallback<ChangeEvent<TreeChestConfig.RewardInfo>>(ValueChangedHandler);
            target.UnregisterCallback<ChangeEvent<TreeChestConfig>>(ValueChangedHandler);
            target.UnregisterCallback<ChangeEvent<object>>(ValueChangedHandler);
            target.UnregisterCallback<SerializedPropertyChangeEvent>(ValueChangedHandler, TrickleDown.TrickleDown);
        }

        private void ValueChangedHandler(ChangeEvent<int> evt)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ValueChangedHandler(ChangeEvent<TreeChestConfig> evt)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ValueChangedHandler(ChangeEvent<TreeChestConfig.RewardInfo> evt)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ValueChangedHandler(ChangeEvent<string> evt)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ValueChangedHandler(ChangeEvent<TreeChestConfig.RewardType> evt)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ValueChangedHandler(ChangeEvent<float> evt)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ValueChangedHandler(ChangeEvent<object> evt)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ValueChangedHandler(EventCallback<SerializedPropertyChangeEvent> evt)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ValueChangedHandler(SerializedPropertyChangeEvent evt)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class ItemVisualElementView : VisualElementView
    {
        private const string AssetPath = "Assets/Editor/VisualTree/UXML/RewardContainer.uxml";

        private readonly string _viewSizeIsNotSet = $"Height must be set and its units must be in pixels {AssetPath}";

        private const string ItemContainerId = "itemContainer";
        private const string RewardTypeDropDownViewContainerId = "RewardTypeContainer";

        internal readonly RewardContainerVisualElement RewardContainerVisualElement;

        public ItemVisualElementView() : base(AssetPath)
        {
            var itemContainer = this.Q(ItemContainerId);
            RewardContainerVisualElement = new RewardContainerVisualElement();
            itemContainer.Add(RewardContainerVisualElement);

            var dropdownVisualElement = new DropDownVisualElement<TreeChestConfig.RewardType>("type");
            this.Q(RewardTypeDropDownViewContainerId).Add(dropdownVisualElement);
        }
    }

    public class RewardContainerVisualElement : VisualElement
    {
        public TreeChestConfig.RewardType? RewardTypeKey { get; private set; }

        public SoftRewardVisualElement SoftRewardVisualElement { get; }
        public HardRewardVisualElement HardRewardVisualElement { get; }
        public ChestRewardVisualElement ChestRewardVisualElement { get; }

        public RewardContainerVisualElement()
        {
            SoftRewardVisualElement = new SoftRewardVisualElement();
            HardRewardVisualElement = new HardRewardVisualElement();
            ChestRewardVisualElement = new ChestRewardVisualElement();

            Add(SoftRewardVisualElement);
            Add(HardRewardVisualElement);
            Add(ChestRewardVisualElement);

            SetActiveType(default);
        }

        public void SetActiveType(TreeChestConfig.RewardType key)
        {
            RewardTypeKey = key;

            var invisible = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            var visible = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

            SoftRewardVisualElement.style.display = key == TreeChestConfig.RewardType.Soft ? visible : invisible;
            HardRewardVisualElement.style.display = key == TreeChestConfig.RewardType.Hard ? visible : invisible;
            ChestRewardVisualElement.style.display = key == TreeChestConfig.RewardType.Chest ? visible : invisible;
        }
    }
}
