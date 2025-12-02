using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EInvoice.Models
{
    public class Appsettings
    {
        [JsonPropertyName("loginId")]
        public string LoginId { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
        [JsonPropertyName("domain")]
        public string Domain { get; set; }
        [JsonPropertyName("loginpath")]
        public string LogPath { get; set; }
        [JsonPropertyName("frXPathForInvoice")]
        public List<FRXPathModel> FRXPathForInvoice { get; set; }
        [JsonPropertyName("aifportalconnectionstring")]
        public string AIFPortalConnectionString { get; set; }
        [JsonPropertyName("outputfolderpath")]
        public string OutputFolderPath { get; set; }
        [JsonPropertyName("apiurl")]
        public string APIUrl { get; set; }
        [JsonPropertyName("documenttypes")]
        public string DocumentTypes { get; set; }

    }
    public class FRXPathModel
    {
        [JsonPropertyName("frxpath")]
        public string FRXPath { get; set; }
        [JsonPropertyName("invoicetype")]
        public string InvoiceType { get; set; }
    }
}
