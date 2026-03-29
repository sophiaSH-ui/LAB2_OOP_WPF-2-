using System.Windows.Controls;

namespace lab2_wpf
{
    public interface ICalculatorCommand
    {
        void Execute();
        void Undo();
    }

    public abstract class CalculatorCommand : ICalculatorCommand
    {
        protected MainWindow window;
        private string previousDisplay;
        private string previousHistory;
        private bool previousIsNewEntry;

        public CalculatorCommand(MainWindow window) { this.window = window; }

        public void Execute()
        {
            previousDisplay = window.Display.Text;
            previousHistory = window.HistoryDisplay.Text;
            previousIsNewEntry = window.IsNewEntry;

            DoAction();

        }

        public void Undo()
        {
            window.Display.Text = previousDisplay;
            window.HistoryDisplay.Text = previousHistory;
            window.IsNewEntry = previousIsNewEntry;
        }

        public bool HasStateChanged()
        {
            return previousDisplay != window.Display.Text || previousHistory != window.HistoryDisplay.Text;
        }

        protected abstract void DoAction();
    }

}
