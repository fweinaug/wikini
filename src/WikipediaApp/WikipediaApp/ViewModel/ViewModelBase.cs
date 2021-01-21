using System.Threading.Tasks;

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