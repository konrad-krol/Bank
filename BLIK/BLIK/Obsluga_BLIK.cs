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

namespace BLIK
{
    public class Obsluga_BLIK : Blik
    {
        public static string imie = "";
        public static string nazwisko = "";
        public static string saldo = "";
        public static string numer_konta = "";
        public static string id_konta = "";
        public static string kod_BLIK = "";
        public static string kwota_transakcji = "";
        public static string separator_wiadomosci = "_";
        public static string status_BLIK = "false";
        public static bool potwierdzam_transakcje = false;

        public async static void Sprawdzanie_Stanu_BLIK()
        {
            string etap = "13";
            string wiadomosc = Polacz_2_skladniki(etap, id_konta);
            string odebrana_wiadomosc = "";
            for (int i = 1; i < 37; i++)
            {
                Wyslij(wiadomosc);
                odebrana_wiadomosc = Odbierz();
                if(odebrana_wiadomosc == "oczekiwanie")
                {
                    etap = "14";
                    wiadomosc = Polacz_2_skladniki(etap, id_konta);
                    Wyslij(wiadomosc);
                    odebrana_wiadomosc = Odbierz();
                    if(odebrana_wiadomosc != "false")
                    {
                        kwota_transakcji = odebrana_wiadomosc;
                        MainWindow.AppWindow.Potwierdzenie_Transakcji_BLIK(kwota_transakcji);
                        i = 100;
                    }
                    break;
                }
                else if(i <36)
                {
                    await Task.Delay(5000);
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
                        Blik.Wyslij(wiadomosc);
                        wiadomosc = Blik.Odbierz();
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
                        Blik.Wyslij(wiadomosc);
                        if (Blik.Odbierz() == "true")
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

        public static void Pobierz_dane_konta()
        {
            string wiadomosc = "";
            string etap_pobierania_danych_konta = "03";
            bool status_pobierania = false;

            while (status_pobierania == false)
            {
                switch (etap_pobierania_danych_konta)
                {
                    case "03":    //Pobierz_Imie
                        wiadomosc = Polacz_2_skladniki(etap_pobierania_danych_konta, id_konta);
                        Blik.Wyslij(wiadomosc);
                        imie = Blik.Odbierz();
                        MainWindow.AppWindow.Wyswietl_Imie.Text = imie;
                        etap_pobierania_danych_konta = "04";
                        break;
                    case "04":    //Pobierz_Nazwisko
                        wiadomosc = Polacz_2_skladniki(etap_pobierania_danych_konta, id_konta);
                        Blik.Wyslij(wiadomosc);
                        nazwisko = Blik.Odbierz();
                        MainWindow.AppWindow.Wyswietl_Nazwisko.Text = nazwisko;
                        etap_pobierania_danych_konta = "06";
                        break;
                    case "05":   //Pobierz_Saldo
                        wiadomosc = Polacz_2_skladniki(etap_pobierania_danych_konta, id_konta);
                        Blik.Wyslij(wiadomosc);
                        saldo = Blik.Odbierz();
                        MainWindow.AppWindow.Wyswietl_Saldo.Text = saldo;
                        status_pobierania = true;
                        break;
                    case "06":     //Pobierz_Numer_Konta
                        wiadomosc = Polacz_2_skladniki(etap_pobierania_danych_konta, id_konta);
                        Blik.Wyslij(wiadomosc);
                        numer_konta = Blik.Odbierz();
                        MainWindow.AppWindow.Wyswietl_Numer_Konta.Text = numer_konta;
                        etap_pobierania_danych_konta = "05";
                        break;
                    default:
                        status_pobierania = true;
                        break;
                }
            }
        }
        
        public static string Aktualizacja_Statusu_BLIK(string status)
        {
            string etap = "11";        //Aktualizacja Salda : Aktualizacja Statusu
            string wiadomosc = Polacz_3_skladniki(etap, id_konta, status);
            Blik.Wyslij(wiadomosc);
            etap = Blik.Odbierz();
            if(etap == "true" && potwierdzam_transakcje == true)
            {
                etap = "05";        //Pobierz_Saldo
                wiadomosc = Polacz_2_skladniki(etap, id_konta);
                Blik.Wyslij(wiadomosc);
                int nowe_saldo = Int32.Parse(saldo) - Int32.Parse(kwota_transakcji);
                saldo = nowe_saldo.ToString();
                MainWindow.AppWindow.Wyswietl_Saldo.Text = saldo;
                etap = "true";
            }
            return etap;
        }

        public static bool Generuj_BLIK(bool potwierdzenie_generacji_BLIK)
        {
            string etap = "07";     //Generowanie Kodu BLIK
            string wiadomosc = "";
            wiadomosc = Polacz_2_skladniki(etap, id_konta);
            Blik.Wyslij(wiadomosc);
            wiadomosc = Blik.Odbierz();
            if(Oddzielanie_2_skladnikow_1_czesc_wiadomosci(wiadomosc) == "true")
            {
                kod_BLIK = Oddzielanie_2_skladnikow_2_czesc_wiadomosci(wiadomosc);
                MainWindow.AppWindow.Wyswietl_Kod_BLIK.Text = kod_BLIK;
                potwierdzenie_generacji_BLIK = true;
            }
            else
            {
                potwierdzenie_generacji_BLIK = false;
            }

            return potwierdzenie_generacji_BLIK;
        }

        public static void Czyszczenie()
        {
            MainWindow.AppWindow.Wyswietl_Imie.Text = "";
            MainWindow.AppWindow.Wyswietl_Nazwisko.Text = "";
            MainWindow.AppWindow.Wyswietl_Saldo.Text = "";
            MainWindow.AppWindow.Wyswietl_Numer_Konta.Text = "";
            MainWindow.AppWindow.Wyswietl_Kod_BLIK.Text = "";
            MainWindow.AppWindow.Wyswietl_Kwota_Wyplaty.Text = "Kwota";
            MainWindow.AppWindow.Pobierz_Haslo.Clear();
            MainWindow.AppWindow.Pobierz_Login.Text = "";
            MainWindow.AppWindow.Wyswietl_Data_Utworzenia_Blik.Text = "";
            imie = "";
            nazwisko = "";
            saldo = "";
            numer_konta = "";
            id_konta = "";
            kwota_transakcji = "";
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