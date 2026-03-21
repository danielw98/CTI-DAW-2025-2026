using Lab04.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab04.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.Migrate();

        if (context.Categories.Any() || context.Articles.Any())
        {
            return;
        }

        var technology = new Category
        {
            Name = "Tehnologie"
        };

        var sport = new Category
        {
            Name = "Sport"
        };

        var culture = new Category
        {
            Name = "Cultură"
        };

        context.Categories.AddRange(technology, sport, culture);
        context.SaveChanges();

        context.Articles.AddRange(
            new Article
            {
                Title = "Universitățile testează platforme AI pentru predare și evaluare",
                Content = "Mai multe universități europene analizează modul în care instrumentele bazate pe inteligență artificială pot sprijini activitatea didactică. Printre scenariile discutate se numără generarea de exerciții, feedback automat pentru teme și asistență în organizarea materialelor de curs. Cadrele didactice atrag însă atenția că astfel de soluții trebuie folosite cu prudență, mai ales în evaluare.",
                PublishedAt = new DateTime(2026, 3, 10),
                CategoryId = technology.Id
            },
            new Article
            {
                Title = "Noi generații de procesoare promit eficiență energetică mai bună",
                Content = "Producătorii de hardware au prezentat în ultimele luni noi arhitecturi de procesoare orientate atât spre performanță, cât și spre reducerea consumului de energie. Accentul este pus pe laptopuri mai silențioase, autonomie extinsă și sarcini asistate de unități dedicate pentru AI. Analiștii spun că direcția pieței este clară: mai multă performanță, cu accent pe costuri energetice mai mici.",
                PublishedAt = new DateTime(2026, 3, 12),
                CategoryId = technology.Id
            },
            new Article
            {
                Title = "Companiile investesc în centre de date optimizate pentru sarcini AI",
                Content = "Interesul crescut pentru modele de inteligență artificială a determinat companiile să își regândească infrastructura. Tot mai multe investiții sunt direcționate către centre de date optimizate pentru acceleratoare hardware și procesare paralelă. Specialiștii subliniază că provocările nu țin doar de putere de calcul, ci și de răcire, consum energetic și costul operațional pe termen lung.",
                PublishedAt = new DateTime(2026, 3, 16),
                CategoryId = technology.Id
            },
            new Article
            {
                Title = "Start de sezon în Formula 1, cu accent pe noile pachete tehnice",
                Content = "Echipele au prezentat noile monoposturi și au oferit primele indicii despre direcția tehnică a sezonului. Atenția este concentrată pe eficiența aerodinamică, fiabilitate și adaptarea la circuitele din primele curse. Piloții au declarat că diferențele dintre echipe par mai mici decât în sezoanele trecute, ceea ce ar putea duce la un campionat mai echilibrat.",
                PublishedAt = new DateTime(2026, 3, 15),
                CategoryId = sport.Id
            },
            new Article
            {
                Title = "Turneu internațional de tenis aduce la start jucători din topul mondial",
                Content = "Competiția reunește sportivi cu experiență, dar și jucători aflați în plină ascensiune. Organizatorii se așteaptă la meciuri echilibrate și la un interes crescut din partea publicului, mai ales după rezultatele surprinzătoare din ultimele turnee. Antrenorii spun că programul încărcat al sezonului va influența ritmul de joc și strategia participanților.",
                PublishedAt = new DateTime(2026, 3, 11),
                CategoryId = sport.Id
            },
            new Article
            {
                Title = "Cluburile europene își pregătesc loturile pentru fazele decisive ale sezonului",
                Content = "În competițiile continentale, perioada următoare este considerată decisivă pentru obiectivele sportive și financiare ale cluburilor. Staff-urile tehnice pun accent pe rotația jucătorilor, recuperare și gestionarea accidentărilor. Comentatorii sportivi remarcă faptul că diferența dintre echipe este tot mai des făcută de organizarea defensivă și de consistența lotului pe termen lung.",
                PublishedAt = new DateTime(2026, 3, 18),
                CategoryId = sport.Id
            },
            new Article
            {
                Title = "Festivalul de film european aduce proiecții speciale și dezbateri cu regizori",
                Content = "Ediția din acest an include atât filme premiate recent, cât și producții independente prezentate pentru prima dată publicului larg. Organizatorii au pregătit sesiuni de întrebări și răspunsuri, întâlniri cu regizori și discuții despre transformările industriei cinematografice. Publicul este invitat să participe nu doar la proiecții, ci și la ateliere dedicate studenților și tinerilor cineaști.",
                PublishedAt = new DateTime(2026, 3, 9),
                CategoryId = culture.Id
            },
            new Article
            {
                Title = "Muzeele extind programele educaționale pentru publicul tânăr",
                Content = "Tot mai multe instituții culturale dezvoltă programe interactive pentru elevi și studenți, încercând să apropie patrimoniul de noile generații. Atelierele includ ghidaje tematice, activități digitale și expoziții cu componente multimedia. Reprezentanții muzeelor spun că interesul pentru astfel de inițiative este în creștere, mai ales atunci când conținutul este prezentat într-o formă accesibilă și actuală.",
                PublishedAt = new DateTime(2026, 3, 14),
                CategoryId = culture.Id
            },
            new Article
            {
                Title = "Expoziție de artă contemporană explorează relația dintre tehnologie și memorie",
                Content = "Noua expoziție reunește lucrări multimedia, instalații și proiecte video care discută felul în care tehnologia influențează modul în care păstrăm și reinterpretăm memoria colectivă. Curatorii au construit traseul astfel încât vizitatorii să treacă prin mai multe forme de expresie artistică, de la fotografie și sunet până la instalații interactive. Evenimentul este însoțit de dezbateri și tururi ghidate.",
                PublishedAt = new DateTime(2026, 3, 17),
                CategoryId = culture.Id
            }
        );

        context.SaveChanges();
    }
}