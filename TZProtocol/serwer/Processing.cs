using System;
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

        public string[] Run()
        {
            if (a.L == null) a.L = new List<int>();

            foreach (var item in map)
                Console.WriteLine(item.Key + " => " + item.Value);
            Console.WriteLine("-----------------");

            if (!map["ID"].Equals(LID))
            {
                send.ID = map["ID"];
                send.ST = "zajety";
                return send.gen();
            }
            send.ID = LID;
            a.ID = LID;

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
                    a.com = false;
                    _st();
                }
            }

            if (s == true)
            {
                var q = send.gen();
                foreach (var s in q)
                    Console.WriteLine("Serwer ...\n" + s + "\n-----------");

                return q;
            }
            return null;
        }

        private void _st()
        {
            switch (a.ST)
            {
                case "nowy":
                    newSession();
                    break;
                case "operacja":
                    licz();
                    break;
                case "id":
                    _id();
                    break;
                case "io":
                    _io();
                    break;
                case "koniec":
                    endSession();
                    break;
                default:
                    send.ST = "STblad";
                    s = true;
                    break;
            }
        }

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
                tmp.Add("ST: operacja");
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
        private void _io()
        {
            s = true;
            if (db.his.Count < a.IO)
            {
                send.ST = "ioblad";
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
                    tmp.Add("ST: operacja");
                    tmp.Add("LL: " + io.L[0]);
                    if (a.OF)
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
                    tmp.Add("ST: operacja");
                    tmp.Add("LL: " + io.L[0]);
                    tmp.Add("LL: " + io.L[1]);
                    if (a.OF)
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
                            w = send.L = new List<int>() {a.L.Sum(item => item)};
                            break;
                        case "odejmowanie":
                            w = send.L = new List<int>() {a.L[0] - a.L[1]};
                            break;
                        case "mnożenie":
                            w = send.L = new List<int>() {a.L[0] * a.L[1]};
                            break;
                        case "dzielenie":
                            w = send.L = new List<int>() {a.L[0] / a.L[1]};
                            break;
                        case "silnia":
                            w = send.L = new List<int>() {silnia(a.L[0])};
                            break;
                        default:
                            send.ID = a.ID;
                            send.ST = "HWDP";
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
                send.OP_ID = db.his.Count.ToString();
                db.his.Add(a);
                s = true;
            }
        }

        private int silnia(int i)
        {
            if (i < 1)
                return 1;
            else
                return i * silnia(i - 1);
        }


        private void endSession()
        {
            LID = "";
            send.ST = "OK";
            s = true;
        }
        private void newSession()
        {
            LID = send.ID = db.newSession();
            send.ST = "OK";
            s = true;
        }
    }
}