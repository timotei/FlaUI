﻿using System;
using System.Globalization;
using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.Patterns;
using FlaUI.Core.WindowsAPI;

namespace FlaUI.Core.AutomationElements
{
    public class Slider : AutomationElement
    {
        public Slider(BasicAutomationElementBase basicAutomationElement) : base(basicAutomationElement)
        {
        }

        private IRangeValuePattern RangeValuePattern => PatternFactory.GetRangeValuePattern();

        private IValuePattern ValuePattern => PatternFactory.GetValuePattern();

        private Button LargeIncreaseButton => GetLargeIncreaseButton();

        private Button LargeDecreaseButton => GetLargeDecreaseButton();

        public Thumb Thumb => FindFirst(TreeScope.Children, ConditionFactory.ByControlType(ControlType.Thumb)).AsThumb();

        public bool IsOnlyValue => RangeValuePattern == null;

        public double Value
        {
            get
            {
                var rangeValuePattern = RangeValuePattern;
                if (rangeValuePattern != null)
                {
                    return RangeValuePattern.Value;
                }
                // UIA3 for WinForms does not have the RangeValue pattern, only the value pattern
                // The value in this case is always between 0 and 100
                return Convert.ToDouble(ValuePattern.Value);
            }
            set
            {
                var rangeValuePattern = RangeValuePattern;
                if (rangeValuePattern != null)
                {
                    RangeValuePattern.SetValue(value);
                }
                else
                {
                    // UIA3 for WinForms does not have the RangeValue pattern, only the value pattern
                    // The value in this case is always between 0 and 100
                    ValuePattern.SetValue(value.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        public void SmallIncrement()
        {
            Keyboard.PressVirtualKeyCode(VirtualKeyShort.RIGHT);
            Helpers.WaitUntilInputIsProcessed();
        }

        public void SmallDecrement()
        {
            Keyboard.PressVirtualKeyCode(VirtualKeyShort.LEFT);
            Helpers.WaitUntilInputIsProcessed();
        }

        public void LargeIncrement()
        {
            LargeIncreaseButton.Invoke();
        }

        public void LargeDecrement()
        {
            LargeDecreaseButton.Invoke();
        }

        private Button GetLargeIncreaseButton()
        {
            if (FrameworkType == FrameworkType.Wpf)
            {
                // For WPF, this is simple
                return FindFirstChild(cf => cf.ByAutomationId("IncreaseLarge")).AsButton();
            }
            // For WinForms, we loop thru the buttons and find the one right of the thumb
            var buttons = FindAllChildren(cf => cf.ByControlType(ControlType.Button));
            foreach (var button in buttons)
            {
                if (button.Info.BoundingRectangle.Value.Left > Thumb.Info.BoundingRectangle.Value.Left)
                {
                    return button.AsButton();
                }
            }
            return null;
        }

        private Button GetLargeDecreaseButton()
        {
            if (FrameworkType == FrameworkType.Wpf)
            {
                // For WPF, this is simple
                return FindFirstChild(cf => cf.ByAutomationId("DecreaseLarge")).AsButton();
            }
            // For WinForms, we loop thru the buttons and find the one left of the thumb
            var buttons = FindAllChildren(cf => cf.ByControlType(ControlType.Button));
            foreach (var button in buttons)
            {
                if (button.Info.BoundingRectangle.Value.Right < Thumb.Info.BoundingRectangle.Value.Right)
                {
                    return button.AsButton();
                }
            }
            return null;
        }
    }
}
