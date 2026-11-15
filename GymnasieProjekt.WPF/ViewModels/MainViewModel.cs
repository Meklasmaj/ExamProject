using System.Collections.ObjectModel;
using System.Windows.Input;
using GymnasieProjekt.Core.Models;
using GymnasieProjekt.Core.Repositories;
using GymnasieProjekt.WPF.MVVM;

namespace GymnasieProjekt.WPF.ViewModels;

public class MainViewModel : ViewModelBase
{
    private const string AllClasses = "Alle klasser";

    private readonly IStudentRepository _studentRepository;

    public ObservableCollection<Student> Students { get; } = new();
    public ObservableCollection<string> Classes { get; } = new();

    private Student? _selectedStudent;
    public Student? SelectedStudent
    {
        get => _selectedStudent;
        set { _selectedStudent = value; OnPropertyChanged(); }
    }

    private string _selectedClass = AllClasses;
    public string SelectedClass
    {
        get => _selectedClass;
        set
        {
            _selectedClass = value;
            OnPropertyChanged();
            LoadStudents();
        }
    }

    private string _errorMessage = "";
    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); }
    }

    public ICommand ReloadCommand { get; }

    public MainViewModel(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
        ReloadCommand = new RelayCommand(_ => LoadData());
        LoadData();
    }

    private void LoadData()
    {
        LoadClasses();
        LoadStudents();
    }

    private void LoadClasses()
    {
        try
        {
            var classNames = _studentRepository.GetAll()
                .Select(s => s.ClassName)
                .Distinct()
                .OrderBy(c => c);

            Classes.Clear();
            Classes.Add(AllClasses);
            foreach (var className in classNames)
                Classes.Add(className);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Kunne ikke hente data fra databasen: {ex.Message}";
        }
    }

    private void LoadStudents()
    {
        try
        {
            var students = SelectedClass == AllClasses
                ? _studentRepository.GetAll()
                : _studentRepository.GetByClass(SelectedClass);

            Students.Clear();
            foreach (var student in students)
                Students.Add(student);

            ErrorMessage = "";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Kunne ikke hente data fra databasen: {ex.Message}";
        }
    }
}
