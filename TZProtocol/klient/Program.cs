﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace klient
{
    class Program
    {
        private static IPAddress DEFAULT_SERVER;
        private static int DEFAULT_PORT = 9999;

        private static UdpClient serwer = new UdpClient();

        private static string ID;

        ///<summary>
        ///Metoda inicjalizacja klienta. Wraz z główną pętlą oraz interfejsem.
        ///</summary>
        static void Main(string[] args)
        {
            Console.WriteLine("Witaj!");
            Console.WriteLine("Podaj IP serwera:");
            while (true)
            {
                try
                {
                    DEFAULT_SERVER = IPAddress.Parse(Console.ReadLine());
                    break;
                }
                catch
                {
                    Console.WriteLine("Coś poszło nie tak... Spróbuj jeszcze raz");
                }
            }

            //Nawiązanie połączenia i wysłanie pierwszego pakietu 
            serwer.Connect(DEFAULT_SERVER, DEFAULT_PORT);

            var kom = new Datagram.Datagram();
            kom.ST = "nowy";
            Send(kom);

            //główna pętla
            bool running = true;
            while (running)
            {
                //odebranie pakietu od serwera
                var data = new List<Dictionary<string, string>>();
                try
                {
                    data = ReceiveLoop();
                    odpS(data);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadKey();
                    return;
                }

                //przygotowywanie odpowiedzi 
                kom = new Datagram.Datagram {ID = ID};

                //interface
                Console.WriteLine("\nCo chcesz zrobić?");
                Console.WriteLine("1 - wykonanie operacji");
                Console.WriteLine("2 - przeglądanie historii obliczeń przez moje ID");
                Console.WriteLine("3 - przeglądanie historii obliczeń przez ID obliczenia");
                Console.WriteLine("4 - zakończenie transmisji");
                Console.WriteLine("0 - edytuj ręcznie");

                int choice;
                while (true)
                {
                    try
                    {
                        choice = Int32.Parse(Console.ReadLine());
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("Błąd odczytu. Spróbuj ponownie");
                    }
                }

                switch (choice)
                {
                    case 0:
                        edit();
                        continue;
                    case 1:
                        kom.ST = "operacja";
                        break;
                    case 2:
                        kom.ST = "id";
                        break;
                    case 3:
                        kom.ST = "io";
                        break;
                    case 4:
                        kom.ST = "koniec";
                        break;
                    default:
                        Console.WriteLine("Bład wejscia. Ustawiam na domyślny (operacja)");
                        kom.ST = "operacja";
                        break;
                }

                if (kom.ST == "operacja")
                {
                    //ustalenie operacji

                    Console.WriteLine("\nJaka operacje na liczbach chcesz wykonac?");
                    Console.WriteLine("1 - dodawanie");
                    Console.WriteLine("2 - odejmowanie");
                    Console.WriteLine("3 - mnożenie");
                    Console.WriteLine("4 - dzielenie");
                    Console.WriteLine("5 - silnia");

                    while (true)
                    {
                        try
                        {
                            choice = Int32.Parse(Console.ReadLine());
                            break;
                        }
                        catch
                        {
                            Console.WriteLine("Błąd odczytu. Spróbuj ponownie");
                        }
                    }

                    switch (choice)
                    {
                        case 1:
                            kom.OP = "dodawanie";
                            break;
                        case 2:
                            kom.OP = "odejmowanie";
                            break;
                        case 3:
                            kom.OP = "mnozenie";
                            break;
                        case 4:
                            kom.OP = "dzielenie";
                            break;
                        case 5:
                            kom.OP = "silnia";
                            break;
                        default:
                            Console.WriteLine("Nie ma takiej operacji. Ustawiam na domyślny (dodawanie)");
                            kom.OP = "dodawanie";
                            break;
                    }

                    if (kom.OP == "silnia")
                    {
                        Console.WriteLine("\nPodaj liczbę:");
                        while (true)
                        {
                            try
                            {
                                kom.LL.Add(int.Parse(Console.ReadLine()));
                                break;
                            }
                            catch
                            {
                                Console.WriteLine("Błąd odczytu. Spróbuj ponownie");
                            }
                        }
                    }

                    else
                    {
                        Console.WriteLine("\nPodaj pierwszą liczbę:");
                        while (true)
                        {
                            try
                            {
                                kom.LL.Add(int.Parse(Console.ReadLine()));
                                break;
                            }
                            catch
                            {
                                Console.WriteLine("Błąd odczytu. Spróbuj ponownie");
                            }
                        }

                        while (true)
                        {
                            Console.WriteLine("\nPodaj drugą liczbę:");
                            try
                            {
                                kom.LL.Add(int.Parse(Console.ReadLine()));
                                break;
                            }
                            catch
                            {
                                Console.WriteLine("Błąd odczytu. Spróbuj ponownie");
                            }
                        }
                    }
                }

                if (kom.ST == "io")
                {
                    Console.WriteLine("\nPodaj ID obliczenia:");

                    int io = 0;
                    while (true)
                    {
                        try
                        {
                            io = Convert.ToInt32(Console.ReadLine());
                            break;
                        }
                        catch
                        {
                            Console.WriteLine("Błąd odczytu. Spróbuj ponownie");
                        }
                    }
                    kom.IO = Convert.ToString(io);
                }

                //Wysłanie pakietu
                Send(kom);
            }
        }

        private static List<Dictionary<string, string>> ReceiveLoop(byte k = 4, byte s = 0)
        {
            var data = new List<Dictionary<string, string>>();
            while (true)
            {
                try
                {
                    var tmp = Receive();
                    data.Add(Datagram.Datagram.analyze(tmp));
                    if (Datagram.Datagram.analyze(tmp)["NS"] == "0")
                        break;
                }
                catch (Exception)
                {
                    Console.WriteLine("Serwer nie odsyła odpowiedzi, próba: " + (s + 1));
                    if (s++ < k)
                    {
                        var tmp = ReceiveLoop(s);
                        data.AddRange(tmp);
                        return data;
                    }
                    else
                    {
                        throw new Exception("Brak odpowiedzi od serwera");
                    }
                }
            }
            return data;
        }

        ///<summary>
        ///Metoda odbierająca pakiet, zwraca wyjątek gdy nie odbierze żadnego pakietu w przeciągu 4s.
        ///</summary>>
        ///<return>Zwraca zawartość odebranego pakietu</return>
        private static string Receive()
        {
            var timeToWait = TimeSpan.FromSeconds(4);

            var asyncResult = serwer.BeginReceive(null, null);
            asyncResult.AsyncWaitHandle.WaitOne(timeToWait);
            if (asyncResult.IsCompleted)
                try
                {
                    IPEndPoint remoteEP = null;
                    return Encoding.ASCII.GetString(serwer.EndReceive(asyncResult, ref remoteEP));
                }
                catch (Exception)
                {
                    throw new Exception();
                }
            throw new Exception();
        }

        ///<summary>
        ///Metoda wyświetlająca odpowiedz serwera
        ///</summary>
        private static void odpS(List<Dictionary<string, string>> data)
        {
            switch (data[1]["ST"])
            {
                case "ok":
                    ID = data[1]["ID"];
                    Console.WriteLine("Nawiązano połączenie, ID to " + ID);
                    break;
                case "zajety":
                    Console.WriteLine("Serwer jest zajęty, spróbuj ponownie później");
                    Console.ReadKey();
                    Environment.Exit(1);
                    break;
                case "wynik":
                    Console.WriteLine("Obliczenie nr " + data[2]["IO"] + ": " + data[3]["LL"] + "");
                    break;
                case "pelny":
                    Console.WriteLine("Obliczenie nr " + data[2]["IO"] + ": " + "wartość wyniku poza zakresem zmiennej");
                    break;
                case "idzwrot":
                    int loop = Convert.ToInt32(data[1]["NS"]);
                    int offset = 0;

                    while (loop > 0)
                    {
                        int counter = 0; 
                        Console.Write(data[2 + offset]["IO"] + ": ");
                        counter++;

                        if (data[3 + offset]["OP"] == "silnia")
                        {
                            Console.Write(data[4 + offset]["LL"] + "! = ");
                            counter += 2;
                            switch (data[5 + offset]["ST"])
                            {
                                case "wynik":
                                    Console.WriteLine(data[6 + offset]["LL"]);
                                    counter += 2;
                                    break;
                                case "pelny":
                                    Console.WriteLine("przepełnienie");
                                    counter++;
                                    break;
                            }
                        }
                        else
                        {
                            switch (data[3 + offset]["OP"])
                            {
                                case "dodawanie":
                                    Console.Write(data[4 + offset]["LL"] + " + " + data[5 + offset]["LL"] + " = ");
                                    break;
                                case "odejmowanie":
                                    Console.Write(data[4 + offset]["LL"] + " - " + data[5 + offset]["LL"] + " = ");
                                    break;
                                case "mnozenie":
                                    Console.Write(data[4 + offset]["LL"] + " * " + data[5 + offset]["LL"] + " = ");
                                    break;
                                case "dzielenie":
                                    Console.Write(data[4 + offset]["LL"] + " / " + data[5 + offset]["LL"] + " = ");
                                    break;
                            }

                            counter += 3;

                            switch (data[6 + offset]["ST"])
                            {
                                case "wynik":
                                    Console.WriteLine(data[7 + offset]["LL"]);
                                    counter += 2;
                                    break;
                                case "pelny":
                                    Console.WriteLine("przepełnienie");
                                    counter++;
                                    break;
                            }
                        }

                        loop -= counter;
                        offset += counter;
                    }
                    
                    break;
                case "iozwrot":
                    if (data[2]["OP"] == "silnia")
                    {
                        Console.Write(data[3]["LL"] + "! = ");
                        switch (data[4]["ST"])
                        {
                            case "wynik":
                                Console.WriteLine(data[5]["LL"]);
                                break;
                            case "pelny":
                                Console.WriteLine("przepełnienie");
                                break;
                        }
                    }
                    else {
                        switch (data[2]["OP"])
                        {
                            case "dodawanie":
                                Console.Write(data[3]["LL"] + " + " + data[4]["LL"] + " = ");
                                break;
                            case "odejmowanie":
                                Console.Write(data[3]["LL"] + " - " + data[4]["LL"] + " = ");
                                break;
                            case "mnozenie":
                                Console.Write(data[3]["LL"] + " * " + data[4]["LL"] + " = ");
                                break;
                            case "dzielenie":
                                Console.Write(data[3]["LL"] + " / " + data[4]["LL"] + " = ");
                                break;
                        }

                        switch (data[5]["ST"])
                        {
                            case "wynik":
                                Console.WriteLine(data[6]["LL"]);
                                break;
                            case "pelny":
                                Console.WriteLine("przepełnienie");
                                break;
                        }
                    }
                    break;
                case "nietwoje":
                    Console.WriteLine("Historia należy do innego klienta - brak dostępu");
                    break;
                case "niema":
                    Console.WriteLine("Nie ma takiego IO w historii obliczeń");
                    break;
                case "opblad":
                    Console.WriteLine("Błąd OP");
                    break;
                case "stblad":
                    Console.WriteLine("Błąd ST");
                    break;
                case "idblad":
                    Console.WriteLine("Błąd ID");
                    break;
                case "ioblad":
                    Console.WriteLine("Błąd IO");
                    break;
                case "koniec":
                    Console.WriteLine("Koniec sesji\nNaciśnij dowolny klawisz by zakończyć pracę");
                    Console.ReadKey();
                    Environment.Exit(1);
                    break;
                case "blad":
                    Console.WriteLine("Błąd domyślny serwera");
                    break;
                default:
                    Console.WriteLine("Nie rozpoznano statusu serwera");
                    break;
            }
        }

        ///<summary>
        ///Metoda wysyłająca pakiet
        ///</summary>
        private static void Send(Datagram.Datagram fr)
        {
            var data = fr.gen();
            foreach (var item in data)
            {
                var tmp = Encoding.ASCII.GetBytes(item);
                serwer.Send(tmp, tmp.Length);
            }
        }

        ///<summary>
        ///Metoda pozwalająca użytkownikowi samemu wprowadzić zawartość pakietu 
        ///</summary>
        private static void edit()
        {
            Console.WriteLine("\nIle komunikatów chcesz wysłać?");
            uint choice = 0;
            while (true)
            {
                try
                {
                    choice = UInt32.Parse(Console.ReadLine());
                    break;
                }
                catch
                {
                    Console.WriteLine("Błąd odczytu. Spróbuj ponownie");
                }
            }

            string[] data = new string[choice];
            int i = 1;
            while (i <= choice)
            {
                Console.WriteLine("\nKomunikat nr " + i + ":");
                Console.WriteLine("Podaj pierwszą linijkę/pole:");
                var line1 = Console.ReadLine();
                Console.WriteLine("Podaj drugą linijkę/pole:");
                var line2 = Console.ReadLine();
                Console.WriteLine("Podaj trzecią linijkę/pole:");
                var line3 = Console.ReadLine();
                Console.WriteLine("Podaj czwartą linijkę/pole:");
                var line4 = Console.ReadLine();
                data[i - 1] = line1 + "\n" + line2 + "\n" + line3 + "\n" + line4;
                i++;
            }

            Console.WriteLine("Wysyłam komunikaty...");

            foreach (var item in data)
            {
                var tmp = Encoding.ASCII.GetBytes(item);
                serwer.Send(tmp, tmp.Length);
            }

        }
    }
}
