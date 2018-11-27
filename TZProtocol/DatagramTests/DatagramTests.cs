using Microsoft.VisualStudio.TestTools.UnitTesting;
using Datagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datagram.Tests
{
    [TestClass()]
    public class DatagramTests
    {
        [TestMethod()]
        public void analyzeTest()
        {
            Datagram a = new Datagram();
            a.ID = 1.ToString();
            a.ST = 3.ToString();

            //a.L = new List<long> {1};

            //a.OP = "silnia";

            var z = a.gen();

            foreach (var item in z)
                Console.WriteLine(item + "\n-----------");

            foreach (var s in z)
            {
                foreach (var item in Datagram.analyze(s))
                {
                    Console.WriteLine(item.Key + " => " + item.Value);
                }
                Console.WriteLine("-----------");
            }

        }

        [TestMethod()]
        public void analyzeTest2()
        {
            Datagram a =new Datagram();
            a.ID = 1.ToString();
            a.ST = 3.ToString();

            a.L = new List<int>{1,2,3,4,5,6};

            a.OP = "dod";

            var z = a.gen();

            foreach (var item in z)
                Console.WriteLine(item+"\n-----------");
        }
    }
}