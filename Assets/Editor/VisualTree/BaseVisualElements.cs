using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SomeGame.Editor.VisualElements
{
    /// <summary>
    /// a manipulator that reacts to any changes in VisualElement
    /// </summary>
    public interface IItemChangesManipulator : IManipulator
    {
        event EventHandler ValueChanged;
    }

    /// <summary>
    /// VisualElement that is for UIDocument using
    /// </summary>
    public abstract class VisualElementView : VisualElement
    {
        private static Dictionary<string, VisualTreeAsset> _assets = new Dictionary<string, VisualTreeAsset>();

        private VisualTreeAsset GetAsset(string path)
        {
            if (!_assets.TryGetValue(path, out var value))
            {
                value = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
                _assets[path] = value;
            }

            return value;
        }

        public VisualElementView(string path)
        {
            Add(GetAsset(path).CloneTree());
        }
    }

    /// <summary>
    /// Replacement for builtin dropdown. The builtin is broken and doesn't work more than one time.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public class DropDownVisualElement<TEnum> : BindableElement, INotifyValueChanged<string> where TEnum : Enum
    {
        private Button _dropDownButton;
        private GroupBox _itemsBox;

        private TEnum _value;

        public string value
        {
            get => _value.ToString();
            set
            {
                using (ChangeEvent<string> pooled = ChangeEvent<string>.GetPooled(_value.ToString(), value))
                {
                    pooled.target = (IEventHandler)this;
                    SetValueWithoutNotify(value);
                    SendEvent((EventBase)pooled);
                }
            }
        }

        public DropDownVisualElement()
        {
            _dropDownButton = new Button(DropDownButtonClickedHandler);
            Add(_dropDownButton);
        }

        public DropDownVisualElement(string bindPath) : this()
        {
            bindingPath = bindPath;
            SetValueWithoutNotify(((TEnum)default).ToString());
        }

        private void ShowDropDownMenu()
        {
            void ItemClickedHandler(int index)
            {
                value = Enum.GetValues(typeof(TEnum)).GetValue(index).ToString();
            }

            var _genericDropdownMenu = new GenericDropdownMenu();
            var names = Enum.GetNames(typeof(TEnum));
            for (var i = 0; i < names.Length; i++)
            {
                var index = i;
                _genericDropdownMenu.AddItem(names[i], false, () => ItemClickedHandler(index));
            }

            _genericDropdownMenu.DropDown(worldBound, _dropDownButton, true);
        }

        private void DropDownButtonClickedHandler()
        {
            ShowDropDownMenu();
        }

        public void SetValueWithoutNotify(string newValue)
        {
            _value = Enum.Parse<TEnum>(newValue);
            _dropDownButton.text = _value.ToString();
        }
    }

    /// <summary>
    /// Base class for properties that has min max fields
    /// </summary>
    /// <typeparam name="TFieldType"></typeparam>
    public abstract class RangedRewardVisualElementView<TFieldType> : VisualElementView where TFieldType : BindableElement
    {
        private const string LessThanZeroText = "is less than zero";
        private const string MaxValueIsLessThanMinValue = "is less than min value";
        private const string MinValueText = "MinValue";
        private const string MaxValueText = "MaxValue";

        private const string AssetPath = "Assets/Editor/VisualTree/UXML/CurrencyContainer.uxml";

        private const string MinFieldContainerId = "MinFieldContainer";
        private const string MaxFieldContainerId = "MaxFieldContainer";

        private const string MinValueContainerId = "MinValueContainer";
        private const string MaxValueContainerId = "MaxValueContainer";

        private const string AssertionsLabelId = "InfoText";

        private readonly Color _warningColor = new Color(1, 0, 0, 0.3f);
        protected readonly StyleColor WarningStyleColor;
        protected readonly StyleColor RegularStyleColor;

        protected readonly (TFieldType min, TFieldType max) MinMaxFields;

        public readonly VisualElement MinFieldContainer;
        public readonly VisualElement MaxFieldContainer;

        public readonly VisualElement MinValueContainer;
        public readonly VisualElement MaxValueContainer;

        private Label AssertionsLabel;

        protected abstract bool IsMinFieldIsLessThanZero { get; }
        protected abstract bool IsMaxFieldIsLessThanZero { get; }
        protected abstract bool IsMaxFieldIsLessThanMin { get; }

        public RangedRewardVisualElementView(TFieldType min, TFieldType max, IItemChangesManipulator manipulator) : base(AssetPath)
        {
            MinMaxFields = (min, max);

            MinFieldContainer = this.Q(MinFieldContainerId);
            MaxFieldContainer = this.Q(MaxFieldContainerId);

            RegularStyleColor = MinFieldContainer.style.backgroundColor;
            WarningStyleColor = RegularStyleColor;
            WarningStyleColor.value = _warningColor;

            MinValueContainer = this.Q(MinValueContainerId);
            MaxValueContainer = this.Q(MaxValueContainerId);

            MinValueContainer.Add(MinMaxFields.min);
            MaxValueContainer.Add(MinMaxFields.max);

            AssertionsLabel = this.Q<Label>(AssertionsLabelId);

            manipulator.ValueChanged += ItemChangedHandler;
            this.AddManipulator(manipulator);

            AssertErrors();
        }

        private void ItemChangedHandler(object sender, EventArgs e)
        {
            AssertErrors(true);
        }

        public Exception AssertErrors(bool displayAssertions = false)
        {
            var exceptions = new List<Exception>();

            var isMinValueFieldLessThanZero = IsMinFieldIsLessThanZero;
            if (isMinValueFieldLessThanZero)
            {
                exceptions.Add(new Exception($"{MinValueText} {LessThanZeroText}") );
            }

            var isMaxFieldIsLessThanZero = IsMaxFieldIsLessThanZero;
            if (isMaxFieldIsLessThanZero)
            {
                exceptions.Add(new Exception($"{MaxValueText} {LessThanZeroText}"));
            }

            var isMaxValueLessThanMin = IsMaxFieldIsLessThanMin;
            if (isMaxValueLessThanMin)
            {
                exceptions.Add(new Exception($"{MaxValueText} {MaxValueIsLessThanMinValue}"));
            }

            if (displayAssertions)
            {
                SetStyleColor(MinFieldContainer, isMinValueFieldLessThanZero);
                SetStyleColor(MaxFieldContainer, isMaxFieldIsLessThanZero || isMaxValueLessThanMin);
            }
            else
            {
                SetStyleColor(MinFieldContainer, false);
                SetStyleColor(MaxFieldContainer, false);
            }

            if (displayAssertions && exceptions.Any())
            {
                AssertionsLabel.visible = true;
                AssertionsLabel.text = exceptions.First().Message;
            }
            else
            {
                AssertionsLabel.visible = false;
            }

            return exceptions.Count > 1 ? new AggregateException(exceptions) : exceptions.FirstOrDefault();
        }

        private void SetStyleColor(VisualElement visualElement, bool isHighlighted)
        {
            visualElement.style.backgroundColor = isHighlighted ? WarningStyleColor : RegularStyleColor;
        }
    }
}
