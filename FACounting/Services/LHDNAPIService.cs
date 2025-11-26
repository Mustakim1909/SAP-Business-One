using EInvoice.Models;
using EInvoice.Services.Interface;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static EInvoice.Models.Advin;

namespace EInvoice.Services
{
    public class LHDNAPIService : ILHDNAPIService
    {

        String strBaseURL = AppConfigManager.Config.APIUrl;

        public (LoginResponse objResults, String rawResponse) fnLogin()
        {
            try
            {
                Log.Information("Login Api Called");
                String strValidation = "";
                var url = strBaseURL + "api/2024.1/JSONLogin";
                ServicePointManager.ServerCertificateValidationCallback +=
    (sender, cert, chain, sslPolicyErrors) =>
    {
        Log.Error($"SSL Policy Errors: {sslPolicyErrors}");
        Log.Error($"Requested Host: {((HttpWebRequest)sender).RequestUri.Host}");
        Log.Error($"Certificate Subject: {cert.Subject}");


        foreach (var status in chain.ChainStatus)
        {
            Log.Error($"Chain Status: {status.Status} - {status.StatusInformation}");
        }

        return sslPolicyErrors == SslPolicyErrors.None;
    };


                RestClient client = new RestClient(url);
                RestRequest request = new RestRequest(url, Method.Post);
                request.AddHeader("Content-Type", "application/json");

                var json = JsonConvert.SerializeObject(AppConfigManager.Config, Formatting.Indented);
                /*var strJSON = @"{
                ""loginId"": ""superadmin"",
                ""password"": ""admin"",
                ""domain"": ""ixtelecom""
            }";*/

                var body = json;
                request.AddParameter("text/plain", body, ParameterType.RequestBody);

                //            ServicePointManager.ServerCertificateValidationCallback +=
                //(sender, cert, chain, sslPolicyErrors) => true;

                var response = client.Execute(request, Method.Post);
                Log.Information("Api response :" + JsonConvert.SerializeObject(response));
                LoginResponse objResults = JsonConvert.DeserializeObject<LoginResponse>(response.Content);
                if(objResults.StatusCode == 403 || objResults.StatusCode == 429 || objResults.StatusCode == 400)
                {
                    return fnLogin();
                }
                if (objResults == null)
                {
                    objResults = new LoginResponse();
                }
                return (objResults, response.Content);


            }
            catch (Exception ex)
            {
                Log.Error($"Exception In LoginApi : {JsonConvert.SerializeObject(ex)}");
                SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Exception in Login", SAPbouiCOM.BoMessageTime.bmt_Medium, true);
            }
            return (new LoginResponse(), string.Empty);
        }

        public (ApiResponse objResults, String rawResponse) fnSubmitDocument(String strJSON, String strToken)
        
        {
            try
            {
                Log.Information("SubmitDocument Api Called");
                String strValidation = "";
                var url = strBaseURL + "api/2024.1/JSONSubmitDocument";

                RestClient client = new RestClient(url);
                RestRequest request = new RestRequest(url, Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Bearer " + strToken);
                var body = strJSON;
                request.AddParameter("text/plain", body, ParameterType.RequestBody);

                ApiResponse objRootObj = null;
                var response = client.Execute(request, Method.Post);
                var objResults = JsonConvert.DeserializeObject<ApiResponse>(response.Content);
                if (objResults.StatusCode == 400)
                {
                    return fnSubmitDocument(strJSON, strToken);
                }
                if (objResults == null)
                {
                    objResults = new ApiResponse();
                }


                return (objResults, response.Content);
            }
            catch (Exception ex)
            {
                Log.Error($"Exception in SubmitDocument Api : {JsonConvert.SerializeObject(ex)}");
                SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Exception in SubmitDocument", SAPbouiCOM.BoMessageTime.bmt_Medium, true);
            }
            return (new ApiResponse(), string.Empty);
        }

        public (DocumentStatusResponse objResults, String rawResponse) fnCheckStatus(String strUUID, String strToken)
        {
            try
            {
                Log.Information("GetDocumentDetails Api Called");
                String strValidation = "";

                var url = strBaseURL + "api/2024.1/JSONGetDocumentDetails?uuid=" + strUUID;

                RestClient client = new RestClient(url);
                RestRequest request = new RestRequest(url, Method.Get);
                //	request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Bearer " + strToken);

                ApiResponse objRootObj = null;
                var response = client.Execute(request, Method.Get);
                var objResults = JsonConvert.DeserializeObject<DocumentStatusResponse>(response.Content);
                if(objResults.StatusCode == 500)
                {
                    return fnCheckStatus(strUUID, strToken);
                }
                if (objResults == null)
                {
                    objResults = new DocumentStatusResponse();
                }


                return (objResults, response.Content);
            }
            catch (Exception ex)
            {
                Log.Error($"Exception in GetDocumentDetails Api : {JsonConvert.SerializeObject(ex)}");
                SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Exception in GetDocumentDetails", SAPbouiCOM.BoMessageTime.bmt_Medium, true);
            }
            return (new DocumentStatusResponse(), string.Empty);
        }
        public (DocumentStatusResponse objResults, String rawResponse) fnCancelInvoice(String strJSON, String strToken)
        {
            try
            {
                Log.Information("CancelDocument Api Called");
                String strValidation = "";

                var url = strBaseURL + "api/2024.1/JSONCancelDocument";

                RestClient client = new RestClient(url);
                RestRequest request = new RestRequest(url, Method.Put);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Bearer " + strToken);
                var body = strJSON;
                request.AddParameter("text/plain", body, ParameterType.RequestBody);

                ApiResponse objRootObj = null;
                var response = client.Execute(request, Method.Put);
                var objResults = JsonConvert.DeserializeObject<DocumentStatusResponse>(response.Content);
                if (objResults.StatusCode == 400)
                {
                    return fnCancelInvoice(strJSON, strToken);
                }
                if (objResults == null)
                {
                    objResults = new DocumentStatusResponse();
                }


                return (objResults, response.Content);
            }
            catch (Exception ex)
            {
                Log.Error($"Exception in CancelDocument Api : {JsonConvert.SerializeObject(ex)}");
                SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Exception in CancelDocument", SAPbouiCOM.BoMessageTime.bmt_Medium, true);
            }
            return (new DocumentStatusResponse(), string.Empty);
        }
        public (DocumentStatusResponse objResults, String rawResponse) fnSendEmail(String strJSON, String strToken)
        {
            try
            {
                Log.Information("EmailSender Api Called");
                String strValidation = "";

                var url = strBaseURL + "api/2024.1/EmailSender";

                RestClient client = new RestClient(url);
                RestRequest request = new RestRequest(url, Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Bearer " + strToken);
                var body = strJSON;
                request.AddParameter("text/plain", body, ParameterType.RequestBody);

                ApiResponse objRootObj = null;
                var response = client.Execute(request, Method.Post);
                var objResults = JsonConvert.DeserializeObject<DocumentStatusResponse>(response.Content);
                if (objResults == null)
                {
                    objResults = new DocumentStatusResponse();
                }


                return (objResults, response.Content);
            }
            catch (Exception ex)
            {
                Log.Error($"Exception in EmailSender Api : {JsonConvert.SerializeObject(ex)}");
                SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Exception in EmailSender", SAPbouiCOM.BoMessageTime.bmt_Medium, true);
            }
            return (new DocumentStatusResponse(), string.Empty);
        }

    }
}
