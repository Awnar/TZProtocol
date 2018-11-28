using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

            serwer.Connect(DEFAULT_SERVER, DEFAULT_PORT);

            var kom = new Datagram.Datagram();
            kom.ST = "nowy";
            Send(kom);

            bool running = true;
            while (running)
            {
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

                kom = new Datagram.Datagram {ID = ID};

                // jakie ST
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

                Send(kom);
            }
        }

        private static List<Dictionary<string, string>> ReceiveLoop(byte i = 0)
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
                    Console.WriteLine("Serwer nie odsyła odpowiedzi, próba: " + (i + 1));
                    if (i++ < 3)
                    {
                        return ReceiveLoop(i);
                    }
                    else
                    {
                        throw new Exception("Brak odpowiedzi od serwera");
                    }
                }
            }

            return data;
        }

        private static string Receive()
        {
            var timeToWait = TimeSpan.FromSeconds(3);

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
                    Console.WriteLine("Obliczenie " + data[2]["IO"] + ": " + data[3]["LL"] + "\n");
                    break;
                case "pelny":
                    Console.WriteLine("Obliczenie " + data[2]["IO"] + ": " + "wartość wyniku poza zakresem zmiennej\n");
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
                    Console.WriteLine("Historia należy do innego klienta - brak dostępu\n");
                    break;
                case "niema":
                    Console.WriteLine("Nie ma takiego IO w historii obliczeń\n");
                    break;
                case "opblad":
                    Console.WriteLine("Błąd OP\n");
                    break;
                case "stblad":
                    Console.WriteLine("Błąd ST\n");
                    break;
                case "idblad":
                    Console.WriteLine("Błąd ID\n");
                    break;
                case "ioblad":
                    Console.WriteLine("Błąd IO");
                    break;
                case "koniec":
                    Console.WriteLine("Koniec sesji\nNaciśnij dowolny klawisz by zakończyć pracę");
                    Console.ReadKey();
                    Environment.Exit(1);
                    break;
                default:
                    Console.WriteLine("Nie rozpoznano statusu serwera");
                    break;
            }
        }

        private static void Send(Datagram.Datagram fr)
        {
            var data = fr.gen();
            foreach (var item in data)
            {
                var tmp = Encoding.ASCII.GetBytes(item);
                serwer.Send(tmp, tmp.Length);
            }
        }

        private static void edit()
        {

        }
    }
}
