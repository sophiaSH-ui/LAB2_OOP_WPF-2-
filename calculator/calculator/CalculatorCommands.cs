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


    }
}