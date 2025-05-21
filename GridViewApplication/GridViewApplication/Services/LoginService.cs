using GridViewApplication.Dto;
using GridViewApplication.Dto.RestLogin;
using GridViewApplication.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace GridViewApplication.Services
{
    public class LoginService
    {
        public static string errorMsg { get; set; }
        private static Boolean _isActiveService = ConfigurationManager.AppSettings["ActiveTokenSvc"].ToString().ToLower().Equals("true")==true?true:false;
        private static Int32 _timeout = Convert.ToInt32(ConfigurationManager.AppSettings["timeoutTokenSvc"].ToString());

        public static rest_response_login Login(LoginDto login)
        {
            string UrlTokenSvc = ConfigurationManager.AppSettings["UrlTokenSvc"].ToString();
            HttpWebResponse response = null;
            rest_response_login mrp;
            try
            {
                string url = "http://localhost:5001/api/integrasi/token";
                if(_isActiveService)
                    url = string.Format(UrlTokenSvc, "");
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                Util.CreateLog(url);
                request.ContentType = "application/json";
                request.Method = "POST";
                request.Timeout = _timeout;
                request.ReadWriteTimeout = _timeout;
                ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(login);
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                    Util.CreateLog(json);
                }
                response = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    mrp = JsonConvert.DeserializeObject<rest_response_login>(result);
                    Util.CreateLog(result);
                }                
                return mrp;
            }
            catch (WebException e)
            {
                errorMsg = string.Format("{0} - {1}", errorMsg, Util.getDetail(e));
                Util.CreateLog(errorMsg);

                var result = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    response = (HttpWebResponse)e.Response;
                    errorMsg = string.Format("Errorcode: {0} {1} {2}", (int)response.StatusCode, UrlTokenSvc, e.Message);
                    errorMsg = string.Format("{0} - {1} - {2}", errorMsg, Util.getDetail(e), result);
                }
                else
                {
                    errorMsg = string.Format("Error: {0} {1} {2}", e.Status, UrlTokenSvc, e.Message);
                    errorMsg = string.Format("{0} - {1} - {2}", errorMsg, Util.getDetail(e), result);
                }
                Util.CreateLog(errorMsg);
                return null;
            }
            catch (Exception e)
            {
                errorMsg = string.Format("Error: {0} {1}", UrlTokenSvc, e.Message);
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
    }
}