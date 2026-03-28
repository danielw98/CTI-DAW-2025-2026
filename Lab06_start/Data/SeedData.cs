using Lab06.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab06.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.Migrate();

        if (context.Categories.Any() || context.Articles.Any())
        {
            return;
        }

        var technology = new Category { Name = "Tehnologie" };
        var sport = new Category { Name = "Sport" };
        var culture = new Category { Name = "Cultură" };
        var actualitate = new Category { Name = "Actualitate" };

        context.Categories.AddRange(technology, sport, culture, actualitate);
        context.SaveChanges();

        context.Articles.AddRange(
            // Tehnologie
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
            // Sport
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
            // Cultură
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
            },


            // Actualitate
            new Article
            {
                Title = "A murit Chuck Norris. Celebrul actor și artist martial avea 86 de ani",
                Content = "Lumea cinematografiei și a artelor marțiale este în doliu după dispariția lui Chuck Norris, actorul american cunoscut pentru rolurile din seriale și filme de acțiune care au marcat generații întregi. Norris s-a născut pe 10 martie 1940 în Ryan, Oklahoma, și a devenit un simbol al culturii pop mondiale. Cariera sa a inclus titluri de campion mondial la karate, apoi zeci de filme și serialul Walker, Texas Ranger, difuzat între 1993 și 2001. Familia a confirmat decesul printr-un comunicat, mulțumind fanilor pentru mesajele de condoleanțe primite din toată lumea.",
                PublishedAt = new DateTime(2026, 3, 20),
                ImagePath = "/images/chuck-norris-in-the-expendables-2.png",
                CategoryId = actualitate.Id
            },
            new Article
            {
                Title = "Criza carburanților. Economiștii avertizează: prețul la pompă ar putea depăși 12 lei/litru",
                Content = "Prețurile carburanților au crescut semnificativ în ultima perioadă, iar economiștii spun că scenariul în care benzina și motorina ajung la 12-13 lei pe litru nu este de domeniul ficțiunii. Factorii principali invocați sunt tensiunile geopolitice din Orientul Mijlociu, reducerea producției OPEC+ și deprecierea leului față de dolar. Reprezentanții companiilor de transport avertizează că o nouă creștere va afecta direct prețurile la raft, în special la produsele alimentare. Guvernul a anunțat că analizează mecanisme de plafonare temporară, dar nu a oferit detalii concrete.",
                PublishedAt = new DateTime(2026, 3, 21),
                ImagePath = "/images/alimentare_pompa_benzina.jpg",
                CategoryId = actualitate.Id
            },
            new Article
            {
                Title = "Război în Orientul Mijlociu. Trump anunță o posibilă retragere parțială a trupelor americane",
                Content = "Președintele american Donald Trump a declarat că Statele Unite analizează o retragere parțială a forțelor militare din Orientul Mijlociu, în contextul escaladării conflictului dintre Israel și Iran. Iranul a lansat o serie de rachete balistice asupra teritoriului israelian, vizând inclusiv zona Dimona, unde se află instalații nucleare. Israelul a ripostat cu atacuri aeriene asupra pozițiilor militare iraniene din Siria și Liban. Comunitatea internațională a cerut o dezescaladare imediată, iar ONU a convocat o sesiune de urgență a Consiliului de Securitate.",
                PublishedAt = new DateTime(2026, 3, 21),
                ImagePath = "/images/trump.png",
                CategoryId = actualitate.Id
            },
            new Article
            {
                Title = "Clasamentul reciclării din București și Ilfov. Localitățile cu cele mai bune rezultate",
                Content = "Autoritatea Națională pentru Protecția Mediului a publicat datele oficiale privind rata de reciclare în București și județul Ilfov pentru anul 2025. Rezultatele arată diferențe semnificative între sectoare și localități: sectorul 2 și orașul Voluntari se situează în fruntea clasamentului, cu rate de colectare selectivă de peste 35%, în timp ce alte zone rămân sub media națională. Specialiștii atrag atenția că infrastructura de colectare este insuficientă și că educația cetățenilor rămâne principala barieră în calea îmbunătățirii indicatorilor.",
                PublishedAt = new DateTime(2026, 3, 21),
                ImagePath = "/images/clasamentul_reciclarii.png",
                CategoryId = actualitate.Id
            }
        );

        context.SaveChanges();
    }
}
