using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Configuration;
using System.Data.SqlClient;

namespace SERWER
{
    public class Serwer
    {
       
        public static int Port = 6666;
        public static string wiadomosc = "";
        public static bool serwer_uruchomiony = false;
        private static byte[] buffer = new byte[1024];

        private static Socket serverSocket =
            new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private static List<Socket> clientSockets = new List<Socket>();
        
        public static void Uruchom_serwer()
        {
            if(serwer_uruchomiony == false)
            {
                try
                {
                    Baza.Polacz_SQL(Baza.adres);

                    Serwer.AddStatus("Uruchomienie serwera...");
                    serverSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
                    serverSocket.Listen(10);
                    serverSocket.BeginAccept(new AsyncCallback(Dodaj_Uzytkownika), null);

                    Serwer.AddStatus("Serwer nasłuchuje na adresie " + serverSocket.LocalEndPoint.ToString());

                    serwer_uruchomiony = true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Serwer jest już uruchomiony!");
            }
        }

        public static void Zatrzymaj_serwer()
        {
            try
            {
                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.Close();

                Serwer.AddStatus("Zatrzymano serwer...");
            }
            catch (Exception e)
            {
                Serwer.AddStatus(e.Message);
            }
        }

        public static void Dodaj_Uzytkownika(IAsyncResult asyncResult)
        {
            Socket clientSocket = serverSocket.EndAccept(asyncResult);
            clientSockets.Add(clientSocket);
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Wyslij_Odbierz_Wiadomosc),
                clientSocket);
            serverSocket.BeginAccept(new AsyncCallback(Dodaj_Uzytkownika), null);
            Serwer.AddStatus("Połaczono klienta..." + Environment.NewLine);
        }
        
        public static void Wyslij_Odbierz_Wiadomosc(IAsyncResult asyncResult)
        {
            Socket clientSocket = (Socket)asyncResult.AsyncState;
            int received;
            try
            {
                received = clientSocket.EndReceive(asyncResult);
            }
            catch (Exception e)
            {
                Serwer.AddStatus("Rozłączono klienta...");
                return;
            }
            
            byte[] dateBuf = new byte[received];
            Array.Copy(buffer, dateBuf, received);

            wiadomosc = Encoding.Unicode.GetString(dateBuf);
            Serwer.AddStatus(wiadomosc);
            if(wiadomosc == "")
            {
                Serwer.AddStatus("Rozłączono klienta...");
                wiadomosc = "error";
            }
            else
            {
                wiadomosc = Baza.Menu_glowne(wiadomosc);
            }

            try
            {
                byte[] data = Encoding.Unicode.GetBytes(wiadomosc);

                clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback),
                clientSocket);
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Wyslij_Odbierz_Wiadomosc),
                    clientSocket);
            }
            catch (Exception e)
            {
                Serwer.AddStatus("Rozłączono klienta...");
                return;
            }
        }
                
        private static void SendCallback(IAsyncResult asyncResult)
        {
            Socket socket = (Socket)asyncResult.AsyncState;
            socket.EndSend(asyncResult);
        }
        
        public static void AddStatus(string text)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MainWindow.AppWindow.Komunikat.Inlines.Add(text + Environment.NewLine);
                ;
            }));
        }
    }
}