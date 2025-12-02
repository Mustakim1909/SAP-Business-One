using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbouiCOM.Framework;
using System.IO;
using static EInvoice.Models.Advin;
using static EInvoice.Models.Advin.ADVIN;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using EInvoice.Services.Interface;
using EInvoice.Services;
using EInvoice.Models;
using Serilog;
using System.Globalization;
using Microsoft.Extensions.Options;

namespace EInvoice
{

	public class Invoice
	{
		public Appsettings _appsettings;
		private static ILHDNAPIService _ILhdnApiService;
		
		public static Boolean BaseB2B(String strTable, String strTable1, String strTable12, String strTable9, String strTable11, SAPbobsCOM.Company oCompany = null, String strCaller = null, String strToken = null)
		{
			SAPbobsCOM.UserFieldsMD oUserField;
			SAPbobsCOM.Recordset oRecordset;

			// 1. Check if the field exists

			_ILhdnApiService = new LHDNAPIService();
			#region Definations 

			//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Starting BaseB2B " + DateTime.Now);
			Log.Information("Starting BaseB2B");
			Boolean bolValid = false;


			ADVIN objRoot = new ADVIN();
			var invoiceLines = new LineItem();
			var tax = new DocTaxTotal();


			String strVATNo = "";
			Decimal dcVATSum;
			Decimal dcVATSumFC;
			String strVATSum = "";		
			Decimal dcInvTotal;
			Decimal dcInvTotalFC;

			String strInvTotal = "";
			String strCompanyName = "";
			String strPosting = "";
			String strPostingDate = "";
			String strFinalDateTime = "";
			String strBase64 = "";
			String strCreateTS = "";
			Int32 intObjType = 0;
			String strDocCur = "";
			String strCardName = "";
			String strXMLFileName = "";
			String strXMLFilePath = "";
			String strAttachmentFolder = "";
			Decimal dcmlTotalCharge = 0.0M;
			String strInvoiceTypeCode = "01";
			#endregion

			#region Retrieval
			String strFormType = String.Empty;
			SAPbouiCOM.Form oForm;
			if (strTable == "OINV")
			{
				strFormType = "133";
			}
			else
			{
				strFormType = "179";
			}
			// Application.SBO_Application.Forms.ActiveForm.TypeEx;
			Int32 intCount = Application.SBO_Application.Forms.ActiveForm.TypeCount;

			oForm = Application.SBO_Application.Forms.GetForm(strFormType, intCount);


			SAPbouiCOM.DBDataSource db = oForm.DataSources.DBDataSources.Item(strTable);
			SAPbouiCOM.DBDataSource db12 = oForm.DataSources.DBDataSources.Item(strTable12);

			//String strFlickStatus = Convert.ToString(db.GetValue("U_ADVIN_Status", 0));

			//if (strFlickStatus.Length > 0 && strFlickStatus.ToUpper().Trim() == "VALID")
			//{
			//	SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("It seems Document is already Valid.", SAPbouiCOM.BoMessageTime.bmt_Medium, true);
			//	System.Runtime.InteropServices.Marshal.ReleaseComObject(oForm);
			//	System.Runtime.InteropServices.Marshal.ReleaseComObject(db);
			//	System.Runtime.InteropServices.Marshal.ReleaseComObject(db12);
			//	GC.WaitForPendingFinalizers();
			//	throw new Exception("It seems Document is already Valid.");
			//}


			String strInvNo = Convert.ToString(db.GetValue("DocEntry", 0));
			String strWddStatus = Convert.ToString(db.GetValue("WddStatus", 0));

			String strInvDocNum = Convert.ToString(db.GetValue("DocNum", 0));

			if (strInvNo.Length == 0)
				return bolValid;

			if (strWddStatus == "P" || strWddStatus == "-")
				goto case1;
			else
				return bolValid;

			case1:

			SAPbobsCOM.Recordset oRecordSet;
			oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
			String strQry = "";

			//////////////////////////2.Change here Doc Type  Purchase Request is Valid for Budget

			strQry = String.Format(@"

select 
T0.""CompnyName"" ""Name""
,T0.""TaxIdNum2"" ""VAT No""
,T0.""Phone1"" ""Phone""
,T0.""CompnyAddr"" ""Add0""
--,ifnull(T0.""TaxIdNum2"",T0.""TaxIdNum"") ""VAT No""

,T1.""Street""
,T1.""StreetNo""
,T1.""Block""
,T1.""Building""
,T1.""City""
,T1.""ZipCode""
,T0.""Country""
,T0.""State"" ""State""
,T0.""FreeZoneNo""

from ""OADM"" T0 
cross join ""ADM1"" T1
left outer join ""OCST"" T2 on T0.""State"" = T2.""Code""

"

, "1");


			//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Starting OADM Query " + strQry + DateTime.Now);

			oRecordSet.DoQuery(strQry);
			strCompanyName = Convert.ToString(oRecordSet.Fields.Item("Name").Value);
			strVATNo = Convert.ToString(oRecordSet.Fields.Item("VAT No").Value);

			String strSellerStreetName = Convert.ToString(oRecordSet.Fields.Item("Street").Value);
			String strSellerStreetAddr0 = Convert.ToString(oRecordSet.Fields.Item("Add0").Value);
			String strSellerBuildingNumber = Convert.ToString(oRecordSet.Fields.Item("Building").Value);
			String strSellerCity = Convert.ToString(oRecordSet.Fields.Item("City").Value);
			String strSellerZipCode = Convert.ToString(oRecordSet.Fields.Item("ZipCode").Value);
			String strSellerCountry = Convert.ToString(oRecordSet.Fields.Item("Country").Value);
			String strSellerState = Convert.ToString(oRecordSet.Fields.Item("State").Value);
			String strSellerPhone = Convert.ToString(oRecordSet.Fields.Item("Phone").Value);
			String strSellerCRNo = Convert.ToString(oRecordSet.Fields.Item("FreeZoneNo").Value);

			strQry = String.Format($@"
SELECT 
    T0.""Street"", 
    T0.""Block"", 
    T0.""StreetNo"",
    T0.""Building"",
    T0.""City"",
    T0.""ZipCode"",
    T2.""Code"" AS ""State"",
    T0.""Country"",
    T0.""Address"",
    T0.""Address2"",
    T0.""Address3"",
    T3.""U_SSTexp"" ,
    T3.""U_TaxExemptionReason""
FROM ""CRD1"" T0
LEFT OUTER JOIN ""{strTable}"" T1 ON T0.""CardCode"" = T1.""CardCode""
LEFT OUTER JOIN ""OCST"" T2 ON T0.""State"" = T2.""Code""
LEFT OUTER JOIN ""OCRD"" T3 ON T0.""CardCode"" = T3.""CardCode""  -- Join with Business Partner Master
WHERE T0.""CardCode"" = T1.""CardCode""
  AND T0.""Address"" = T1.""PayToCode""
  AND T1.""DocEntry"" = '{strInvNo}'
");

			Log.Information("Retrieving BuyerDetails");
			oRecordSet.DoQuery(strQry);
			String strCustomer_Add_StreetNameM = "";
			String strCustomer_Add_Address2 = "";
			String strCustomer_Add_Address3 = "";
			String strCustomer_Add_AdditionalStreetNameM = "";
			String intCustomer_Add_BuildingNumberM = "";
			String intCustomer_Add_PlotIdentification4DigM = "";
			String strCustomer_Add_CityNameM = "";
			String intCustomer_Add_PostalZone5DigM = "";
			String strCustomer_Add_CountrySubentityM = "";
			String strCustomer_Add_CitySubdivisionNameM = "";
			String strCustomer_Add_CountryM = "";
			String strCustomer_sst = "";
			String taxexemptionreason = "";

			if (oRecordSet.RecordCount > 0)
			{
				strCustomer_Add_StreetNameM = Convert.ToString(oRecordSet.Fields.Item("Street").Value);
				strCustomer_Add_Address2 = Convert.ToString(oRecordSet.Fields.Item("Block").Value);
				strCustomer_Add_Address3 = Convert.ToString(oRecordSet.Fields.Item("Address3").Value);
				strCustomer_Add_AdditionalStreetNameM = Convert.ToString(oRecordSet.Fields.Item("StreetNo").Value);
				intCustomer_Add_BuildingNumberM = Convert.ToString(oRecordSet.Fields.Item("Building").Value);
				intCustomer_Add_PlotIdentification4DigM = Convert.ToString(oRecordSet.Fields.Item("Building").Value);
				strCustomer_Add_CityNameM = Convert.ToString(oRecordSet.Fields.Item("City").Value);
				intCustomer_Add_PostalZone5DigM = Convert.ToString(oRecordSet.Fields.Item("ZipCode").Value);
				strCustomer_Add_CountrySubentityM = Convert.ToString(oRecordSet.Fields.Item("State").Value);
				strCustomer_Add_CitySubdivisionNameM = Convert.ToString(oRecordSet.Fields.Item("State").Value);
                strCustomer_Add_CountryM = Convert.ToString(oRecordSet.Fields.Item("Country").Value);
				strCustomer_sst = Convert.ToString(oRecordSet.Fields.Item("U_SSTexp").Value);
				taxexemptionreason = Convert.ToString(oRecordSet.Fields.Item("U_TaxExemptionReason").Value);
				//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Country Code from DB12" + strCustomer_Add_CountryM + DateTime.Now);


			}

            strQry = String.Format($@"
		SELECT
	T0.""Street"",
    T0.""Block"",
    T0.""StreetNo"",
    T0.""Building"",
    T0.""City"",
    T0.""ZipCode"",
    T2.""Code"" ""State"",
    T0.""Country"",
    T0.""Address"",
    T0.""Address2"",
    T0.""Address3""
FROM ""CRD1"" T0
LEFT OUTER JOIN ""{strTable}"" T1 ON T0.""CardCode"" = T1.""CardCode""
LEFT OUTER JOIN ""OCST"" T2 ON T0.""State"" = T2.""Code"" AND T0.""Country"" = T2.""Country""
WHERE T0.""CardCode"" = T1.""CardCode""
  AND T0.""Address"" = T1.""ShipToCode""
  AND T1.""DocEntry"" = '{strInvNo}'
");


			Log.Information("Retrieving ShippingDetails");
			oRecordSet.DoQuery(strQry);
			String strShipping_Add_StreetNameM = "";
			String strShipping_Add_Address2 = "";
			String strShipping_Add_Address3 = "";
			String strShipping_Add_AdditionalStreetNameM = "";
			String Shipping_Add_BuildingNumberM = "";
			String Shipping_Add_PlotIdentification4DigM = "";
			String strShipping_Add_CityNameM = "";
			String Shipping_Add_PostalZone5DigM = "";
			String strShipping_Add_CountrySubentityM = "";
			String strShipping_Add_CitySubdivisionNameM = "";
			String strShipping_Add_CountryM = "";

			strShipping_Add_StreetNameM = Convert.ToString(oRecordSet.Fields.Item("Street").Value);
			strShipping_Add_Address2 = Convert.ToString(oRecordSet.Fields.Item("Block").Value);
			strShipping_Add_Address3 = Convert.ToString(oRecordSet.Fields.Item("Address3").Value);
			strShipping_Add_AdditionalStreetNameM = Convert.ToString(oRecordSet.Fields.Item("StreetNo").Value);
			Shipping_Add_BuildingNumberM = Convert.ToString(oRecordSet.Fields.Item("Building").Value);
			Shipping_Add_PlotIdentification4DigM = Convert.ToString(oRecordSet.Fields.Item("Building").Value);
			strShipping_Add_CityNameM = Convert.ToString(oRecordSet.Fields.Item("City").Value);
			Shipping_Add_PostalZone5DigM = Convert.ToString(oRecordSet.Fields.Item("ZipCode").Value);
			strShipping_Add_CountrySubentityM = Convert.ToString(oRecordSet.Fields.Item("State").Value);
			strShipping_Add_CitySubdivisionNameM = Convert.ToString(oRecordSet.Fields.Item("State").Value);
			strShipping_Add_CountryM = Convert.ToString(oRecordSet.Fields.Item("Country").Value);
			//objRoot.document.dcmlTaxTotals_TaxTotalM = Convert.ToDecimal(strVATSuSAR);


			dcInvTotal = Convert.ToDecimal(db.GetValue("DocTotal", 0));
			dcInvTotalFC = Convert.ToDecimal(db.GetValue("DocTotalFC", 0));

			if (dcInvTotalFC != 0.00M)
				dcInvTotal = dcInvTotalFC;


			strInvTotal = Convert.ToString(Decimal.Round(dcInvTotal, 2, MidpointRounding.AwayFromZero));


			dcVATSum = Convert.ToDecimal(db.GetValue("VATSum", 0));

			strVATSum = Convert.ToString(Decimal.Round(dcVATSum, 2, MidpointRounding.AwayFromZero));

			Decimal dcDiscountSum = Convert.ToDecimal(db.GetValue("DiscSum", 0));
			Decimal dcDiscountSumFC = Convert.ToDecimal(db.GetValue("DiscSumFC", 0));
			if (dcDiscountSumFC != 0.00M)
				dcDiscountSum = dcDiscountSumFC;

			String strDiscountSum = Convert.ToString(Decimal.Round(dcDiscountSum, 2, MidpointRounding.AwayFromZero));

			intObjType = Convert.ToInt32(db.GetValue("ObjType", 0));

			strDocCur = Convert.ToString(db.GetValue("DocCur", 0));
			strCardName = Convert.ToString(db.GetValue("CardName", 0));
			String strCardCode = Convert.ToString(db.GetValue("CardCode", 0));

			strCardCode = strCardCode.Trim();

			strPosting = db.GetValue("CreateDate", 0);
			strCreateTS = db.GetValue("CreateTS", 0);



			if (strCreateTS.Length == 1)
				strCreateTS = "00000" + strCreateTS;
			if (strCreateTS.Length == 2)
				strCreateTS = "0000" + strCreateTS;
			if (strCreateTS.Length == 3)
				strCreateTS = "000" + strCreateTS;
			if (strCreateTS.Length == 4)
				strCreateTS = "00" + strCreateTS;
			if (strCreateTS.Length == 5)
				strCreateTS = "0" + strCreateTS;


			List<string> parts = new List<string>(strCreateTS.Length / 2);
			for (int i = 0; i < strCreateTS.Length; i += 2)
			{
				parts.Add(strCreateTS.Substring(i, 2));

			}

			String strFinalTime = String.Join(":", parts);

			strPostingDate = DateTime.ParseExact(strPosting, "yyyyMMdd", null).ToString("yyyy-MM-dd");

			strFinalDateTime = strPostingDate + "T" + strFinalTime + "Z";

			strXMLFileName = strVATNo + "_" + strPostingDate.Replace("-", String.Empty) + "T" + strFinalTime.Replace(":", String.Empty) + "_" + strInvDocNum;




			String strTable3 = "INV3";


			strQry = String.Format(@"

select 
T0.""RegNum"" ""RegNum""
,T0.""LicTradNum"" ""LicTradNum""
,T0.""VatIdUnCmp"" ""VatIdUnCmp""
,T0.""AddID"" ""National ID""
,T0.""Phone1"" ""Tel 1""
,T0.""E_Mail"" ""E_Mail""
,T0.""U_SSTexp"" ""U_SSTexp""
,T0.""U_Category"" ""U_Category""



from ""OCRD"" T0

where T0.""CardCode"" = '{0}'


"

		  , strCardCode);


			//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Starting OCRD Query " + strQry + DateTime.Now);
			oRecordSet.DoQuery(strQry);
			String strCustomer_CRNC = Convert.ToString(oRecordSet.Fields.Item("VatIdUnCmp").Value);
			String strCustomer_VATNoS3E3L15M = Convert.ToString(oRecordSet.Fields.Item("LicTradNum").Value);
			String strCustomer_NationalID = Convert.ToString(oRecordSet.Fields.Item("National ID").Value);
			String strCustomer_Email = Convert.ToString(oRecordSet.Fields.Item("E_Mail").Value);

			//String strCustomer_TIN = Convert.ToString(oRecordSet.Fields.Item("CustomerTIN").Value);
			String strCustomer_TEL1 = Convert.ToString(oRecordSet.Fields.Item("Tel 1").Value);
			String strCustomer_category = Convert.ToString(oRecordSet.Fields.Item("U_Category").Value);
			String SST = Convert.ToString(oRecordSet.Fields.Item("U_SSTexp").Value);





			#endregion
			strQry = String.Format(@"
select ""Rate"" ""Rate"" from ""ORTT"" 
", strCardCode);

			SAPbobsCOM.Recordset oRecordSet1;
			oRecordSet1 = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
			var pyment = String.Format($@"
SELECT o.""PymntGroup"" ""PymntGroup"" FROM ""{strTable}"" inv LEFT JOIN ""OCTG"" o on inv.""GroupNum"" = o.""GroupNum""",
strCardCode);
			if (strFormType == "133")
			{
				objRoot.EInvoiceTypeCode = "01";
			}
			/*else
            {
				objRoot.EInvoiceTypeCode = "02";
				objRoot.OriginalInvoiceNumber = Convert.ToString(db.GetValue("U_OriginalInvoiceNumber",0));
				objRoot.OriginalInvoiceIRBMUniqueNo = Convert.ToString(db.GetValue("U_OriginalInvoiceIRBMUniqueNo", 0));
			}*/

			Log.Information("Retrieving InvoiceData");
			#region Set Invoice
			oRecordSet.DoQuery(strQry);
			oRecordSet1.DoQuery(pyment);
			objRoot.EInvoiceVersion = "1.0";
			objRoot.EInvoiceNumber = strInvDocNum;
			objRoot.EInvoiceDate = strPostingDate;
			objRoot.EInvoiceTime = strFinalTime;
			objRoot.InvoiceCurrencyCode = Convert.ToString(db.GetValue("DocCur", 0));
			objRoot.CurrencyExchangeRate = Convert.ToDecimal(oRecordSet.Fields.Item("Rate").Value).ToString("F2");
			objRoot.PaymentMode = Convert.ToString(db.GetValue("U_PaymentMode", 0));
			objRoot.PaymentTerms = Convert.ToString(oRecordSet1.Fields.Item("PymntGroup").Value);
			objRoot.PaymentDueDate = strPostingDate;
			//objRoot.ShippingRecipientsName = Convert.ToString(db.GetValue("ShipToCode", 0));
			objRoot.BillReferenceNumber = "";
			Log.Information("Retrieving SupplierDetails");
			var supdetails = String.Format(@"Select ""U_SupplierName"",""U_Supplier_TIN"",""U_Supplier_Category""
,""U_Supplier_BRN"",""U_Supplier_Email"",""U_Supplier_MSIC"",""U_Supplier_BusinessActivityDescription"",
""U_Supplier_ContactNumber"",""U_Supplier_AddressLine0"",""U_Supplier_Addressline1"",""U_Supplier_Addressline2""
,""U_Supplier_Postalzone"",""U_Supplier_CityName"",""U_Supplier_State"",""U_Supplier_Country"" from [@SupplierData]");

			oRecordSet1.DoQuery(supdetails);
			//Seller Party
			objRoot.SellerBankAccountNumber = "";
			objRoot.SellerName = Convert.ToString(oRecordSet1.Fields.Item("U_SupplierName").Value);
			//objRoot.SellerTIN = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_TIN").Value); ; //strVATNo;
			objRoot.SellerTIN = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_TIN").Value);
			objRoot.SellerCategory = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_Category").Value);
			objRoot.SellerBusinessRegistrationNumber = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_BRN").Value); ;
			//objRoot.SellerBusinessRegistrationNumber = "202201040795";
			objRoot.SellerSSTRegistrationNumber = "";

			objRoot.SellerContactNumber = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_ContactNumber").Value); ;
			objRoot.SellerAddressLine0 = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_AddressLine0").Value); ;
			objRoot.SellerAddressLine1 = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_Addressline1").Value); ;
			objRoot.SellerAddressLine2 = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_Addressline2").Value); ;
			objRoot.SellerPostalZone = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_Postalzone").Value); ;
			objRoot.SellerCityName = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_CityName").Value); ;
			objRoot.SellerState = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_State").Value); ;
			objRoot.SellerCountry = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_Country").Value); ;
			objRoot.MSICBusinessActivity = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_BusinessActivityDescription").Value); ;
			objRoot.MSICCode = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_MSIC").Value);
			objRoot.SellerEmail = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_Email").Value);

			// CustomerParty
			objRoot.BuyersName = strCardName.Trim();
			//objRoot.BuyersTIN = "C26072927020";//strCustomer_VATNoS3E3L15M.Trim();
			objRoot.BuyersTIN = strCustomer_VATNoS3E3L15M.Trim();
			objRoot.BuyersCategory = strCustomer_category;
			//objRoot.BuyersCategory = "BRN";
			//objRoot.BuyersBRN = "201901029037";	//strCustomer_CRNC.Trim();
			objRoot.BuyersBRN = strCustomer_CRNC.Trim();
			objRoot.BuyersSST = "";
			//objRoot.BuyersEmail = "mustakim.codexlancers@gmail.com";// strCustomer_Email;
			objRoot.BuyersEmail = strCustomer_Email;
			objRoot.BuyersContactNumber = strCustomer_TEL1.Trim();
			//objRoot.BuyersContactNumber = "+63012345678";// strCustomer_TEL1.Trim();
			objRoot.BuyersAddressLine0 = strCustomer_Add_StreetNameM;
			objRoot.BuyersAddressLine1 = strCustomer_Add_Address2; //strCustomer_Add_Address2;
			objRoot.BuyersAddressLine2 = "";//strCustomer_Add_Address2;
			objRoot.BuyersPostalZone = intCustomer_Add_PostalZone5DigM;
			objRoot.BuyersCityName = strCustomer_Add_CityNameM.Trim();
			objRoot.BuyersState = strCustomer_Add_CountrySubentityM.Trim();
			objRoot.BuyersCountry = strCustomer_Add_CountryM.Trim();
			//objRoot.Incoterms = Convert.ToString(db.GetValue("U_Incm", 0));
			objRoot.BuyersIdentification = "";
			objRoot.BuyersSST = "";

			objRoot.ShippingRecipientName = Convert.ToString(db.GetValue("ShipToCode", 0));
			//objRoot.ShippingRecipientTIN = strCustomer_VATNoS3E3L15M.Trim();
			//objRoot.ShippingRecipientCategory = strCustomer_category;
			//objRoot.ShippingRecipientBusinessRegistrationNumber = strCustomer_CRNC.Trim();
			objRoot.ShippingAddressLine0 = strShipping_Add_StreetNameM;
			objRoot.ShippingAddressLine1 = strShipping_Add_Address2;
			objRoot.ShippingPostalZone = Shipping_Add_PostalZone5DigM;
			objRoot.ShippingCityName = strShipping_Add_CityNameM.Trim();
			objRoot.ShippingState = strShipping_Add_CountrySubentityM.Trim();
			objRoot.ShippingCountry = strShipping_Add_CountryM.Trim();
			//objRoot.BuyersCountry = "MYS";

			try
			{
				if (!string.IsNullOrEmpty(objRoot.BuyersCountry) && objRoot.BuyersCountry.Length == 2)
				{
					RegionInfo buyer = new RegionInfo(objRoot.BuyersCountry.ToUpper());
					string buyercountry = buyer.ThreeLetterISORegionName;
					objRoot.BuyersCountry = buyercountry;
				}
			}
			catch (Exception ex)
			{
				Log.Error($"Invalid country code: {ex.Message}");
			}
			try
			{
				if (!string.IsNullOrEmpty(objRoot.ShippingCountry) && objRoot.ShippingCountry.Length == 2)
				{
					RegionInfo shipping = new RegionInfo(objRoot.ShippingCountry.ToUpper());
					string shippingcountry = shipping.ThreeLetterISORegionName;
					objRoot.ShippingCountry = shippingcountry;
				}
			}
			catch (Exception ex)
			{
				Log.Error($"Invalid country code: {ex.Message}");
			}
			//Invoice Totals

			//objRoot.SumOfInvoiceLineNetAmount = "3740.00000";
			//objRoot.SumOfAllowances = "0.00";
			//objRoot.TotalFeeOrChargeAmount = "0.00";
			//objRoot.TotalExcludingTax = "3740.00000";
			//objRoot.TotalIncludingTax = "3740.00000";
			//objRoot.RoundingAmount = "0.00";
			//objRoot.PaidAmount = "3740.00000";
			//objRoot.TotalPayableAmount = "3740";
			//objRoot.TotalNetAmount = "3740.00000";




			objRoot.SumOfInvoiceLineNetAmount = Convert.ToString(Convert.ToDecimal(strInvTotal) + Convert.ToDecimal(strDiscountSum) - dcmlTotalCharge);
			objRoot.SumOfAllowances = "0.00";
			objRoot.TotalFeeOrChargeAmount = "0.00";
			objRoot.TotalExcludingTax = Convert.ToString(Convert.ToDecimal(strInvTotal) - Convert.ToDecimal(strVATSum) + Convert.ToDecimal(strDiscountSum) - dcmlTotalCharge);
			objRoot.TotalIncludingTax = Convert.ToString(Convert.ToDecimal(strInvTotal) + Convert.ToDecimal(strDiscountSum) - dcmlTotalCharge);
			objRoot.RoundingAmount = "0.00";
			objRoot.PaidAmount = "0.00000";
			objRoot.TotalPayableAmount = objRoot.TotalIncludingTax;
			objRoot.TotalNetAmount = objRoot.TotalIncludingTax;

			Log.Information("Retrieving DocTaxSubTotal");
			objRoot.DocTaxTotal = new DocTaxTotal();
			objRoot.DocTaxTotal.TAXCategoryTaxAmountInAccountingCurrency = "0.00";
			objRoot.DocTaxTotal.TotalTaxableAmountPerTaxType = "0";
			objRoot.DocTaxTotal.TaxCategoryId = "06";//Convert.ToString(db.GetValue("U_TaxType", 0));
			objRoot.DocTaxTotal.TaxCategorySchemeID = "UN/ECE 5153";
			objRoot.DocTaxTotal.TaxCategorySchemeAgencyID = "6";
			objRoot.DocTaxTotal.TaxCategorySchemeAgencyCode = "OTH";
			objRoot.DocTaxTotal.TAXCategoryRate = "0.00";// Convert.ToString(db.GetValue("U_TaxRate", 0));
			objRoot.DocTaxTotal.DetailsOfTaxExemption = taxexemptionreason;




			// AllowanceCharges
			objRoot.AllowanceCharges = new List<object>(); // Empty list

			// Consolidated Buyer Info
			/*objRoot.ConsolidatedBuyerTIN = "EI00000000010";
			objRoot.ConsolidatedBuyerCategory = "BRN";
			objRoot.ConsolidatedBuyerIdentificationNumberOrPassportNumber = "N/A";*/




			//Line Items

			strQry = String.Format(@"

select 
T0.""VisOrder""+1 ""LineNum""
, T0.""UomCode"" ""UoM""
,T0.""Quantity""
,T0.""Currency"" 
,T0.""unitMsr"" 
,T0.""Quantity""*T0.""Price"" ""Line Total""
,'MYS' ""Tax Amount Currency""
,round(case when T0.""Rate"" = 0 then T0.""LineVat""
else round(T0.""LineVat""/T0.""Rate"",2) end,2)  ""LineVatOld""

,case when (T0.""VatSumFrgn"") > 0 then (T0.""VatSumFrgn"")
else (T0.""VatSum"") end ""LineVat""


,round((T0.""Quantity""*T0.""Price"")+(T0.""Quantity""*T0.""Price""*(T0.""VatPrcnt""/100)),2)  ""Line Total with VAT Old""

,round((T0.""Quantity""*T0.""Price"")+case when (T0.""VatSumFrgn"") > 0 then (T0.""VatSumFrgn"")
else (T0.""VatSum"") end ,2)  ""Line Total with VAT""


,T0.""Dscription""

,T0.""VatPrcnt""
,T0.""VatGroup""
,T0.""VatSumFrgn""
,T0.""Currency""  ""Price Amount Currency""
,T0.""U_ClassificationCode""
,T0.""U_ClassificationClass""
,T0.""U_ProductTariffCode""
,T0.""U_ProductTariffClass""
,T0.""U_Vxml_UoM"" 
,T0.""CountryOrg""



,round((T0.""Quantity""*T0.""Price""),2) ""LineTotal""
,T0.""Price"" ""Price""
,T0.""UomCode"" ""Base UoM""
,T0.""Quantity"" ""Base Qty""

,T0.""PriceBefDi""-T0.""Price"" ""Line Discount""
,T0.""BaseRef""
--,case when T0.""BaseType"" = '13' then T0.""BaseRef""
--else T0.""U_EIN_BASE"" end ""BaseRef""

from ""{1}"" T0
left outer join ""OVTG"" T1 on T0.""VatGroup"" = T1.""Code""
left outer join ""OUOM"" T2 on T0.""UomCode"" = T2.""UomCode""
where T0.""DocEntry"" = '{0}'

"

, strInvNo, strTable1);
			Log.Information("Retrieving InvoiceLineItems");
			//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Starting Line Query " + strQry + DateTime.Now);

			oRecordSet.DoQuery(strQry);

			String strBaseRef = "";
			Int32 inLineCounter = 1;
			objRoot.LineItem = new List<LineItem>();
			while (oRecordSet.EoF == false)
			{
				var line1 = new LineItem();
				line1.LineId = inLineCounter;
				line1.ClassificationClass = Convert.ToString(oRecordSet.Fields.Item("U_ClassificationClass").Value);//CLASS";
				line1.ClassificationCode = Convert.ToString(oRecordSet.Fields.Item("U_ClassificationCode").Value); //"022";
				//line1.ProductID = "NA";																						   //line1.ProductID = "Resell (INT) - Internet Access";;
				line1.DescriptionOfProductOrService = Convert.ToString(oRecordSet.Fields.Item("Dscription").Value); ;
				line1.ProductTariffCode = Convert.ToString(oRecordSet.Fields.Item("U_ProductTariffCode").Value); //"4001.10.00";
				line1.ProductTariffClass =  Convert.ToString(oRecordSet.Fields.Item("U_ProductTariffClass").Value); //"PTC";
				line1.Country = Convert.ToString(oRecordSet.Fields.Item("CountryOrg").Value);
				line1.UnitPrice = Convert.ToDecimal(oRecordSet.Fields.Item("Price").Value).ToString("0.000000");
				line1.Quantity = Convert.ToDecimal(oRecordSet.Fields.Item("Quantity").Value).ToString("0.000000");
				//line1.Measurement = "C62";
				//line1.Measurement = Convert.ToString(oRecordSet.Fields.Item("unitMsr").Value); ;
				line1.Measurement = Convert.ToString(oRecordSet.Fields.Item("U_Vxml_UoM").Value); ;
				line1.Subtotal = Convert.ToDecimal(oRecordSet.Fields.Item("Line Total").Value).ToString("0.000000");
				//line1.SSTTaxCategory = strVATNo;
				line1.Country = Convert.ToString(oRecordSet.Fields.Item("CountryOrg").Value);
				//line1.TaxType = Convert.ToString(oRecordSet.Fields.Item("VatGroup").Value);//"02"
				line1.TaxType = Convert.ToString(db.GetValue("U_TaxType", 0));//"02"
																			  //line1.TaxType = "02";
				line1.TaxRate = Convert.ToString(oRecordSet.Fields.Item("VatPrcnt").Value);
				// Assign result as a formatted string
				line1.TaxAmount = Convert.ToString(oRecordSet.Fields.Item("VatSumFrgn").Value);
				line1.DetailsOfTaxExemption = taxexemptionreason;
				line1.AmountExemptedFromTax = null;
				line1.TaxCategoryId = Convert.ToString(db.GetValue("U_TaxType", 0));
				line1.TotalExcludingTax = Convert.ToDecimal(oRecordSet.Fields.Item("Line Total").Value).ToString("0.000000");
				line1.InvoiceLineNetAmount = Convert.ToDecimal(oRecordSet.Fields.Item("Line Total with VAT").Value).ToString("0.000000");
				line1.NettAmount = Convert.ToDecimal(oRecordSet.Fields.Item("Line Total with VAT").Value).ToString("0.000000");
				line1.TaxCategorySchemeID = "UN/ECE 5153";
				line1.TaxCategorySchemeAgencyID = "6";
				line1.TaxCategorySchemeAgencyCode = "OTH";
				/*line1.ConsolidatedClassificationCode = "004";
				line1.ConsolidatedDescription = "IN026075";*/
				try
				{
					if (!string.IsNullOrEmpty(line1.Country) && line1.Country.Length == 2)
					{
						RegionInfo lineitemcountry = new RegionInfo(line1.Country);
						line1.Country = lineitemcountry.ThreeLetterISORegionName;
					}
				}
				catch (Exception ex)
				{
					Log.Error("Exception in LineitemCountry", ex.Message);
				}
				objRoot.LineItem.Add(line1);
				oRecordSet.MoveNext();
				inLineCounter++;
			}


			// DocTaxTotal

			String strJSON = JsonConvert.SerializeObject(objRoot, Formatting.Indented);

			//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Starting JSON " + strJSON + DateTime.Now);

			//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Before API Call " + strJSON + DateTime.Now);

			Invoice invoice = new Invoice();
			(ApiResponse objResults, String strRawResponse) = _ILhdnApiService.fnSubmitDocument(strJSON, strToken);

			var outerJson = JsonDocument.Parse(strRawResponse);
			string allMessages = "";

			var dataRaw = outerJson.RootElement.GetProperty("data").GetString();
			if (dataRaw != null)
			{
				var innerJson = JsonDocument.Parse(dataRaw);
				bool hasRejected = innerJson.RootElement.TryGetProperty("rejectedDocuments", out JsonElement rejectedDocs)
				   && rejectedDocs.ValueKind == JsonValueKind.Array
				   && rejectedDocs.GetArrayLength() > 0;

				bool hasAccepted = innerJson.RootElement.TryGetProperty("acceptedDocuments", out JsonElement acceptedDocs)
								   && acceptedDocs.ValueKind == JsonValueKind.Array
								   && acceptedDocs.GetArrayLength() > 0;



				if (!hasRejected && !hasAccepted)
				{
					// Case when both "rejectedDocuments" and "acceptedDocuments" are missing or empty
					JsonElement root = innerJson.RootElement;

					string message = root
						.GetProperty("error")
						.GetProperty("details")[0]
						.GetProperty("message")
						.GetString();

					allMessages += message + "\n";
					Log.Information($"Response from SubmitApi : {allMessages}");
				}
				else if (hasRejected)
				{
					// Case when "rejectedDocuments" exists and has items
					foreach (var doc in rejectedDocs.EnumerateArray())
					{
						if (doc.TryGetProperty("error", out JsonElement errorElement) &&
							errorElement.TryGetProperty("details", out JsonElement detailsElement) &&
							detailsElement.ValueKind == JsonValueKind.Array)
						{
							foreach (var error in detailsElement.EnumerateArray())
							{
								if (error.TryGetProperty("message", out JsonElement messageElement))
								{
									string message = messageElement.GetString();
									allMessages += message + "\n";
									Log.Information($"Response from SubmitApi : {allMessages}");
								}
							}
						}
					}
				}
			}
			//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Response from API Result " + objResults.StatusCode + DateTime.Now);

			//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Response from API Call " + strRawResponse + DateTime.Now);

			//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Response from API Result " + objResults.StatusCode + DateTime.Now);


			#endregion


			#region Update

			String strValid = "";
			String strQRCode = "";
			String strObjType = string.Empty;

			SAPbobsCOM.Documents oInvoice;

			if (strTable == "OINV")
			{
				strObjType = "13";
				oInvoice = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
			}

			else
			{
				strObjType = "14";
				oInvoice = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes);
			}


			//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Before Geting B1 Object " + strTable + DateTime.Now);
			if (oInvoice.GetByKey(Convert.ToInt32(strInvNo)))
			{
				String strError = "";
				//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Loaded B1 Object " + strTable + " DocEntry " + strInvNo + DateTime.Now);

				Log.Information($"Loaded B1 Object");
				if (objResults.Message == "The request has succeeded.")
				{


					String strSubmissionUUI = "";
					String strUUID = "";
					String strInv = "";
					if (objResults.ParsedData.SubmissionUid != null)
					{
						strSubmissionUUI = objResults.ParsedData.SubmissionUid;
					}
					foreach (var acceptance in objResults.ParsedData.AcceptedDocuments)
					{
						strUUID = acceptance.Uuid;
						strInv = acceptance.InvoiceCodeNumber;
					}
					var parsedData = JObject.Parse(objResults.Data);
					JArray acceptedDocuments = parsedData["acceptedDocuments"] as JArray;
					if (acceptedDocuments.Count != 0)
					{

						foreach (var doc in acceptedDocuments)
						{
							string uuid = (string)doc["uuid"];
							if (!string.IsNullOrEmpty(uuid))
							{

								//oInvoice.UserFields.Fields.Item("U_QRPath").Value = qrpath;
								oInvoice.UserFields.Fields.Item("U_ADVIN_Status").Value = objResults.Message;
								oInvoice.UserFields.Fields.Item("U_ADVIN_SubmId").Value = strSubmissionUUI;
								oInvoice.UserFields.Fields.Item("U_ADVIN_UUID").Value = strUUID;
								oInvoice.UserFields.Fields.Item("U_ADVIN_InvNo").Value = strInv;
								oInvoice.UserFields.Fields.Item("U_ADVIN_ErrMsg").Value = "";
								oInvoice.UserFields.Fields.Item("U_ADVIN_QrUrl").Value = "";
								oInvoice.UserFields.Fields.Item("U_LongId").Value = "";
								oInvoice.UserFields.Fields.Item("U_ValidatedDateTime").Value = "";
							}

						}
					}
					else
					{
						oInvoice.UserFields.Fields.Item("U_ADVIN_Status").Value = "IRBResponseFailed";
						oInvoice.UserFields.Fields.Item("U_ADVIN_ErrMsg").Value = allMessages;
						oInvoice.UserFields.Fields.Item("U_ADVIN_QrUrl").Value = "";
						oInvoice.UserFields.Fields.Item("U_LongId").Value = "";
						oInvoice.UserFields.Fields.Item("U_ValidatedDateTime").Value = "";

					}

					// Option 1: Store base64 string
					//oInvoice.UserFields.Fields.Item("U_QRImage").Value = base64String;







				}
				else
				{
					//foreach (var err in objResults.Errors)
					//{
					//	strError = err.Errors[0].Error;
					//}


					oInvoice.UserFields.Fields.Item("U_ADVIN_Status").Value = objResults.Message;


					oInvoice.UserFields.Fields.Item("U_ADVIN_ErrMsg").Value = allMessages;
					oInvoice.UserFields.Fields.Item("U_ADVIN_QrUrl").Value = "";
					oInvoice.UserFields.Fields.Item("U_LongId").Value = "";
					oInvoice.UserFields.Fields.Item("U_ValidatedDateTime").Value = "";
					oInvoice.UserFields.Fields.Item("U_ADVIN_SubmId").Value = "";
					oInvoice.UserFields.Fields.Item("U_ADVIN_UUID").Value = "";


				}

				oInvoice.UserFields.Fields.Item("U_ADVIN_Req").Value = strJSON;

				if (oInvoice.Update() != 0)
				{


					//File.AppendAllText(strXMLFilePath + "_log.txt", Environment.NewLine + "Error Updating B1 Object with ZATCA Statuses.");
					Application.SBO_Application.SetStatusBarMessage(oCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Medium, true);
				}
				//File.AppendAllText(".\\Log.txt", Environment.NewLine + "B1 Object Updated" + DateTime.Now
				Log.Information("B1 Object Updated");
				Log.Information("---------------------------------");
			}




			SAPbobsCOM.CompanyService oCompanyService;
			//SAPbobsCOM.QRCodeService oQRCodeService;
			//SAPbobsCOM.QRCodeData oQRCodeData;

			oCompanyService = oCompany.GetCompanyService();
			//oQRCodeService = (SAPbobsCOM.QRCodeService)oCompanyService.GetBusinessService(SAPbobsCOM.ServiceTypes.QRCodeService);
			//oQRCodeData = (SAPbobsCOM.QRCodeData)oQRCodeService.GetDataInterface(SAPbobsCOM.QRCodeServiceDataInterfaces.qrcsQRCodeData);





			bolValid = true;

			System.Runtime.InteropServices.Marshal.ReleaseComObject(db);
			GC.WaitForPendingFinalizers();
			System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet);
			oRecordSet = null;

			System.Runtime.InteropServices.Marshal.ReleaseComObject(oCompanyService);
			//System.Runtime.InteropServices.Marshal.ReleaseComObject(oQRCodeService);
			//System.Runtime.InteropServices.Marshal.ReleaseComObject(oQRCodeData);
			System.Runtime.InteropServices.Marshal.ReleaseComObject(oInvoice);


			GC.Collect();


			try
			{
				oForm.Refresh();
			}
			catch
			{

			}


			#endregion





			return bolValid;

		}

		public static async Task<bool> fnCheckStatusUpdate(String strTable, SAPbobsCOM.Company oCompany = null)
		{
			Log.Information("CheckStatusUpdate Service Called");
			_ILhdnApiService = new LHDNAPIService();
			Boolean bolValid = false;
			SAPbouiCOM.Form oForm;
			String strFormType = String.Empty;
			string invtypcode = string.Empty;
			if (strTable == "OINV")
			{
				strFormType = "133";
				invtypcode = "Invoice";
			}
			/* else
			 {
				 strFormType = "179";
				 invtypcode = "CreditNote";
			 }*/

			Int32 intCount = Application.SBO_Application.Forms.ActiveForm.TypeCount;

			oForm = Application.SBO_Application.Forms.GetForm(strFormType, intCount);


			SAPbouiCOM.DBDataSource db = oForm.DataSources.DBDataSources.Item(strTable);
			String strInvNo = Convert.ToString(db.GetValue("DocEntry", 0));
			String strInvDocNum = Convert.ToString(db.GetValue("DocNum", 0));
			string curcode = Convert.ToString(db.GetValue("DocCur", 0));
			String strUUID = Convert.ToString(db.GetValue("U_ADVIN_UUID", 0));
			if (strUUID.Length == 0)
			{
				SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("It seems Document is not submitted yet Please first submitt the document.", SAPbouiCOM.BoMessageTime.bmt_Medium, true);
				return bolValid = false;
			}
			Invoice obj = new Invoice();
			(LoginResponse objResults, String rawResponse) = _ILhdnApiService.fnLogin();
			if (objResults.IsSuccess)
			{
				String strToken = "";
				if (objResults.Data.Token != null)
				{
					strToken = objResults.Data.Token;

					(DocumentStatusResponse objResult, String strRawResponse) = _ILhdnApiService.fnCheckStatus(strUUID.Trim(), strToken);

					dynamic data = JsonConvert.DeserializeObject(strRawResponse);

					string status = string.Empty;
					string allErrors = string.Empty;

					if (data != null && data.isSuccess == true)
					{
						status = (string)data["data"]?["status"];

						string datetimevaidated = (string)data["data"]?["dateTimeValidated"];
						List<string> errMsg = new List<string>();

						if (status == "Invalid")
						{
							status = "IRBResponseFailed";
							Log.Information($"Invoice Status : {status}");

							// Step 1: Safely parse validationResults string into JObject
							string validationResultsJson = (string)data["data"]?["validationResults"];
							if (!string.IsNullOrWhiteSpace(validationResultsJson))
							{
								JObject validationResults;
								try
								{
									validationResults = JObject.Parse(validationResultsJson);
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error parsing validationResults: " + ex.Message);
									return false;
								}

								// Step 2: Safely access ValidationSteps array
								JToken stepsToken = validationResults["ValidationSteps"];
								if (stepsToken is JArray stepsArray)
								{
									foreach (JToken step in stepsArray)
									{
										var errorToken = step["error"];

										// Step 3: Check if errorToken is an object and contains "innerError" array
										if (errorToken != null && errorToken.Type == JTokenType.Object)
										{
											var innerErrorToken = errorToken["innerError"];
											if (innerErrorToken is JArray innerErrors)
											{
												foreach (var inner in innerErrors)
												{
													string singleError = (string)inner["error"];
													if (!string.IsNullOrEmpty(singleError))
													{
														errMsg.Add(singleError);
														allErrors = string.Join(Environment.NewLine, errMsg);
														Log.Information($"Rejected Reason : {allErrors}");
													}
												}
											}
										}
									}
								}
								else
								{
									Console.WriteLine("ValidationSteps not found or not an array.");
								}
							}
							else
							{
								Console.WriteLine("validationResults is null or empty.");
							}
						}
						else
						{
							status = "IRBResponseSuccess";
							string LongId = (string)data["data"]?["longId"];
							Log.Information($"Invoice Status : {status}");
						}

					}
					else
					{
						status = data.message.ToString();
					}

					#region Update
					PDFGenerator pDFGenerator = new PDFGenerator();
					String strValid = "";
					String strQRCode = "";
					String strObjType = "";

					SAPbobsCOM.Documents oInvoice;
					if (!oCompany.Connected)
					{
						oCompany.Connect(); // ya reconnection logic
					}
					if (strTable == "OINV")
					{
						strObjType = "13";
						oInvoice = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
					}


					else
					{
						strObjType = "14";
						oInvoice = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes);
					}

					//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Before Geting B1 Object " + strTable + DateTime.Now);
					if (oInvoice.GetByKey(Convert.ToInt32(strInvNo)))
					{
						String strError = "";
						//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Loaded B1 Object " + strTable + " DocEntry " + strInvNo + DateTime.Now);
						Log.Information("Loaded B1 Objects");
						String strStatus = Convert.ToString(oInvoice.UserFields.Fields.Item("U_ADVIN_Status").Value);
						//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Document Current Status is :" + strStatus + DateTime.Now);

						oInvoice.UserFields.Fields.Item("U_ADVIN_Status").Value = status;
						if (status == "IRBResponseFailed")
						{
							oInvoice.UserFields.Fields.Item("U_ADVIN_ErrMsg").Value = allErrors;
							oInvoice.UserFields.Fields.Item("U_LongId").Value = "";

						}
						else if (status == "IRBResponseSuccess")
						{
							string path = string.Empty;
							string qrUrl = objResult.Data.ValidationLink;
							var json = JsonConvert.SerializeObject(AppConfigManager.Config, Formatting.Indented);
							var deserialized = JsonConvert.DeserializeObject<Appsettings>(json);
							//QrGenerator qrGenerator = new QrGenerator();

							/*if (strTable == "OINV")
							{
								var inv = Path.Combine(deserialized.QrPath, "Invoice");
								path = Path.Combine(inv, strInvNo);
							}
							else 
							{
								var cn = Path.Combine(deserialized.QrPath, "CreditNote");
								path = Path.Combine(cn, strInvNo);
							}*/

							/*qrGenerator.GenerateQRCodeAsBytes(qrUrl, path, strInvNo);
							var qrimage = $"{strInvNo}.png";
							var qrpath = Path.Combine(path, qrimage);*/
						}
						else
						{
							oInvoice.UserFields.Fields.Item("U_ADVIN_QrUrl").Value = "";
							oInvoice.UserFields.Fields.Item("U_LongId").Value = "";
							oInvoice.UserFields.Fields.Item("U_ValidatedDateTime").Value = "";
							oInvoice.UserFields.Fields.Item("U_ADVIN_ErrMsg").Value = "";
						}
						if (objResult.Data != null)
						{
							if (objResult.Data.ValidationLink != null && objResult.Data.ValidationLink != "")
							{
								oInvoice.UserFields.Fields.Item("U_ADVIN_QrUrl").Value = objResult.Data.ValidationLink;
								oInvoice.UserFields.Fields.Item("U_LongId").Value = objResult.Data.LongId;
								oInvoice.UserFields.Fields.Item("U_ValidatedDateTime").Value = objResult.Data.DateTimeValidated.ToString();
								oInvoice.UserFields.Fields.Item("U_ADVIN_ErrMsg").Value = "";
							}
						}
					}

					if (oInvoice.Update() != 0)
					{


						//File.AppendAllText(strXMLFilePath + "_log.txt", Environment.NewLine + "Error Updating B1 Object with ZATCA Statuses.");
						Application.SBO_Application.SetStatusBarMessage(oCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Medium, true);
					}
					else if (!string.IsNullOrEmpty(objResult.Data.LongId))
					{
						String strCardCode = Convert.ToString(db.GetValue("CardCode", 0));
						SAPbobsCOM.Recordset oRecordSet;
						strCardCode = strCardCode.Trim();
						oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
						var strQry = String.Format(@"
							select
							T0.""E_Mail"" ""E_Mail""
							from ""OCRD"" T0
							where T0.""CardCode"" = '{0}'", strCardCode);
						oRecordSet.DoQuery(strQry);
						String strCustomer_Email = Convert.ToString(oRecordSet.Fields.Item("E_Mail").Value);
						var pdf = await pDFGenerator.GeneratePDFWithMSSQL(Convert.ToInt32(strInvNo), strInvDocNum, invtypcode, curcode);
						byte[] pdfBytes = File.ReadAllBytes(pdf);

						// Convert to Base64
						string base64Pdf = Convert.ToBase64String(pdfBytes);
						(LoginResponse objResults1, String rawResponse1) = _ILhdnApiService.fnLogin();
						if (objResults.IsSuccess)
						{
							String strToken1 = "";
							if (objResults.Data.Token != null)
							{
								strToken = objResults.Data.Token;
								var email = new EmailRequest();
								email.Base64Pdf = base64Pdf;
								email.EmailAddress = strCustomer_Email;
								email.InvoiceNo = strInvDocNum;
								var emailjson = JsonConvert.SerializeObject(email);
								Log.Information($"EmailData :- {emailjson}");
								(DocumentStatusResponse Result, String Response) = _ILhdnApiService.fnSendEmail(emailjson, strToken);
                                if (Result.IsSuccess)
                                {
									Log.Information("Email Sent Successfully");
                                }
                                else
                                {
									Log.Information("There was an error sending the mail");
								}
							}
						}
					}
					//File.AppendAllText(".\\Log.txt", Environment.NewLine + "B1 Object Updated" + DateTime.Now);
					Log.Information("B1 Object Updated");
					Log.Information("---------------------------------");





					SAPbobsCOM.CompanyService oCompanyService;
					oCompanyService = oCompany.GetCompanyService();


					bolValid = true;

					System.Runtime.InteropServices.Marshal.ReleaseComObject(db);
					GC.WaitForPendingFinalizers();

					System.Runtime.InteropServices.Marshal.ReleaseComObject(oCompanyService);
					//System.Runtime.InteropServices.Marshal.ReleaseComObject(oQRCodeService);
					//System.Runtime.InteropServices.Marshal.ReleaseComObject(oQRCodeData);
					System.Runtime.InteropServices.Marshal.ReleaseComObject(oInvoice);


					GC.Collect();


					try
					{
						oForm.Refresh();
					}
					catch
					{

					}


					#endregion

				}
				else
				{
					SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Client Token Not Found", SAPbouiCOM.BoMessageTime.bmt_Medium, true);
				}

			}
			else
			{
				SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Failed", SAPbouiCOM.BoMessageTime.bmt_Medium, true);
			}



			return bolValid;
		}
		public static void fnCancelDocument(string strTable, string reason, SAPbobsCOM.Company oCompany = null)
		{
			Log.Information("CancelDocument Service Called");
			_ILhdnApiService = new LHDNAPIService();
			SAPbouiCOM.Form oForm;
			String strFormType = String.Empty;
			if (strTable == "OINV")
			{
				strFormType = "133";
			}
			String strObjType = "";
			SAPbobsCOM.Documents oInvoice;
			if (strTable == "OINV")
			{
				strObjType = "13";
				oInvoice = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
			}
			else
			{
				strObjType = "14";
				oInvoice = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes);
			}

			//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Before Geting B1 Object " + strTable + DateTime.Now);
			Int32 intCount = Application.SBO_Application.Forms.ActiveForm.TypeCount;
			oForm = Application.SBO_Application.Forms.GetForm(strFormType, intCount);
			SAPbouiCOM.DBDataSource db = oForm.DataSources.DBDataSources.Item(strTable);
			String strUUID = Convert.ToString(db.GetValue("U_ADVIN_UUID", 0));
		String strInvNo = Convert.ToString(db.GetValue("DocEntry", 0));
			if (oInvoice.GetByKey(Convert.ToInt32(strInvNo)))
			{

				if (strUUID.Length == 0)
				{
					SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("It seems Document is not submitted yet Please first submitt the document.", SAPbouiCOM.BoMessageTime.bmt_Medium, true);
					return;
				}
				Invoice obj = new Invoice();
				(LoginResponse objResults, String rawResponse) = _ILhdnApiService.fnLogin();
				if (objResults.IsSuccess)
				{
					String strToken = "";
					if (objResults.Data.Token != null)
					{
						strToken = objResults.Data.Token;
						CancelDocument objRoot = new CancelDocument();
						objRoot.uuid = strUUID.Trim();
						objRoot.status = "Cancelled";
						objRoot.reason = reason;

						String strJSON = JsonConvert.SerializeObject(objRoot, Formatting.Indented);
						(DocumentStatusResponse objResult, String strRawResponse) = _ILhdnApiService.fnCancelInvoice(strJSON, strToken);
						dynamic data = JsonConvert.DeserializeObject(strRawResponse);
						if (data != null && data.isSuccess == true)
						{
							string status = (string)data["data"]?["status"];
							oInvoice.UserFields.Fields.Item("U_ADVIN_Status").Value = status;
						}
                        else
                        {
							string ErrorMessage = string.Empty;
							try
							{
								ErrorMessage = (string)data["data"]?["error"]?["details"]?[0]?["message"];
							}
                            catch
                            {
								ErrorMessage = JsonConvert.SerializeObject(data);

							}
							oInvoice.UserFields.Fields.Item("U_ADVIN_ErrMsg").Value = ErrorMessage;
							
						}
						}
				}
			}
			if (oInvoice.Update() != 0)
			{


				//File.AppendAllText(strXMLFilePath + "_log.txt", Environment.NewLine + "Error Updating B1 Object with ZATCA Statuses.");
				Application.SBO_Application.SetStatusBarMessage(oCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Medium, true);
			}


			SAPbobsCOM.CompanyService oCompanyService;
			oCompanyService = oCompany.GetCompanyService();



			System.Runtime.InteropServices.Marshal.ReleaseComObject(db);
			GC.WaitForPendingFinalizers();

			System.Runtime.InteropServices.Marshal.ReleaseComObject(oCompanyService);
			//System.Runtime.InteropServices.Marshal.ReleaseComObject(oQRCodeService);
			//System.Runtime.InteropServices.Marshal.ReleaseComObject(oQRCodeData);
			System.Runtime.InteropServices.Marshal.ReleaseComObject(oInvoice);


			GC.Collect();


			try
			{
				oForm.Refresh();
			}
			catch
			{

			}
		}
	}
}
