using System;
using System.Collections.Generic;
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
        private Datagram.Datagram send;

        public Processing(byte[] b) => map = Datagram.Datagram.analyze(Encoding.ASCII.GetString(b));

        public string[] Run()
        {
            Console.WriteLine("-----------------");
            Console.WriteLine(map);

            switch (map["ST:"])
            {
                case "NewSession":

                    break;
                case "1":
                    break;
                default:
                    break;
            }

            return null;
        }

        private void newSession()
        {
            var z = db.newSession(); 
        }
    }
}