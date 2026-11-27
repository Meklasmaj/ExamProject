using System.Windows.Input;
using GymnasieProjekt.Core.Repositories;
using GymnasieProjekt.Core.Services;
using GymnasieProjekt.WPF.MVVM;

namespace GymnasieProjekt.WPF.ViewModels;

// The main window: a menu, a back button and the current page (via Navigator).
public class MainViewModel : ViewModelBase
{
    public Navigator Navigator { get; }

    public ICommand ShowHomeCommand { get; }
    public ICommand ShowStudentsCommand { get; }

    public MainViewModel(IStudentRepository studentRepository, AbsenceService absenceService)
    {
        var home = new HomeViewModel();
        Navigator = new Navigator(home);

        // The pages are kept, so the student list isn't loaded again each time
        var students = new StudentListViewModel(studentRepository, absenceService, Navigator);

        ShowHomeCommand = new RelayCommand(_ => Navigator.NavigateFromMenu(home));
        ShowStudentsCommand = new RelayCommand(_ => Navigator.NavigateFromMenu(students));
    }
}
