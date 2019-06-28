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
    public class Blik
    {
        public static int Port = 6666;
        public static string Host = "127.0.0.1";
        private static byte[] buffer = new byte[1024];
        public static bool IsConnected = false;

        private static Socket clientSocket =
            new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public static void Polacz()
        {
            if(IsConnected == false)
            {
                try
                {
                    clientSocket.Connect(IPAddress.Parse(Host), Port);
                    IsConnected = true;
                }
                catch (SocketException e)
                {
                    MessageBox.Show(e.Message);
                    return;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return;
                }
            }
        }

        public static void Wyslij(string tekst_do_wyslania)
        {
            try
            {
                byte[] buffer = Encoding.Unicode.GetBytes(tekst_do_wyslania);
                clientSocket.Send(buffer);
            }
            catch (SocketException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }

        public static string Odbierz()
        {
            string Odebrany_tekst;
            try
            {
                byte[] receivedBuffer = new byte[1024];
                int receivedLength = clientSocket.Receive(receivedBuffer);
                byte[] data = new byte[receivedLength];
                Array.Copy(receivedBuffer, data, receivedLength);
                Odebrany_tekst = Encoding.Unicode.GetString(data);
                return Odebrany_tekst;
            }
            catch (SocketException e)
            {
                MessageBox.Show(e.Message);
                return "";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return "";
            }
        }

        public static void Rozlacz()
        {
            try
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();

                MessageBox.Show("Rozłączono z Serwerem!");
            }
            catch (SocketException e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }
}