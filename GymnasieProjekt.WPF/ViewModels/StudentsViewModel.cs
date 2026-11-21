using GymnasieProjekt.Core.Models;
using GymnasieProjekt.Core.Repositories;
using GymnasieProjekt.Core.Services;
using GymnasieProjekt.WPF.MVVM;

namespace GymnasieProjekt.WPF.ViewModels;

// The "Elever" tab. It shows one page at a time: the student list,
// or the page for one student. The view shows whatever CurrentPage is.
public class StudentsViewModel : ViewModelBase
{
    private readonly AbsenceService _absenceService;
    private readonly StudentListViewModel _listPage;

    public StudentsViewModel(IStudentRepository studentRepository, AbsenceService absenceService)
    {
        _absenceService = absenceService;

        // The list page is kept, so going back doesn't load all students again
        _listPage = new StudentListViewModel(studentRepository, OpenStudent);
        _currentPage = _listPage;
    }

    private ViewModelBase _currentPage;
    public ViewModelBase CurrentPage
    {
        get => _currentPage;
        private set { _currentPage = value; OnPropertyChanged(); }
    }

    private void OpenStudent(Student student)
    {
        CurrentPage = new StudentViewModel(student, _absenceService, BackToList);
    }

    private void BackToList()
    {
        _listPage.Reset();
        CurrentPage = _listPage;
    }
}
