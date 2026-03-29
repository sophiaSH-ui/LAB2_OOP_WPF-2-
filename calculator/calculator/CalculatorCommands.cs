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

    public class OperatorCommand : CalculatorCommand
    {
        private string op;
        public OperatorCommand(MainWindow window, string op) : base(window) { this.op = op; }

        protected override void DoAction()
        {
            if (window.Display.Text == "Error") return;

            if (window.IsNewEntry) window.IsNewEntry = false;

            string text = window.Display.Text.Trim();
            char last = text.Length > 0 ? text.Last() : '\0';

            if (op == "(")
            {
                if (text == "0" || text == "") window.Display.Text = "(";
                else if (char.IsDigit(last) || last == ')' || last == 'π' || last == 'e')
                    window.Display.Text = text + " × (";
                else
                    window.Display.Text = text + " (";
            }
            else if (op == ")")
            {
                string[] forbiddenBeforeClose = { "(", "+", "-", "×", "÷", "^" };
                if (forbiddenBeforeClose.Any(o => text.EndsWith(o))) return;

                int openCount = text.Count(f => f == '(');
                int closeCount = text.Count(f => f == ')');
                if (openCount > closeCount) window.Display.Text = text + " )";
            }
            else if (text.EndsWith("("))
            {
                if (op == "-") window.Display.Text = text + op;
                else return;
            }
            else if (text == "0" || text == "")
            {
                if (op == "-") window.Display.Text = op;
                else return;
            }
            else
            {
                string[] operators = { "+", "-", "×", "÷", "^" };
                bool endsWithOp = operators.Any(o => text.EndsWith(o));

                if (endsWithOp)
                {
                    int lastIdx = text.LastIndexOfAny(new char[] { '+', '-', '×', '÷', '^' });
                    window.Display.Text = text.Substring(0, lastIdx).Trim() + " " + op + " ";
                }
                else
                {
                    window.Display.Text = text + " " + op + " ";
                }
            }

        }
    }

    public class CalculateCommand : CalculatorCommand
    {
        public CalculateCommand(MainWindow window) : base(window) { }

        protected override void DoAction()
        {
            try
            {
                string expression = window.Display.Text.Trim();
                if (string.IsNullOrEmpty(expression) || expression == "0") return;

                int diff = expression.Count(f => f == '(') - expression.Count(f => f == ')');
                for (int i = 0; i < diff; i++) expression += " )";

                string formula = expression
                    .Replace("×", "*").Replace("÷", "/").Replace(",", ".")
                    .Replace("π", Math.PI.ToString(CultureInfo.InvariantCulture))
                    .Replace("e", Math.E.ToString(CultureInfo.InvariantCulture))
                    .Replace("√", "sqrt").Replace("ln", "ln");

                double result = ExpressionParser.Evaluate(formula);

                window.HistoryDisplay.Text = expression + " =";
                window.Display.Text = result.ToString(new CultureInfo("uk-UA"));
                window.IsNewEntry = true;
            }
            catch
            {
                window.Display.Text = "Error";
                window.IsNewEntry = true;
            }
        }
    }

    public class ScientificCommand : CalculatorCommand
    {
        private string op;
        public ScientificCommand(MainWindow window, string op) : base(window) { this.op = op; }

        protected override void DoAction()
        {
            if (window.IsNewEntry) { window.Display.Text = "0"; window.HistoryDisplay.Text = ""; window.IsNewEntry = false; }
            string text = window.Display.Text.Trim();

            if (op == "π" || op == "e")
            {
                string val = op;
                if (text == "0") window.Display.Text = val;
                else if (char.IsDigit(text.Last()) || text.Last() == ')') window.Display.Text = text + " × " + val;
                else window.Display.Text = text + " " + val;
            }
            else
            {
                if (text == "0") window.Display.Text = op + " (";
                else if (char.IsDigit(text.Last()) || text.Last() == ')') window.Display.Text = text + " × " + op + " (";
                else window.Display.Text = text + " " + op + " (";
            }


        }
    }

    public class ClearCommand : CalculatorCommand
    {
        public ClearCommand(MainWindow window) : base(window) { }
        protected override void DoAction()
        {
            window.Display.Text = "0";
            window.HistoryDisplay.Text = "";
            window.IsNewEntry = true;
        }
    }

    public class BackspaceCommand : CalculatorCommand
    {
        public BackspaceCommand(MainWindow window) : base(window) { }
        protected override void DoAction()
        {
            if (window.IsNewEntry || window.Display.Text == "Error")
            {
                window.Display.Text = "0";
                window.HistoryDisplay.Text = "";
                window.IsNewEntry = true;
                return;
            }

            string text = window.Display.Text;
            if (text.Length <= 1 || text == "0")
            {
                window.Display.Text = "0";
                window.IsNewEntry = true;
            }
            else
            {
                if (text.EndsWith(" "))
                {
                    if (text.Length >= 3) window.Display.Text = text.Substring(0, text.Length - 3);
                    else window.Display.Text = "0";
                }
                else
                {
                    window.Display.Text = text.Substring(0, text.Length - 1);
                }
                if (string.IsNullOrEmpty(window.Display.Text)) window.Display.Text = "0";
            }


        }
    }

    public static class ExpressionParser
    {
        private static string _formula;
        private static int _pos;

        public static double Evaluate(string formula)
        {
            _formula = formula.Replace(" ", "");
            _pos = 0;
            return ParseExpression();
        }

        private static double ParseExpression()
        {
            double left = ParseTerm();
            while (_pos < _formula.Length)
            {
                char op = _formula[_pos];
                if (op != '+' && op != '-') break;
                _pos++;
                double right = ParseTerm();
                if (op == '+') left += right; else left -= right;
            }
            return left;
        }

       

        private static double ParseFactor()
        {
            if (_pos >= _formula.Length) return 0;

            if (_formula[_pos] == '(')
            {
                _pos++;
                double result = ParseExpression();
                if (_pos < _formula.Length && _formula[_pos] == ')') _pos++;
                return result;
            }

            if (Char.IsLetter(_formula[_pos]))
            {
                int start = _pos;
                while (_pos < _formula.Length && Char.IsLetter(_formula[_pos])) _pos++;
                string func = _formula.Substring(start, _pos - start);
                double arg = ParseFactor();
                if (func == "sqrt") return Math.Sqrt(arg);
                if (func == "ln") return Math.Log(arg);
            }

            int numStart = _pos;
            if (_pos < _formula.Length && _formula[_pos] == '-') _pos++;
            while (_pos < _formula.Length && (Char.IsDigit(_formula[_pos]) || _formula[_pos] == '.')) _pos++;

            string numStr = _formula.Substring(numStart, _pos - numStart);
            return double.Parse(numStr, CultureInfo.InvariantCulture);
        }
    }