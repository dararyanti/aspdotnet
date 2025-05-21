using GridViewApplication.Dto.RestPosition;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace GridViewApplication.Services
{
    public class PositionService
    {
        public static List<rest_position> GetPositions(string token)
        {
            string url = "https://localhost:44327/api/Position"; // adjust URL as needed
            List<rest_position> positions = new List<rest_position>();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add("Authorization", $"Bearer {token}");

            ServicePointManager.ServerCertificateValidationCallback =
                (senderX, certificate, chain, sslPolicyErrors) => true;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                string json = reader.ReadToEnd();
                positions = JsonConvert.DeserializeObject<List<rest_position>>(json);
            }

            return positions;
        }
    }
}