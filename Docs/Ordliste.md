# Ordliste – dansk til engelsk

HLD-artefakter (use cases, domænemodel, SSD, operationskontrakter) skrives på **dansk**.
LLD-artefakter (DCD, sekvensdiagrammer, ER-diagram, kode og database) skrives på **engelsk**.
Tabellen viser, hvilket engelsk navn hvert domænebegreb har i koden og i databasen.

## Begreber (klasser og tabeller)

| Dansk (HLD) | Engelsk klasse (kode) | Tabel (database) | Forklaring |
|---|---|---|---|
| Person | `Person` | – (felterne ligger i `Student`) | Abstrakt fællesbegreb for personer på skolen |
| Elev | `Student` | `Student` | En elev på gymnasiet |
| Klasse | `SchoolClass` | `SchoolClass` | Elevens stamklasse, fx 1.a |
| Hold | `Team` | `Team` | Et undervisningshold, fx "1.a Dansk" |
| Elev går på hold | – | `StudentTeam` | Koblingstabel mellem elev og hold |
| Modul | `Module` | `Module` | Én lektion for et hold på en bestemt dato |
| Tilstedeværelse | `Attendance` | `Attendance` | En elevs registrering på ét modul |
| Status (til stede / fraværende) | `AttendanceStatus` (`Present` / `Absent`) | `Attendance.Status` | Om eleven var der |
| Fraværstype | `AbsenceType` | `AbsenceType` | Fx sygdom eller skolerelateret aktivitet |
| Samlet fravær | `AbsenceSummary` | – (beregnes) | Resultatet af fraværsberegningen |

## Attributter

| Dansk (HLD) | Engelsk (kode / database) |
|---|---|
| fornavn | `FirstName` |
| efternavn | `LastName` |
| navn | `Name` |
| årgang (klasse) | `Year` |
| år (hold) | `Year` |
| dato | `Date` |
| starttid | `StartTime` |
| sluttid | `EndTime` |
| modulnummer | `ModuleNumber` |
| status | `Status` |
| fraværsandel | `AbsencePercent` |
| medregnes | `CountsTowardsAbsence` |

## Systemoperationer (SSD → sekvensdiagrammer)

| Dansk (SSD) | Engelsk (sekvensdiagram / kode) |
|---|---|
| visElever() | `ShowStudentsCommand` → `Navigator.NavigateFromMenu(...)` |
| søgElev(tekst) | `StudentListViewModel.SearchText` → `Search()` |
| vælgElev(elev) | `StudentListViewModel.SelectedStudent` → `OpenStudent(...)` |
| gåTilbage() | `Navigator.BackCommand` → `GoBack()` |
