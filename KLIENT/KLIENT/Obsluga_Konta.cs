using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Cryptography;
using System.Threading;

namespace KLIENT
{
    public class Obsluga_Konta : Klient
    {
        public static string imie = "";
        public static string nazwisko = "";
        public static string saldo = "";
        public static string numer_konta = "";
        public static string id_konta = "";
        public static string kod_BLIK = "";
        public static string id_BLIK = "";
        public static string separator_wiadomosci = "_";
        public static string potwierdzenie_BLIK = "true";

        public async static void Sprawdzanie_Statusu_BLIK()
        {
            potwierdzenie_BLIK = "true";
            string etap = "13";
            string wiadomosc = Polacz_2_skladniki(etap, id_BLIK);
            string odbierz = "";
            for (int i = 1; i < 7; i++)
            {
                Klient.Wyslij(wiadomosc);
                odbierz = Klient.Odbierz();
                if(odbierz == "potwierdzono" || odbierz == "odrzucono")
                {
                    potwierdzenie_BLIK = odbierz;
                    MainWindow.AppWindow.Potwierdzenie_Kodu_Przez_Klienta();
                    i = 10;
                    break;
                }
                else if (i < 6)
                {
                    await Task.Delay(5000);
                }
                else
                {
                    potwierdzenie_BLIK = "false";
                    MainWindow.AppWindow.Brak_Potwierdzenia_Kodu_BLIK();
                }
            }
        }

        public static string Polacz_2_skladniki(string podaj_numer_polecenia, string podaj_dane_do_wyslania)
        {
            string wiadomosc_do_wyslania = podaj_numer_polecenia + separator_wiadomosci + podaj_dane_do_wyslania;
            return wiadomosc_do_wyslania;
        }

        public static string Polacz_3_skladniki(string podaj_numer_polecenia, string podaj_id_konta, string podaj_dane_do_wyslania)
        {
            string wiadomosc_do_wyslania = podaj_numer_polecenia + separator_wiadomosci + podaj_id_konta + separator_wiadomosci + podaj_dane_do_wyslania;
            return wiadomosc_do_wyslania;
        }

        public static string Oddzielanie_2_skladnikow_2_czesc_wiadomosci(string podaj_wiadomosc)
        {
            string odzielona_wiadomosc = podaj_wiadomosc.Substring((podaj_wiadomosc.IndexOf(separator_wiadomosci) + 1));
            return odzielona_wiadomosc;
        }

        public static string Oddzielanie_2_skladnikow_1_czesc_wiadomosci(string podaj_wiadomosc)
        {
            string odzielona_wiadomosc = podaj_wiadomosc.Substring(0, podaj_wiadomosc.IndexOf(separator_wiadomosci));
            return odzielona_wiadomosc;
        }

        public static int Zamiana_na_int_odejmowanie(string kwota1, string kwota2)
        {
            try
            {
                int s1 = Int32.Parse(kwota1);
                int s2 = Int32.Parse(kwota2);
                int s3 = s1 - s2;
                return s3;
            }
            catch
            {
                MessageBox.Show("Podana kwota jest za duża!");
                return 0;
            }
        }

        public static int Zamiana_na_int_dodawanie(string kwota1, string kwota2)
        {
            try
            {
                int s1 = Int32.Parse(kwota1);
                int s2 = Int32.Parse(kwota2);
                int s3 = s1 + s2;
                return s3;
            }
            catch
            {
                MessageBox.Show("Podana kwota jest za duża!");
                return 0;
            }
        }

        public static bool Zaloguj(bool potwierdzenie)
        {
            string etap_logowania = "01";
            bool status_logowania = false;
            string wiadomosc = "";

            while (status_logowania == false)
            {
                switch (etap_logowania)
                {
                    case "01":   //Sprawdz_Login
                        wiadomosc = Polacz_2_skladniki(etap_logowania, MainWindow.AppWindow.Pobierz_Login.Text);
                        Klient.Wyslij(wiadomosc);
                        wiadomosc = Klient.Odbierz();
                        etap_logowania = Oddzielanie_2_skladnikow_1_czesc_wiadomosci(wiadomosc);
                        if (etap_logowania == "true")
                        {
                            id_konta = Oddzielanie_2_skladnikow_2_czesc_wiadomosci(wiadomosc);
                            etap_logowania = "02";
                        }
                        else
                        {
                            potwierdzenie = false;
                            status_logowania = true;
                        }
                        break;
                    case "02":   //Sprawdz_Haslo
                        wiadomosc = MainWindow.AppWindow.Pobierz_Haslo.Password;
                        wiadomosc = Polacz_3_skladniki(etap_logowania, id_konta, Kodowanie(MainWindow.AppWindow.Pobierz_Haslo.Password));
                        Klient.Wyslij(wiadomosc);
                        if (Klient.Odbierz() == "true")
                        {
                            potwierdzenie = true;
                            status_logowania = true;
                        }
                        else
                        {
                            potwierdzenie = false;
                            status_logowania = true;
                        }
                        break;
                    default:
                        status_logowania = true;
                        potwierdzenie = false;
                        break;
                }
            }
            return potwierdzenie;

        }

        public static void Pobierz_dane_konta(string etap)
        {
            string wiadomosc = "";
            string etap_pobierania_danych_konta = etap;
            bool status_pobierania = false;

            while (status_pobierania == false)
            {
                switch (etap_pobierania_danych_konta)
                {
                    case "03":    //Pobierz_Imie
                        wiadomosc = Polacz_2_skladniki(etap_pobierania_danych_konta, id_konta);
                        Klient.Wyslij(wiadomosc);
                        imie = Klient.Odbierz();
                        MainWindow.AppWindow.Wyswietl_Imie.Text = imie;
                        etap_pobierania_danych_konta = "04";
                        break;
                    case "04":    //Pobierz_Nazwisko
                        wiadomosc = Polacz_2_skladniki(etap_pobierania_danych_konta, id_konta);
                        Klient.Wyslij(wiadomosc);
                        nazwisko = Klient.Odbierz();
                        MainWindow.AppWindow.Wyswietl_Nazwisko.Text = nazwisko;
                        etap_pobierania_danych_konta = "06";
                        break;
                    case "05":   //Pobierz_Saldo
                        wiadomosc = Polacz_2_skladniki(etap_pobierania_danych_konta, id_konta);
                        Klient.Wyslij(wiadomosc);
                        saldo = Klient.Odbierz();
                        MainWindow.AppWindow.Wyswietl_Saldo.Text = saldo;
                        MainWindow.AppWindow.Wyswietl_Saldo_BLIK.Text = saldo;
                        status_pobierania = true;
                        break;
                    case "06":     //Pobierz_Numer_Konta
                        wiadomosc = Polacz_2_skladniki(etap_pobierania_danych_konta, id_konta);
                        Klient.Wyslij(wiadomosc);
                        numer_konta = Klient.Odbierz();
                        MainWindow.AppWindow.Wyswietl_Numer_Konta.Text = numer_konta;
                        etap_pobierania_danych_konta = "05";
                        break;
                    default:
                        status_pobierania = true;
                        break;
                }
            }
        }

        public static string Aktualizacja_Salda()
        {
            string etap = "10";
            Klient.Wyslij(Polacz_3_skladniki(etap, id_konta, saldo));
            string wiadomosc = Klient.Odbierz();
            return wiadomosc;
        }

        public static void Pobierz_Saldo()
        {
            string wiadomosc = "";
            string etap_pobierania_danych_konta = "05";
            wiadomosc = Polacz_2_skladniki(etap_pobierania_danych_konta, id_BLIK);
            Klient.Wyslij(wiadomosc);
            saldo = Klient.Odbierz();
            MainWindow.AppWindow.Wyswietl_Saldo_BLIK.Text = saldo;
        }

        public static string Wyplata(string potwierdzenie)
        {
            string wiadomosc = "";
            string etap_wyplaty = "Sprawdz_mozliwosc_wyplaty";
            bool status_wyplaty = false;
            string pobierz_kwote = MainWindow.AppWindow.Pobierz_Kwote_Do_Wyplaty.Text;
            int nowe_saldo = Zamiana_na_int_odejmowanie(saldo, pobierz_kwote);

            while (status_wyplaty == false)
            {
                switch (etap_wyplaty)
                {
                    case "Sprawdz_mozliwosc_wyplaty":
                        if (nowe_saldo >= 0)
                        {
                            etap_wyplaty = "Zgoda_na_wyplate";
                        }
                        else
                        {
                            potwierdzenie = "brak_srodkow_na_koncie";
                            status_wyplaty = true;
                        }
                        break;
                    case "Zgoda_na_wyplate":
                        etap_wyplaty = "10";       //10 == Wyplata
                        wiadomosc = Polacz_3_skladniki(etap_wyplaty, id_konta, nowe_saldo.ToString());
                        Klient.Wyslij(wiadomosc);
                        if (Klient.Odbierz() == "true")
                        {
                            saldo = nowe_saldo.ToString();
                            MainWindow.AppWindow.Wyswietl_Saldo.Text = saldo;
                            potwierdzenie = "true";
                            status_wyplaty = true;
                        }
                        else
                        {
                            potwierdzenie = "false";
                            status_wyplaty = true;
                        }
                        break;
                    default:
                        potwierdzenie = "false";
                        status_wyplaty = true;
                        break;
                }
            }
            return potwierdzenie;
        }

        public static void Wyplata_BLIK()
        {
            string potwierdzenie = "";
            string wiadomosc = "";
            string etap_wyplaty = "Sprawdz_mozliwosc_wyplaty";
            bool status_wyplaty = false;
            string pobierz_kwote = MainWindow.AppWindow.Pobierz_Kwote_Do_Wyplaty_BLIK.Text;
            int nowe_saldo = Zamiana_na_int_odejmowanie(saldo, pobierz_kwote);

            while (status_wyplaty == false)
            {
                switch (etap_wyplaty)
                {
                    case "Sprawdz_mozliwosc_wyplaty":
                        if (nowe_saldo >= 0)
                        {
                            etap_wyplaty = "Zgoda_na_wyplate";
                        }
                        else
                        {
                            potwierdzenie = "brak_srodkow_na_koncie";
                            status_wyplaty = true;
                        }
                        break;
                    case "Zgoda_na_wyplate":
                        etap_wyplaty = "11";        //Aktualizacja Stanu BLIK
                        wiadomosc = Polacz_3_skladniki(etap_wyplaty, id_BLIK, "oczekiwanie");
                        Klient.Wyslij(wiadomosc);
                        if(Klient.Odbierz() == "true")
                        {
                            etap_wyplaty = "12";    //Aktualizacja Kwoty Transakcji BLIK
                        }
                        else
                        {
                            potwierdzenie = "false";
                            status_wyplaty = true;
                        }
                        Sprawdzanie_Statusu_BLIK();
                        break;
                    case "12":
                        wiadomosc = Polacz_3_skladniki(etap_wyplaty, id_BLIK,pobierz_kwote);
                        Klient.Wyslij(wiadomosc);
                        if(Klient.Odbierz() == "true")
                        {
                            potwierdzenie = "oczekiwanie";
                            saldo = nowe_saldo.ToString(); ;
                        }
                        else
                        {
                            potwierdzenie = "false";
                        }
                        status_wyplaty = true;
                        break;
                    default:
                        potwierdzenie = "false";
                        status_wyplaty = true;
                        break;
                }
            }
            MainWindow.potwierdzenie_operacji = potwierdzenie;
        }

        public static bool Wplata(bool potwierdzenie)
        {
            string wiadomosc = "";
            string kwota_do_wplaty = MainWindow.AppWindow.Pobierz_Kwote_Do_Wplaty.Text;
            int nowe_saldo = Zamiana_na_int_dodawanie(saldo, kwota_do_wplaty);
            string etap_wplaty = "09";  //09 == Wplata

            wiadomosc = Polacz_3_skladniki(etap_wplaty, id_konta, nowe_saldo.ToString());
            Klient.Wyslij(wiadomosc);
            if (Klient.Odbierz() == "true")
            {
                saldo = nowe_saldo.ToString();
                MainWindow.AppWindow.Wyswietl_Saldo.Text = saldo;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string Sprawdz_BLIK()
        {
            kod_BLIK = MainWindow.AppWindow.Pobierz_BLIK.Text;
            string etap = "08";      //Sprawdz BLIK
            string wiadomosc = "";
            wiadomosc = Polacz_2_skladniki(etap, kod_BLIK);
            Klient.Wyslij(wiadomosc);
            wiadomosc = Klient.Odbierz();
            etap = Oddzielanie_2_skladnikow_1_czesc_wiadomosci(wiadomosc);
            if (etap == "true")
            {
                id_BLIK = Oddzielanie_2_skladnikow_2_czesc_wiadomosci(wiadomosc);
                id_konta = id_BLIK;
                Pobierz_Saldo();
            }
            return etap;
        }
        
        public static void Czyszczenie()
        {
            MainWindow.AppWindow.Wyswietl_Imie.Text = "";
            MainWindow.AppWindow.Wyswietl_Nazwisko.Text = "";
            MainWindow.AppWindow.Wyswietl_Numer_Konta.Text = "";
            MainWindow.AppWindow.Wyswietl_Saldo.Text = "";
            MainWindow.AppWindow.Wyswietl_Saldo_BLIK.Text = "";
            MainWindow.AppWindow.Pobierz_Haslo.Clear();
            MainWindow.AppWindow.Pobierz_Login.Text = "";
            MainWindow.AppWindow.Pobierz_Kwote_Do_Wplaty.Text = "";
            MainWindow.AppWindow.Pobierz_Kwote_Do_Wyplaty.Text = "";
            MainWindow.AppWindow.Pobierz_Kwote_Do_Wyplaty_BLIK.Text = "";
            MainWindow.AppWindow.Pobierz_BLIK.Text = "";
            imie = "";
            nazwisko = "";
            saldo = "";
            numer_konta = "";
            id_konta = "";
        }

        private static string Kodowanie(string zakodowana_wiadomosc)
        {
            string pobierz_wiadomosc = zakodowana_wiadomosc;
            MD5 md5Hash = MD5.Create();
            string zakodowany_tekst = GetMd5Hash(md5Hash, pobierz_wiadomosc);

            return zakodowany_tekst;
        }

        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.Unicode.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

    }
}
