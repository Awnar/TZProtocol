using Datagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using serwer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serwer.Tests
{
    [TestClass()]
    public class ProcessingTests
    {
        [TestMethod()]
        public void RunTest()
        {
            Datagram.Datagram a = new Datagram.Datagram();
            a.ID = "";
            a.ST = "nowy";

            var z = a.gen();
            string[] q = new string[] { };

            foreach (var item in z)
            {
                var bytes = Encoding.ASCII.GetBytes(item);
                Processing p = new Processing(bytes);
                q = p.Run();
            }

            var w = Datagram.Datagram.analyze(q[0]);

            a = new Datagram.Datagram();
            a.ID = w["ID"];
            a.ST = "operacja";
            a.OP = "dzielenie";
            a.LL = new List<int>() {4, 2};

            z = a.gen();

            foreach (var item in z)
            {
                var bytes = Encoding.ASCII.GetBytes(item);
                Processing p = new Processing(bytes);
                p.Run();
            }


        }


        [TestMethod()]
        public void RunTest2()
        {
            Datagram.Datagram a = new Datagram.Datagram();
            a.ID = "";
            a.ST = "nowy";

            var z = a.gen();
            string[] q = new string[] { };

            foreach (var item in z)
            {
                var bytes = Encoding.ASCII.GetBytes(item);
                Processing p = new Processing(bytes);
                q = p.Run();
            }

            var w = Datagram.Datagram.analyze(q[0]);

            a = new Datagram.Datagram();
            a.ID = w["ID"];
            a.ST = "operacja";
            a.OP = "dzielenie";
            a.LL = new List<int>() {4, 2};

            z = a.gen();

            foreach (var item in z)
            {
                var bytes = Encoding.ASCII.GetBytes(item);
                Processing p = new Processing(bytes);
                p.Run();
            }

            a = new Datagram.Datagram();
            a.ID = w["ID"];
            a.ST = "id";

            z = a.gen();

            foreach (var item in z)
            {
                var bytes = Encoding.ASCII.GetBytes(item);
                Processing p = new Processing(bytes);
                p.Run();
            }

        }
    }
}