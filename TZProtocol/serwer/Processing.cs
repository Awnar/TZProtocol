using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datagram;

namespace serwer
{
    class Processing
    {
        private static DB db = new DB();

        private Dictionary<string, string> map;
        private Datagram.Datagram send=new Datagram.Datagram();

        public Processing(byte[] b) => map = Datagram.Datagram.analyze(Encoding.ASCII.GetString(b));

        public string[] Run()
        {
            Console.WriteLine("-----------------");
            Console.WriteLine(map);

            var a= new DB.zzzz();

            if (map.ContainsKey("NS"))
            {
                a = db.his.Last();
                if (a.com == true)
                {
                    a = new DB.zzzz();
                    a.ID = map["ID"];
                    db.his.Add(a);
                }
                else if (a.ID != map["ID"]) throw new Exception("Zła sesja");

                if (map.ContainsKey("OP"))
                    a.OP = map["OP"];
                if (map.ContainsKey("ST"))
                    a.ST = map["ST"];
                if (map.ContainsKey("LL"))
                    a.L.Add(int.Parse(map["LL"]));
                if (map["NS"].Equals("0"))
                {
                    a.com = false;
                    throw new NotImplementedException();
                }

                db.his[db.his.Count - 1] = a;
            }

            //if (map.ContainsKey("ST"))
            //    switch (map["ST"])
            //    {
            //        case "NewSession":
            //            newSession();
            //            break;
            //        default:
            //            break;
            //    }

            return null;
        }

        private void newSession()
        {
            send.ID = db.newSession(); 
        }
    }
}