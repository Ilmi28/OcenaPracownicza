namespace OcenaPracownicza.API.Entities
{
    public class AchievementInfo
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Unit { get; set; } = null!;
        public string Area { get; set; } = null!;
        public decimal BasePoints { get; set; }
        public string ScoreType { get; set; } = "Fixed";
    }

    public static class AchievementDictionary
    {
        public static readonly Dictionary<int, AchievementInfo> Map = new()
        {
            // =========================================================================================
            // A. DZIAŁ SPRAW PERSONALNYCH
            // =========================================================================================
            { 1001, new AchievementInfo { Code = "A.1/V.10", Name = "Członkostwo w zespołach eksperckich (za każdy rok)", Unit = "Dział Spraw Personalnych", Area = "Scientific", BasePoints = 8, ScoreType = "Multiplied" } },
            { 1002, new AchievementInfo { Code = "A.1/V.11", Name = "Działalność w zespołach i panelach instytucji centralnych (rocznie)", Unit = "Dział Spraw Personalnych", Area = "Scientific", BasePoints = 13, ScoreType = "Multiplied" } },

            { 1003, new AchievementInfo { Code = "A.2/II.12", Name = "Uzyskanie certyfikatu z języka obcego na poziomie wyższym niż B2", Unit = "Dział Spraw Personalnych", Area = "Didactic", BasePoints = 8, ScoreType = "Fixed" } },
            { 1004, new AchievementInfo { Code = "A.2/IV.1", Name = "Pełnienie funkcji prorektora / dziekana / dyrektora / kierownika", Unit = "Dział Spraw Personalnych", Area = "Didactic", BasePoints = 32, ScoreType = "Variable" } },
            { 1005, new AchievementInfo { Code = "A.2/IV.2", Name = "Pełnienie funkcji pełnomocnika rektora lub dziekana", Unit = "Dział Spraw Personalnych", Area = "Didactic", BasePoints = 8, ScoreType = "Variable" } },
            { 1006, new AchievementInfo { Code = "A.2/IV.3", Name = "Pełnienie funkcji kierownika studiów doktoranckich / Dyrektora Szkoły Doktorskiej", Unit = "Dział Spraw Personalnych", Area = "Didactic", BasePoints = 15, ScoreType = "Variable" } },
            { 1007, new AchievementInfo { Code = "A.2/IV.4", Name = "Udział we władzach centralnych towarzystw naukowych / organizacji", Unit = "Dział Spraw Personalnych", Area = "Didactic", BasePoints = 16, ScoreType = "Variable" } },
            { 1008, new AchievementInfo { Code = "A.2/IV.5", Name = "Członkostwo w organach kolegialnych Uczelni / wydziału", Unit = "Dział Spraw Personalnych", Area = "Didactic", BasePoints = 4, ScoreType = "Fixed" } },
            { 1009, new AchievementInfo { Code = "A.2/IV.6", Name = "Członkostwo w zespołach i komisjach rektorskich / fundacjach Uczelni", Unit = "Dział Spraw Personalnych", Area = "Didactic", BasePoints = 4, ScoreType = "Fixed" } },
            { 1010, new AchievementInfo { Code = "A.2/IV.7", Name = "Uczestnictwo w Komisji ds. Jakości Kształcenia", Unit = "Dział Spraw Personalnych", Area = "Didactic", BasePoints = 12, ScoreType = "Variable" } },
            { 1011, new AchievementInfo { Code = "A.2/IV.8", Name = "Członkostwo w komisji rekrutacyjnej (za każdą rekrutację)", Unit = "Dział Spraw Personalnych", Area = "Didactic", BasePoints = 8, ScoreType = "Variable" } },
            { 1012, new AchievementInfo { Code = "A.2/IV.9", Name = "Przewodniczenie organom i komisjom", Unit = "Dział Spraw Personalnych", Area = "Didactic", BasePoints = 4, ScoreType = "Fixed" } },
            { 1013, new AchievementInfo { Code = "A.2/IV.13", Name = "Członkostwo we władzach zagranicznych towarzystw naukowych (min. 10 państw)", Unit = "Dział Spraw Personalnych", Area = "Didactic", BasePoints = 8, ScoreType = "Fixed" } },
            { 1014, new AchievementInfo { Code = "A.2/IV.14", Name = "Członkostwo w PAN / komitecie / ekspert", Unit = "Dział Spraw Personalnych", Area = "Didactic", BasePoints = 30, ScoreType = "Variable" } },
            { 1015, new AchievementInfo { Code = "A.2/IV.15", Name = "Działalność w PKA, Radzie Nauki, RGNiSW", Unit = "Dział Spraw Personalnych", Area = "Didactic", BasePoints = 25, ScoreType = "Variable" } },
            { 1016, new AchievementInfo { Code = "A.2/V.1", Name = "Nagroda prezydenta, premiera, ministra, marszałka", Unit = "Dział Spraw Personalnych", Area = "Didactic", BasePoints = 50, ScoreType = "Variable" } },
            { 1017, new AchievementInfo { Code = "A.2/V.4", Name = "Uzyskanie innej nagrody (międzynarodowej/krajowej/regionalnej)", Unit = "Dział Spraw Personalnych", Area = "Didactic", BasePoints = 40, ScoreType = "Variable" } },
            { 1018, new AchievementInfo { Code = "A.2/V.5", Name = "Nagroda rektora (indywidualna/zespołowa)", Unit = "Dział Spraw Personalnych", Area = "Didactic", BasePoints = 12, ScoreType = "Variable" } },

            // =========================================================================================
            // B. DZIAŁ NAUKI
            // =========================================================================================
            { 2001, new AchievementInfo { Code = "B.1/I.1", Name = "Uzyskanie tytułu profesora", Unit = "Dział Nauki", Area = "Scientific", BasePoints = 160, ScoreType = "Fixed" } },
            { 2002, new AchievementInfo { Code = "B.1/I.2", Name = "Uzyskanie stopnia doktora habilitowanego", Unit = "Dział Nauki", Area = "Scientific", BasePoints = 120, ScoreType = "Fixed" } },
            { 2003, new AchievementInfo { Code = "B.1/I.3", Name = "Uzyskanie stopnia doktora", Unit = "Dział Nauki", Area = "Scientific", BasePoints = 60, ScoreType = "Fixed" } },
            { 2004, new AchievementInfo { Code = "B.1/I.4", Name = "Pełnienie funkcji promotora w postępowaniu o nadanie stopnia doktora", Unit = "Dział Nauki", Area = "Scientific", BasePoints = 40, ScoreType = "Variable" } },
            { 2005, new AchievementInfo { Code = "B.1/I.5", Name = "Pełnienie funkcji promotora pomocniczego", Unit = "Dział Nauki", Area = "Scientific", BasePoints = 12, ScoreType = "Variable" } },

            { 2006, new AchievementInfo { Code = "B.1/III", Name = "Projekty badawcze i rozwojowe finansowane w trybie konkursowym (Wniosek/Uzyskanie/Udział)", Unit = "Dział Nauki", Area = "Scientific", BasePoints = 0, ScoreType = "Formula" } },

            { 2007, new AchievementInfo { Code = "B.1/V.1", Name = "Opracowanie recenzji w postępowaniu habilitacyjnym/profesorskim", Unit = "Dział Nauki", Area = "Scientific", BasePoints = 16, ScoreType = "Fixed" } },
            { 2008, new AchievementInfo { Code = "B.1/V.2", Name = "Udział w komisji habilitacyjnej (przewodniczący/sekretarz/członek/recenzent)", Unit = "Dział Nauki", Area = "Scientific", BasePoints = 8, ScoreType = "Variable" } },
            { 2009, new AchievementInfo { Code = "B.1/V.3", Name = "Recenzja wydawnicza monografii lub pracy doktorskiej", Unit = "Dział Nauki", Area = "Scientific", BasePoints = 12, ScoreType = "Fixed" } },
            { 2010, new AchievementInfo { Code = "B.1/V.4", Name = "Opracowanie recenzji artykułu z Impact Factor", Unit = "Dział Nauki", Area = "Scientific", BasePoints = 1, ScoreType = "Multiplied" } },
            { 2011, new AchievementInfo { Code = "B.1/V.5", Name = "Recenzja projektu w konkursach krajowych / międzynarodowych", Unit = "Dział Nauki", Area = "Scientific", BasePoints = 4, ScoreType = "Variable" } },
            { 2012, new AchievementInfo { Code = "B.1/V.7", Name = "Członkostwo w komitecie redakcyjnym czasopisma MNiSW (za rok)", Unit = "Dział Nauki", Area = "Scientific", BasePoints = 4, ScoreType = "Multiplied" } },
            { 2013, new AchievementInfo { Code = "B.1/V.8", Name = "Redaktor naukowy wydawnictw (za rok)", Unit = "Dział Nauki", Area = "Scientific", BasePoints = 5, ScoreType = "Multiplied" } },
            { 2014, new AchievementInfo { Code = "B.1/V.9", Name = "Sekretarz naukowy czasopisma w Uczelni (za numer)", Unit = "Dział Nauki", Area = "Scientific", BasePoints = 4, ScoreType = "Multiplied" } },

            { 2015, new AchievementInfo { Code = "B.2/VI.1", Name = "Organizacja międzynarodowej konferencji naukowej", Unit = "Dział Nauki", Area = "Didactic", BasePoints = 30, ScoreType = "Variable" } },
            { 2016, new AchievementInfo { Code = "B.2/VI.2", Name = "Organizacja krajowej konferencji naukowej", Unit = "Dział Nauki", Area = "Didactic", BasePoints = 16, ScoreType = "Variable" } },
            { 2017, new AchievementInfo { Code = "B.2/VI.4", Name = "Przewodniczący komitetu naukowego konferencji", Unit = "Dział Nauki", Area = "Didactic", BasePoints = 20, ScoreType = "Variable" } },
            { 2018, new AchievementInfo { Code = "B.2/VIII.1", Name = "Opracowanie recenzji wydawniczej skryptu lub podręcznika", Unit = "Dział Nauki", Area = "Didactic", BasePoints = 8, ScoreType = "Fixed" } },

            // =========================================================================================
            // C. ODDZIAŁ INFORMACJI NAUKOWEJ BIBLIOTEKI PB
            // =========================================================================================
            { 3001, new AchievementInfo { Code = "C.1/II.1", Name = "Publikacje artykułów i monografii naukowych (wg wykazu MEiN)", Unit = "Biblioteka PB", Area = "Scientific", BasePoints = 0, ScoreType = "Formula" } },

            { 3002, new AchievementInfo { Code = "C.1/IV.1", Name = "Indywidualna autorska wystawa artystyczna", Unit = "Biblioteka PB", Area = "Scientific", BasePoints = 200, ScoreType = "Variable" } },
            { 3003, new AchievementInfo { Code = "C.1/IV.2", Name = "Autorstwo dzieła plastycznego / projektu artystycznego", Unit = "Biblioteka PB", Area = "Scientific", BasePoints = 200, ScoreType = "Variable" } },
            { 3004, new AchievementInfo { Code = "C.1/IV.3", Name = "Udział w wystawie zbiorowej / galeryjnej", Unit = "Biblioteka PB", Area = "Scientific", BasePoints = 200, ScoreType = "Variable" } },
            { 3005, new AchievementInfo { Code = "C.1/IV.4", Name = "Udział w jury konkursu / funkcja kuratora wystawy", Unit = "Biblioteka PB", Area = "Scientific", BasePoints = 200, ScoreType = "Variable" } },
            { 3006, new AchievementInfo { Code = "C.1/IV.5", Name = "Autorstwo publikacji z zakresu sztuk plastycznych", Unit = "Biblioteka PB", Area = "Scientific", BasePoints = 200, ScoreType = "Variable" } },
            { 3007, new AchievementInfo { Code = "C.1/IV.6", Name = "Redakcja publikacji z zakresu sztuk plastycznych", Unit = "Biblioteka PB", Area = "Scientific", BasePoints = 100, ScoreType = "Variable" } },
            { 3008, new AchievementInfo { Code = "C.1/IV.7", Name = "Autorstwo rozdziału w publikacji z zakresu sztuk plastycznych", Unit = "Biblioteka PB", Area = "Scientific", BasePoints = 50, ScoreType = "Variable" } },

            { 3009, new AchievementInfo { Code = "C.1/V.6", Name = "Redaktor naczelny / zastępca czasopisma (za rok)", Unit = "Biblioteka PB", Area = "Scientific", BasePoints = 16, ScoreType = "Variable" } },
            { 3010, new AchievementInfo { Code = "C.1/V.12", Name = "Index Hirscha (Scopus)", Unit = "Biblioteka PB", Area = "Scientific", BasePoints = 0, ScoreType = "Formula" } },

            { 3011, new AchievementInfo { Code = "C.2/I.1", Name = "Autorstwo podręcznika akademickiego", Unit = "Biblioteka PB", Area = "Didactic", BasePoints = 80, ScoreType = "Fixed" } },
            { 3012, new AchievementInfo { Code = "C.2/I.2", Name = "Autorstwo skryptu", Unit = "Biblioteka PB", Area = "Didactic", BasePoints = 60, ScoreType = "Fixed" } },
            { 3013, new AchievementInfo { Code = "C.2/I.3", Name = "Tłumaczenie podręcznika akademickiego / skryptu", Unit = "Biblioteka PB", Area = "Didactic", BasePoints = 24, ScoreType = "Variable" } },
            { 3014, new AchievementInfo { Code = "C.2/I.4", Name = "Publikacje dydaktyczne / ze studentami", Unit = "Biblioteka PB", Area = "Didactic", BasePoints = 0, ScoreType = "Formula" } },

            // =========================================================================================
            // D. BIURO DS. ROZWOJU I PROGRAMÓW MIĘDZYNARODOWYCH
            // =========================================================================================
            { 4001, new AchievementInfo { Code = "D.1/III", Name = "Projekty obejmujące badania naukowe (Złożenie/Uzyskanie/Realizacja)", Unit = "Biuro ds. Rozwoju", Area = "Scientific", BasePoints = 0, ScoreType = "Formula" } },

            { 4002, new AchievementInfo { Code = "D.2/II.1", Name = "Projekty dydaktyczne i inwestycyjne (Wniosek/Uzyskanie/Udział)", Unit = "Biuro ds. Rozwoju", Area = "Didactic", BasePoints = 0, ScoreType = "Formula" } },
            { 4003, new AchievementInfo { Code = "D.2/II.2", Name = "Koordynator / członek zespołu w projekcie dydaktycznym (za miesiąc)", Unit = "Biuro ds. Rozwoju", Area = "Didactic", BasePoints = 4, ScoreType = "Variable" } },

            // =========================================================================================
            // E. OŚRODEK WŁASNOŚCI INTELEKTUALNEJ
            // =========================================================================================
            { 5001, new AchievementInfo { Code = "E.1/II.2.1", Name = "Patenty na wynalazki (krajowe/europejskie)", Unit = "Ośrodek Własności Intelektualnej", Area = "Scientific", BasePoints = 0, ScoreType = "Formula" } },
            { 5002, new AchievementInfo { Code = "E.1/II.2.2", Name = "Wyłączne prawa hodowcy do odmiany roślin", Unit = "Ośrodek Własności Intelektualnej", Area = "Scientific", BasePoints = 0, ScoreType = "Formula" } },
            { 5003, new AchievementInfo { Code = "E.1/II.2.3", Name = "Prawa ochronne na wzór użytkowy", Unit = "Ośrodek Własności Intelektualnej", Area = "Scientific", BasePoints = 0, ScoreType = "Formula" } },
            { 5004, new AchievementInfo { Code = "E.1/II.2.4", Name = "Zgłoszenie wynalazku za granicą lub w UP RP", Unit = "Ośrodek Własności Intelektualnej", Area = "Scientific", BasePoints = 10, ScoreType = "Variable" } },

            // =========================================================================================
            // F. BIURO DS. WSPÓŁPRACY MIĘDZYNARODOWEJ
            // =========================================================================================
            { 6001, new AchievementInfo { Code = "F.1/III", Name = "Projekty badawcze - współpraca międzynarodowa (Złożenie/Realizacja)", Unit = "Biuro Współpracy Międzynarodowej", Area = "Scientific", BasePoints = 0, ScoreType = "Formula" } },

            { 6002, new AchievementInfo { Code = "F.2/II.1", Name = "Projekty dydaktyczne międzynarodowe", Unit = "Biuro Współpracy Międzynarodowej", Area = "Didactic", BasePoints = 0, ScoreType = "Formula" } },
            { 6003, new AchievementInfo { Code = "F.2/II.2", Name = "Koordynator w międzynarodowym projekcie dydaktycznym (za miesiąc)", Unit = "Biuro Współpracy Międzynarodowej", Area = "Didactic", BasePoints = 4, ScoreType = "Variable" } },
            { 6004, new AchievementInfo { Code = "F.2/III.1", Name = "Zajęcia dla studentów w ośrodku zagranicznym (np. Erasmus+)", Unit = "Biuro Współpracy Międzynarodowej", Area = "Didactic", BasePoints = 1, ScoreType = "Multiplied" } },
            { 6005, new AchievementInfo { Code = "F.2/III.2", Name = "Prowadzenie zajęć w języku obcym w Uczelni", Unit = "Biuro Współpracy Międzynarodowej", Area = "Didactic", BasePoints = 1, ScoreType = "Multiplied" } },
            { 6006, new AchievementInfo { Code = "F.2/VI.6", Name = "Wydziałowy koordynator Erasmus+ (za każdych 10 studentów rocznie)", Unit = "Biuro Współpracy Międzynarodowej", Area = "Didactic", BasePoints = 8, ScoreType = "Multiplied" } },
            { 6007, new AchievementInfo { Code = "F.2/VI.7", Name = "Koordynator Erasmus+ w SJO (rocznie)", Unit = "Biuro Współpracy Międzynarodowej", Area = "Didactic", BasePoints = 8, ScoreType = "Fixed" } },
            { 6008, new AchievementInfo { Code = "F.2/VI.9", Name = "Kierownik międzynarodowej umowy o współpracy naukowej (za rok)", Unit = "Biuro Współpracy Międzynarodowej", Area = "Didactic", BasePoints = 8, ScoreType = "Multiplied" } },

            // =========================================================================================
            // G. CENTRUM SPRAW STUDENCKICH, DYDAKTYKI I REKRUTACJI
            // =========================================================================================
            { 7001, new AchievementInfo { Code = "G.1/II.14", Name = "Promotorstwo obronionej pracy dyplomowej", Unit = "CSSDiR", Area = "Didactic", BasePoints = 3, ScoreType = "Multiplied" } },
            { 7002, new AchievementInfo { Code = "G.1/IV.10", Name = "Opiekun roku / sekcji / koła naukowego / zespołu dydaktycznego", Unit = "CSSDiR", Area = "Didactic", BasePoints = 8, ScoreType = "Variable" } },
            { 7003, new AchievementInfo { Code = "G.1/IV.11", Name = "Koordynator przedmiotu", Unit = "CSSDiR", Area = "Didactic", BasePoints = 4, ScoreType = "Variable" } },
            { 7004, new AchievementInfo { Code = "G.1/IV.12", Name = "Członkostwo w komisjach doskonalących proces dydaktyczny", Unit = "CSSDiR", Area = "Didactic", BasePoints = 4, ScoreType = "Fixed" } },
            { 7005, new AchievementInfo { Code = "G.1/V.2", Name = "Promotorstwo wyróżnionych prac dyplomowych", Unit = "CSSDiR", Area = "Didactic", BasePoints = 25, ScoreType = "Variable" } },
            { 7006, new AchievementInfo { Code = "G.1/V.3", Name = "Osiągnięcia studenckiego koła naukowego", Unit = "CSSDiR", Area = "Didactic", BasePoints = 25, ScoreType = "Variable" } },
            { 7007, new AchievementInfo { Code = "G.1/VI.1", Name = "Organizacja międzynarodowej konferencji dydaktycznej", Unit = "CSSDiR", Area = "Didactic", BasePoints = 30, ScoreType = "Variable" } },
            { 7008, new AchievementInfo { Code = "G.1/VI.2", Name = "Organizacja krajowej konferencji dydaktycznej", Unit = "CSSDiR", Area = "Didactic", BasePoints = 16, ScoreType = "Variable" } },
            { 7009, new AchievementInfo { Code = "G.1/VI.3", Name = "Organizacja konferencji studenckiej", Unit = "CSSDiR", Area = "Didactic", BasePoints = 12, ScoreType = "Variable" } },
            { 7010, new AchievementInfo { Code = "G.1/VI.5", Name = "Opiekun wyprawy / wymiany studenckiej (za dzień)", Unit = "CSSDiR", Area = "Didactic", BasePoints = 4, ScoreType = "Multiplied" } },
            { 7011, new AchievementInfo { Code = "G.1/VI.8", Name = "Organizacja wycieczek / konkursów / egzaminów zewnętrznych", Unit = "CSSDiR", Area = "Didactic", BasePoints = 4, ScoreType = "Variable" } },
            { 7012, new AchievementInfo { Code = "G.1/VIII.2", Name = "Opracowanie recenzji pracy dyplomowej (max 12/rok)", Unit = "CSSDiR", Area = "Didactic", BasePoints = 1, ScoreType = "Multiplied" } },

            // =========================================================================================
            // H. DZIAŁ JAKOŚCI KSZTAŁCENIA
            // =========================================================================================
            { 8001, new AchievementInfo { Code = "H.1/II.3", Name = "Autorstwo programu nowego przedmiotu", Unit = "Dział Jakości Kształcenia", Area = "Didactic", BasePoints = 8, ScoreType = "Fixed" } },
            { 8002, new AchievementInfo { Code = "H.1/II.4", Name = "Autorstwo materiałów dydaktycznych (za pakiet)", Unit = "Dział Jakości Kształcenia", Area = "Didactic", BasePoints = 8, ScoreType = "Multiplied" } },
            { 8003, new AchievementInfo { Code = "H.1/II.5", Name = "Nowe stanowisko laboratoryjne / program komputerowy", Unit = "Dział Jakości Kształcenia", Area = "Didactic", BasePoints = 25, ScoreType = "Variable" } },
            { 8004, new AchievementInfo { Code = "H.1/II.6", Name = "Raport samooceny / wniosek o nowy kierunek lub uprawnienia", Unit = "Dział Jakości Kształcenia", Area = "Didactic", BasePoints = 80, ScoreType = "Formula" } },
            { 8005, new AchievementInfo { Code = "H.1/II.7", Name = "Program nowych studiów podyplomowych / kierowanie edycją", Unit = "Dział Jakości Kształcenia", Area = "Didactic", BasePoints = 20, ScoreType = "Variable" } },
            { 8006, new AchievementInfo { Code = "H.1/II.8", Name = "Program nowych kursów / prowadzenie edycji", Unit = "Dział Jakości Kształcenia", Area = "Didactic", BasePoints = 8, ScoreType = "Variable" } },
            { 8007, new AchievementInfo { Code = "H.1/II.9", Name = "Przeprowadzanie egzaminów doktorskich / kwalifikacji językowej", Unit = "Dział Jakości Kształcenia", Area = "Didactic", BasePoints = 4, ScoreType = "Multiplied" } },
            { 8008, new AchievementInfo { Code = "H.1/II.10", Name = "Ocena zajęć dydaktycznych (z ankiet studenckich)", Unit = "Dział Jakości Kształcenia", Area = "Didactic", BasePoints = 15, ScoreType = "Variable" } },
            { 8009, new AchievementInfo { Code = "H.1/II.11", Name = "Ocena wyróżniająca z arkuszy hospitacji", Unit = "Dział Jakości Kształcenia", Area = "Didactic", BasePoints = 8, ScoreType = "Variable" } },
            { 8010, new AchievementInfo { Code = "H.1/II.13", Name = "Autorstwo egzaminu UCJ / sesja TOEIC", Unit = "Dział Jakości Kształcenia", Area = "Didactic", BasePoints = 8, ScoreType = "Variable" } },
            { 8011, new AchievementInfo { Code = "H.1/II.15", Name = "Bezpłatne zajęcia dla szkół (za 2 godziny, max 16/rok)", Unit = "Dział Jakości Kształcenia", Area = "Didactic", BasePoints = 2, ScoreType = "Multiplied" } },
            { 8012, new AchievementInfo { Code = "H.1/II.16", Name = "Podniesienie kwalifikacji zawodowych (certyfikat / szkolenie)", Unit = "Dział Jakości Kształcenia", Area = "Didactic", BasePoints = 4, ScoreType = "Fixed" } },
            { 8013, new AchievementInfo { Code = "H.1/IX.1", Name = "Nieodpłatne tłumaczenia w SJO (za stronę, max 10)", Unit = "Dział Jakości Kształcenia", Area = "Didactic", BasePoints = 4, ScoreType = "Multiplied" } },

            // =========================================================================================
            // I. DZIAŁ PROMOCJI
            // =========================================================================================
            { 9001, new AchievementInfo { Code = "I.1/VII.1", Name = "Wydziałowy koordynator / organizator wydarzenia promującego", Unit = "Dział Promocji", Area = "Didactic", BasePoints = 8, ScoreType = "Variable" } },
            { 9002, new AchievementInfo { Code = "I.1/VII.2", Name = "Zajęcia o charakterze edukacyjnym / promocyjnym", Unit = "Dział Promocji", Area = "Didactic", BasePoints = 4, ScoreType = "Fixed" } },
            { 9003, new AchievementInfo { Code = "I.1/VII.3", Name = "Udział w targach i spotkaniach edukacyjnych", Unit = "Dział Promocji", Area = "Didactic", BasePoints = 8, ScoreType = "Variable" } },
            { 9004, new AchievementInfo { Code = "I.1/VII.4", Name = "Organizacja i udział w zawodach sportowych", Unit = "Dział Promocji", Area = "Didactic", BasePoints = 8, ScoreType = "Variable" } },
            { 9005, new AchievementInfo { Code = "I.1/VII.5", Name = "Wystąpienia w mediach jako ekspert", Unit = "Dział Promocji", Area = "Didactic", BasePoints = 3, ScoreType = "Multiplied" } },
            { 9006, new AchievementInfo { Code = "I.1/VII.6", Name = "Wydziałowy koordynator wystawy kulturalnej", Unit = "Dział Promocji", Area = "Didactic", BasePoints = 8, ScoreType = "Variable" } },
            { 9007, new AchievementInfo { Code = "I.1/VII.7", Name = "Wizyta gości: prezentacja / opieka", Unit = "Dział Promocji", Area = "Didactic", BasePoints = 4, ScoreType = "Variable" } },
            { 9008, new AchievementInfo { Code = "I.1/VII.8", Name = "Inne działania promujące wizerunek Uczelni (max 12/rok)", Unit = "Dział Promocji", Area = "Didactic", BasePoints = 12, ScoreType = "Variable" } },
            //inne
            { 9999, new AchievementInfo { Code = "BRAK", Name = "Inne osiągnięcie, wpisz", Unit = "Inne", Area = "Inne", BasePoints = 0, ScoreType = "Variable" } }
        };
    }
}