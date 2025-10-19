# 💰 BudgetTrack

Ett enkelt C#-program för att hantera inkomster och utgifter i en personlig budget.  
Användaren kan lägga till, visa, filtrera och ta bort transaktioner samt se aktuell balans.

---

## 🚀 Hur man kör programmet

1. **Klona projektet eller ladda ner det som ZIP**
    
    ```bash
    git clone https://github.com/Andersson76/BudgetTrack.git
    ```

2. **Navigera till projektmappen**
    
    ```bash
    cd BudgetTrack
    ```

3. **Kör programmet**
    
    ```bash
    dotnet run
    ```

4. **Följ instruktionerna i konsolen för att**
    - Lägga till transaktioner (beskrivning, belopp, kategori, datum)
    - Visa alla transaktioner
    - Filtrera på kategori
    - Ta bort transaktioner
    - Visa total balans

---

## Utvecklare

**Namn:** Martin Andersson  
**Datum:** 19 oktober 2025

---

## Reflektionsfrågor

** Hur hjälpte klasser och metoder dig att organisera programmet? **  
Klasser och metoder gjorde det mycket enklare att hålla koden strukturerad.  
Genom att lägga logiken i `BudgetManager` och datan i `Transaction` kunde jag separera ansvar, återanvända kod och lättare förstå hur allt hänger ihop.  
Metoderna hjälpte också till att göra koden mer läsbar och lätt att testa.

---

** Vilken del av projektet var mest utmanande? **  
Den mest utmanande delen var att få all funktionalitet att samspela korrekt, särskilt att hantera listan av transaktioner och kontrollera att borttagning och summering fungerade som tänkt.  
Det tog lite tid att felsöka logiken, men det gav samtidigt en bättre förståelse för hur objekt och metoder interagerar.

---
