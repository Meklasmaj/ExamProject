using System.Collections.ObjectModel;
using GymnasieProjekt.Core.Models;
using GymnasieProjekt.Core.Repositories;
using GymnasieProjekt.WPF.MVVM;

namespace GymnasieProjekt.WPF.ViewModels;

// The page with all students and the search field.
// It only reports which student was chosen (openStudent); it doesn't show them itself.
public class StudentListViewModel : ViewModelBase
{
    private readonly IStudentRepository _studentRepository;
    private readonly Action<Student> _openStudent;

    public StudentListViewModel(IStudentRepository studentRepository, Action<Student> openStudent)
    {
        _studentRepository = studentRepository;
        _openStudent = openStudent;
        LoadAllStudents();
    }

    // ----- List of all students -----

    public ObservableCollection<Student> AllStudents { get; } = new();

    // Bound to the list's SelectedItem, so clicking a student lands here
    private Student? _selectedStudent;
    public Student? SelectedStudent
    {
        get => _selectedStudent;
        set
        {
            _selectedStudent = value;
            OnPropertyChanged();

            // Reset() sets it back to null: that shouldn't open anything
            if (value != null)
                _openStudent(value);
        }
    }

    // ----- Search field and dropdown -----

    public ObservableCollection<Student> SearchResults { get; } = new();

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            Search();
        }
    }

    private bool _isDropDownOpen;
    public bool IsDropDownOpen
    {
        get => _isDropDownOpen;
        set { _isDropDownOpen = value; OnPropertyChanged(); }
    }

    // Set by the dropdown list when the user picks a student
    private Student? _selectedSearchResult;
    public Student? SelectedSearchResult
    {
        get => _selectedSearchResult;
        set
        {
            _selectedSearchResult = value;
            OnPropertyChanged();

            // The dropdown also sets this to null when its items are cleared: ignore that
            if (value != null)
            {
                IsDropDownOpen = false;
                _openStudent(value);
            }
        }
    }

    private string _errorMessage = "";
    public string ErrorMessage
    {
        get => _errorMessage;
        private set { _errorMessage = value; OnPropertyChanged(); }
    }

    // ----- Logic -----

    // Called when the user comes back to the list, so nothing is selected
    // and the same student can be clicked again
    public void Reset()
    {
        SelectedStudent = null;
        SearchText = "";   // also clears the dropdown
    }

    private void LoadAllStudents()
    {
        try
        {
            foreach (var student in _studentRepository.GetAll())
                AllStudents.Add(student);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Kunne ikke hente elever: {ex.Message}";
        }
    }

    private void Search()
    {
        SearchResults.Clear();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            IsDropDownOpen = false;
            return;
        }

        try
        {
            foreach (var student in _studentRepository.SearchByName(SearchText.Trim()))
                SearchResults.Add(student);

            ErrorMessage = "";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Kunne ikke søge i databasen: {ex.Message}";
        }

        IsDropDownOpen = SearchResults.Count > 0;
    }
}
