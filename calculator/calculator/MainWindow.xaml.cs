using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace lab2_wpf
{
    public partial class MainWindow : Window
    {
        Stack<CalculatorCommand> undoStack = new Stack<CalculatorCommand>();
        Stack<CalculatorCommand> redoStack = new Stack<CalculatorCommand>();

        public bool IsNewEntry { get; set; } = true;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void Execute(CalculatorCommand command)
        {
            command.Execute();
            if (command.HasStateChanged())
            {
                undoStack.Push(command);
                redoStack.Clear();
            }
        }

        private void Digit_Click(object sender, RoutedEventArgs e) => Execute(new DigitCommand(this, (sender as Button).Content.ToString()));
        private void Op_Click(object sender, RoutedEventArgs e) => Execute(new OperatorCommand(this, (sender as Button).Content.ToString()));
        private void Equal_Click(object sender, RoutedEventArgs e) => Execute(new CalculateCommand(this));
        private void Clear_Click(object sender, RoutedEventArgs e) => Execute(new ClearCommand(this));

        private void Scientific_Click(object sender, RoutedEventArgs e)
        {
            string op = (sender as Button).Content.ToString();
            if (op == "^") Execute(new OperatorCommand(this, "^"));
            else Execute(new ScientificCommand(this, op));
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (undoStack.Count == 0) return;
            var cmd = undoStack.Pop();
            cmd.Undo();
            redoStack.Push(cmd);
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (redoStack.Count == 0) return;
            var cmd = redoStack.Pop();
            cmd.Execute();
            undoStack.Push(cmd);
        }


        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) { e.Handled = true; return; }

            bool shift = Keyboard.Modifiers == ModifierKeys.Shift;
            bool ctrl = Keyboard.Modifiers == ModifierKeys.Control;


            if (ctrl)
            {
                if (e.Key == Key.Z) { Undo_Click(null, null); e.Handled = true; return; }
                if (e.Key == Key.Y) { Redo_Click(null, null); e.Handled = true; return; }
                return;
            }


            if (shift)
            {
                e.Handled = true;
                if (e.Key == Key.D6) { Execute(new OperatorCommand(this, "^")); return; }
                if (e.Key == Key.D9) { Execute(new OperatorCommand(this, "(")); return; }
                if (e.Key == Key.D0) { Execute(new OperatorCommand(this, ")")); return; }
                if (e.Key == Key.D8) { Execute(new OperatorCommand(this, "×")); return; }


                if (e.Key == Key.OemPlus) { Execute(new OperatorCommand(this, "+")); return; }

                e.Handled = false;
                return;
            }


            if (!shift)
            {

                if (e.Key == Key.OemPlus || e.Key == Key.Enter)
                {
                    Execute(new CalculateCommand(this));
                    e.Handled = true;
                    return;
                }

                if (e.Key >= Key.D0 && e.Key <= Key.D9)
                {
                    Execute(new DigitCommand(this, (e.Key - Key.D0).ToString()));
                    e.Handled = true;
                    return;
                }
            }


            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                Execute(new DigitCommand(this, (e.Key - Key.NumPad0).ToString()));
            else if (e.Key == Key.Add)
                Execute(new OperatorCommand(this, "+"));
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
                Execute(new OperatorCommand(this, "-"));
            else if (e.Key == Key.Multiply)
                Execute(new OperatorCommand(this, "×"));
            else if (e.Key == Key.Divide || e.Key == Key.OemQuestion)
                Execute(new OperatorCommand(this, "÷"));
            else if (e.Key == Key.Escape)
                Execute(new ClearCommand(this));
            else if (e.Key == Key.Decimal || e.Key == Key.OemComma || e.Key == Key.OemPeriod)
                Execute(new DigitCommand(this, ","));
            else if (e.Key == Key.Back)
                Execute(new BackspaceCommand(this));
            else
                return;

            e.Handled = true;
        }

        private void MenuToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (ScientificPanel == null) return;


            DoubleAnimation widthAnim = new DoubleAnimation(80, TimeSpan.FromSeconds(0.3));
            widthAnim.EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut };


            DoubleAnimation opacityAnim = new DoubleAnimation(1, TimeSpan.FromSeconds(0.3));

            DoubleAnimation rotateAnim = new DoubleAnimation(90, TimeSpan.FromSeconds(0.3));

            ScientificPanel.BeginAnimation(WidthProperty, widthAnim);
            ScientificPanel.BeginAnimation(OpacityProperty, opacityAnim);
            MenuRotate.BeginAnimation(RotateTransform.AngleProperty, rotateAnim);

            MenuToggle.Content = "▼";
        }

        private void MenuToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ScientificPanel == null) return;


            DoubleAnimation widthAnim = new DoubleAnimation(0, TimeSpan.FromSeconds(0.3));
            widthAnim.EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseIn };

            DoubleAnimation opacityAnim = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));

            DoubleAnimation rotateAnim = new DoubleAnimation(0, TimeSpan.FromSeconds(0.3));

            ScientificPanel.BeginAnimation(WidthProperty, widthAnim);
            ScientificPanel.BeginAnimation(OpacityProperty, opacityAnim);
            MenuRotate.BeginAnimation(RotateTransform.AngleProperty, rotateAnim);

            MenuToggle.Content = "☰";
        }
        private void Backspace_Click(object sender, RoutedEventArgs e) => Execute(new BackspaceCommand(this));

    }
}