using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace lab2_wpf
{
    public class DigitCommand : CalculatorCommand
    {
        private string digit;
        public DigitCommand(MainWindow window, string digit) : base(window) { this.digit = digit; }

        protected override void DoAction()
        {
            if (window.IsNewEntry || window.Display.Text == "Error")
            {
                if (digit == "00") window.Display.Text = "0";
                else window.Display.Text = (digit == ",") ? "0," : digit;

                window.HistoryDisplay.Text = "";
                window.IsNewEntry = false;
                return;
            }

            string text = window.Display.Text;
            string lastPart = text.Split(new[] { ' ', '(', ')' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? "";

            if (digit == ",")
            {
                if (lastPart.Contains(",")) return;
                window.Display.Text += ",";
            }
            else if (digit == "0" || digit == "00")
            {

                if (lastPart == "0") return;

                window.Display.Text += digit;
            }

            else
            {
                if (lastPart == "0")
                    window.Display.Text = text.Remove(text.Length - 1) + digit;
                else
                    window.Display.Text += digit;
            }
        }
    }
}