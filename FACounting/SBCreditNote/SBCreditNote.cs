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
using SAPbobsCOM;
using System.Globalization;
using Serilog;

namespace EInvoice
{

    public class SBCreditNote
    {
		private static ILHDNAPIService _ILhdnApiService;
		public static Boolean BaseB2B(String strTable, String strTable1, String strTable12, String strTable9, String strTable11, SAPbobsCOM.Company oCompany = null, String strCaller = null,String strToken = null)
		{

			#region Definations 
			_ILhdnApiService = new LHDNAPIService();
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
			Decimal dcInvroundimgAmt;
			String strInvTotal = "";
			String strrounding = "";
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
			String strInvoiceTypeCode = "12";
			#endregion

			#region Retrieval
			String strFormType =String.Empty;
			SAPbouiCOM.Form oForm;
			if(strTable == "ORPC")
            {
				 strFormType = "181";
			}else
            {
				strFormType = "141";
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

select 
T0.""Street"" 
,T0.""Block""
,T0.""StreetNo""
,T0.""Building""
,T0.""City""
,T0.""ZipCode""
,T2.""Code"" ""State""
,T0.""Country""
,T0.""Address""
,T0.""Address2""
,T0.""Address3""
,T0.""CardCode""
,T3.""U_SSTexp"" ,
T3.""U_TaxExemptionReason""
from ""CRD1"" T0
left outer join ""{strTable}"" T1 on T0.""CardCode"" = T1.""CardCode""
left outer join ""OCST"" T2 on T0.""State"" = T2.""Code""
LEFT OUTER JOIN ""OCRD"" T3 ON T0.""CardCode"" = T3.""CardCode""  
and T0.""Address"" = T1.""PayToCode""
WHERE T0.""CardCode"" = T1.""CardCode""
and T1.""DocEntry"" = '{strInvNo}'

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
			String strCustomer_cardcode = "";
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
				strCustomer_cardcode = Convert.ToString(oRecordSet.Fields.Item("CardCode").Value);
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




			//objRoot.document.dcmlTaxTotals_TaxTotalM = Convert.ToDecimal(strVATSuSAR);


			dcInvTotal = Convert.ToDecimal(db.GetValue("DocTotal", 0));
			dcInvTotalFC = Convert.ToDecimal(db.GetValue("DocTotalFC", 0));
			dcInvroundimgAmt = Convert.ToDecimal(db.GetValue("RoundDif", 0));
			if (dcInvTotalFC != 0.00M)
				dcInvTotal = dcInvTotalFC;


			strInvTotal = Convert.ToString(Decimal.Round(dcInvTotal, 2, MidpointRounding.AwayFromZero));
			strrounding = Convert.ToString(Decimal.Round(dcInvroundimgAmt, 2, MidpointRounding.AwayFromZero));


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
,T0.""VatIdUnCmp"" ""CustomerTIN""
,T0.""AddID"" ""National ID""
,T0.""Phone1"" ""Tel 1""
,T0.""E_Mail"" ""E_Mail""
,T0.""U_Category"" ""U_Category""
,T0.""U_BusinessActivityDesc"" ""U_BusinessActivityDesc""
,T0.""U_MSICCode"" ""U_MSICCode""
,T0.""U_SSTexp"" ""U_SSTexp""

from ""OCRD"" T0

where T0.""CardCode"" = '{0}'


"

		  , strCardCode);


			//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Starting OCRD Query " + strQry + DateTime.Now);
			oRecordSet.DoQuery(strQry);
			String strCustomer_CRNC = Convert.ToString(oRecordSet.Fields.Item("RegNum").Value);
			String strCustomer_VATNoS3E3L15M = Convert.ToString(oRecordSet.Fields.Item("LicTradNum").Value);
			String strCustomer_NationalID = Convert.ToString(oRecordSet.Fields.Item("National ID").Value);
			String strCustomer_Email = Convert.ToString(oRecordSet.Fields.Item("E_Mail").Value);

			//String strCustomer_TIN = Convert.ToString(oRecordSet.Fields.Item("CustomerTIN").Value);
			String strCustomer_TEL1 = Convert.ToString(oRecordSet.Fields.Item("Tel 1").Value);
			String strCustomer_category = Convert.ToString(oRecordSet.Fields.Item("U_Category").Value);
			String strCustomer_businessactivitydesc = Convert.ToString(oRecordSet.Fields.Item("U_BusinessActivityDesc").Value);
			String strCustomer_msiccode = Convert.ToString(oRecordSet.Fields.Item("U_MSICCode").Value);
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
			if (strFormType == "181")
            {
				objRoot.EInvoiceTypeCode = "12";
				objRoot.OriginalInvoiceNumber = Convert.ToString(db.GetValue("U_OriginalInvoiceNumber", 0));
				objRoot.OriginalInvoiceIRBMUniqueNo = Convert.ToString(db.GetValue("U_OriginalInvoiceIRBMUniqueIdentifierNumber", 0));
			}
			/*else
            {
				objRoot.EInvoiceTypeCode = "12";
				objRoot.OriginalInvoiceNumber = Convert.ToString(db.GetValue("U_OriginalInvoiceNumber", 0));
				objRoot.OriginalInvoiceIRBMUniqueNo = Convert.ToString(db.GetValue("U_OriginalInvoiceIRBMUniqueNo", 0));
			}*/
			Log.Information("Retrieving SBCreditNoteData");
			#region Set Invoice
			oRecordSet.DoQuery(strQry);
			oRecordSet1.DoQuery(pyment);	
			objRoot.EInvoiceVersion = "1.0";
			objRoot.EInvoiceNumber = strInvDocNum;
			objRoot.EInvoiceDate = strPostingDate;
			objRoot.EInvoiceTime = strFinalTime;
			objRoot.InvoiceCurrencyCode = Convert.ToString(db.GetValue("DocCur",0));
			objRoot.CurrencyExchangeRate = Convert.ToDecimal(oRecordSet.Fields.Item("Rate").Value).ToString("F2");
			objRoot.PaymentMode = Convert.ToString(db.GetValue("U_PaymentMode", 0));
			objRoot.PaymentTerms = Convert.ToString(oRecordSet1.Fields.Item("PymntGroup").Value);
			objRoot.PaymentDueDate = strPostingDate;

			objRoot.BillReferenceNumber = "";

			Log.Information("Retrieving SupplierDetails");
			var supdetails = String.Format(@"Select ""U_SupplierName"",""U_Supplier_TIN"",""U_Supplier_Category""
,""U_Supplier_BRN"",""U_Supplier_Email"",""U_Supplier_MSIC"",""U_Supplier_BusinessActivityDescription"",
""U_Supplier_ContactNumber"",""U_Supplier_AddressLine0"",""U_Supplier_Addressline1"",""U_Supplier_Addressline2""
,""U_Supplier_Postalzone"",""U_Supplier_CityName"",""U_Supplier_State"",""U_Supplier_Country"" from [@SupplierData]");

			oRecordSet1.DoQuery(supdetails);
			//Seller Party
			objRoot.SellerBankAccountNumber = "";
			objRoot.SellerName = strCardName.Trim();
			objRoot.SellerTIN = strCustomer_VATNoS3E3L15M;
			objRoot.SellerCategory = strCustomer_category;
			objRoot.SellerBusinessRegistrationNumber = strCustomer_CRNC.Trim();
			objRoot.SellerSSTRegistrationNumber = "NA";
			objRoot.SellerEmail =  strCustomer_Email;
			 objRoot.SellerContactNumber = strCustomer_TEL1.Trim();
			objRoot.SellerAddressLine0 = strCustomer_Add_StreetNameM;
			objRoot.SellerAddressLine1 = strCustomer_Add_Address2; //strCustomer_Add_Address2;
		    //objRoot.SellerAddressLine2 = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_Addressline2").Value);  ;
			objRoot.SellerPostalZone = intCustomer_Add_PostalZone5DigM;
			objRoot.SellerCityName = strCustomer_Add_CityNameM.Trim(); 
			objRoot.SellerState = strCustomer_Add_CountrySubentityM.Trim();
			objRoot.SellerCountry = strCustomer_Add_CountryM.Trim();
			objRoot.MSICBusinessActivity = strCustomer_businessactivitydesc;
			objRoot.MSICCode = strCustomer_msiccode;

			// CustomerParty
			objRoot.BuyersName = Convert.ToString(oRecordSet1.Fields.Item("U_SupplierName").Value);
			//objRoot.BuyersTIN = "C20948360010";//strCustomer_VATNoS3E3L15M.Trim();
			objRoot.BuyersTIN = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_TIN").Value);//strCustomer_VATNoS3E3L15M.Trim();
			objRoot.BuyersCategory = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_Category").Value);
			objRoot.BuyersBRN =  Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_BRN").Value);    
			objRoot.BuyersSST = "";
			objRoot.BuyersEmail = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_Email").Value); ;
			objRoot.BuyersContactNumber = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_ContactNumber").Value);
			objRoot.BuyersAddressLine0 = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_AddressLine0").Value); 
			objRoot.BuyersAddressLine1 = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_Addressline1").Value); 
			objRoot.BuyersPostalZone = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_Postalzone").Value);
			objRoot.BuyersCityName = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_CityName").Value);
			objRoot.BuyersState = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_State").Value);
			objRoot.BuyersCountry = Convert.ToString(oRecordSet1.Fields.Item("U_Supplier_Country").Value);
			objRoot.BuyersIdentification = "";
			objRoot.BuyersSST = "";

			try
			{
				if (!string.IsNullOrEmpty(objRoot.SellerCountry) && objRoot.SellerCountry.Length == 2)
				{
					RegionInfo buyer = new RegionInfo(objRoot.SellerCountry.ToUpper());
					string sellercountry = buyer.ThreeLetterISORegionName;
					objRoot.SellerCountry = sellercountry;
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
			objRoot.RoundingAmount = strrounding;
			objRoot.PaidAmount = "0.00000";
			objRoot.TotalPayableAmount = objRoot.TotalIncludingTax;
			objRoot.TotalNetAmount = objRoot.TotalIncludingTax;



			// DocTaxTotal
			Log.Information("Retrieving DocTaxSubTotal");
			objRoot.DocTaxTotal = new DocTaxTotal();
			objRoot.DocTaxTotal.TAXCategoryTaxAmountInAccountingCurrency = "0.00";
			objRoot.DocTaxTotal.TotalTaxableAmountPerTaxType = "0";
			objRoot.DocTaxTotal.TaxCategoryId = Convert.ToString(db.GetValue("U_TaxType", 0));
			objRoot.DocTaxTotal.TaxCategorySchemeID = "UN/ECE 5153";
			objRoot.DocTaxTotal.TaxCategorySchemeAgencyID = "6";
			objRoot.DocTaxTotal.TaxCategorySchemeAgencyCode = "OTH";
			objRoot.DocTaxTotal.TAXCategoryRate = Convert.ToString(db.GetValue("U_TaxRate", 0));
			objRoot.DocTaxTotal.DetailsOfTaxExemption = taxexemptionreason;


			// AllowanceCharges
			objRoot.AllowanceCharges = new List<object>(); // Empty list

            // Consolidated Buyer Info
            /*objRoot.ConsolidatedBuyerTIN = "EI00000000010";
			objRoot.ConsolidatedBuyerCategory = "BRN";
			objRoot.ConsolidatedBuyerIdentificationNumberOrPassportNumber = "N/A";*/

            if (objRoot.SellerCountry.ToLower() != "malysia" && objRoot.SellerCountry.ToLower() != "mys")
            {
				objRoot.SellerTIN = "EI00000000030";
                objRoot.SellerBusinessRegistrationNumber = "NA";
				objRoot.SellerState = "NA";
                UpdateVendor(oCompany, strCustomer_cardcode);

			}
            else
            {
				objRoot.SellerTIN = strCustomer_VATNoS3E3L15M;
				objRoot.SellerBusinessRegistrationNumber = strCustomer_CRNC.Trim();
				objRoot.SellerState = strCustomer_Add_CountrySubentityM.Trim();
			}


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
																												   //line1.ProductID = "Resell (INT) - Internet Access";
				line1.DescriptionOfProductOrService = Convert.ToString(oRecordSet.Fields.Item("Dscription").Value); ;
				line1.ProductTariffCode = Convert.ToString(oRecordSet.Fields.Item("U_ProductTariffCode").Value); //"4001.10.00";
				line1.ProductTariffClass = Convert.ToString(oRecordSet.Fields.Item("U_ProductTariffClass").Value); //"PTC";
				line1.Country = Convert.ToString(oRecordSet.Fields.Item("CountryOrg").Value);
				line1.UnitPrice = Convert.ToDecimal(oRecordSet.Fields.Item("Price").Value).ToString("0.000000");
				line1.Quantity = Convert.ToDecimal(oRecordSet.Fields.Item("Quantity").Value).ToString("0.000000");
				//line1.Measurement = "C62";
				line1.Measurement = Convert.ToString(oRecordSet.Fields.Item("U_Vxml_UoM").Value); ;
				line1.Subtotal = Convert.ToDecimal(oRecordSet.Fields.Item("Line Total").Value).ToString("0.000000");
				//line1.SSTTaxCategory = strVATNo;

				line1.TaxType = Convert.ToString(db.GetValue("U_TaxType", 0));
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

			

			String strJSON = JsonConvert.SerializeObject(objRoot, Formatting.Indented);

			//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Starting JSON " + strJSON + DateTime.Now);

			//File.AppendAllText(".\\Log.txt", Environment.NewLine + "Before API Call " + strJSON + DateTime.Now);

			//SBInvoice invoice = new SBInvoice();
			(ApiResponse objResults ,String strRawResponse)= _ILhdnApiService.fnSubmitDocument(strJSON,strToken);

			var outerJson = JsonDocument.Parse(strRawResponse);
			var dataRaw = outerJson.RootElement.GetProperty("data").GetString();

			// Now parse the inner JSON (inside the "data" string)
			
			string allMessages = "";

			// Try to get the "rejectedDocuments" property
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
			if (strTable == "ORPC")
			{
				strObjType = "19";
				oInvoice = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes);
			}
			
			
            else
            {
				strObjType = "18";
				oInvoice = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
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


					////File.AppendAllText(strXMLFilePath + "_log.txt", Environment.NewLine + "Error Updating B1 Object with ZATCA Statuses.");
					Application.SBO_Application.SetStatusBarMessage(oCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Medium, true);
				}
				Log.Information("B1 Object Updated");
				//File.AppendAllText(".\\Log.txt", Environment.NewLine + "B1 Object Updated" + DateTime.Now);
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

		public static Boolean fnCheckStatusUpdate(String strTable, SAPbobsCOM.Company oCompany = null)
		{
			Log.Information("CheckStatusUpdate Service Called");
			Boolean bolValid = false;
			SAPbouiCOM.Form oForm;
			String strFormType = String.Empty;
			string invtypcode = string.Empty;
			if (strTable == "ORPC")
			{
				strFormType = "181";
				invtypcode = "SBCreditNote";
			}
            else
            {
				strFormType = "141";
				invtypcode = "SBInvoice";
			}

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
			SBInvoice obj = new SBInvoice();
			(LoginResponse objResults, String rawResponse) = _ILhdnApiService.fnLogin();
			if (objResults.IsSuccess)
			{
				String strToken = "";
				if (objResults.Data.Token != null)
				{
					strToken = objResults.Data.Token;
					
					(DocumentStatusResponse objResult, String strRawResponse) = _ILhdnApiService.fnCheckStatus(strUUID.Trim(), strToken);
					dynamic data = JsonConvert.DeserializeObject(strRawResponse);

					string status = (string)data["data"]?["status"];
					string datetimevaidated = (string)data["data"]?["dateTimeValidated"];
					List<string> errMsg = new List<string>();
					string allErrors = string.Empty;

					if (status == "Invalid")
					{
						status = "IRBResponseFailed";
						Log.Information($"SBInvoice Status : {status}");

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


					#region Update
					//PDFGenerator pDFGenerator = new PDFGenerator();
					String strValid = "";
					String strQRCode = "";
					String strObjType = "";

					SAPbobsCOM.Documents oInvoice;
					if (!oCompany.Connected)
					{
						oCompany.Connect(); // ya reconnection logic
					}
					if (strTable == "ORPC")
					{
						strObjType = "19";
						oInvoice = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes);
					}


					else
					{
						strObjType = "18";
						oInvoice = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
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
							QrGenerator qrGenerator = new QrGenerator();
							/*if (strTable == "OPCH")
							{
								var inv = Path.Combine(deserialized.QrPath, "SBInvoice");
								path = Path.Combine(inv, strInvNo);
							}
							else
							{
								var cn = Path.Combine(deserialized.QrPath, "SBCreditNote");
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
						if (objResult.Data != null && objResult.Data.ValidationLink != "")
						{
							if (objResult.Data.ValidationLink != null)
								oInvoice.UserFields.Fields.Item("U_ADVIN_QrUrl").Value = objResult.Data.ValidationLink;
								oInvoice.UserFields.Fields.Item("U_LongId").Value = objResult.Data.LongId;
								oInvoice.UserFields.Fields.Item("U_ValidatedDateTime").Value = objResult.Data.DateTimeValidated.ToString();
								oInvoice.UserFields.Fields.Item("U_ADVIN_ErrMsg").Value = "";
						}



						if (oInvoice.Update() != 0)
						{


							////File.AppendAllText(strXMLFilePath + "_log.txt", Environment.NewLine + "Error Updating B1 Object with ZATCA Statuses.");
							Application.SBO_Application.SetStatusBarMessage(oCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Medium, true);
						}
						/*else if (!string.IsNullOrEmpty(objResult.Data.LongId))
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
						}*/
						Log.Information("B1 Object Updated");
						Log.Information("---------------------------------");
						//File.AppendAllText(".\\Log.txt", Environment.NewLine + "B1 Object Updated" + DateTime.Now);
					}




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
			if (strTable == "ORPC")
			{
				strFormType = "181";
			}
			String strObjType = "";
			SAPbobsCOM.Documents oInvoice;
			if (strTable == "ORPC")
			{
				strObjType = "19";
				oInvoice = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes);
			}
			else
			{
				strObjType = "18";
				oInvoice = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
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
				SBInvoice obj = new SBInvoice();
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
		public static void UpdateVendor(Company company, string vendorCode)
	{
		// Get the Business Partner (Vendor) object
		BusinessPartners vendor = (BusinessPartners)company.GetBusinessObject(BoObjectTypes.oBusinessPartners);

		// Load the vendor by CardCode
		if (vendor.GetByKey(vendorCode))
		{
			// Check if it's a vendor
			if (vendor.CardType != BoCardTypes.cSupplier)
			{
				Console.WriteLine($"CardCode '{vendorCode}' is not a vendor (CardType: {vendor.CardType}).");
				return;
			}

			// Update vendor fields
			vendor.FederalTaxID = "EI00000000030";
			vendor.UnifiedFederalTaxID = "NA";
			vendor.BillToState = "NA";

			// Commit the update
			int result = vendor.Update();
			if (result == 0)
			{
				Console.WriteLine("Vendor updated successfully.");
			}
			else
			{
				company.GetLastError(out int errCode, out string errMsg);
				Console.WriteLine($"Update failed: [{errCode}] {errMsg}");
			}
		}
		else
		{
			Console.WriteLine($"Vendor with CardCode '{vendorCode}' not found.");
		}
	}
		public static void ClearCombo(SAPbouiCOM.ComboBox combo)
		{
			try
			{
				while (combo.ValidValues.Count > 0)
				{
					combo.ValidValues.Remove(combo.ValidValues.Count - 1, SAPbouiCOM.BoSearchKey.psk_Index);
				}
			}
			catch (Exception ex)
			{

			}
		}
		public static void FillComboFromDB(SAPbouiCOM.ComboBox combo, SAPbobsCOM.Company oCompany, string selectedCardCode)
		{
			ClearCombo(combo);

			if (string.IsNullOrEmpty(selectedCardCode))
			{
				return;
			}

			SAPbobsCOM.Recordset oRS =
				(SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

			string query = $@"
            SELECT DocNum, U_ADVIN_UUID,U_ValidatedDateTime
            FROM OPCH
            WHERE U_ADVIN_Status = 'IRBResponseSuccess'
            AND CardCode = '{selectedCardCode}' ORDER BY DocEntry DESC
        ";

			try
			{
				oRS.DoQuery(query);

				while (!oRS.EoF)
				{

					string docNum = oRS.Fields.Item("DocNum").Value.ToString();
					string uuid = oRS.Fields.Item("U_ADVIN_UUID").Value.ToString();
					string validatedDateTime = oRS.Fields.Item("U_ValidatedDateTime").Value.ToString();
					string description = $"{uuid}  {validatedDateTime}";
					combo.ValidValues.Add(
						docNum,
						description
					);
					oRS.MoveNext();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error filling ComboBox from DB: " + ex.Message, ex);
			}
			finally
			{
				System.Runtime.InteropServices.Marshal.ReleaseComObject(oRS);
				oRS = null;
				GC.Collect();
			}
		}
	}


}
