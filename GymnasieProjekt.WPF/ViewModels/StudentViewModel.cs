using System.Globalization;
using System.Windows.Input;
using GymnasieProjekt.Core.Models;
using GymnasieProjekt.Core.Services;
using GymnasieProjekt.WPF.MVVM;

namespace GymnasieProjekt.WPF.ViewModels;

// The page for one student. It doesn't know where it was opened from:
// whoever opens it passes in what "Tilbage" should do. That way the same page
// can be opened from the student list, a class view, or anywhere else.
public class StudentViewModel : ViewModelBase
{
    private static readonly CultureInfo Danish = new("da-DK");

    public StudentViewModel(Student student, AbsenceService absenceService, Action goBack)
    {
        Student = student;
        BackCommand = new RelayCommand(_ => goBack());
        LoadAbsence(absenceService);
    }

    public Student Student { get; }

    public ICommand BackCommand { get; }

    public string AbsenceText { get; private set; } = "";

    public string ErrorMessage { get; private set; } = "";

    // Runs once, in the constructor, before the view binds,
    // so the properties don't need to raise PropertyChanged
    private void LoadAbsence(AbsenceService absenceService)
    {
        try
        {
            var summary = absenceService.GetTotalAbsence(Student.StudentId);

            AbsenceText = string.Format(Danish,
                "Samlet fravær: {0:0.0} % ({1:0.#} af {2} moduler)",
                summary.AbsencePercent, summary.AbsentModules, summary.ModuleCount);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Kunne ikke hente fravær: {ex.Message}";
        }
    }
}
