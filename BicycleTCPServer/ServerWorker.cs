using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using obligatorisk_opgave;

namespace BicycleTCPServer
{
    class ServerWorker
    {

        private static List<Bicycle> _bicycleList = new List<Bicycle>()
        {
            new Bicycle(1, "Green", 400, 7),
            new Bicycle(2, "Green", 1000, 21),
            new Bicycle(3, "Blue", 5000, 32),
            new Bicycle(4, "Red", 300, 3)
        };

        public void Start()
        {
            TcpListener server =  new TcpListener(IPAddress.Loopback, 4646);
            server.Start();

            while (true)
            {
                Task.Run(() =>
                {
                    TcpClient tempSocket = server.AcceptTcpClient();
                    DoClient(tempSocket);
                    tempSocket.Close();
                });
            }
        }

        public void DoClient(TcpClient socket)
        {
            StreamReader Sr = new StreamReader(socket.GetStream());
            StreamWriter Sw = new StreamWriter(socket.GetStream());

            string str1 = Sr.ReadLine();
            string str2 = Sr.ReadLine();
            Console.WriteLine(str1 +"    " +str2);
            if (str1 == "HentAlle" && str2 == "")
            {
                //udskriv alle fra bicyleList
                GetAll(Sr, Sw);
                
            }
            else if (str1 == "Hent" && str2 != "")
            {
                //udskriv cykel hvor id == str2
                
                GetOne(Sr, Sw, str2);

            }
            else if (str1 == "Gem")
            {
                //gem i bicycleList
                SaveOne(Sr, Sw, str2);
            }
            else
            {
                Sw.WriteLine("fejl i input");
            }
            Sw.Flush();
        }

        private void GetAll(StreamReader sr, StreamWriter sw)
        {
            foreach (Bicycle bicycle in _bicycleList)
            {
                string jsonStr = JsonConvert.SerializeObject(bicycle);

                sw.WriteLine(jsonStr);
            }
        }

        private void GetOne(StreamReader sr, StreamWriter sw, string str)
        {
            try
            {
                int str2Int = int.Parse(str);
                Bicycle bicycle = _bicycleList.Find(b => b.Id == str2Int);
                string jsonStr = JsonConvert.SerializeObject(bicycle);
                sw.WriteLine(jsonStr);
            }
            catch (FormatException e)
            {
                sw.WriteLine(e.Message);
            }
        }

        private void SaveOne(StreamReader sr, StreamWriter sw, string str)
        {
            Bicycle b1 = JsonConvert.DeserializeObject<Bicycle>(str);
            _bicycleList.Add(b1);
        }

    }
}
