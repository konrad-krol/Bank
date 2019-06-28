using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Security.Cryptography;

namespace SERWER
{
    class Baza : Serwer
    {
        public static string separator_wiadomosci = "_";
        private static SqlConnection connection;
        public static string adres = ConfigurationManager.ConnectionStrings["SERWER.Properties.Settings.BANKConnectionString"].ConnectionString;
        
        public static void Polacz_SQL(string connectionString)
        {
            try
            {
                connection = new SqlConnection(connectionString);
                Serwer.AddStatus("Połączono z bazą danych");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void Rozlacz_SQL()
        {
            Zamknij_polaczenie_SQL();
        }

        private static bool Otworz_polaczenie_SQL()
        {
            try
            {
                connection.Open();
                Serwer.AddStatus("Otwarto połączenie z bazą danych");
                return true;
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 1042:
                        MessageBox.Show("Nie można połączyć się z serwerem");
                        break;
                    default:
                        MessageBox.Show("Nieznany błąd połaczenia");
                        break;
                }
                return false;
            }
            catch (Exception e)
            {
                Serwer.AddStatus(e.ToString());
                return false;
            }
        }

        private static bool Zamknij_polaczenie_SQL()
        {
            try
            {
                connection.Close();
                Serwer.AddStatus("Zamknięto połączenie z bazą danych");
                return true;
            }
            catch (SqlException ex)
            {
                Serwer.AddStatus(ex.ToString());
                return false;
            }
            catch (Exception e)
            {
                Serwer.AddStatus(e.ToString());
                return false;
            }
        }
        
        public static string Polacz_2_skladniki(string skladnik_1, string skladnik_2)
        {
            string wiadomosc_do_wyslania = skladnik_1 + separator_wiadomosci + skladnik_2;
            return wiadomosc_do_wyslania;
        }

        public static string Oddzielanie_skladnikow_1_czesc_wiadomosci(string podaj_wiadomosc)
        {
            podaj_wiadomosc = podaj_wiadomosc.Substring(0, podaj_wiadomosc.IndexOf(separator_wiadomosci));
            return podaj_wiadomosc;
        }

        public static string Oddzielanie_skladnikow_2_czesc_wiadomosci(string podaj_wiadomosc)
        {
            podaj_wiadomosc = podaj_wiadomosc.Substring((podaj_wiadomosc.IndexOf(separator_wiadomosci) + 1));
            return podaj_wiadomosc;
        }

        public static string Menu_glowne(string pobrana_wiadomosc)
        {
            string etap = Oddzielanie_skladnikow_1_czesc_wiadomosci(pobrana_wiadomosc);
            string wiadomosc = Oddzielanie_skladnikow_2_czesc_wiadomosci(pobrana_wiadomosc);

            switch (etap)
            {
                case "01":      //Login
                    pobrana_wiadomosc = Baza.Sprawdz_login(wiadomosc);
                    break;
                case "02":      //Haslo
                    pobrana_wiadomosc = Sprawdz_haslo(wiadomosc);
                    break;
                case "03":      //Imie
                case "04":      //Nazwisko
                case "05":      //Saldo
                case "06":      //Numer_Konta
                    pobrana_wiadomosc = Pobierz_dane_konta(pobrana_wiadomosc);
                    break;
                case "07":    //Generuj_BLIK
                    pobrana_wiadomosc = Generuj_kod_BLIK(wiadomosc);
                    break;
                case "08":    //Sprawdz_BLIK
                    pobrana_wiadomosc = Sprawdz_Poprawnosc_BLIK(wiadomosc);
                    break;
                case "09":      //Wplata
                case "10":     //Wyplata
                    pobrana_wiadomosc = Aktualizacja_saldo(wiadomosc);
                    break;
                case "11":      //Aktualizacja_Stanu_BLIK
                case "12":      //Aktualizacja_Kwoty_Transakcji_BLIK
                    pobrana_wiadomosc = Aktualizacja_Stanu_BLIK(etap, wiadomosc);
                    break;
                case "13":      //Sprawdz_Status_BLIK
                case "14":      //Kwota Transakcji BLIK
                    pobrana_wiadomosc = Sprawdz_potwierdzenie_uzycia_kodu_BLIK(etap, wiadomosc);
                    break;
                default:
                    pobrana_wiadomosc = "false";
                    break;
            }
            return pobrana_wiadomosc;
        }

        public static string Generuj_kod_BLIK(string podaj_id)
        {
            string id_konta = podaj_id;
            string wygenerowany_kod = "";
            bool poprawnosc_wygenerowanego_kodu = false;
            string weryfikacja = "false";

            while (poprawnosc_wygenerowanego_kodu == false)
            {
                Random losuj = new Random();
                string generacja_kodu = "";
                for (int i = 1; i <= 6; i++)
                {
                    generacja_kodu += losuj.Next(7).ToString();
                }
                wygenerowany_kod = generacja_kodu;
                if(Sprawdz_Istnienie_BLIK(wygenerowany_kod) == "false")
                {
                    weryfikacja = Aktualizacja_BLIK(wygenerowany_kod, id_konta);
                    if (weryfikacja == "true")
                    {
                        poprawnosc_wygenerowanego_kodu = true;
                        wygenerowany_kod = Polacz_2_skladniki("true", generacja_kodu);
                    }
                    else
                    {
                        poprawnosc_wygenerowanego_kodu = false;
                        wygenerowany_kod = Polacz_2_skladniki("false", wygenerowany_kod);
                    }
                }
                
            }
            return wygenerowany_kod;
        }

        public static string Aktualizacja_BLIK(string podaj_BLIK, string podaj_id_konta)
        {
            DateTime data_obecna = DateTime.Now;
            string potwierdzenie_uzycia = "brak";
            string query = "UPDATE dane_BLIK SET kod_BLIK=@kod_BLIK, " +
                "potwierdzenie_uzycia=@potwierdzenie_uzycia, " +
                "data_utworzenia=@data_utworzenia WHERE id_BLIK=@id_BLIK";

            if (Baza.Otworz_polaczenie_SQL() == true)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = query;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@kod_BLIK", podaj_BLIK);
                cmd.Parameters.AddWithValue("@id_BLIK", podaj_id_konta);
                cmd.Parameters.AddWithValue("@data_utworzenia", data_obecna);
                cmd.Parameters.AddWithValue("@potwierdzenie_uzycia", potwierdzenie_uzycia);
                cmd.ExecuteNonQuery();
                Baza.Zamknij_polaczenie_SQL();
                return "true";
            }
            else
            {
                return "false";
            }
        }

        public static string Aktualizacja_Stanu_BLIK(string etap, string dane)
        {
            string id_BLIK = Oddzielanie_skladnikow_1_czesc_wiadomosci(dane);
            string pobrana_wiadomosc = Oddzielanie_skladnikow_2_czesc_wiadomosci(dane);
            string query = "";

            switch(etap)
            {
                case "11":      //Aktualizacja_Stanu_BLIK
                    query = $"UPDATE dane_BLIK SET potwierdzenie_uzycia=\'{pobrana_wiadomosc}\' WHERE id_BLIK={id_BLIK}";
                    break;
                case "12":      //Aktualizacja_Kwoty_Transakcji_BLIK
                    query = $"UPDATE dane_BLIK SET kwota_transakcji=\'{pobrana_wiadomosc}\' WHERE id_BLIK={id_BLIK}";
                    break;
                default:
                    dane = "false";
                    break;
            }

            if (Baza.Otworz_polaczenie_SQL() == true)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = query;
                cmd.Connection = connection;
                cmd.ExecuteNonQuery();
                Baza.Zamknij_polaczenie_SQL();
                dane = "true";
            }
            else
            {
                dane = "false";
            }
            return dane;
        }

        public static string Sprawdz_Istnienie_BLIK(string podaj_BLIK)
        {
            string pobierz_dane = "";
            int cyfra_BLIK = Int32.Parse(podaj_BLIK);
            string query = $"SELECT * FROM dane_BLIK WHERE kod_BLIK=@kod_BLIK";

            if (Baza.Otworz_polaczenie_SQL() == true)
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.CommandText = query;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@kod_BLIK", cyfra_BLIK);
                SqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    pobierz_dane = dataReader["kod_BLIK"].ToString();
                }

                dataReader.Close();
                Baza.Zamknij_polaczenie_SQL();
            }

            if (pobierz_dane == "")
            {
                podaj_BLIK = "false";
                return podaj_BLIK;
            }
            else
            {
                podaj_BLIK = "true";
                return podaj_BLIK;
            }
        }

        public static string Sprawdz_Poprawnosc_BLIK(string podaj_BLIK)
        {
            string potwierdzenie;
            TimeSpan czas_istnienia_kodu_BLIK = new TimeSpan(0, 3, 0);
            DateTime data_obecna = DateTime.Now;
            DateTime data_pobrana = new DateTime(1, 1, 1, 0, 0, 0);
            string kod = podaj_BLIK;
            string id_konta = "";
            string pobierz_status = "";
            string query = $"SELECT * FROM dane_BLIK WHERE kod_BLIK={podaj_BLIK}";

            if (Baza.Otworz_polaczenie_SQL() == true)
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    id_konta = dataReader["id_BLIK"].ToString();
                    pobierz_status = dataReader["potwierdzenie_uzycia"].ToString();
                    DateTime.TryParse(dataReader["data_utworzenia"].ToString(), out data_pobrana);
                }

                dataReader.Close();
                Baza.Zamknij_polaczenie_SQL();
            }

            if (id_konta == "")
            {
                potwierdzenie = Polacz_2_skladniki("false", "false");
            }
            else
            {
                TimeSpan y = data_obecna.Subtract(data_pobrana);
                int x = TimeSpan.Compare(y, czas_istnienia_kodu_BLIK);
                if (x < 0 && pobierz_status == "brak")
                {
                    potwierdzenie = Polacz_2_skladniki("true", id_konta);
                }
                else
                {
                    potwierdzenie = Polacz_2_skladniki("niewazny", "false");
                }
            }
            return potwierdzenie;
        }

        public static string Sprawdz_potwierdzenie_uzycia_kodu_BLIK(string podaj_rodzaj, string pobierz_wiadomosc)
        {
            string potwierdzenie;
            string pobierz_dane = "";
            string query = "";

            switch(podaj_rodzaj)
            {
                case "13":
                    query = $"SELECT potwierdzenie_uzycia FROM dane_BLIK WHERE id_BLIK={pobierz_wiadomosc}";
                    break;
                case "14":
                    query = $"SELECT kwota_transakcji FROM dane_BLIK WHERE id_BLIK={pobierz_wiadomosc}";
                    break;
                default:
                    potwierdzenie = "false";
                    break;
            }
            if (Baza.Otworz_polaczenie_SQL() == true)
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                pobierz_dane = cmd.ExecuteScalar().ToString();
                Baza.Zamknij_polaczenie_SQL();
            }
            potwierdzenie = pobierz_dane;
            return potwierdzenie;
        }

        public static string Sprawdz_login(string podaj_login)
        {
            string potwierdzenie;
            string pobierz_dane = "";
            string query = $"SELECT * FROM dane_logowania WHERE login=\'{podaj_login}\'";
            
            if (Baza.Otworz_polaczenie_SQL() == true)
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader dataReader = cmd.ExecuteReader();
                
                while (dataReader.Read())
                {
                    pobierz_dane = dataReader["id_konta"].ToString();
                }

                dataReader.Close();
                Baza.Zamknij_polaczenie_SQL();
            }

            if(pobierz_dane == "")
            {
                potwierdzenie = "false_false";
            }
            else
            {
                potwierdzenie = Polacz_2_skladniki("true", pobierz_dane);
            }
            return potwierdzenie;
        }

        public static string Sprawdz_haslo(string podaj_haslo)
        {
            string potwierdzenie;
            string haslo = Oddzielanie_skladnikow_2_czesc_wiadomosci(podaj_haslo);
            string id_konta = Oddzielanie_skladnikow_1_czesc_wiadomosci(podaj_haslo);
            string pobierz_dane = "";
            string query = $"SELECT * FROM dane_logowania WHERE haslo=\'{haslo}\' AND id_konta=\'{id_konta}\'";

            if (Baza.Otworz_polaczenie_SQL() == true)
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    pobierz_dane = dataReader["id_konta"].ToString();
                }

                dataReader.Close();
                Baza.Zamknij_polaczenie_SQL();
            }

            if (pobierz_dane != id_konta)
            {
                potwierdzenie = "false";
            }
            else
            {
                potwierdzenie = "true";
            }
            return potwierdzenie;
        }

        public static string Pobierz_dane_konta(string podaj_etap)
        {
            string wyslij_dane;
            string etap = Oddzielanie_skladnikow_1_czesc_wiadomosci(podaj_etap);
            string id_konta = Oddzielanie_skladnikow_2_czesc_wiadomosci(podaj_etap);
            string pobierz_dane = "";
            string query = "";

            switch(etap)
            {
                case "03":    //Pobierz_Imie
                    query = $"SELECT imie FROM dane_konta WHERE id_konta={id_konta}";
                    break;
                case "04":    //Pobierz_Nazwisko
                    query = $"SELECT nazwisko FROM dane_konta WHERE id_konta={id_konta}";
                    break;
                case "05":    //Pobierz_Saldo
                    query = $"SELECT saldo FROM dane_konta WHERE id_konta={id_konta}";
                    break;
                case "06":     //Pobierz_Numer_Konta
                    query = $"SELECT numer_konta FROM dane_konta WHERE id_konta={id_konta}";
                    break;
                default:
                    break;
            }

            if (Baza.Otworz_polaczenie_SQL() == true)
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                pobierz_dane = cmd.ExecuteScalar().ToString();
                Baza.Zamknij_polaczenie_SQL();
                wyslij_dane = pobierz_dane;
            }
            else
            {
                wyslij_dane = "false";
            }
            return wyslij_dane;
        }

        public static string Aktualizacja_saldo(string podaj_saldo)
        {
            string potwierdzenie;
            string id_konta = Oddzielanie_skladnikow_1_czesc_wiadomosci(podaj_saldo);
            int saldo = Int32.Parse(Oddzielanie_skladnikow_2_czesc_wiadomosci(podaj_saldo));
            string query = "UPDATE dane_konta SET saldo=@saldo WHERE id_konta=@id_konta";
            
            if (Baza.Otworz_polaczenie_SQL() == true)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = query;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@saldo", saldo);
                cmd.Parameters.AddWithValue("@id_konta", id_konta);
                cmd.ExecuteNonQuery();
                Baza.Zamknij_polaczenie_SQL();
                potwierdzenie = "true";
            }
            else
            {
                potwierdzenie = "false";
            }
            return potwierdzenie;
        }

    }
    
}