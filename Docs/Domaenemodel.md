# Domænemodel

HLD-artefakt, derfor på dansk.

Modellen dækker de begreber, Elever-delen af appen bruger. Medarbejder, Rolle, Fag og Uddannelse
fra den oprindelige domænemodel er ikke med endnu.

```mermaid
classDiagram
    direction LR

    class Person {
        <<abstract>>
        fornavn
        efternavn
    }

    class Elev

    class Klasse {
        navn
        årgang
    }

    class Hold {
        navn
        år
    }

    class Modul {
        dato
        starttid
        sluttid
        modulnummer
    }

    class Tilstedevaerelse["Tilstedeværelse"] {
        status [til stede / fraværende]
        fraværsandel [0..1]
    }

    class Fravaerstype["Fraværstype"] {
        navn
        medregnes [ja/nej]
    }

    Person <|-- Elev
    Elev "*" -- "1" Klasse : går i ▶
    Elev "*" -- "*" Hold : går på ▶
    Hold "1" -- "*" Modul : har ▶
    Elev "1" -- "*" Tilstedevaerelse : har ▶
    Tilstedevaerelse "*" -- "1" Modul : registreres på ▶
    Tilstedevaerelse "*" -- "0..1" Fravaerstype : har ▶
```

## Læsevejledning

- ▶ viser læseretningen: "Elev går i Klasse", "Tilstedeværelse registreres på Modul".
- **fraværsandel [0..1]** kan være tom. Er den tom ved fravær, tæller fraværet som et helt modul (100 %).
- **status** er enten *til stede* eller *fraværende*. En fraværstype findes kun ved fravær, derfor 0..1.
- **medregnes** angiver, om fravær af denne type tæller med i elevens samlede fravær
  (fx tæller skolerelateret aktivitet ikke med).

## Afvigelser fra den oprindelige domænemodel

- En elev går i præcis **1** klasse (den oprindelige model tillod 1..*).
