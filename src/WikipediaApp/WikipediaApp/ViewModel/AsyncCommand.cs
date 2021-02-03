using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WikipediaApp
{
  public class AsyncCommand : ICommand
  {
    public event EventHandler CanExecuteChanged;

    private readonly Func<Task> action;
    private readonly Func<bool> canExecute;
    private bool isExecuting;

    public AsyncCommand(Func<Task> action, Func<bool> canExecute = null)
    {
      this.action = action;
      this.canExecute = canExecute;
    }

    public bool CanExecute()
    {
      return !isExecuting && (canExecute == null || canExecute());
    }

    public async Task Execute()
    {
      if (CanExecute())
      {
        try
        {
          isExecuting = true;
          await action();
        }
        finally
        {
          isExecuting = false;
        }

        RaiseCanExecuteChanged();
      }
    }

    public void RaiseCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    bool ICommand.CanExecute(object parameter)
    {
      return CanExecute();
    }

    void ICommand.Execute(object parameter)
    {
      Execute().FireAndForgetSafeAsync();
    }
  }

  public static class TaskExecutor
  {
    public static async void FireAndForgetSafeAsync(this Task task, Action<Exception> handleErrorAction = null)
    {
      try
      {
        await task.ConfigureAwait(true);
      }
      catch (Exception ex)
      {
        handleErrorAction?.Invoke(ex);
      }
    }
  }
}