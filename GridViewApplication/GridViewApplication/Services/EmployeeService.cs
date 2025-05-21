using GridViewApplication.Dto;
using GridViewApplication.Dto.RestEmployee;
using GridViewApplication.Dto.RestLogin;
using GridViewApplication.Library;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Web;


namespace GridViewApplication.Services
{
    public class EmployeeService
    {
        public static string errorMsg { get; set; }
        private static Boolean _isActiveService = ConfigurationManager.AppSettings["ActiveTokenSvc"].ToString().ToLower().Equals("true")==true?true:false;
        private static Int32 _timeout = Convert.ToInt32(ConfigurationManager.AppSettings["timeoutTokenSvc"].ToString());
        public static List<rest_employee> getListEmployee(string token,
            string filterOn, 
            string filterQuery, 
            string sortBy, 
            string isAscending, 
            string pageNumber, 
            string pageSize)
        {
            string UrlEmployeeSvc = "https://localhost:44327/api/Employee";
            string url = "";
            HttpWebResponse response = null;
            List<rest_employee> mrp;
            try
            {
                url = "http://localhost:5001/api/integrasi/insert-mobitek";
                if (_isActiveService)
                    url = string.Format(UrlEmployeeSvc, "");

                List<string> param = new List<string>();
                if (!string.IsNullOrEmpty(filterOn))
                {
                    param.Add("filterOn=" + filterOn);
                }
                if (!string.IsNullOrEmpty(filterQuery))
                {
                    param.Add("filterQuery=" + filterQuery);
                }
                if (!string.IsNullOrEmpty(sortBy))
                {
                    param.Add("sortBy=" + sortBy);
                }
                if (!string.IsNullOrEmpty(isAscending))
                {
                    param.Add("isAscending=" + isAscending);
                }
                if (!string.IsNullOrEmpty(pageNumber))
                {
                    param.Add("pageNumber=" + pageNumber);
                }
                if (!string.IsNullOrEmpty(pageSize))
                {
                    param.Add("pageSize=" + pageSize);
                }
                string parameters = string.Join("&", param);
                if (!string.IsNullOrEmpty(parameters))
                {
                    url = string.Format("{0}?{1}", url, parameters);
                }

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";
                request.Timeout = _timeout;
                request.ReadWriteTimeout = _timeout;
                request.Headers.Add("Authorization", string.Format("Bearer {0}", token));
                ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
                
                response = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    mrp = JsonConvert.DeserializeObject<List<rest_employee>>(result);
                }
                return mrp;
            }
            catch (WebException e)
            {
                if (e.Response == null)
                {
                    errorMsg = string.Format("Error: {0} {1} {2}", e.Status, url, e.Message);
                    errorMsg = string.Format("{0} - {1} - {2}", errorMsg, Util.getDetail(e), "e.RESPONSE = NULL");
                    Util.CreateLog(errorMsg);
                    return null;
                }
                var result = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    response = (HttpWebResponse)e.Response;
                    if ((int)response.StatusCode == 401)
                    {
                        errorMsg = string.Format("Error: {0} {1} {2}", e.Status, url, e.Message);
                        errorMsg = string.Format("{0} - {1} - {2}", errorMsg, Util.getDetail(e), result);
                        Util.CreateLog(errorMsg);
                        return null;
                    }
                    else
                    {
                        errorMsg = string.Format("Error: {0} {1} {2}", e.Status, url, e.Message);
                        errorMsg = string.Format("{0} - {1} - {2}", errorMsg, Util.getDetail(e), result);
                        Util.CreateLog(errorMsg);
                        return null;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                errorMsg = string.Format("Error: {0} {1}", UrlEmployeeSvc, e.Message);
                errorMsg = string.Format("{0} - {1}", errorMsg, Util.getDetail(e));
                Util.CreateLog(errorMsg);
                return null;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        public static void deleteEmployee(string token, string id)
        {
            string url = $"https://localhost:44327/api/Employee/{id}";
            HttpWebResponse response = null;

            try
            {
                if (_isActiveService)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "DELETE";
                    request.Timeout = _timeout;
                    request.ReadWriteTimeout = _timeout;
                    request.Headers.Add("Authorization", $"Bearer {token}");

                    ServicePointManager.ServerCertificateValidationCallback =
                        (senderX, certificate, chain, sslPolicyErrors) => true;

                    response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.OK)
                    {
                        Util.CreateLog($"Successfully deleted employee with ID {id}");
                    }
                }
            }
            catch (WebException e)
            {
                string result = "";
                if (e.Response != null)
                {
                    using (var reader = new StreamReader(e.Response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }
                }

                errorMsg = $"Error: {e.Status} {url} {e.Message}";
                errorMsg = $"{errorMsg} - {Util.getDetail(e)} - {result}";
                Util.CreateLog(errorMsg);
            }
            catch (Exception e)
            {
                errorMsg = $"Error: {url} {e.Message} - {Util.getDetail(e)}";
                Util.CreateLog(errorMsg);
            }
            finally
            {
                response?.Close();
            }
        }

        public static void updateEmployee(string token, string id, string name, string dept, string posId)
        {
            string url = $"https://localhost:44327/api/Employee/{id}";
            HttpWebResponse response = null;
            try
            {
                if (_isActiveService)
                {
                    rest_employee data = getEmployeeById(token, id);

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "PUT";
                    request.Timeout = _timeout;
                    request.ReadWriteTimeout = _timeout;
                    request.Headers.Add("Authorization", $"Bearer {token}");

                    ServicePointManager.ServerCertificateValidationCallback =
                        (senderX, certificate, chain, sslPolicyErrors) => true;

                    if (name.Length > 30)
                    {
                        Util.CreateLog("Validation error: Name exceeds 30 characters.");
                        return;
                    }

                    var requestBody = new
                    {
                        name = name,
                        department = dept,
                        levelId = data.level.id,
                        positionId = posId
                    };

                    string jsonBody = JsonConvert.SerializeObject(requestBody);

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(jsonBody);
                        streamWriter.Flush();
                    }

                    response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.OK)
                    {
                        Util.CreateLog($"Successfully updated employee with ID {id}");
                    }
                }
            }
            catch (WebException e)
            {
                string result = "";
                if (e.Response != null)
                {
                    using (var reader = new StreamReader(e.Response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }
                }

                errorMsg = $"Error: {e.Status} {url} {e.Message}";
                errorMsg = $"{errorMsg} - {Util.getDetail(e)} - {result}";
                Util.CreateLog(errorMsg);
            }
            catch (Exception e)
            {
                errorMsg = $"Error: {url} {e.Message} - {Util.getDetail(e)}";
                Util.CreateLog(errorMsg);
            }
            finally
            {
                response?.Close();
            }
        }

        public static rest_employee getEmployeeById(string token, string id)
        {
            string url = $"https://localhost:44327/api/Employee/{id}";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", $"Bearer {token}");

                ServicePointManager.ServerCertificateValidationCallback =
                    (senderX, certificate, chain, sslPolicyErrors) => true;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseBody = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<rest_employee>(responseBody);
                }
            }
            catch (Exception e)
            {
                Util.CreateLog($"Error retrieving employee {id}: {e.Message}");
                return null;
            }
        }


    }
}