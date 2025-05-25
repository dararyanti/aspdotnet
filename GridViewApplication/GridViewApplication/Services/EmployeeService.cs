using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using GridViewApplication.Dto.RestEmployee;
using Newtonsoft.Json;

namespace GridViewApplication.Services
{
    public class EmployeeService
    {
        public static string errorMsg { get; set; }
        private static readonly string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"]?.TrimEnd('/') ?? "";

        private void AddAuthorizationHeader(HttpWebRequest request, string token)
        {
            request.Headers.Add("Authorization", $"Bearer {token}");
        }

        public (List<rest_employee> Data, int TotalCount) GetEmployees(
            string token,
            int pageNumber,
            int pageSize,
            string sortColumn,
            string sortDirection,
            string searchTerm)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token), "Authorization token is null or empty.");

            var queryParams = new List<string>
            {
                $"pageNumber={pageNumber}",
                $"pageSize={pageSize}",
                $"sortColumn={sortColumn}",
                $"sortDirection={sortDirection}"
            };
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
            }

            string url = $"{BaseUrl}/api/Employee?{string.Join("&", queryParams)}";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            AddAuthorizationHeader(request, token);
            request.Timeout = 30000;

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    var pagedResponse = JsonConvert.DeserializeObject<PagedResponse<rest_employee>>(json);
                    if (pagedResponse == null) throw new Exception("Invalid response from employee API");
                    return (pagedResponse.Data, pagedResponse.TotalCount);
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        string error = reader.ReadToEnd();
                        Console.WriteLine("API Error Response: " + error);
                    }
                }
                throw;
            }
        }

        public rest_employee GetEmployeeById(string token, string id)
        {
            string url = $"{BaseUrl}/api/Employee/{id}";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            AddAuthorizationHeader(request, token);
            request.Timeout = 30000;

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<rest_employee>(json);
                }
            }
            catch (WebException ex) when (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public rest_create_employee CreateEmployee(string token, rest_create_employee dto)
        {
            string url = $"{BaseUrl}/api/Employee";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            AddAuthorizationHeader(request, token);
            request.Timeout = 30000;

            string jsonBody = JsonConvert.SerializeObject(dto);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonBody);
            request.ContentLength = bytes.Length;

            using (var reqStream = request.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<rest_create_employee>(json) ?? throw new Exception("Failed to deserialize created employee");
            }
        }

        public void UpdateEmployee(string token, string id, rest_employee dto)
        {
            string url = $"{BaseUrl}/api/Employee/{id}";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "PUT";
            request.ContentType = "application/json";
            AddAuthorizationHeader(request, token);
            request.Timeout = 30000;

            string jsonBody = JsonConvert.SerializeObject(dto);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonBody);
            request.ContentLength = bytes.Length;

            using (var reqStream = request.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Failed to update employee, status code: {response.StatusCode}");
            }
        }

        public void DeleteEmployee(string token, Guid id)
        {
            string url = $"{BaseUrl}/api/Employee/{id}";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "DELETE";
            AddAuthorizationHeader(request, token);
            request.Timeout = 30000;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Failed to delete employee, status code: {response.StatusCode}");
            }
        }

        private class PagedResponse<T>
        {
            [JsonProperty("data")]
            public List<T> Data { get; set; } = new List<T>();

            [JsonProperty("totalCount")]
            public int TotalCount { get; set; }
        }
    }
}
