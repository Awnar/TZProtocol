﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datagram;

namespace serwer
{
    public class Processing
    {
        private static DB db = new DB();
        private static DB.zzzz a = new DB.zzzz();
        private static string LID = "";
        private bool s = false;

        private Dictionary<string, string> map;
        private Datagram.Datagram send = new Datagram.Datagram();

        public Processing(byte[] b) => map = Datagram.Datagram.analyze(Encoding.ASCII.GetString(b));

        ///<summary>
        ///Analizuje odebrane pakiety i przygotowuje odpowiedz
        ///</summary>
        ///<return>Zwraca tablice gotowych danych do wysłania</return>
        public string[] Run()
        {
            if (a.L == null) a.L = new List<int>();
            if (db.his.Count==0)db.his.Add(new DB.zzzz());

            foreach (var item in map)
                Console.WriteLine(item.Key + " => " + item.Value);
            Console.WriteLine("-----------------");

            //Zapisywanie poszczególnych pól w celu dalszego wykorzystania 
            if (map.ContainsKey("NS"))
            {
                if (map.ContainsKey("OP"))
                    a.OP = map["OP"];
                if (map.ContainsKey("IO"))
                    a.IO = int.Parse(map["IO"]);
                if (map.ContainsKey("ST"))
                    a.ST = map["ST"];
                if (map.ContainsKey("LL"))
                    a.L.Add(int.Parse(map["LL"]));
                if (map["NS"].Equals("0"))
                {
                    //Jeśli ID w pakiecie jest inny niż obecnie obsługiwany wyślij informacje o błędzie 
                    if (!map["ID"].Equals(LID))
                    {
                        send.ID = map["ID"];
                        send.ST = "zajety";
                        var q = send.gen();
                        foreach (var s in q)
                            Console.WriteLine("Serwer ...\n" + s + "-----------");
                        return q;
                    }
                    send.ID = LID;
                    a.ID = LID;

                    a.com = false;

                    //analiza zebranych danych
                    _st();
                }
            }

            if (s == true)
            {
                var q = send.gen();
                foreach (var s in q)
                    Console.WriteLine("Serwer ...\n" + s + "-----------");
                a = new DB.zzzz();
                return q;
            }
            return null;
        }

        ///<summary>
        ///Uruchamia poszczególne metody w zależności do żądania klienta
        ///</summary>
        private void _st()
        {
            switch (a.ST)
            {
                case "nowy": //nowa sesja
                    newSession();
                    break;
                case "operacja": //liczenie
                    licz();
                    break;
                case "id": //historia danego ID klienta
                    _id();
                    break;
                case "io": //historia ID operacji
                    _io();
                    break;
                case "koniec": //zakończ sesję
                    endSession();
                    break;
                default:
                    send.ST = "stblad";
                    s = true;
                    break;
            }
        }

        ///<summary>
        ///Przygotowywane historię klienta do wysłania
        ///</summary>
        private void _id()
        {
            s = true;
            List<string> tmp = new List<string>();
            send.ST = "idzwrot";
            for (int i = 0; i < db.his.Count; i++)
            {
                var item = db.his[i];
                if (item.ID != LID) continue;

                tmp.Add("IO: "+i);
                tmp.Add("OP: "+item.OP);
                tmp.Add("LL: " + item.L[0]);
                if (!item.OP.Equals("silnia"))
                    tmp.Add("LL: " + item.L[1]);
                if (item.OF)tmp.Add("ST: pelny");
                    else
                {
                    tmp.Add("ST: wynik");
                    tmp.Add("LL: "+item.L.Last());
                }
            }
            send.inne = tmp.ToArray();
        }

        ///<summary>
        ///Przygotowywane historię operacji do wysłania
        ///</summary>
        private void _io()
        {
            s = true;
            if ((db.his.Count-1 < a.IO) || (a.IO < 1))
            {
                send.ST = "niema";
                return;
            }

            var io = db.his[a.IO];
            if (io.ID == LID)
            {
                send.ST = "iozwrot";
                List<string> tmp=new List<string>();
                if (io.OP.Equals("silnia"))
                {
                    tmp.Add("OP: silnia");
                    tmp.Add("LL: " + io.L[0]);
                    if (io.OF)
                        tmp.Add("ST: pelny");
                    else
                    {
                        tmp.Add("ST: wynik");
                        tmp.Add("LL: " + io.L[1]);
                    }

                }
                else
                {
                    tmp.Add("OP: "+io.OP);
                    tmp.Add("LL: " + io.L[0]);
                    tmp.Add("LL: " + io.L[1]);
                    if (io.OF)
                        tmp.Add("ST: pelny");
                    else
                    {
                        tmp.Add("ST: wynik");
                        tmp.Add("LL: " + io.L[2]);
                    }
                }
                send.inne = tmp.ToArray();
            }
            else
                send.ST = "nietwoje";
        }

        ///<summary>
        ///Wykonuje operacje zlecone przez klienta
        ///</summary>
        private void licz()
        {
            send.ST = "wynik";
            var w = new List<int>();
            try
            {
                checked
                {
                    switch (a.OP)
                    {
                        case "dodawanie":
                            w = send.LL = new List<int>() {a.L.Sum(item => item)};
                            break;
                        case "odejmowanie":
                            w = send.LL = new List<int>() {a.L[0] - a.L[1]};
                            break;
                        case "mnozenie":
                            w = send.LL = new List<int>() {a.L[0] * a.L[1]};
                            break;
                        case "dzielenie":
                            w = send.LL = new List<int>() {a.L[0] / a.L[1]};
                            break;
                        case "silnia":
                            w = send.LL = new List<int>() {silnia(a.L[0])};
                            break;
                        default:
                            send.ID = a.ID;
                            send.ST = "opblad";
                            break;
                    }
                }
                a.L.AddRange(w);
            }
            catch (OverflowException)
            {
                send.ST = "pelny";
                a.OF = true;
            }
            finally
            {
                send.IO = db.his.Count.ToString();
                db.his.Add(a);
                s = true;
            }
        }

        private int silnia(int n)
        {
            try
            {
                checked
                {
                    int result = 1;
                    for (int i = 1; i <= n; i++)
                    {
                        result *= i;
                    }
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///<summary>
        ///Zamykanie sesji
        ///</summary>
        private void endSession()
        {
            LID = "";
            send.ST = "koniec";
            s = true;
        }

        ///<summary>
        ///Otwarcie nowej sesji
        ///</summary>
        private void newSession()
        {
            LID = send.ID = db.newSession();
            send.ST = "ok";
            s = true;
        }
    }
}