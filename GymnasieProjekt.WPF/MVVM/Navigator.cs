using System.Windows.Input;

namespace GymnasieProjekt.WPF.MVVM;

// Keeps track of which page is shown, and of the pages before it (for "Tilbage").
// MainWindow shows CurrentPage; the DataTemplates in App.xaml pick the view for it.
public class Navigator : ViewModelBase
{
    private readonly Stack<ViewModelBase> _history = new();

    public Navigator(ViewModelBase startPage)
    {
        _currentPage = startPage;
        BackCommand = new RelayCommand(_ => GoBack(), _ => CanGoBack);
    }

    private ViewModelBase _currentPage;
    public ViewModelBase CurrentPage
    {
        get => _currentPage;
        private set
        {
            _currentPage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanGoBack));

            // Ask WPF to check BackCommand.CanExecute again, so the button
            // is enabled/greyed out right away
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public bool CanGoBack => _history.Count > 0;

    public ICommand BackCommand { get; }

    // Go to a new page and remember the current one
    public void NavigateTo(ViewModelBase page)
    {
        _history.Push(CurrentPage);
        CurrentPage = page;
    }

    // Used by the menu: start over from this page, with no history
    public void NavigateFromMenu(ViewModelBase page)
    {
        _history.Clear();
        CurrentPage = page;
    }

    public void GoBack()
    {
        if (CanGoBack)
            CurrentPage = _history.Pop();
    }
}
