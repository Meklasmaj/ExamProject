# Design Class Diagram (DCD)

LLD-artefakt, derfor på engelsk med de faktiske navne fra koden. Se [Ordliste.md](Ordliste.md) for
koblingen til domænemodellen.

Notation som Larman (s. 250): `visibility name : Type` for attributter og
`visibility Method(param : Type) : ReturnType` for metoder. `+` public, `-` private, `#` protected,
understreget = static, `<<abstract>>` / `<<interface>>` som stereotyper.

```mermaid
---
config:
  layout: dagre
---
classDiagram
    direction TB

    namespace Models {
        class Person {
            <<abstract>>
            +FirstName : string
            +LastName : string
            +FullName : string
        }
        class Student {
            +StudentId : int
            +SchoolClass : SchoolClass?
            +DisplayName : string
        }
        class SchoolClass {
            +SchoolClassId : int
            +Name : string
            +Year : int
        }
        class Team {
            +TeamId : int
            +Name : string
            +Year : int
        }
        class Module {
            +ModuleId : int
            +TeamId : int
            +Date : DateOnly
            +StartTime : TimeOnly
            +EndTime : TimeOnly
            +ModuleNumber : int
        }
        class Attendance {
            +AttendanceId : int
            +StudentId : int
            +ModuleId : int
            +Status : AttendanceStatus
            +AbsencePercent : int?
            +AbsenceType : AbsenceType?
        }
        class AttendanceStatus {
            <<enumeration>>
            Present
            Absent
        }
        class AbsenceType {
            +AbsenceTypeId : int
            +Name : string
            +CountsTowardsAbsence : bool
        }
        class AbsenceSummary {
            +ModuleCount : int
            +AbsentModules : double
            +AbsencePercent : double
        }
    }

    namespace Repositories {
        class IRepository~T~ {
            <<interface>>
            +GetAll() List~T~
            +GetById(id : int) T?
        }
        class IStudentRepository {
            <<interface>>
            +GetByClass(className : string) List~Student~
            +SearchByName(searchText : string) List~Student~
        }
        class IAttendanceRepository {
            <<interface>>
            +GetByStudent(studentId : int) List~Attendance~
        }
        class StudentRepository {
            -_connectionString : string
            +StudentRepository(connectionString : string)
            +GetAll() List~Student~
            +GetById(id : int) Student?
            +GetByClass(className : string) List~Student~
            +SearchByName(searchText : string) List~Student~
            -ReadStudents(command : SqlCommand)$ List~Student~
        }
        class AttendanceRepository {
            -_connectionString : string
            +AttendanceRepository(connectionString : string)
            +GetAll() List~Attendance~
            +GetById(id : int) Attendance?
            +GetByStudent(studentId : int) List~Attendance~
            -ReadAttendance(command : SqlCommand)$ List~Attendance~
        }
    }

    namespace Services {
        class AbsenceService {
            -_attendanceRepository : IAttendanceRepository
            +AbsenceService(attendanceRepository : IAttendanceRepository)
            +GetTotalAbsence(studentId : int) AbsenceSummary
            +Calculate(registrations : List~Attendance~)$ AbsenceSummary
        }
    }

    namespace MVVM {
        class ViewModelBase {
            <<abstract>>
            +PropertyChanged : PropertyChangedEventHandler?
            #OnPropertyChanged(propertyName : string?) void
        }
        class RelayCommand {
            -_execute : Action~object?~
            -_canExecute : Predicate~object?~?
            +RelayCommand(execute : Action~object?~, canExecute : Predicate~object?~?)
            +CanExecute(parameter : object?) bool
            +Execute(parameter : object?) void
        }
        class Navigator {
            -_history : Stack~ViewModelBase~
            +CurrentPage : ViewModelBase
            +CanGoBack : bool
            +BackCommand : ICommand
            +Navigator(startPage : ViewModelBase)
            +NavigateTo(page : ViewModelBase) void
            +NavigateFromMenu(page : ViewModelBase) void
            +GoBack() void
        }
    }

    namespace ViewModels {
        class MainViewModel {
            +Navigator : Navigator
            +ShowHomeCommand : ICommand
            +ShowStudentsCommand : ICommand
            +MainViewModel(studentRepository : IStudentRepository, absenceService : AbsenceService)
        }
        class HomeViewModel
        class StudentListViewModel {
            -_studentRepository : IStudentRepository
            -_absenceService : AbsenceService
            -_navigator : Navigator
            +AllStudents : ObservableCollection~Student~
            +SearchResults : ObservableCollection~Student~
            +SelectedStudent : Student?
            +SelectedSearchResult : Student?
            +SearchText : string
            +IsDropDownOpen : bool
            +ErrorMessage : string
            +StudentListViewModel(studentRepository : IStudentRepository, absenceService : AbsenceService, navigator : Navigator)
            -OpenStudent(student : Student) void
            -Reset() void
            -LoadAllStudents() void
            -Search() void
        }
        class StudentViewModel {
            +Student : Student
            +AbsenceText : string
            +ErrorMessage : string
            +StudentViewModel(student : Student, absenceService : AbsenceService)
            -LoadAbsence(absenceService : AbsenceService) void
        }
    }

    namespace Views {
        class MainWindow {
            <<view>>
        }
        class HomeView {
            <<view>>
        }
        class StudentListView {
            <<view>>
        }
        class StudentView {
            <<view>>
        }
    }

    %% Models – same relations as the domain model
    Person <|-- Student
    Student "*" --> "1" SchoolClass : is in
    Student "*" -- "*" Team : is on
    Team "1" -- "*" Module : has
    Student "1" -- "*" Attendance : has
    Attendance "*" -- "1" Module : registered on
    Attendance "*" --> "0..1" AbsenceType : has

    %% Repositories and service
    IRepository~T~ <|-- IStudentRepository : T = Student
    IRepository~T~ <|-- IAttendanceRepository : T = Attendance
    IStudentRepository <|.. StudentRepository
    IAttendanceRepository <|.. AttendanceRepository
    StudentRepository ..> Student : creates
    AttendanceRepository ..> Attendance : creates
    AbsenceService --> "1" IAttendanceRepository
    AbsenceService ..> AbsenceSummary : creates

    %% MVVM and ViewModels
    ViewModelBase <|-- Navigator
    ViewModelBase <|-- MainViewModel
    ViewModelBase <|-- HomeViewModel
    ViewModelBase <|-- StudentListViewModel
    ViewModelBase <|-- StudentViewModel
    Navigator --> "1" ViewModelBase : CurrentPage
    Navigator --> "*" ViewModelBase : _history
    Navigator --> "1" RelayCommand : BackCommand
    MainViewModel --> "1" Navigator
    MainViewModel --> "2" RelayCommand : menu commands
    MainViewModel ..> HomeViewModel : creates
    MainViewModel ..> StudentListViewModel : creates
    StudentListViewModel --> "1" Navigator
    StudentListViewModel --> "1" IStudentRepository
    StudentListViewModel --> "1" AbsenceService
    StudentListViewModel --> "*" Student
    StudentListViewModel ..> StudentViewModel : creates
    StudentViewModel --> "1" Student
    StudentViewModel ..> AbsenceService : uses

    %% Views (DataContext)
    MainWindow ..> MainViewModel : DataContext
    HomeView ..> HomeViewModel : DataContext
    StudentListView ..> StudentListViewModel : DataContext
    StudentView ..> StudentViewModel : DataContext
```

## Noter

- **Relationerne mellem modelklasserne er de samme som i domænemodellen.** I koden er nogle af dem
  kun implementeret som id-attributter (`Module.TeamId`, `Attendance.StudentId`,
  `Attendance.ModuleId`) og ikke som objektreferencer. Derfor har de ingen navigationspil.
  Elev–Hold findes kun i databasen (koblingstabellen `StudentTeam`).
- **`Student.SchoolClass` er `SchoolClass?` i koden**, men i databasen er `SchoolClassId` NOT NULL.
  Repositoriet sætter den altid, så multipliciteten er 1 som i domænemodellen.
- **Views** har ingen egne medlemmer ud over `InitializeComponent()`. Koblingen til ViewModel sker via
  `DataContext` og DataTemplates i `App.xaml`.
- `MainViewModel` opretter `HomeViewModel` og `StudentListViewModel` og giver dem til `Navigator`
  via menukommandoerne, men gemmer dem ikke selv i felter.
