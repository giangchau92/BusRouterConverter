using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BusRouterConverter
{
    class GetSourceTool
    {
        static void Main(string[] args)
        {
            int[] routerids = {1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 13, 14, 15, 16, 17, 18, 19, 20,
                              22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
                              41, 43, 44, 45, 46, 47, 48, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62,
                              };
            for (int i = 0; i <= 152; i++)
            {
                string url = String.Format("http://mapbus.ebms.vn/ajax.aspx?action=listRouteStations&rid={0}&isgo=true", i);
                string data = GET(url);
                string savePath = String.Format(@"input\{0}-1.json", i);
                File.WriteAllText(savePath, data);
                System.Console.WriteLine(savePath + " completed");
            }

            for (int i = 0; i <= 152; i++)
            {
                string url = String.Format("http://mapbus.ebms.vn/ajax.aspx?action=listRouteStations&rid={0}&isgo=false", i);
                string data = GET(url);
                string savePath = String.Format(@"input\{0}-2.json", i);
                File.WriteAllText(savePath, data);
                System.Console.WriteLine(savePath + " completed");
            }
        }

        public static string GET(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                string data = reader.ReadToEnd();

                reader.Close();
                stream.Close();

                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }
    }
}
