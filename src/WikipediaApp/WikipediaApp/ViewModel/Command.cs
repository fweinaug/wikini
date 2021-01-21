using System;
using System.Windows.Input;

namespace WikipediaApp
{
  public class Command<T> : ICommand
  {
    public event EventHandler CanExecuteChanged;

    private readonly Action<T> action;
    private readonly Func<T, bool> canExecute;

    public Command(Action<T> action, Func<T, bool> canExecute = null)
    {
      this.action = action;
      this.canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
      return canExecute == null || canExecute((T)parameter);
    }

    public void Execute(object parameter)
    {
      if (CanExecute(parameter))
      {
        action((T)parameter);
      }
    }

    public void RaiseCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
  }

  public class Command : ICommand
  {
    public event EventHandler CanExecuteChanged;

    private readonly Action action;
    private readonly Func<bool> canExecute;

    public Command(Action action, Func<bool> canExecute = null)
    {
      this.action = action;
      this.canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
      return canExecute == null || canExecute();
    }

    public void Execute(object parameter)
    {
      if (CanExecute(parameter))
      {
        action();
      }
    }

    public void RaiseCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}