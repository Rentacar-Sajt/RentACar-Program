using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RVS_Aplikacija.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

namespace RVS_Aplikacija.Servisi.Pdf
{
    // Uloga klase: PdfUgovorServis grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class PdfUgovorServis : IPdfUgovorServis
    {
        private const string BojaLinije = "#4A4A4A";
        private const string BojaZaglavlja = "#EDEDED";
        private const string BojaPozadinePolja = "#FAFAFA";

        private static readonly CultureInfo SrpskaKultura =
            CultureInfo.GetCultureInfo("sr-Latn-RS");

        // Generiše kompletan PDF ugovor na osnovu podataka iz UgovorViewModel-a i vraća dokument kao niz bajtova.
        public byte[] GenerisiPdf(UgovorViewModel ugovor)
        {
            ArgumentNullException.ThrowIfNull(ugovor);

            var dokument = Document.Create(document =>
            {
                document.Page(page =>
                {
                    PodesiStranicu(page);

                    page.Content()
                        .PaddingTop(3f, Unit.Millimetre)
                        .Column(column =>
                        {
                            column.Spacing(3f);

                            column.Item()
                                .Element(x => NapraviPodatkeOKlijentu(x, ugovor));

                            column.Item()
                                .Element(x => NapraviPodatkeOIznajmljivanju(x, ugovor));

                            column.Item()
                                .Element(x => NapraviTabeluVozila(x, ugovor));

                            column.Item().Row(row =>
                            {
                                row.RelativeItem(1.65f)
                                    .Element(x => NapraviDodatneUsluge(x, ugovor));

                                row.ConstantItem(3);

                                row.RelativeItem(1f)
                                    .Element(x => NapraviObracun(x, ugovor));
                            });

                            column.Item().Row(row =>
                            {
                                row.RelativeItem(1.15f)
                                    .Element(NapraviUslove);

                                row.ConstantItem(3);

                                row.RelativeItem(1.35f)
                                    .Element(NapraviPregledVozilaCompact);
                            });
                        });
                });
            });

            return dokument.GeneratePdf();
        }

        // Podešava format A4 stranice, margine i osnovni izgled stranice PDF ugovora.
        private static void PodesiStranicu(PageDescriptor page)
        {
            page.Size(PageSizes.A4);
            page.MarginHorizontal(8, Unit.Millimetre);
            page.MarginVertical(7, Unit.Millimetre);

            page.DefaultTextStyle(style =>
                style.FontFamily("Arial")
                    .FontSize(7.2f)
                    .FontColor(Colors.Black));

            page.Header()
                .Element(NapraviZaglavlje);
        }

        // Formira zaglavlje PDF ugovora sa osnovnim naslovom i podacima dokumenta.
        private static void NapraviZaglavlje(IContainer container)
        {
            container
                .PaddingBottom(5)
                .BorderBottom(1f)
                .BorderColor(BojaLinije)
                .Row(row =>
                {
                    row.RelativeItem(1.05f)
                        .AlignMiddle()
                        .Column(column =>
                        {
                            column.Spacing(0.3f);

                            column.Item()
                                .Text("DRIVERENT")
                                .FontSize(15f)
                                .Bold();

                            column.Item()
                                .PaddingLeft(6)
                                .Text("R E N T  A  C A R")
                                .FontSize(6.2f)
                                .Bold();
                        });

                    row.RelativeItem(2.25f)
                        .AlignMiddle()
                        .AlignCenter()
                        .Text("UGOVOR O IZNAJMLJIVANJU VOZILA")
                        .FontSize(12.5f)
                        .Bold();

                    row.RelativeItem(1.15f)
                        .AlignMiddle()
                        .AlignRight()
                        .Column(column =>
                        {
                            column.Spacing(0.1f);
                            column.Item().Text("DRIVERENT d.o.o.").Bold();
                            column.Item().Text("Bulevar Oslobođenja 123");
                            column.Item().Text("11000 Beograd, Srbija");
                            column.Item().Text("PIB: 123456789");
                            column.Item().Text("Tel: 011/123-4567");
                            column.Item().Text("www.driverent.rs");
                        });
                });
        }

        // Dodaje u PDF sekciju sa podacima klijenta koji je zaključio ugovor.
        private static void NapraviPodatkeOKlijentu(
            IContainer container,
            UgovorViewModel ugovor)
        {
            var klijent = ugovor.KlijentObjekat;

            Sekcija(container, "1. PODACI O KLIJENTU", content =>
            {
                content.Padding(7).Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Spacing(5);

                        RedPodataka(
                            column,
                            "Ime i prezime:",
                            klijent == null
                                ? "-"
                                : $"{klijent.Ime} {klijent.Prezime}");

                        var jmbgIliPasos =
                            !string.IsNullOrWhiteSpace(klijent?.JMBG)
                                ? klijent.JMBG
                                : klijent?.BrojPasosa ?? "-";

                        RedPodataka(
                            column,
                            "JMBG / Broj pasoša:",
                            jmbgIliPasos);

                        RedPodataka(
                            column,
                            "Broj vozačke dozvole:",
                            klijent?.BrojVozackeDozvole ?? "-");

                        RedPodataka(
                            column,
                            "Datum izdavanja dozvole:",
                            klijent == null
                                ? "-"
                                : FormatirajDatum(klijent.DatumIzdavanjaDozvole));
                    });

                    row.ConstantItem(18);

                    row.RelativeItem().Column(column =>
                    {
                        column.Spacing(5);

                        RedPodataka(
                            column,
                            "Telefon:",
                            klijent?.Telefon ?? "-");

                        RedPodataka(
                            column,
                            "E-mail:",
                            klijent?.Email ?? "-");

                        RedPodataka(
                            column,
                            "Adresa:",
                            klijent?.Adresa ?? "-");

                        RedPodataka(
                            column,
                            "Datum isteka dozvole:",
                            klijent == null
                                ? "-"
                                : FormatirajDatum(klijent.DatumIstekaVozackeDozvole));
                    });
                });
            });
        }

        // Dodaje u PDF podatke o periodu najma, mestima preuzimanja i vraćanja i drugim podacima iznajmljivanja.
        private static void NapraviPodatkeOIznajmljivanju(
            IContainer container,
            UgovorViewModel ugovor)
        {
            Sekcija(container, "2. PODACI O IZNAJMLJIVANJU", content =>
            {
                content.Padding(7).Column(glavnaKolona =>
                {
                    glavnaKolona.Item().Row(row =>
                    {
                        row.RelativeItem(1.6f).Column(column =>
                        {
                            column.Spacing(4f);

                            RedPodataka(column, "Broj ugovora:", ugovor.BrojUgovora);
                            RedPodataka(column, "Datum izdavanja:", FormatirajDatum(ugovor.DatumIzdavanja));
                            RedPodataka(column, "Datum preuzimanja:", FormatirajDatumVreme(ugovor.DatumPreuzimanja));
                            RedPodataka(column, "Datum vraćanja:", FormatirajDatumVreme(ugovor.DatumVracanja));
                            RedPodataka(column, "Mesto preuzimanja:", ugovor.MestoPreuzimanja);
                            RedPodataka(column, "Mesto vraćanja:", ugovor.MestoVracanja);
                            RedPodataka(column, "Način plaćanja:", NapraviNacinPlacanja(ugovor.NacinPlacanja));
                            RedPodataka(column, "Depozit:", $"{FormatirajNovac(ugovor.Depozit)} RSD");
                        });

                        row.ConstantItem(11);

                        row.RelativeItem()
                            .BorderLeft(0.7f)
                            .BorderColor(BojaLinije)
                            .PaddingLeft(10)
                            .Column(column =>
                            {
                                column.Spacing(4f);

                                column.Item()
                                    .Text("Status ugovora:")
                                    .Bold();

                                StatusRed(column, "Aktivan", ugovor.StatusUgovora);
                                StatusRed(column, "Završen", ugovor.StatusUgovora);
                                StatusRed(column, "Otkazan", ugovor.StatusUgovora);

                                column.Item()
                                    .PaddingTop(2)
                                    .Border(0.7f)
                                    .BorderColor(BojaLinije)
                                    .Background(BojaPozadinePolja)
                                    .MinHeight(45)
                                    .Padding(6)
                                    .Column(napomena =>
                                    {
                                        napomena.Item()
                                            .Text("Napomena:")
                                            .Bold();

                                        napomena.Item()
                                            .PaddingTop(2)
                                            .Text(string.IsNullOrWhiteSpace(ugovor.Napomena)
                                                ? "-"
                                                : ugovor.Napomena);
                                    });
                            });
                    });

                    glavnaKolona.Item()
                        .PaddingTop(6)
                        .BorderTop(0.7f)
                        .BorderColor(BojaLinije)
                        .PaddingTop(5)
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .Text("Potpis klijenta:  ________________________")
                                .Bold();

                            row.RelativeItem()
                                .AlignRight()
                                .Text("Potpis zaposlenog:  ________________________")
                                .Bold();
                        });
                });
            });
        }

        // Formira tabelu vozila i stavki ugovora u PDF dokumentu.
        private static void NapraviTabeluVozila(
            IContainer container,
            UgovorViewModel ugovor)
        {
            Sekcija(container, "3. STAVKE UGOVORA O IZNAJMLJIVANJU (VOZILA)", content =>
            {
                content.Column(column =>
                {
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(22);
                            columns.RelativeColumn(2.2f);
                            columns.RelativeColumn(1.4f);
                            columns.RelativeColumn(1f);
                            columns.RelativeColumn(1.3f);
                            columns.RelativeColumn(0.9f);
                            columns.RelativeColumn(1.35f);
                        });

                        table.Header(header =>
                        {
                            ZaglavljeTabele(header, "RB");
                            ZaglavljeTabele(header, "Vozilo (marka i model)");
                            ZaglavljeTabele(header, "Registracija");
                            ZaglavljeTabele(header, "Broj dana");
                            ZaglavljeTabele(header, "Cena po danu (RSD)");
                            ZaglavljeTabele(header, "Popust (%)");
                            ZaglavljeTabele(header, "Ukupno (RSD)");
                        });

                        var stavke = ugovor.StavkeUgovora
                            ?? new List<StavkaUgovoraViewModel>();

                        var brojRedova = Math.Max(5, stavke.Count);

                        for (var i = 0; i < brojRedova; i++)
                        {
                            var stavka = i < stavke.Count ? stavke[i] : null;
                            var vozilo = stavka?.VoziloObjekat;

                            CelijaTabele(table, (i + 1).ToString());
                            CelijaTabele(
                                table,
                                vozilo == null
                                    ? string.Empty
                                    : $"{vozilo.Marka} {vozilo.Model}");
                            CelijaTabele(table, vozilo?.Registracija ?? string.Empty);
                            CelijaTabele(table, stavka == null ? string.Empty : stavka.BrojDana.ToString());
                            CelijaTabele(table, stavka == null ? string.Empty : FormatirajNovac(stavka.CenaPoDanu));
                            CelijaTabele(table, stavka == null ? string.Empty : stavka.PopustProcenat.ToString("0.##"));
                            CelijaTabele(table, stavka == null ? string.Empty : FormatirajNovac(stavka.Ukupno));
                        }
                    });

                    var medjuzbir = ugovor.StavkeUgovora?
                        .Sum(stavka => stavka.Ukupno) ?? 0;

                    column.Item()
                        .AlignRight()
                        .Width(200)
                        .BorderLeft(0.7f)
                        .BorderRight(0.7f)
                        .BorderBottom(0.7f)
                        .BorderColor(BojaLinije)
                        .Padding(5)
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .Text("Međuzbir vozila:")
                                .Bold();

                            row.ConstantItem(70)
                                .AlignRight()
                                .Text(FormatirajNovac(medjuzbir));
                        });
                });
            });
        }

        // Prikazuje u PDF-u dodatne usluge koje su izabrane uz ugovor i njihove iznose.
        private static void NapraviDodatneUsluge(
    IContainer container,
    UgovorViewModel ugovor)
        {
            HashSet<int> izabraniIdjevi =
                (ugovor.IzabraneDodatneUsluge ?? string.Empty)
                    .Split(
                        ',',
                        StringSplitOptions.RemoveEmptyEntries
                    )
                    .Select(vrednost =>
                    {
                        return int.TryParse(
                            vrednost.Trim(),
                            out int id
                        )
                            ? id
                            : 0;
                    })
                    .Where(id => id > 0)
                    .ToHashSet();

            int brojDana =
                Math.Max(
                    1,
                    (int)Math.Ceiling(
                        (ugovor.DatumVracanja -
                         ugovor.DatumPreuzimanja).TotalDays
                    )
                );

            var definisaneUsluge = new[]
            {
        new
        {
            Id = 1,
            Naziv = "GPS navigacija",
            CenaPoDanu = 300m
        },

        new
        {
            Id = 2,
            Naziv = "Dečije sedište",
            CenaPoDanu = 400m
        },

        new
        {
            Id = 3,
            Naziv = "Full osiguranje",
            CenaPoDanu = 1500m
        },

        new
        {
            Id = 4,
            Naziv = "Dodatni vozač",
            CenaPoDanu = 700m
        },

        new
        {
            Id = 5,
            Naziv = "Ostalo",
            CenaPoDanu = 0m
        }
    };

            List<DodatnaUslugaUgovoraViewModel> detaljiUsluga =
                ugovor.DodatneUsluge?
                    .ToList()
                ?? new List<DodatnaUslugaUgovoraViewModel>();

            Sekcija(container, "4. DODATNE USLUGE", content =>
            {
                content.Padding(7).Column(column =>
                {
                    column.Spacing(4);

                    foreach (var definisanaUsluga
                             in definisaneUsluge)
                    {
                        DodatnaUslugaUgovoraViewModel? detalji =
                            detaljiUsluga.FirstOrDefault(
                                x => x.DodatnaUslugaId ==
                                     definisanaUsluga.Id
                            );

                        bool izabrana =
                            izabraniIdjevi.Contains(
                                definisanaUsluga.Id
                            )
                            || detalji?.Izabrana == true;

                        decimal cenaPoDanu = 0;
                        int dana = 0;
                        decimal ukupno = 0;

                        if (izabrana)
                        {
                            cenaPoDanu =
                                detalji != null &&
                                detalji.CenaPoDanu > 0
                                    ? detalji.CenaPoDanu
                                    : definisanaUsluga.CenaPoDanu;

                            dana =
                                detalji != null &&
                                detalji.BrojDana > 0
                                    ? detalji.BrojDana
                                    : brojDana;

                            ukupno =
                                detalji != null &&
                                detalji.Ukupno > 0
                                    ? detalji.Ukupno
                                    : cenaPoDanu * dana;
                        }

                        DodatnaUslugaPdfRed(
                            column,
                            definisanaUsluga.Naziv,
                            izabrana,
                            cenaPoDanu,
                            dana,
                            ukupno
                        );
                    }
                });
            });
        }

        // Formira deo PDF dokumenta sa finansijskim obračunom ugovora, uključujući cenu, popust, depozit i ukupan iznos.
        private static void NapraviObracun(
    IContainer container,
    UgovorViewModel ugovor)
        {
            decimal ukupnoVozila =
                ugovor.StavkeUgovora?
                    .Sum(stavka => stavka.Ukupno)
                ?? 0;

            decimal iznosPopusta =
                ukupnoVozila *
                ugovor.PopustProcenat /
                100m;

            HashSet<int> izabraniIdjevi =
                (ugovor.IzabraneDodatneUsluge ?? string.Empty)
                    .Split(
                        ',',
                        StringSplitOptions.RemoveEmptyEntries
                    )
                    .Select(vrednost =>
                    {
                        return int.TryParse(
                            vrednost.Trim(),
                            out int id
                        )
                            ? id
                            : 0;
                    })
                    .Where(id => id > 0)
                    .ToHashSet();

            int brojDana =
                Math.Max(
                    1,
                    (int)Math.Ceiling(
                        (ugovor.DatumVracanja -
                         ugovor.DatumPreuzimanja).TotalDays
                    )
                );

            Dictionary<int, decimal> ceneUsluga =
                new Dictionary<int, decimal>
                {
            { 1, 300m },
            { 2, 400m },
            { 3, 1500m },
            { 4, 700m },
            { 5, 0m }
                };

            decimal ukupnoDodatneUsluge = 0;

            foreach (int id in izabraniIdjevi)
            {
                DodatnaUslugaUgovoraViewModel? detalji =
                    ugovor.DodatneUsluge?
                        .FirstOrDefault(
                            x => x.DodatnaUslugaId == id
                        );

                if (detalji != null &&
                    detalji.Ukupno > 0)
                {
                    ukupnoDodatneUsluge +=
                        detalji.Ukupno;

                    continue;
                }

                if (ceneUsluga.TryGetValue(
                        id,
                        out decimal cenaPoDanu))
                {
                    ukupnoDodatneUsluge +=
                        cenaPoDanu * brojDana;
                }
            }

            decimal ukupnoPreDepozita =
     ukupnoVozila +
     ukupnoDodatneUsluge -
     iznosPopusta;

            decimal depozit =
                Math.Max(0, ugovor.Depozit);

            decimal ukupnoZaPlacanje =
                Math.Max(
                    0,
                    ukupnoPreDepozita - depozit
                );

            ugovor.UkupnoZaPlacanje =
                ukupnoZaPlacanje;

            container
                .Border(0.8f)
                .BorderColor(BojaLinije)
                .Padding(7)
                .Column(column =>
                {
                    column.Spacing(5);

                    RedObracuna(
                        column,
                        "Ukupno vozila:",
                        ukupnoVozila
                    );

                    RedObracuna(
                        column,
                        "Ukupno dodatne usluge:",
                        ukupnoDodatneUsluge
                    );

                    column.Item().Row(row =>
                    {
                        row.RelativeItem()
                            .Text("Popust (%):");

                        row.ConstantItem(70)
                            .AlignRight()
                            .Text(
                                ugovor.PopustProcenat
                                    .ToString("0.##") +
                                " %"
                            );
                    });

                    RedObracuna(
                        column,
                        "Iznos popusta:",
                        iznosPopusta
                    );

                    column.Item()
                        .PaddingTop(5)
                        .BorderTop(1)
                        .BorderColor(BojaLinije)
                        .PaddingTop(6)
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .AlignMiddle()
                                .Text(
                                    "UKUPNO ZA PLAĆANJE:"
                                )
                                .FontSize(8.5f)
                                .Bold();

                            row.ConstantItem(85)
                                .Border(1)
                                .BorderColor(BojaLinije)
                                .Background(
                                    BojaPozadinePolja
                                )
                                .Padding(7)
                                .AlignRight()
                                .Text(
                                    FormatirajNovac(
                                        ukupnoZaPlacanje
                                    )
                                )
                                .FontSize(12)
                                .Bold();
                        });
                });
        }

        // Dodaje tekst uslova iznajmljivanja u završni deo PDF ugovora.
        private static void NapraviUslove(IContainer container)
        {
            Sekcija(container, "5. USLOVI I NAPOMENE", content =>
            {
                content.Padding(7).MinHeight(132).Column(column =>
                {
                    column.Spacing(4f);
                    Uslov(column, "Klijent je odgovoran za vozilo tokom perioda iznajmljivanja.");
                    Uslov(column, "Kašnjenje pri vraćanju vozila naplaćuje se prema važećem cenovniku.");
                    Uslov(column, "Pušenje u vozilu je zabranjeno.");
                    Uslov(column, "Vozilo se preuzima sa punim rezervoarom i vraća sa punim rezervoarom.");
                    Uslov(column, "Potpisivanjem ugovora klijent potvrđuje da je upoznat sa uslovima.");
                });
            });
        }

        // Formira sažet pregled izabranih vozila u PDF dokumentu.
        private static void NapraviPregledVozilaCompact(
            IContainer container)
        {
            Sekcija(container, "6. PREGLED VOZILA", content =>
            {
                content.Padding(7).Column(column =>
                {
                    column.Spacing(3f);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem()
                            .AlignCenter()
                            .Text("Preuzimanje vozila")
                            .FontSize(6.8f)
                            .Bold();

                        row.RelativeItem()
                            .AlignCenter()
                            .Text("Vraćanje vozila")
                            .FontSize(6.8f)
                            .Bold();
                    });

                    column.Item()
                        .Height(128)
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .BorderRight(0.6f)
                                .BorderColor(BojaLinije)
                                .PaddingRight(3)
                                .Element(
                                    NapraviBlokPregledaSaIkonicama);

                            row.RelativeItem()
                                .PaddingLeft(3)
                                .Element(
                                    NapraviBlokPregledaSaIkonicama);
                        });

                    column.Item()
                        .PaddingTop(2)
                        .Text(
                            "Napomene / oštećenja: " +
                            "______________________________")
                        .FontSize(6.7f);
                });
            });
        }

        // Kreira vizuelni blok sa ikonicama i ključnim podacima koji se prikazuje u PDF ugovoru.
        private static void NapraviBlokPregledaSaIkonicama(
            IContainer container)
        {
            byte[] front = UcitajSliku("front.png");
            byte[] top = UcitajSliku("top.png");
            byte[] rear = UcitajSliku("rear.png");
            byte[] side = UcitajSliku("side.png");

            container.Column(column =>
            {
                column.Spacing(3);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item()
                            .AlignCenter()
                            .Text("PREDNJI POGLED")
                            .FontSize(5.2f);

                        c.Item()
                            .PaddingTop(2)
                            .Height(24)
                            .PaddingHorizontal(3)
                            .AlignCenter()
                            .AlignMiddle()
                            .Element(x =>
                            {
                                if (front.Length > 0)
                                    x.Image(front).FitArea();
                                else
                                    x.Text("[front]").FontSize(5);
                            });
                    });

                    row.RelativeItem().Column(c =>
                    {
                        c.Item()
                            .AlignCenter()
                            .Text("GORNJI POGLED")
                            .FontSize(5.2f);

                        c.Item()
                            .PaddingTop(2)
                            .Height(24)
                            .PaddingHorizontal(3)
                            .AlignCenter()
                            .AlignMiddle()
                            .Element(x =>
                            {
                                if (top.Length > 0)
                                    x.Image(top).FitArea();
                                else
                                    x.Text("[top]").FontSize(5);
                            });
                    });

                    row.RelativeItem().Column(c =>
                    {
                        c.Item()
                            .AlignCenter()
                            .Text("ZADNJI POGLED")
                            .FontSize(5.2f);

                        c.Item()
                            .PaddingTop(2)
                            .Height(24)
                            .PaddingHorizontal(3)
                            .AlignCenter()
                            .AlignMiddle()
                            .Element(x =>
                            {
                                if (rear.Length > 0)
                                    x.Image(rear).FitArea();
                                else
                                    x.Text("[rear]").FontSize(5);
                            });
                    });
                });

                column.Item()
                    .PaddingTop(2)
                    .AlignCenter()
                    .Text("BOČNI POGLED VOZILA")
                    .FontSize(5.2f);

                column.Item()
                    .Height(32)
                    .PaddingHorizontal(12)
                    .AlignCenter()
                    .AlignMiddle()
                    .Element(x =>
                    {
                        if (side.Length > 0)
                            x.Image(side).FitArea();
                        else
                            x.Text("[side]").FontSize(5);
                    });
            });
        }

        // Učitava sliku sa prosleđene putanje i vraća njene bajtove za ubacivanje u PDF dokument.
        private static byte[] UcitajSliku(
            string nazivFajla)
        {
            string putanja = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "Slike",
                "pdf",
                nazivFajla);

            if (!File.Exists(putanja))
            {
                string rezervnaPutanja = Path.Combine(
                    AppContext.BaseDirectory,
                    "wwwroot",
                    "Slike",
                    "pdf",
                    nazivFajla);

                if (File.Exists(rezervnaPutanja))
                    putanja = rezervnaPutanja;
            }

            return File.Exists(putanja)
                ? File.ReadAllBytes(putanja)
                : Array.Empty<byte>();
        }

        // Kreira standardizovanu PDF sekciju sa naslovom i prosleđenim sadržajem.
        private static void Sekcija(
            IContainer container,
            string naslov,
            Action<IContainer> sadrzaj)
        {
            container
                .Border(0.7f)
                .BorderColor(BojaLinije)
                .Column(column =>
                {
                    column.Item()
                        .Background(BojaZaglavlja)
                        .BorderBottom(0.7f)
                        .BorderColor(BojaLinije)
                        .PaddingVertical(4f)
                        .PaddingHorizontal(7)
                        .Text(naslov)
                        .FontSize(8.2f)
                        .Bold();

                    column.Item().Element(sadrzaj);
                });
        }

        // Dodaje jedan red naziv–vrednost u PDF dokument, na primer „Broj ugovora: UG-...“.
        private static void RedPodataka(
            ColumnDescriptor column,
            string naziv,
            string? vrednost)
        {
            column.Item().Row(row =>
            {
                row.ConstantItem(112).Text(naziv);

                row.RelativeItem()
                    .BorderBottom(0.5f)
                    .BorderColor(BojaLinije)
                    .PaddingBottom(1.5f)
                    .Text(string.IsNullOrWhiteSpace(vrednost)
                        ? "-"
                        : vrednost);
            });
        }

        // Dodaje red za prikaz jedne statusne opcije i označava da li je ona izabrana.
        private static void StatusRed(
            ColumnDescriptor column,
            string opcija,
            string? izabranaOpcija)
        {
            var izabrano = string.Equals(
                opcija,
                izabranaOpcija,
                StringComparison.OrdinalIgnoreCase);

            column.Item().Row(row =>
            {
                row.ConstantItem(13)
                    .Width(10)
                    .Height(10)
                    .Border(0.7f)
                    .BorderColor(BojaLinije)
                    .AlignCenter()
                    .AlignMiddle()
                    .Text(izabrano ? "X" : string.Empty)
                    .FontSize(6.2f)
                    .Bold();

                row.ConstantItem(4);
                row.RelativeItem().Text(opcija);
            });
        }

        // Formatira i dodaje jednu ćeliju zaglavlja u PDF tabeli.
        private static void ZaglavljeTabele(
            TableCellDescriptor header,
            string tekst)
        {
            header.Cell()
                .Background(BojaZaglavlja)
                .Border(0.5f)
                .BorderColor(BojaLinije)
                .Padding(3.5f)
                .AlignCenter()
                .AlignMiddle()
                .Text(tekst)
                .FontSize(6.8f)
                .Bold();
        }

        // Formatira i dodaje običnu ćeliju sa podatkom u PDF tabeli.
        private static void CelijaTabele(
            TableDescriptor table,
            string? tekst)
        {
            table.Cell()
                .Border(0.5f)
                .BorderColor(BojaLinije)
                .MinHeight(18)
                .Padding(3)
                .AlignCenter()
                .AlignMiddle()
                .Text(tekst ?? string.Empty)
                .FontSize(6.7f);
        }

        // Dodaje jedan red dodatne usluge u PDF i prikazuje da li je usluga izabrana i koliki je njen iznos.
        private static void DodatnaUslugaPdfRed(
            ColumnDescriptor column,
            string naziv,
            bool izabrana,
            decimal cenaPoDanu,
            int brojDana,
            decimal ukupno)
        {
            column.Item().Row(row =>
            {
                row.ConstantItem(12)
                    .Width(9)
                    .Height(9)
                    .Border(0.6f)
                    .BorderColor(BojaLinije)
                    .AlignCenter()
                    .AlignMiddle()
                    .Text(izabrana ? "X" : string.Empty)
                    .FontSize(6)
                    .Bold();

                row.ConstantItem(3);

                row.RelativeItem(1.2f)
                    .Text(naziv)
                    .FontSize(6.7f);

                row.RelativeItem()
                    .Text(
                        izabrana
                            ? $"Cena: {FormatirajNovac(cenaPoDanu)}"
                            : "Cena: —")
                    .FontSize(6.7f);

                row.RelativeItem()
                    .Text(
                        izabrana
                            ? $"Dana: {brojDana}"
                            : "Dana: —")
                    .FontSize(6.7f);

                row.RelativeItem()
                    .AlignRight()
                    .Text(
                        izabrana
                            ? $"Ukupno: {FormatirajNovac(ukupno)}"
                            : "Ukupno: —")
                    .FontSize(6.7f);
            });
        }

        // Dodaje jedan finansijski red obračuna sa nazivom stavke i novčanim iznosom.
        private static void RedObracuna(
            ColumnDescriptor column,
            string naziv,
            decimal iznos)
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Text(naziv);

                row.ConstantItem(68)
                    .BorderBottom(0.5f)
                    .BorderColor(BojaLinije)
                    .AlignRight()
                    .Text(FormatirajNovac(iznos));
            });
        }

        // Dodaje jednu stavku teksta uslova iznajmljivanja u PDF dokument.
        private static void Uslov(
            ColumnDescriptor column,
            string tekst)
        {
            column.Item()
                .Text("• " + tekst)
                .FontSize(6.7f);
        }

        // Pretvara sačuvanu vrednost načina plaćanja u tekst pogodan za prikaz u PDF ugovoru.
        private static string NapraviNacinPlacanja(string? nacinPlacanja)
        {
            return string.Join(
                "   ",
                Cekiranje("Gotovina", nacinPlacanja),
                Cekiranje("Kartica", nacinPlacanja),
                Cekiranje("Online", nacinPlacanja));
        }

        // Vraća oznaku kojom se u PDF-u prikazuje da li je određena opcija izabrana.
        private static string Cekiranje(
            string opcija,
            string? izabranaOpcija)
        {
            var izabrano = string.Equals(
                opcija,
                izabranaOpcija,
                StringComparison.OrdinalIgnoreCase);

            return izabrano
                ? "[X] " + opcija
                : "[ ] " + opcija;
        }

        // Formatira decimalni iznos kao novčanu vrednost prema srpskom formatu brojeva.
        private static string FormatirajNovac(decimal iznos)
        {
            return iznos.ToString("#,##0.00", SrpskaKultura);
        }

        // Formatira datum u oblik dd.MM.yyyy. za prikaz u PDF dokumentu.
        private static string FormatirajDatum(DateTime datum)
        {
            return datum.ToString("dd.MM.yyyy.");
        }

        // Formatira datum i vreme u oblik pogodan za prikaz u PDF ugovoru.
        private static string FormatirajDatumVreme(DateTime datum)
        {
            return datum.ToString("dd.MM.yyyy. 'u' HH:mm");
        }
    }
}
