# Book Analysis Application

## Projekt áttekintés

Ez a Book Analysis Application egy .NET Core alapú backend rendszer, amely lehetővé teszi könyvek feltöltését, elemzését és nyelvtanulási célú feldolgozását. A rendszer elsődleges célja a könyvekben előforduló kifejezések gyakoriságának elemzése, valamint a nyelvtanulást segítő funkciók biztosítása.

## Projekt struktúra

A projekt több rétegből áll, amelyek különböző felelősségi körökkel rendelkeznek:

### 1. BookAnalysisApp.Entities

Az alapvető adatmodelleket tartalmazza:

- **Book**: Egy könyvet reprezentál (Id, Title, Content, CreatedAt)
- **EnglishPhrase**: Angol kifejezéseket tárol (Id, Phrase)
- **BookPhrase**: Kapcsolótábla könyvek és kifejezések között, amely tárolja a kifejezések gyakoriságát egy adott könyvben
- **EnglishHungarianPhrase**: Angol-magyar kifejezéspárok tárolására szolgál
- **WordFrequency**: Szógyakoriság elemzések eredményeinek tárolására

### 2. BookAnalysisApp.Data

Adatbázis-kezelés és adathozzáférési réteg:

- **ApplicationDbContext**: Entity Framework DbContext az adatbázis műveletek végrehajtásához
- **BookEditor**: Könyvszerkesztési funkciók (pl. szöveg tisztítása, kisbetűsítés)
- **DatabaseSeeder**: Kezdeti adatok betöltése az adatbázisba
- **AppUser**: Felhasználói adatmodell az Identity rendszerhez

### 3. BookAnalysisApp.Logic

Üzleti logikát tartalmazó réteg.

### 4. BookAnalysisApp.Endpoint

REST API végpontok és a projekt belépési pontja:

- **Program.cs**: Alkalmazás konfigurációja, szolgáltatások regisztrálása, middleware beállítások
- **Controllers**: API végpontokat definiáló vezérlők

#### Fontosabb Controller-ek:
- **BooksController**: Könyvek feltöltése, lekérése
- **PhraseAnalysisController**: Kifejezések elemzése könyvekben, gyakoriság számítás
- **PhraseStorageController**: Kifejezések tárolása, kezelése
- **DictionaryController**: Szótár funkciók
- **ExportController**: Adatok exportálása
- **UserController**: Felhasználókezelés (regisztráció, bejelentkezés)
- **WordsController**: Szógyakorisági elemzések

### 5. BookUploaderConsoleApp

Külön konzolos alkalmazás a könyvek feltöltésére.

## Főbb funkciók

### 1. Könyvkezelés
- Könyvek feltöltése és tárolása
- Könyvek szerkesztése (speciális karakterek eltávolítása, kisbetűsítés)
- Könyvlista és könyvcímek lekérdezése

### 2. Kifejezéselemzés
- Könyvekben található kifejezések gyakoriságának elemzése
- Párhuzamos feldolgozás a teljesítmény optimalizálásáért
- Leggyakoribb kifejezések rangsorolása

### 3. Kifejezéstárolás
- Angol-magyar kifejezéspárok tárolása és kezelése
- Kifejezések hozzáadása, frissítése, törlése

### 4. Felhasználókezelés
- Regisztráció és bejelentkezés JWT token hitelesítéssel
- Felhasználói adatok kezelése

### 5. Export funkciók
- Elemzési eredmények és szótárak exportálása

## Technikai részletek

- **.NET Core**: A projekt .NET Core keretrendszerre épül
- **Entity Framework Core**: ORM réteg az adatbázis-műveletekhez
- **Identity**: Felhasználókezelés és hitelesítés
- **JWT Authentication**: Token alapú hitelesítés
- **Swagger/OpenAPI**: API dokumentáció
- **In-Memory Database**: Fejlesztési célú adatbázis (éles környezetben SQL Server-re cserélhető)

## Frontend fejlesztéshez

A backend API végpontokat kínál, amelyek könnyen felhasználhatók egy Angular vagy más frontend keretrendszer által. A legfontosabb végpontok:

- `POST /api/Books/upload`: Új könyv feltöltése
- `GET /api/Books/list`: Könyvek listázása
- `GET /api/Books/GetBookTitles`: Könyvcímek listázása
- `GET /api/PhraseAnalysis/analyze/{bookId}`: Könyv kifejezéseinek elemzése
- `GET /api/PhraseStorage/list`: Tárolt kifejezések listázása
- `POST /api/User/register`: Felhasználó regisztrálása
- `POST /api/User/login`: Bejelentkezés

