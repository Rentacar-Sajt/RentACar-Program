using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RVS_Aplikacija.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace RVS_Aplikacija.Servisi.Pdf
{
    public class PdfUgovorServis : IPdfUgovorServis
    {
        private const string BojaLinije = "#4A4A4A";
        private const string BojaZaglavlja = "#EDEDED";
        private const string BojaPozadinePolja = "#FAFAFA";

        private static readonly CultureInfo SrpskaKultura =
            CultureInfo.GetCultureInfo("sr-Latn-RS");

        public byte[] GenerisiPdf(UgovorViewModel ugovor)
        {
            ArgumentNullException.ThrowIfNull(ugovor);

            var dokument = Document.Create(document =>
            {
                document.Page(page =>
                {
                    PodesiStranicu(page);

                    page.Content()
                        .PaddingTop(3, Unit.Millimetre)
                        .Column(column =>
                        {
                            column.Spacing(3);

                            column.Item().Element(x => NapraviPodatkeOKlijentu(x, ugovor));
                            column.Item().Element(x => NapraviPodatkeOIznajmljivanju(x, ugovor));
                            column.Item().Element(x => NapraviTabeluVozila(x, ugovor));

                            column.Item().Row(row =>
                            {
                                row.RelativeItem(1.65f).Element(NapraviDodatneUsluge);
                                row.ConstantItem(4);
                                row.RelativeItem(1).Element(x => NapraviObracun(x, ugovor));
                            });
                        });
                });

                document.Page(page =>
                {
                    PodesiStranicu(page);

                    page.Content()
                        .PaddingTop(3, Unit.Millimetre)
                        .Column(column =>
                        {
                            column.Spacing(4);
                            column.Item().Element(NapraviUslove);
                            column.Item().Element(NapraviPregledVozila);
                            column.Item().Element(x => NapraviPotpise(x, ugovor));
                            column.Item().Element(x => NapraviPodatkeOIzdavaocu(x, ugovor));
                        });
                });
            });

            return dokument.GeneratePdf();
        }

        private static void PodesiStranicu(PageDescriptor page)
        {
            page.Size(PageSizes.A4);
            page.MarginHorizontal(9, Unit.Millimetre);
            page.MarginVertical(8, Unit.Millimetre);
            page.DefaultTextStyle(style => style.FontFamily("Arial").FontSize(7.4f).FontColor(Colors.Black));
            page.Header().Element(NapraviZaglavlje);
            page.Footer().Height(7, Unit.Millimetre).Element(NapraviFooter);
        }

        private static void NapraviZaglavlje(IContainer container)
        {
            container
                .PaddingBottom(4)
                .BorderBottom(1.2f)
                .BorderColor(BojaLinije)
                .Row(row =>
                {
                    row.RelativeItem(1.05f)
                        .AlignMiddle()
                        .Column(column =>
                        {
                            column.Spacing(1);

                            column.Item()
                                .Text("DRIVERENT")
                                .FontSize(16)
                                .Bold();

                            column.Item()
                                .PaddingLeft(8)
                                .Text("R E N T  A  C A R")
                                .FontSize(6.5f)
                                .Bold();
                        });

                    row.RelativeItem(2.2f)
                        .AlignMiddle()
                        .AlignCenter()
                        .Text("UGOVOR O IZNAJMLJIVANJU VOZILA")
                        .FontSize(13)
                        .Bold();

                    row.RelativeItem(1.15f)
                        .AlignMiddle()
                        .AlignRight()
                        .Column(column =>
                        {
                            column.Spacing(0.5f);

                            column.Item()
                                .Text("DRIVERENT d.o.o.")
                                .Bold();

                            column.Item()
                                .Text("Bulevar Oslobođenja 123");

                            column.Item()
                                .Text("11000 Beograd, Srbija");

                            column.Item()
                                .Text("PIB: 123456789");

                            column.Item()
                                .Text("Tel: 011/123-4567");

                            column.Item()
                                .Text("www.driverent.rs");
                        });
                });
        }

        private static void NapraviFooter(IContainer container)
        {
            container
                .BorderTop(0.7f)
                .BorderColor(BojaLinije)
                .PaddingTop(3)
                .Row(row =>
                {
                    row.RelativeItem()
                        .Text(text =>
                        {
                            text.DefaultTextStyle(
                                style => style.FontSize(7));

                            text.Span("Rent a Car © 2026");
                        });

                    row.RelativeItem()
                        .AlignRight()
                        .Text(text =>
                        {
                            text.DefaultTextStyle(
                                style => style.FontSize(7));

                            text.Span("Strana ");
                            text.CurrentPageNumber();
                            text.Span(" od ");
                            text.TotalPages();
                        });
                });
        }

        private static void NapraviPodatkeOKlijentu(IContainer container, UgovorViewModel ugovor)
        {
            var klijent = ugovor.KlijentObjekat;

            Sekcija(container, "1. PODACI O KLIJENTU", content =>
            {
                content.Padding(7).Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Spacing(5);
                        RedPodataka(column, "Ime i prezime:", klijent == null ? "-" : $"{klijent.Ime} {klijent.Prezime}");
                        var jmbgIliPasos = !string.IsNullOrWhiteSpace(klijent?.JMBG) ? klijent.JMBG : klijent?.BrojPasosa ?? "-";
                        RedPodataka(column, "JMBG / Broj pasoša:", jmbgIliPasos);
                        RedPodataka(column, "Broj vozačke dozvole:", klijent?.BrojVozackeDozvole ?? "-");
                        RedPodataka(column, "Datum izdavanja dozvole:", klijent == null ? "-" : FormatirajDatum(klijent.DatumIzdavanjaDozvole));
                    });

                    row.ConstantItem(18);

                    row.RelativeItem().Column(column =>
                    {
                        column.Spacing(5);
                        RedPodataka(column, "Telefon:", klijent?.Telefon ?? "-");
                        RedPodataka(column, "E-mail:", klijent?.Email ?? "-");
                        RedPodataka(column, "Adresa:", klijent?.Adresa ?? "-");
                        RedPodataka(column, "Datum isteka dozvole:", klijent == null ? "-" : FormatirajDatum(klijent.DatumIstekaVozackeDozvole));
                    });
                });
            });
        }

        private static void NapraviPodatkeOIznajmljivanju(IContainer container, UgovorViewModel ugovor)
        {
            Sekcija(container, "2. PODACI O IZNAJMLJIVANJU", content =>
            {
                content.Padding(7).Column(glavnaKolona =>
                {
                    glavnaKolona.Item().Row(row =>
                    {
                        row.RelativeItem(1.5f).Column(column =>
                        {
                            column.Spacing(4);
                            RedPodataka(column, "Broj ugovora:", ugovor.BrojUgovora);
                            RedPodataka(column, "Datum izdavanja:", FormatirajDatum(ugovor.DatumIzdavanja));
                            RedPodataka(column, "Datum preuzimanja:", FormatirajDatumVreme(ugovor.DatumPreuzimanja));
                            RedPodataka(column, "Datum vraćanja:", FormatirajDatumVreme(ugovor.DatumVracanja));
                            RedPodataka(column, "Mesto preuzimanja:", ugovor.MestoPreuzimanja);
                            RedPodataka(column, "Mesto vraćanja:", ugovor.MestoVracanja);
                            RedPodataka(column, "Način plaćanja:", NapraviNacinPlacanja(ugovor.NacinPlacanja));
                            RedPodataka(column, "Depozit:", $"{FormatirajNovac(ugovor.Depozit)} RSD");
                        });

                        row.ConstantItem(9);

                        row.RelativeItem().BorderLeft(0.8f).BorderColor(BojaLinije).PaddingLeft(10).Column(column =>
                        {
                            column.Spacing(4);
                            column.Item().Text("Status ugovora:").Bold();
                            StatusRed(column, "Aktivan", ugovor.StatusUgovora);
                            StatusRed(column, "Završen", ugovor.StatusUgovora);
                            StatusRed(column, "Otkazan", ugovor.StatusUgovora);

                            column.Item().PaddingTop(3).Border(0.7f).BorderColor(BojaLinije)
                                .Background(BojaPozadinePolja).MinHeight(42).Padding(5).Column(napomena =>
                                {
                                    napomena.Item().Text("Napomena:").Bold();
                                    napomena.Item().PaddingTop(3).Text(string.IsNullOrWhiteSpace(ugovor.Napomena) ? "-" : ugovor.Napomena);
                                });
                        });
                    });

                    glavnaKolona.Item().PaddingTop(7).BorderTop(0.7f).BorderColor(BojaLinije).PaddingTop(5).Row(row =>
                    {
                        row.RelativeItem().Text("Potpis klijenta:  ________________________").Bold();
                        row.RelativeItem().AlignRight().Text("Potpis zaposlenog:  ________________________").Bold();
                    });
                });
            });
        }

        private static void NapraviTabeluVozila(IContainer container, UgovorViewModel ugovor)
        {
            Sekcija(container, "3. STAVKE UGOVORA O IZNAJMLJIVANJU (VOZILA)", content =>
            {
                content.Column(column =>
                {
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(24);
                            columns.RelativeColumn(2.2f);
                            columns.RelativeColumn(1.4f);
                            columns.RelativeColumn(1);
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

                        var stavke = ugovor.StavkeUgovora ?? new List<StavkaUgovoraViewModel>();
                        var brojRedova = Math.Max(5, stavke.Count);

                        for (var i = 0; i < brojRedova; i++)
                        {
                            var stavka = i < stavke.Count ? stavke[i] : null;
                            var vozilo = stavka?.VoziloObjekat;

                            CelijaTabele(table, (i + 1).ToString());
                            CelijaTabele(table, vozilo == null ? string.Empty : $"{vozilo.Marka} {vozilo.Model}");
                            CelijaTabele(table, vozilo?.Registracija ?? string.Empty);
                            CelijaTabele(table, stavka == null ? string.Empty : stavka.BrojDana.ToString());
                            CelijaTabele(table, stavka == null ? string.Empty : FormatirajNovac(stavka.CenaPoDanu));
                            CelijaTabele(table, stavka == null ? string.Empty : stavka.PopustProcenat.ToString("0.##"));
                            CelijaTabele(table, stavka == null ? string.Empty : FormatirajNovac(stavka.Ukupno));
                        }
                    });

                    var medjuzbir = ugovor.StavkeUgovora?.Sum(stavka => stavka.Ukupno) ?? 0;

                    column.Item().AlignRight().Width(190).BorderLeft(0.7f).BorderRight(0.7f)
                        .BorderBottom(0.7f).BorderColor(BojaLinije).Padding(5).Row(row =>
                        {
                            row.RelativeItem().Text("Međuzbir vozila:").Bold();
                            row.ConstantItem(75).AlignRight().Text(FormatirajNovac(medjuzbir));
                        });
                });
            });
        }

        private static void NapraviDodatneUsluge(IContainer container)
        {
            Sekcija(container, "4. DODATNE USLUGE", content =>
            {
                content.Padding(7).Column(column =>
                {
                    column.Spacing(5);
                    DodatnaUsluga(column, "GPS navigacija");
                    DodatnaUsluga(column, "Dečije sedište");
                    DodatnaUsluga(column, "Full osiguranje");
                    DodatnaUsluga(column, "Dodatni vozač");
                    DodatnaUsluga(column, "Ostalo");
                });
            });
        }

        private static void NapraviObracun(IContainer container, UgovorViewModel ugovor)
        {
            var ukupnoVozila = ugovor.StavkeUgovora?.Sum(stavka => stavka.Ukupno) ?? 0;
            var iznosPopusta = ukupnoVozila * ugovor.PopustProcenat / 100m;

            container.Border(0.8f).BorderColor(BojaLinije).Padding(7).Column(column =>
            {
                column.Spacing(5);
                RedObracuna(column, "Ukupno vozila:", ukupnoVozila);
                RedObracuna(column, "Ukupno dodatne usluge:", 0);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text("Popust (%):");
                    row.ConstantItem(70).AlignRight().Text(ugovor.PopustProcenat.ToString("0.##") + " %");
                });

                RedObracuna(column, "Iznos popusta:", iznosPopusta);

                column.Item().PaddingTop(5).BorderTop(1).BorderColor(BojaLinije).PaddingTop(6).Row(row =>
                {
                    row.RelativeItem().AlignMiddle().Text("UKUPNO ZA PLAĆANJE:").FontSize(8.5f).Bold();
                    row.ConstantItem(85).Border(1).BorderColor(BojaLinije).Background(BojaPozadinePolja)
                        .Padding(7).AlignRight().Text(FormatirajNovac(ugovor.UkupnoZaPlacanje)).FontSize(12).Bold();
                });
            });
        }

        private static void NapraviUslove(IContainer container)
        {
            Sekcija(container, "5. USLOVI I NAPOMENE", content =>
            {
                content.Padding(8).Column(column =>
                {
                    column.Spacing(5);
                    Uslov(column, "Klijent je odgovoran za vozilo tokom perioda iznajmljivanja.");
                    Uslov(column, "Kašnjenje pri vraćanju vozila naplaćuje se prema važećem cenovniku.");
                    Uslov(column, "Pušenje u vozilu je zabranjeno.");
                    Uslov(column, "Vozilo se preuzima sa punim rezervoarom i vraća sa punim rezervoarom.");
                    Uslov(column, "Potpisivanjem ugovora klijent potvrđuje da je upoznat sa uslovima.");
                });
            });
        }

        private static void NapraviPregledVozila(IContainer container)
        {
            Sekcija(container, "6. PREGLED VOZILA", content =>
            {
                content.Padding(8).Column(column =>
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().AlignCenter().Text("Preuzimanje vozila").Bold();
                        row.RelativeItem().AlignCenter().Text("Vraćanje vozila").Bold();
                    });

                    column.Item().PaddingTop(6).Height(125).Row(row =>
                    {
                        row.RelativeItem().BorderRight(0.7f).BorderColor(BojaLinije)
                            .Element(x => NapraviSemuVozila(x, "Označiti oštećenja pri preuzimanju"));
                        row.RelativeItem().Element(x => NapraviSemuVozila(x, "Označiti oštećenja pri vraćanju"));
                    });

                    column.Item().PaddingTop(7).Text("Napomene / oštećenja: _____________________________________________");
                    column.Item().PaddingTop(5).Text("______________________________________________________________");
                    column.Item().PaddingTop(5).Text("______________________________________________________________");
                });
            });
        }

        private static void NapraviSemuVozila(IContainer container, string podnaslov)
        {
            container.PaddingHorizontal(8).Column(column =>
            {
                column.Item().AlignCenter().Text("PREDNJI POGLED     GORNJI POGLED     ZADNJI POGLED").FontSize(6.4f);
                column.Item().PaddingTop(12).AlignCenter().Text(
                    "┌────────┐       ┌──────────────┐       ┌────────┐\n" +
                    "│  ____  │       │      ____    │       │  ____  │\n" +
                    "│ /____\\ │       │  ___/____\\_  │       │ /____\\ │\n" +
                    "│ O    O │       │ O          O │       │ O    O │\n" +
                    "└────────┘       └──────────────┘       └────────┘")
                    .FontFamily("Courier New").FontSize(7);

                column.Item().PaddingTop(12).AlignCenter().Text("BOČNI POGLED VOZILA").FontSize(6.4f);
                column.Item().PaddingTop(5).AlignCenter().Text(
                    "        ______________________\n" +
                    "   ____/                      \\____\n" +
                    " _/      O                O        \\_\n" +
                    "|____________________________________|")
                    .FontFamily("Courier New").FontSize(7);

                column.Item().PaddingTop(8).AlignCenter().Text(podnaslov).Italic().FontSize(6.3f);
            });
        }

        private static void NapraviPotpise(IContainer container, UgovorViewModel ugovor)
        {
            Sekcija(container, "7. POTPISI UGOVORNIH STRANA", content =>
            {
                content.Padding(10).Column(column =>
                {
                    column.Item().Text("Potpisivanjem ovog ugovora obe strane potvrđuju da su saglasne sa svim navedenim uslovima.");
                    column.Item().PaddingTop(18).Row(row =>
                    {
                        row.RelativeItem().AlignCenter().Column(levo =>
                        {
                            levo.Item().AlignCenter().Text("KLIJENT").Bold();
                            levo.Item().PaddingTop(25).AlignCenter().Width(150).BorderBottom(0.8f).BorderColor(BojaLinije);
                            levo.Item().PaddingTop(8).AlignCenter().Text("Datum: ________________");
                        });

                        row.ConstantItem(1).Height(75).Background(BojaLinije);

                        row.RelativeItem().AlignCenter().Column(desno =>
                        {
                            desno.Item().AlignCenter().Text("ZAPOSLENI").Bold();
                            desno.Item().PaddingTop(25).AlignCenter().Width(150).BorderBottom(0.8f).BorderColor(BojaLinije);
                            desno.Item().PaddingTop(8).AlignCenter().Text("Datum: ________________");
                        });
                    });
                });
            });
        }

        private static void NapraviPodatkeOIzdavaocu(IContainer container, UgovorViewModel ugovor)
        {
            var zaposleni = ProcitajImeKorisnika(ugovor.KorisnikObjekat);

            Sekcija(container, "8. PODACI O IZDAVAOCU", content =>
            {
                content.Padding(10).Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("DRIVERENT").FontSize(17).Bold().LetterSpacing(1);
                        column.Item().Text("R E N T   A   C A R").FontSize(7).Bold();
                    });

                    row.RelativeItem().Column(column =>
                    {
                        column.Spacing(2);
                        column.Item().Text("DRIVERENT d.o.o.").Bold();
                        column.Item().Text("Bulevar Oslobođenja 123");
                        column.Item().Text("11000 Beograd, Srbija");
                        column.Item().Text("PIB: 123456789");
                        column.Item().Text("Tel: 011/123-4567");
                        column.Item().Text("www.driverent.rs");
                    });

                    row.RelativeItem().BorderLeft(0.7f).BorderColor(BojaLinije).PaddingLeft(10).Column(column =>
                    {
                        column.Spacing(3);
                        column.Item().Text("ZA INTERNU UPOTREBU").Bold();
                        column.Item().Text($"Korisnik: {zaposleni}");
                        column.Item().Text("Datum kreiranja: " + DateTime.Now.ToString("dd.MM.yyyy."));
                        column.Item().Text("Vreme kreiranja: " + DateTime.Now.ToString("HH:mm"));
                        column.Item().Text($"ID ugovora: {ugovor.Id}");
                    });
                });
            });
        }

        private static void Sekcija(IContainer container, string naslov, Action<IContainer> sadrzaj)
        {
            container.Border(0.8f).BorderColor(BojaLinije).Column(column =>
            {
                column.Item().Background(BojaZaglavlja).BorderBottom(0.8f).BorderColor(BojaLinije)
                    .PaddingVertical(5).PaddingHorizontal(7).Text(naslov).FontSize(8.5f).Bold();
                column.Item().Element(sadrzaj);
            });
        }

        private static void RedPodataka(ColumnDescriptor column, string naziv, string? vrednost)
        {
            column.Item().Row(row =>
            {
                row.ConstantItem(108).Text(naziv);
                row.RelativeItem().BorderBottom(0.5f).BorderColor(BojaLinije).PaddingBottom(1)
                    .Text(string.IsNullOrWhiteSpace(vrednost) ? "-" : vrednost);
            });
        }

        private static void StatusRed(ColumnDescriptor column, string opcija, string? izabranaOpcija)
        {
            var izabrano = string.Equals(opcija, izabranaOpcija, StringComparison.OrdinalIgnoreCase);

            column.Item().Row(row =>
            {
                row.ConstantItem(14).Border(0.7f).BorderColor(BojaLinije).Width(9).Height(9)
                    .AlignCenter().AlignMiddle().Text(izabrano ? "X" : string.Empty).FontSize(6).Bold();
                row.ConstantItem(5);
                row.RelativeItem().Text(opcija);
            });
        }

        private static void ZaglavljeTabele(TableCellDescriptor header, string tekst)
        {
            header.Cell().Background(BojaZaglavlja).Border(0.5f).BorderColor(BojaLinije)
                .Padding(4).AlignCenter().AlignMiddle().Text(tekst).Bold();
        }

        private static void CelijaTabele(TableDescriptor table, string? tekst)
        {
            table.Cell().Border(0.5f).BorderColor(BojaLinije).MinHeight(17)
                .Padding(3).AlignCenter().AlignMiddle().Text(tekst ?? string.Empty);
        }

        private static void DodatnaUsluga(ColumnDescriptor column, string naziv)
        {
            column.Item().Row(row =>
            {
                row.ConstantItem(13).Border(0.6f).BorderColor(BojaLinije).Width(8).Height(8);
                row.ConstantItem(4);
                row.RelativeItem(1.2f).Text(naziv);
                row.RelativeItem().Text("Cena: ______");
                row.RelativeItem().Text("Dana: ____");
                row.RelativeItem().AlignRight().Text("Ukupno: ______");
            });
        }

        private static void RedObracuna(ColumnDescriptor column, string naziv, decimal iznos)
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Text(naziv);
                row.ConstantItem(70).BorderBottom(0.5f).BorderColor(BojaLinije).AlignRight().Text(FormatirajNovac(iznos));
            });
        }

        private static void Uslov(ColumnDescriptor column, string tekst)
        {
            column.Item().Text("• " + tekst);
        }

        private static string NapraviNacinPlacanja(string? nacinPlacanja)
        {
            return string.Join("     ", Cekiranje("Gotovina", nacinPlacanja), Cekiranje("Kartica", nacinPlacanja), Cekiranje("Online", nacinPlacanja));
        }

        private static string Cekiranje(string opcija, string? izabranaOpcija)
        {
            var izabrano = string.Equals(opcija, izabranaOpcija, StringComparison.OrdinalIgnoreCase);
            return izabrano ? "[X] " + opcija : "[ ] " + opcija;
        }

        private static string FormatirajNovac(decimal iznos)
        {
            return iznos.ToString("#,##0.00", SrpskaKultura);
        }

        private static string FormatirajDatum(DateTime datum)
        {
            return datum.ToString("dd.MM.yyyy.");
        }

        private static string FormatirajDatumVreme(DateTime datum)
        {
            return datum.ToString("dd.MM.yyyy. 'u' HH:mm");
        }

        private static string ProcitajImeKorisnika(object? korisnik)
        {
            if (korisnik == null)
                return "-";

            var tip = korisnik.GetType();
            var imePrezime = ProcitajSvojstvo(tip, korisnik, "ImePrezime");
            if (!string.IsNullOrWhiteSpace(imePrezime))
                return imePrezime;

            var ime = ProcitajSvojstvo(tip, korisnik, "Ime");
            var prezime = ProcitajSvojstvo(tip, korisnik, "Prezime");
            var spojeno = $"{ime} {prezime}".Trim();
            if (!string.IsNullOrWhiteSpace(spojeno))
                return spojeno;

            var email = ProcitajSvojstvo(tip, korisnik, "Email");
            return string.IsNullOrWhiteSpace(email) ? "-" : email;
        }

        private static string? ProcitajSvojstvo(Type tip, object objekat, string nazivSvojstva)
        {
            var svojstvo = tip.GetProperty(
                nazivSvojstva,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            return svojstvo?.GetValue(objekat)?.ToString();
        }
    }
}