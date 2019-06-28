using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KLIENT
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow AppWindow;

        public static string potwierdzenie_operacji = "";
        public bool uzycie_kodu_BLIK = false;

        public MainWindow()
        {
            InitializeComponent();
            AppWindow = this;
            Tekst_Niepoprawne_Znaki.Text = Tekst_Niepoprawne_Znaki.Text + Obsluga_Konta.separator_wiadomosci;

        }

        private void Znikanie_bledow()
        {
            Tekst_Puste_Pola_Logowania.Visibility = Visibility.Collapsed;
            Tekst_Puste_Pole_BLIK.Visibility = Visibility.Collapsed;
            Tekst_Bledny_BLIK.Visibility = Visibility.Collapsed;
            Tekst_Niewazny_BLIK.Visibility = Visibility.Collapsed;
            Tekst_Poprawne_Login.Visibility = Visibility.Collapsed;
            Tekst_Niepoprawne_Znaki.Visibility = Visibility.Collapsed;
            Tekst_Brak_Srodkow_Na_koncie.Visibility = Visibility.Collapsed;
            Tekst_Wystapienie_Bledu_Wyplaty.Visibility = Visibility.Collapsed;
            Tekst_Wystapienie_Bledu_Wplaty.Visibility = Visibility.Collapsed;
            Tekst_Potwierdzenie_Przelewu.Visibility = Visibility.Collapsed;
            Tekst_Podaj_Kwote_Do_Wyplaty.Visibility = Visibility.Collapsed;
            Tekst_Podaj_Kowte_Wplaty.Visibility = Visibility.Collapsed;
            Tekst_Odrzucenie_Przelewu_BLIK.Visibility = Visibility.Collapsed;
            Tekst_Potwierdzenie_Przelewu_BLIK.Visibility = Visibility.Collapsed;
            Tekst_Wystapienie_Bledu_Wyplaty_BLIK.Visibility = Visibility.Collapsed;
            Tekst_Podaj_Kwote_Do_Wyplaty_BLIK.Visibility = Visibility.Collapsed;
            Tekst_Brak_Srodkow_Na_koncie_BLIK.Visibility = Visibility.Collapsed;
            Tekst_Blad_Przelewu_BLIK.Visibility = Visibility.Collapsed;
            Przycisk_Wyplac_BLIK.Visibility = Visibility.Visible;
            Tekst_Niewazny_Kod_BLIK.Visibility = Visibility.Collapsed;
        }

        private void Znikanie_ekranow()
        {
            Ekran_Logowania.Visibility = Visibility.Collapsed;
            Ekran_Obsluga_Konta.Visibility = Visibility.Collapsed;
            Ekran_Podaj_BLIK.Visibility = Visibility.Collapsed;
            Ekran_Powitalny.Visibility = Visibility.Collapsed;
            Ekran_Wplaty.Visibility = Visibility.Collapsed;
            Ekran_Wylogowania.Visibility = Visibility.Collapsed;
            Ekran_Wyplaty.Visibility = Visibility.Collapsed;
            Ekran_Obsluga_BLIK.Visibility = Visibility.Collapsed;
            Ekran_Wyplaty_BLIK.Visibility = Visibility.Collapsed;
            Ekran_Oczekiwanie_Potwierdzenia_BLIK.Visibility = Visibility.Collapsed;
        }

        private void Blokada_liter(KeyEventArgs klawisz)
        {
            klawisz.Handled = klawisz.Key >= Key.NumPad0 && klawisz.Key <= Key.NumPad9 ? false : true;
        }

        private void Przycisk_Przejdz_Do_Zaloguj_Click(object sender, RoutedEventArgs e)
        {
            Obsluga_Konta.Czyszczenie();
            Klient.Polacz();
            if (Klient.IsConnected == true)
            {
                Znikanie_ekranow();
                Ekran_Logowania.Visibility = Visibility.Visible;
            }
        }

        private void Przycisk_Przejdz_Do_BLIK_Click(object sender, RoutedEventArgs e)
        {
            Obsluga_Konta.Czyszczenie();
            if (Klient.IsConnected == true)
            {
                Znikanie_ekranow();
                Ekran_Podaj_BLIK.Visibility = Visibility.Visible;
            }
            else
            {
                Klient.Polacz();
            }
            uzycie_kodu_BLIK = false;
        }

        private void Przycisk_logowania(object sender, RoutedEventArgs e)
        {
            Znikanie_bledow();
            if (Pobierz_Login.Text == "" | Pobierz_Haslo.Password == "")
            {
                Tekst_Puste_Pola_Logowania.Visibility = Visibility.Visible;
            }
            else if(Pobierz_Login.Text.IndexOf(Obsluga_Konta.separator_wiadomosci)  >= 0 || 
                Pobierz_Haslo.Password.IndexOf(Obsluga_Konta.separator_wiadomosci) >= 0)
            {
                Tekst_Niepoprawne_Znaki.Visibility = Visibility.Visible;
            }
            else
            {
                bool potwierdzenie = false;
                potwierdzenie = Obsluga_Konta.Zaloguj(potwierdzenie);
                if (potwierdzenie == true)
                {
                    Znikanie_ekranow();
                    Ekran_Obsluga_Konta.Visibility = Visibility.Visible;
                    string etap = "03";
                    Obsluga_Konta.Pobierz_dane_konta(etap);
                }
                else
                {
                    Tekst_Poprawne_Login.Visibility = Visibility.Visible;
                }
            }
        }

        private void Przycisk_wylogowania(object sender, RoutedEventArgs e)
        {
            Obsluga_Konta.Czyszczenie();
            Znikanie_ekranow();
            Znikanie_bledow();
            Ekran_Wylogowania.Visibility = Visibility.Visible;
        }
        
        private void Przycisk_Wyplac_Click(object sender, RoutedEventArgs e)
        {
            Znikanie_bledow();
            if(Pobierz_Kwote_Do_Wyplaty.Text != "")
            {
                string potwierdzenie = "false";
                potwierdzenie = Obsluga_Konta.Wyplata(potwierdzenie);
                if (potwierdzenie == "true")
                {
                    Znikanie_ekranow();
                    Ekran_Obsluga_Konta.Visibility = Visibility.Visible;
                    Tekst_Potwierdzenie_Przelewu.Visibility = Visibility.Visible;
                    Pobierz_Kwote_Do_Wyplaty.Text = "";
                }
                else if (potwierdzenie == "brak_srodkow_na_koncie")
                {
                    Tekst_Brak_Srodkow_Na_koncie.Visibility = Visibility.Visible;
                }
                else
                {
                    Tekst_Wystapienie_Bledu_Wyplaty.Visibility = Visibility.Visible;
                }
            }
            else
            {
                Tekst_Podaj_Kwote_Do_Wyplaty.Visibility = Visibility.Visible;
            }
        }

        private void Przycisk_Wplac_Click(object sender, RoutedEventArgs e)
        {
            Znikanie_bledow();
            if(Pobierz_Kwote_Do_Wplaty.Text != "")
            {
                bool potwierdzenie = false;
                potwierdzenie = Obsluga_Konta.Wplata(potwierdzenie);
                if (potwierdzenie == true)
                {
                    Znikanie_ekranow();
                    Ekran_Obsluga_Konta.Visibility = Visibility.Visible;
                    Tekst_Potwierdzenie_Przelewu.Visibility = Visibility.Visible;
                    Pobierz_Kwote_Do_Wplaty.Text = "";
                }
                else
                {
                    Tekst_Wystapienie_Bledu_Wplaty.Visibility = Visibility.Visible;
                }
            }
            else
            {
                Tekst_Podaj_Kowte_Wplaty.Visibility = Visibility.Visible;
            }
        }

        private void Przycisk_Powrot_Do_Poczatku_Click(object sender, RoutedEventArgs e)
        {
            Znikanie_ekranow();
            Znikanie_bledow();
            Ekran_Powitalny.Visibility = Visibility.Visible;
        }

        private void Przycisk_Przejdz_Do_Wyplata_Click(object sender, RoutedEventArgs e)
        {
            Znikanie_ekranow();
            Znikanie_bledow();
            Ekran_Wyplaty.Visibility = Visibility.Visible;
        }

        private void Przycisk_Przejdz_Do_Wpłata_Click(object sender, RoutedEventArgs e)
        {
            Znikanie_ekranow();
            Znikanie_bledow();
            Ekran_Wplaty.Visibility = Visibility.Visible;
        }

        private void Przycisk_Powrot_Z_Wplaty_Do_Obslugi_Konta_Click(object sender, RoutedEventArgs e)
        {
            Znikanie_ekranow();
            Znikanie_bledow();
            Ekran_Obsluga_Konta.Visibility = Visibility.Visible;
            Pobierz_Kwote_Do_Wplaty.Text = "";
        }

        private void Przycisk_Powrot_Z_Wyplaty_Do_Obslugi_Konta_Click(object sender, RoutedEventArgs e)
        {
            Znikanie_ekranow();
            Znikanie_bledow();
            Ekran_Obsluga_Konta.Visibility = Visibility.Visible;
            Pobierz_Kwote_Do_Wyplaty.Text = "";
        }

        private void Przycisk_Powrot_Z_Wyplaty_Do_Obslugi_BLIK_Click(object sender, RoutedEventArgs e)
        {
            Znikanie_ekranow();
            Znikanie_bledow();
            Ekran_Obsluga_BLIK.Visibility = Visibility.Visible;
            Pobierz_Kwote_Do_Wyplaty_BLIK.Text = "";
        }

        private void FileExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(Klient.IsConnected == true)
            {
                Klient.Rozlacz();
            }
            this.Close();
        }

        private void Przycisk_Zatwierdz_BLIK_Click(object sender, RoutedEventArgs e)
        {
            string potwierdzenie = "false";
            Znikanie_bledow();
            if(Pobierz_BLIK.Text == "")
            {
                Tekst_Puste_Pole_BLIK.Visibility = Visibility.Visible;
            }
            else
            {
                potwierdzenie = Obsluga_Konta.Sprawdz_BLIK();
                if (potwierdzenie == "true")
                {
                    Znikanie_ekranow();
                    Ekran_Obsluga_BLIK.Visibility = Visibility.Visible;
                }
                else if(potwierdzenie == "niewazny")
                {
                    Tekst_Niewazny_BLIK.Visibility = Visibility.Visible;
                }
                else
                {
                    Tekst_Bledny_BLIK.Visibility = Visibility.Visible;
                }
            }
        }

        private void Blokada_Liter_KeyDown(object sender, KeyEventArgs e)
        {
            Blokada_liter(e);
        }

        private void Przycisk_Wyplac_BLIK_Click(object sender, RoutedEventArgs e)
        {
            if (Pobierz_Kwote_Do_Wyplaty_BLIK.Text != "" && uzycie_kodu_BLIK == false)
            {
                Znikanie_bledow();
                Obsluga_Konta.Wyplata_BLIK();
                if (potwierdzenie_operacji == "oczekiwanie")
                {
                    Znikanie_ekranow();
                    Ekran_Oczekiwanie_Potwierdzenia_BLIK.Visibility = Visibility.Visible;
                    uzycie_kodu_BLIK = true;
                    Obsluga_Konta.Sprawdzanie_Statusu_BLIK();
                }
                else if (potwierdzenie_operacji == "brak_srodkow_na_koncie")
                {
                    Tekst_Brak_Srodkow_Na_koncie_BLIK.Visibility = Visibility.Visible;
                }
                else
                {
                    Tekst_Wystapienie_Bledu_Wyplaty_BLIK.Visibility = Visibility.Visible;
                }
            }
            else
            {
                Tekst_Podaj_Kwote_Do_Wyplaty_BLIK.Visibility = Visibility.Visible;
            }
        }

        public void Potwierdzenie_Kodu_Przez_Klienta()
        {
            Znikanie_bledow();
            Znikanie_ekranow();
            Ekran_Obsluga_BLIK.Visibility = Visibility.Visible;
            Przycisk_Wyplac_BLIK.Visibility = Visibility.Collapsed;
            if (Obsluga_Konta.potwierdzenie_BLIK == "potwierdzono")
            {
                if(Obsluga_Konta.Aktualizacja_Salda() == "false")
                {
                    Tekst_Blad_Przelewu_BLIK.Visibility = Visibility.Visible;
                }
                else
                {
                    MainWindow.AppWindow.Pobierz_BLIK.Text = "";
                    Tekst_Potwierdzenie_Przelewu_BLIK.Visibility = Visibility.Visible;
                    Wyswietl_Saldo_BLIK.Text = Obsluga_Konta.saldo;
                }
            }
            else if(Obsluga_Konta.potwierdzenie_BLIK == "odrzucono")
            {
                Tekst_Odrzucenie_Przelewu_BLIK.Visibility = Visibility.Visible;
            }
            else
            {
                Tekst_Blad_Przelewu_BLIK.Visibility = Visibility.Visible;
            }
        }

        private void Przycisk_Przejdz_Do_Wyplata_BLIK_Click(object sender, RoutedEventArgs e)
        {
            if(uzycie_kodu_BLIK == false)
            {
                Znikanie_bledow();
                Znikanie_ekranow();
                Ekran_Wyplaty_BLIK.Visibility = Visibility.Visible;
            }
            else
            {
                Tekst_Niewazny_Kod_BLIK.Visibility = Visibility.Visible;
            }
        }

        public void Brak_Potwierdzenia_Kodu_BLIK()
        {
            Znikanie_ekranow();
            Znikanie_bledow();
            Ekran_Obsluga_BLIK.Visibility = Visibility.Visible;
            Tekst_Wystapienie_Bledu_Wyplaty_BLIK.Visibility = Visibility.Visible;
        }
    }
}
