using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace WikipediaApp
{
  public class ViewModelBase : ObservableObject
  {
    public virtual Task Initialize()
    {
      return Task.CompletedTask;
    }
  }
}