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

namespace BLIK
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow AppWindow;

        public static MainWindow _MainWindow;

        public MainWindow()
        {
            InitializeComponent();
            AppWindow = this;
            Tekst_Niepoprawne_Znaki.Text = Tekst_Niepoprawne_Znaki.Text + Obsluga_BLIK.separator_wiadomosci;
        }

        private void Znikanie_bledow()
        {
            Tekst_Puste_Pola_Logowania.Visibility = Visibility.Collapsed;
            Tekst_Poprawne_Login.Visibility = Visibility.Collapsed;
            Tekst_Niepoprawne_Znaki.Visibility = Visibility.Collapsed;
            Tekst_Blad_Generowania_Kodu_BLIK.Visibility = Visibility.Collapsed;
            Tekst_Potwierdzenie_Generowania_Kodu_BLIK.Visibility = Visibility.Collapsed;
            Tekst_Blad_Transakcji.Visibility = Visibility.Collapsed;
            Tekst_Potwierdzenie_Transakcji.Visibility = Visibility.Collapsed;
        }

        private void Znikanie_ekranow()
        {
            Ekran_Logowania.Visibility = Visibility.Collapsed;
            Ekran_Obsluga_Konta.Visibility = Visibility.Collapsed;
            Ekran_Powitalny.Visibility = Visibility.Collapsed;
            Ekran_Wylogowania.Visibility = Visibility.Collapsed;
            Ekran_Potwierdzenia_Transakcji.Visibility = Visibility.Collapsed;
        }

        private void Przycisk_Przejdz_Do_Zaloguj_Click(object sender, RoutedEventArgs e)
        {
            Blik.Polacz();
            if (Blik.IsConnected == true)
            {
                Znikanie_ekranow();
                Ekran_Logowania.Visibility = Visibility.Visible;
            }
        }

        private void Przycisk_logowania(object sender, RoutedEventArgs e)
        {
            if (Pobierz_Login.Text == "" | Pobierz_Haslo.Password == "")
            {
                Znikanie_bledow();
                Tekst_Puste_Pola_Logowania.Visibility = Visibility.Visible;
            }
            else if (Pobierz_Login.Text.IndexOf(Obsluga_BLIK.separator_wiadomosci) >= 0 |
                Pobierz_Haslo.Password.IndexOf(Obsluga_BLIK.separator_wiadomosci) >= 0)
            {
                Znikanie_bledow();
                Tekst_Niepoprawne_Znaki.Visibility = Visibility.Visible;
            }
            else
            {
                Znikanie_bledow();
                bool potwierdzenie = false;
                potwierdzenie = Obsluga_BLIK.Zaloguj(potwierdzenie);
                if (potwierdzenie == true)
                {
                    Znikanie_ekranow();
                    Znikanie_bledow();
                    Ekran_Obsluga_Konta.Visibility = Visibility.Visible;
                    Obsluga_BLIK.Pobierz_dane_konta();
                }
                else
                {
                    Tekst_Poprawne_Login.Visibility = Visibility.Visible;
                }
            }
        }

        private void Przycisk_wylogowania(object sender, RoutedEventArgs e)
        {
            Obsluga_BLIK.Czyszczenie();
            Znikanie_bledow();
            Znikanie_ekranow();
            Ekran_Wylogowania.Visibility = Visibility.Visible;
        }
        
        private void Przycisk_Powrot_Do_Poczatku_Click(object sender, RoutedEventArgs e)
        {
            Znikanie_ekranow();
            Znikanie_bledow();
            Ekran_Powitalny.Visibility = Visibility.Visible;
        }

        private void FileExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Blik.IsConnected == true)
            {
                Blik.Rozlacz();
            }

            this.Close();
        }

        private void Przycisk_Odrzuc_Transakcje_Click(object sender, RoutedEventArgs e)
        {
            Znikanie_ekranow();
            Obsluga_BLIK.potwierdzam_transakcje = false;
            string potwierdzenie = Obsluga_BLIK.Aktualizacja_Statusu_BLIK("odrzucono");
            Ekran_Obsluga_Konta.Visibility = Visibility.Visible;
        }

        private void Przycisk_Potwierdz_Transakcje_Click(object sender, RoutedEventArgs e)
        {
            Znikanie_ekranow();
            Ekran_Obsluga_Konta.Visibility = Visibility.Visible;
            Obsluga_BLIK.potwierdzam_transakcje = true;
            if (Obsluga_BLIK.Aktualizacja_Statusu_BLIK("potwierdzono") == "true")
            {
                Ekran_Obsluga_Konta.Visibility = Visibility.Visible;
                Tekst_Potwierdzenie_Transakcji.Visibility = Visibility.Visible;
            }
            else
            {
                Tekst_Blad_Transakcji.Visibility = Visibility.Visible;
            }
        }

        private void Przycisk_Generuj_Kod_BLIK_Click(object sender, RoutedEventArgs e)
        {
            bool potwierdzenie = false;
            potwierdzenie = Obsluga_BLIK.Generuj_BLIK(potwierdzenie);
            if(potwierdzenie == true)
            {
                Znikanie_bledow();
                Tekst_Potwierdzenie_Generowania_Kodu_BLIK.Visibility = Visibility.Visible;
                Wyswietl_Data_Utworzenia_Blik.Text = DateTime.Now.ToString();
                Obsluga_BLIK.Sprawdzanie_Stanu_BLIK();
            }
            else
            {
                Znikanie_bledow();
                Tekst_Blad_Generowania_Kodu_BLIK.Visibility = Visibility.Visible;
            }
        }

        public void Potwierdzenie_Transakcji_BLIK(string kwota)
        {
            Znikanie_bledow();
            Znikanie_ekranow();
            Ekran_Potwierdzenia_Transakcji.Visibility = Visibility.Visible;
            Wyswietl_Kwota_Wyplaty.Text = kwota;
        }
    }
}
