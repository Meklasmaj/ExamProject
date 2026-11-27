# Sekvensdiagrammer (design)

LLD-artefakter, derfor på engelsk med de faktiske klasse- og metodenavne. Der er ét diagram pr.
systemoperation fra SSD'et (se [Ordliste.md](Ordliste.md)):

| Systemoperation (SSD) | Sekvensdiagram |
|---|---|
| visElever() | [SD1 – ShowStudents](#sd1--showstudents) |
| søgElev(tekst) | [SD2 – SearchStudent](#sd2--searchstudent) |
| vælgElev(elev) | [SD3 – SelectStudent](#sd3--selectstudent) |
| gåTilbage() | [SD4 – GoBack](#sd4--goback) |

Alle deltagere findes i [DCD.md](DCD.md). `Database` er SQL Server-databasen `GymnasieProjektDB`.
Sætninger som `SearchText = "frej"` er WPF-bindinger, der kalder property-setteren.

## SD1 – ShowStudents

**Start:** Brugeren klikker på "Elever" i menuen. **Slut:** Elevlisten vises, og historikken er tom.

```mermaid
sequenceDiagram
    actor User
    participant MW as : MainWindow
    participant Cmd as ShowStudentsCommand : RelayCommand
    participant Nav as : Navigator

    User->>+MW: click "Elever"
    MW->>+Cmd: Execute(null)
    Cmd->>+Nav: NavigateFromMenu(studentList)
    Nav->>Nav: _history.Clear()
    Nav->>Nav: CurrentPage = studentList
    Note over MW,Nav: PropertyChanged → ContentControl in MainWindow shows StudentListView
    Nav-->>-Cmd: void
    Cmd-->>-MW: void
    MW-->>-User: list of all students
```

## SD2 – SearchStudent

**Start:** Brugeren skriver "frej" i søgefeltet. **Slut:** Dropdownen viser de elever, hvis navn
indeholder teksten.

```mermaid
sequenceDiagram
    actor User
    participant LV as : StudentListView
    participant LVM as studentList : StudentListViewModel
    participant SR as : StudentRepository
    participant DB as Database

    User->>+LV: type "frej" in search box
    LV->>+LVM: SearchText = "frej"
    LVM->>+LVM: Search()
    LVM->>LVM: SearchResults.Clear()
    alt SearchText is empty
        LVM->>LVM: IsDropDownOpen = false
    else SearchText has text
        LVM->>+SR: SearchByName("frej")
        SR->>+DB: SELECT ... WHERE s.FirstName + ' ' + s.LastName LIKE '%frej%'
        DB-->>-SR: rows
        SR-->>-LVM: List<Student>
        loop for each student
            LVM->>LVM: SearchResults.Add(student)
        end
        LVM->>LVM: IsDropDownOpen = SearchResults.Count > 0
    end
    LVM-->>-LVM: void
    LVM-->>-LV: void
    LV-->>-User: dropdown with "Freja Larsen (2.b)"
```

## SD3 – SelectStudent

**Start:** Brugeren klikker på "Emma Andersen (2.b)" i elevlisten. **Slut:** Emmas side vises med
hendes samlede fravær, og elevlisten er nulstillet.

Vælges eleven i stedet i søgedropdownen, sætter bindingen `SelectedSearchResult`. Setteren lukker
dropdownen (`IsDropDownOpen = false`) og kalder derefter `OpenStudent(student)`, så resten er det samme.

```mermaid
sequenceDiagram
    actor User
    participant LV as : StudentListView
    participant LVM as studentList : StudentListViewModel
    participant StVM as emmaPage : StudentViewModel
    participant AS as : AbsenceService
    participant AR as : AttendanceRepository
    participant DB as Database
    participant Nav as : Navigator

    User->>+LV: click "Emma Andersen (2.b)"
    LV->>+LVM: SelectedStudent = emma
    LVM->>+LVM: OpenStudent(emma)
    LVM->>+StVM: new StudentViewModel(emma, absenceService)
    StVM->>+StVM: LoadAbsence(absenceService)
    StVM->>+AS: GetTotalAbsence(emma.StudentId)
    AS->>+AR: GetByStudent(studentId)
    AR->>+DB: SELECT ... FROM Attendance a LEFT JOIN AbsenceType t ... WHERE a.StudentId = @StudentId
    DB-->>-AR: rows
    AR-->>-AS: List<Attendance>
    AS->>+AS: Calculate(registrations)
    AS-->>-AS: AbsenceSummary
    AS-->>-StVM: AbsenceSummary
    StVM->>StVM: AbsenceText = "Samlet fravær: 6,3 % (17,5 af 280 moduler)"
    StVM-->>-StVM: void
    StVM-->>-LVM: emmaPage
    LVM->>+Nav: NavigateTo(emmaPage)
    Nav->>Nav: _history.Push(CurrentPage)
    Nav->>Nav: CurrentPage = emmaPage
    Note over Nav: PropertyChanged → ContentControl in MainWindow shows StudentView
    Nav-->>-LVM: void
    LVM->>+LVM: Reset()
    LVM->>LVM: SelectedStudent = null
    LVM->>LVM: SearchText = ""
    LVM-->>-LVM: void
    LVM-->>-LVM: void
    LVM-->>-LV: void
    LV-->>-User: Emma's page with total absence
```

## SD4 – GoBack

**Start:** Brugeren klikker på "← Tilbage" på en elevs side. **Slut:** Den forrige side (elevlisten)
vises igen.

```mermaid
sequenceDiagram
    actor User
    participant MW as : MainWindow
    participant Cmd as BackCommand : RelayCommand
    participant Nav as : Navigator

    User->>+MW: click "← Tilbage"
    MW->>+Cmd: Execute(null)
    Cmd->>+Nav: GoBack()
    opt CanGoBack
        Nav->>Nav: CurrentPage = _history.Pop()
        Note over MW,Nav: PropertyChanged → ContentControl in MainWindow shows the previous page
    end
    Nav-->>-Cmd: void
    Cmd-->>-MW: void
    MW-->>-User: student list
```

Knappen er grå, når `CanGoBack` er false (`RelayCommand.CanExecute`), så `opt` er normalt sand.
