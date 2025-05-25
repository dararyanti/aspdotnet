using GridViewApplication.Dto.RestDepartment;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;

namespace GridViewApplication.Services
{
    public class DepartmentService
    {
        private static readonly string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"]?.TrimEnd('/') ?? "";
        public static List<rest_department> GetDepartments(string token)
        {
            string url = $"{BaseUrl}/api/Department";
            List<rest_department> departments = new List<rest_department>();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add("Authorization", $"Bearer {token}");

            ServicePointManager.ServerCertificateValidationCallback =
                (senderX, certificate, chain, sslPolicyErrors) => true;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                string json = reader.ReadToEnd();
                departments = JsonConvert.DeserializeObject<List<rest_department>>(json);
            }

            return departments;
        }
    }
}