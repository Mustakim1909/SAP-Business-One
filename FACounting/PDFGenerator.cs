using EInvoice.Models;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastReport;
using FastReport.Data;
using System.ComponentModel;
using FastReport.Export.Pdf;

namespace EInvoice
{
    public class PDFGenerator
    {
        private string _fastreportConnectionString { get; set; }
        public async Task<string> GeneratePDFWithMSSQL(int InvoiceId, string eInvoiceNumber, string InvoiceType, string CurrencyCode)
        {
            try
            {
                Log.Information("Generating Pdf...");
                var json = JsonConvert.SerializeObject(AppConfigManager.Config, Formatting.Indented);
                var deserialized = JsonConvert.DeserializeObject<Appsettings>(json);
                //InvoiceId = 1;
                string currectDate = $"{DateTime.Now:ddMMyyyy}";
                string frxDesign = "";
                var crCode = CurrencyCode.Trim();
                frxDesign = deserialized.FRXPathForInvoice.Where(x => x.InvoiceType == InvoiceType).Select(x => x.FRXPath).FirstOrDefault();


                if (frxDesign == null)
                {
                    Log.Error($"Oops, the ops file was not found.");
                    Console.WriteLine($"Oops, the frx file was not found.");
                    return string.Empty;
                }
                if (!File.Exists(frxDesign))
                {
                    Log.Error($"Oops, the ops file was not found at the path: {frxDesign}. Please check the path and try again.");
                    Console.WriteLine($"Oops, the frx file was not found at the path: {frxDesign}. Please check the path and try again.");
                    return string.Empty;
                }
                var connectionString = deserialized.AIFPortalConnectionString;
                // Set Connection string .
                _fastreportConnectionString = connectionString;
                Report report = new Report();

                //Log.Information("Fast report loaded.");
                // Load the FRX file
                report.Load(frxDesign);

                // Set up a dynamic SQL connection
                MsSqlDataConnection sqlConnection = new MsSqlDataConnection
                {
                    ConnectionString = _fastreportConnectionString
                };
                // Find the existing connection in the report and replace its connection string
                foreach (var connection in report.Dictionary.Connections)
                {
                    if (connection is MsSqlDataConnection existingConnection)
                    {
                        existingConnection.ConnectionString = _fastreportConnectionString;
                    }
                }
                // Ensure the connection is set to active
                sqlConnection.Enabled = true;

                // Prepare the report with the new connection string
                report.Dictionary.Connections.Add(sqlConnection);
                report.Dictionary.Connections[0].Enabled = true;
                string OutputPdfPath = string.Empty;
                try
                {
                    // Set the InvoiceId parameter
                    report.SetParameterValue("Id", InvoiceId);
                    Console.WriteLine($"PDF genrate for Invoice Number : {eInvoiceNumber}");
                    Log.Information($"PDF genrate for Invoice Number : {eInvoiceNumber}");
                    if (report.Report.Prepare())
                    {
                        PDFExport pdfExport = new PDFExport
                        {
                            ShowProgress = true,
                            Subject = $"Invoice {eInvoiceNumber}",
                            Title = $"Invoice {eInvoiceNumber}"
                        };

                        //string OutputPdfPath = Path.Combine(OutputPdfFolderPath, $"{inv.eInvoiceCodeOrNumber}.pdf");
                        OutputPdfPath = Path.Combine(deserialized.OutputFolderPath, currectDate, InvoiceType, "EmbededQRPDF", $"{eInvoiceNumber}_WithQR.pdf");
                        var PDFDirectory = Path.GetDirectoryName(OutputPdfPath);
                        if (!Directory.Exists(PDFDirectory))
                        {
                            Directory.CreateDirectory(PDFDirectory);
                        }

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            pdfExport.Export(report.Report, memoryStream);

                            // Save the MemoryStream content to a file
                            using (FileStream fileStream = new FileStream(OutputPdfPath, FileMode.Create, FileAccess.Write))
                            {
                                memoryStream.WriteTo(fileStream);
                                Log.Information("Pdf Generated Successfully");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to prepare report for InvoiceId: {InvoiceId}");
                        Log.Information($"Failed to prepare report for InvoiceId: {InvoiceId}");
                    }
                }
                catch (Exception innerEx)
                {
                    Log.Error($"Error generating PDF for InvoiceId: {InvoiceId}. Exception: {JsonConvert.SerializeObject(innerEx)}");
                    Console.WriteLine($"Error generating PDF for InvoiceId: {InvoiceId}. Exception: {JsonConvert.SerializeObject(innerEx)}");
                }
                // Dispose of the report after processing all invoices
                report.Dispose();

                return OutputPdfPath;
            }
            catch (Exception ex)
            {
                Log.Information($"Exception in GeneratePDFWithMSSQL: {JsonConvert.SerializeObject(ex)}");
                Console.WriteLine($"Exception in GeneratePDFWithMSSQL: {JsonConvert.SerializeObject(ex)}");
                return string.Empty;
            }
        }
    }
}
