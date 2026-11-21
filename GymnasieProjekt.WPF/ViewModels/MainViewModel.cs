using GymnasieProjekt.Core.Repositories;
using GymnasieProjekt.Core.Services;
using GymnasieProjekt.WPF.MVVM;

namespace GymnasieProjekt.WPF.ViewModels;

// Holds one ViewModel per tab in the main window.
public class MainViewModel : ViewModelBase
{
    public HomeViewModel Home { get; }
    public StudentsViewModel Students { get; }

    public MainViewModel(IStudentRepository studentRepository, AbsenceService absenceService)
    {
        Home = new HomeViewModel();
        Students = new StudentsViewModel(studentRepository, absenceService);
    }
}
