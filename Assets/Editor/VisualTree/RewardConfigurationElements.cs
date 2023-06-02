using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Tree.ChestEditor;
using SomeGame.Editor.VisualElements;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SomeGame.Editor.RewardConfiguration
{
    public class ChestRewardVisualElement : VisualElement
    {
        private const string ChestQuantityBindingPath = "chestCount";
        private const string ChestReferenceBindingPath = "chest";

        private const string ChestQuantityLabel = "Chest quantity";
        private const string ChestReferenceLabel = "Target chest";

        private readonly string _lessThanZeroText = $"{ChestQuantityLabel} is less than zero";
        private readonly string _chestReferenceIsNullText = $"{ChestReferenceLabel} is null";

        private readonly StyleColor _regularStyleColor;
        private readonly StyleColor _warningStyleColor;

        private readonly GroupBox _chestQuantityBox;
        private readonly GroupBox _chestReferenceBox;

        private readonly PropertyField _chestReferenceField;
        private readonly IntegerField _chestQuantityField;

        private readonly Label _assertionsLabel;

        private bool _isChestReferenceNull;

        public ChestRewardVisualElement()
        {
            var root = this;
            root.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
            _chestQuantityField = new IntegerField() { bindingPath = ChestQuantityBindingPath, style = { width = new StyleLength(new Length(100, LengthUnit.Percent)) } };
            _chestReferenceField = new PropertyField() { bindingPath = ChestReferenceBindingPath };
            _chestReferenceField.label = ChestReferenceLabel;
            _chestReferenceField.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var chestQuantityBox = new GroupBox { style = { flexDirection = FlexDirection.Row } };
            chestQuantityBox.Add(new Label(ChestQuantityLabel));
            var chestQuantityFieldContainer = new VisualElement();
            chestQuantityFieldContainer.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            chestQuantityFieldContainer.Add(_chestQuantityField);
            chestQuantityBox.Add(chestQuantityFieldContainer);

            _chestQuantityBox = chestQuantityBox;

            var chestReferenceBox = new GroupBox() { style = { flexDirection = FlexDirection.Row } };
            chestReferenceBox.Add(_chestReferenceField);
            _chestReferenceBox = chestReferenceBox;

            _assertionsLabel = new Label();

            root.Add(chestQuantityBox);
            root.Add(chestReferenceBox);
            root.Add(_assertionsLabel);

            _regularStyleColor = new StyleColor();
            _warningStyleColor = new StyleColor(new Color(1, 0, 0, 0.3f));

            var manipulator = new ItemChangesManipulator();
            manipulator.ValueChanged += (sender, args) => AssertErrors(true);
            this.AddManipulator(manipulator);

            _chestReferenceField.RegisterValueChangeCallback(ChestReferenceChanged);

            AssertErrors(true);
        }

        private void ChestReferenceChanged(SerializedPropertyChangeEvent evt)
        {
            _isChestReferenceNull = evt.changedProperty.objectReferenceValue == null;
            AssertErrors(true);
        }

        public Exception AssertErrors(bool displayAssertions = false)
        {
            var exceptions = new List<Exception>();

            var isQuantityIsLessThanZero = _chestQuantityField.value < 0;
            if (isQuantityIsLessThanZero)
            {
                exceptions.Add(new Exception(_lessThanZeroText));
            }

            if (_isChestReferenceNull)
            {
                exceptions.Add(new Exception(_chestReferenceIsNullText));
            }

            if (displayAssertions)
            {
                SetStyleColor(_chestQuantityBox, isQuantityIsLessThanZero);
                SetStyleColor(_chestReferenceBox, _isChestReferenceNull);
            }
            else
            {
                SetStyleColor(_chestQuantityBox, false);
                SetStyleColor(_chestReferenceBox, false);
            }

            if (displayAssertions && exceptions.Any())
            {
                _assertionsLabel.visible = true;
                _assertionsLabel.text = exceptions.First().Message;
            }
            else
            {
                _assertionsLabel.visible = false;
            }

            return exceptions.Count > 1 ? new AggregateException(exceptions) : exceptions.FirstOrDefault();
        }

        private void SetStyleColor(VisualElement visualElement, bool highlighted)
        {
            visualElement.style.backgroundColor = highlighted ? _warningStyleColor : _regularStyleColor;
        }
    }

    public class SoftRewardVisualElement : RangedRewardVisualElementView<IntegerField>
    {
        public SoftRewardVisualElement() : base(new IntegerField() { bindingPath = "softMin" }, new IntegerField() { bindingPath = "softMax" }, new ItemChangesManipulator()) { }
        protected override bool IsMinFieldIsLessThanZero => MinMaxFields.min.value < 0;
        protected override bool IsMaxFieldIsLessThanZero => MinMaxFields.max.value < 0;
        protected override bool IsMaxFieldIsLessThanMin => MinMaxFields.max.value < MinMaxFields.min.value;
    }

    public class HardRewardVisualElement : RangedRewardVisualElementView<FloatField>
    {
        public HardRewardVisualElement() : base(new FloatField() { bindingPath = "hardMin", }, new FloatField() { bindingPath = "hardMax", }, new ItemChangesManipulator()) { }

        protected override bool IsMinFieldIsLessThanZero => MinMaxFields.min.value < 0;
        protected override bool IsMaxFieldIsLessThanZero => MinMaxFields.max.value < 0;
        protected override bool IsMaxFieldIsLessThanMin => MinMaxFields.max.value < MinMaxFields.min.value;
    }
}
