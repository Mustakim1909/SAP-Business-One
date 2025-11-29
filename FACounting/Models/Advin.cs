using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoice.Models
{
    public class Advin
    {
		public class ADVIN
        {
			
            [JsonProperty("e-Invoice Version")]
			public string EInvoiceVersion { get; set; }

			[JsonProperty("e-Invoice Type Code")]
			public string EInvoiceTypeCode { get; set; }

			[JsonProperty("e-Invoice Number")]
			public string EInvoiceNumber { get; set; }

			[JsonProperty("e-Invoice Date")]
			public string EInvoiceDate { get; set; }

			[JsonProperty("e-Invoice Time")]
			public string EInvoiceTime { get; set; }

			[JsonProperty("Invoice Currency Code")]
			public string InvoiceCurrencyCode { get; set; }

			[JsonProperty("Currency Exchange Rate")]
			public string CurrencyExchangeRate { get; set; }

			[JsonProperty("Payment Mode")]
			public string PaymentMode { get; set; }

			[JsonProperty("Payment Terms")]
			public string PaymentTerms { get; set; }

			[JsonProperty("Payment due date")]
			public string PaymentDueDate { get; set; }

			[JsonProperty("Bill Reference Number")]
			public string BillReferenceNumber { get; set; }

			[JsonProperty("Seller Bank Account Number")]
			public string SellerBankAccountNumber { get; set; }

			[JsonProperty("Seller Name")]
			public string SellerName { get; set; }

			[JsonProperty("Seller TIN")]
			public string SellerTIN { get; set; }

			[JsonProperty("Seller Category")]
			public string SellerCategory { get; set; }

			[JsonProperty("Seller Business Registration Number")]
			public string SellerBusinessRegistrationNumber { get; set; }

			[JsonProperty("Seller SST Registration Number")]
			public string SellerSSTRegistrationNumber { get; set; }

			[JsonProperty("Seller e-mail")]
			public string SellerEmail { get; set; }

			[JsonProperty("Seller Malaysia Standard Industrial Classification Code")]
			public string SellerMSIC { get; set; }

			[JsonProperty("Seller Contact Number")]
			public string SellerContactNumber { get; set; }

			[JsonProperty("Seller Address Line 0")]
			public string SellerAddressLine0 { get; set; }

			[JsonProperty("Seller Address Line 1")]
			public string SellerAddressLine1 { get; set; }

			[JsonProperty("Seller Address Line 2")]
			public string SellerAddressLine2 { get; set; }

			[JsonProperty("Seller Postal Zone")]
			public string SellerPostalZone { get; set; }

			[JsonProperty("Seller City Name")]
			public string SellerCityName { get; set; }

			[JsonProperty("Seller State")]
			public string SellerState { get; set; }

			[JsonProperty("Seller Country")]
			public string SellerCountry { get; set; }

			[JsonProperty("MSIC Business Activity")]
			public string MSICBusinessActivity { get; set; }

			[JsonProperty("MSIC Code")]
			public string MSICCode { get; set; }

			[JsonProperty("Buyer’s Name")]
			public string BuyersName { get; set; }

			[JsonProperty("Buyer’s TIN")]
			public string BuyersTIN { get; set; }

			[JsonProperty("Buyer’s Category")]
			public string BuyersCategory { get; set; }

			[JsonProperty("Buyer’s Business Registration Number")]

			public string BuyersBRN { get; set; }

			[JsonProperty("Buyer’s SST Registration Number")]
			public string BuyersSST { get; set; }

			[JsonProperty("Buyer’s e-mail")]
			public string BuyersEmail { get; set; }

			[JsonProperty("Buyer’s Contact Number")]
			public string BuyersContactNumber { get; set; }

			[JsonProperty("Buyer’s Address Line 0")]
			public string BuyersAddressLine0 { get; set; }

			[JsonProperty("Buyer’s Address Line 1")]
			public string BuyersAddressLine1 { get; set; }
			[JsonProperty("Buyer’s Address Line 2")]
			public string BuyersAddressLine2 { get; set; }

			[JsonProperty("Buyer’s Postal Zone")]
			public string BuyersPostalZone { get; set; }

			[JsonProperty("Buyer’s City Name")]
			public string BuyersCityName { get; set; }

			[JsonProperty("Buyer’s State")]
			public string BuyersState { get; set; }

			[JsonProperty("Buyer’s Country")]
			public string BuyersCountry { get; set; }
			[JsonProperty("Buyer’s Identification Number / Passport Number")]
			public string BuyersIdentification { get; set; }
			[JsonProperty("Shipping Recipient’s Name")]
			public string ShippingRecipientName { get; set; }

			[JsonProperty("Shipping Recipient’s TIN")]
			public string ShippingRecipientTIN { get; set; }

			[JsonProperty("Shipping Recipient’s Category")]
			public string ShippingRecipientCategory { get; set; }

			[JsonProperty("Shipping Recipient’s SubCategory")]
			public string ShippingRecipientSubCategory { get; set; }

			[JsonProperty("Shipping Recipient’s Business Registration Number")]
			public string ShippingRecipientBusinessRegistrationNumber { get; set; }

			[JsonProperty("Shipping Recipient’s Identification Number / Passport Number")]
			public string ShippingRecipientIdentificationOrPassportNumber { get; set; }

			[JsonProperty("Shipping Recipient’s contact point (Person Name)")]
			public string ShippingRecipientContactPointName { get; set; }

			[JsonProperty("Shipping Recipient’s e-mail")]
			public string ShippingRecipientEmail { get; set; }

			[JsonProperty("Shipping Recipient’s Contact Number")]
			public string ShippingRecipientContactNumber { get; set; }

			[JsonProperty("Shipping Address Line 0")]
			public string ShippingAddressLine0 { get; set; }

			[JsonProperty("Shipping Address Line 1")]
			public string ShippingAddressLine1 { get; set; }

			[JsonProperty("Shipping Address Line 2")]
			public string ShippingAddressLine2 { get; set; }

			[JsonProperty("Shipping Postal Zone")]
			public string ShippingPostalZone { get; set; }

			[JsonProperty("Shipping City Name")]
			public string ShippingCityName { get; set; }

			[JsonProperty("Shipping State")]
			public string ShippingState { get; set; }

			[JsonProperty("Shipping Country")]
			public string ShippingCountry { get; set; }
			[JsonProperty("Incoterms")]
			public string Incoterms { get; set; }

			[JsonProperty("Sum of Invoice line net amount")]
			public string SumOfInvoiceLineNetAmount { get; set; }

			[JsonProperty("Sum of allowances on document level")]
			public string SumOfAllowances { get; set; }

			[JsonProperty("Total Fee or Charge Amount")]
			//[JsonProperty("Total Fee / Charge Amount on Document level")]
			public string TotalFeeOrChargeAmount { get; set; }

			[JsonProperty("Total Excluding Tax")]
			public string TotalExcludingTax { get; set; }

			[JsonProperty("Total Including Tax")]
			public string TotalIncludingTax { get; set; }

			[JsonProperty("Rounding amount")]
			public string RoundingAmount { get; set; }

			[JsonProperty("Paid amount")]
			public string PaidAmount { get; set; }

			[JsonProperty("Total Payable Amount")]
			public string TotalPayableAmount { get; set; }

			[JsonProperty("Total Net Amount")]
			public string TotalNetAmount { get; set; }

			[JsonProperty("DocTaxTotal")]
			public DocTaxTotal DocTaxTotal { get; set; }

			[JsonProperty("AllowanceCharges")]
			public List<object> AllowanceCharges { get; set; }

			[JsonProperty("Consolidated Buyer TIN")]
			public string ConsolidatedBuyerTIN { get; set; }

			[JsonProperty("Consolidated Buyer Category")]
			public string ConsolidatedBuyerCategory { get; set; }

			[JsonProperty("Consolidated Buyer Identification Number / Passport Number")]

			public string ConsolidatedBuyerIdentificationNumberOrPassportNumber { get; set; }
			[JsonProperty("Original Invoice Number")]
			public string OriginalInvoiceNumber;
			
			[JsonProperty("Original Invoice IRBM Unique No")]
			public string OriginalInvoiceIRBMUniqueNo;

			[JsonProperty("LineItem")]
			public List<LineItem> LineItem { get; set; }
		}
		public class DocTaxTotal
		{
			[JsonProperty("TAX category tax amount in accounting currency")]
			public string TAXCategoryTaxAmountInAccountingCurrency { get; set; }

			[JsonProperty("Total Taxable Amount Per Tax Type")]
			public string TotalTaxableAmountPerTaxType { get; set; }

			[JsonProperty("TaxCategory Id")]
			public string TaxCategoryId { get; set; }

			[JsonProperty("TaxCategory schemeID")]
			public string TaxCategorySchemeID { get; set; }

			[JsonProperty("TaxCategory schemeAgencyID")]
			public string TaxCategorySchemeAgencyID { get; set; }

			[JsonProperty("TaxCategory schemeAgency code")]
			public string TaxCategorySchemeAgencyCode { get; set; }

			[JsonProperty("TAX category rate")]
			public string TAXCategoryRate { get; set; }

			[JsonProperty("Details of Tax Exemption")]
			public string DetailsOfTaxExemption { get; set; }
			[JsonProperty("Incoterms")]
			public string Incoterms { get; set; }
			
			

		}

		public class LineItem
		{
			[JsonProperty("LineId")]
			public int LineId { get; set; }

			[JsonProperty("Classification Class")]
			public string ClassificationClass { get; set; }

			[JsonProperty("Classification Code")]
			public string ClassificationCode { get; set; }

			[JsonProperty("Product ID")]
			public string ProductID { get; set; }

			[JsonProperty("Description of Product or Service")]
			public string DescriptionOfProductOrService { get; set; }

			[JsonProperty("Product Tariff Code")]
			public string ProductTariffCode { get; set; }

			[JsonProperty("Product Tariff Class")]
			public string ProductTariffClass { get; set; }

			[JsonProperty("Country of Origin")]
			public string Country { get; set; }

			[JsonProperty("Unit Price")]
			public string UnitPrice { get; set; }

			[JsonProperty("Quantity")]
			public string Quantity { get; set; }

			[JsonProperty("Measurement")]
			public string Measurement { get; set; }

			[JsonProperty("Subtotal")]
			public string Subtotal { get; set; }

			[JsonProperty("SST Tax Category")]
			public string SSTTaxCategory { get; set; }

			[JsonProperty("Tax Type")]
			public string TaxType { get; set; }
			[JsonProperty("TaxCategory Id")]
			public string TaxCategoryId { get; set; }

			[JsonProperty("Tax Rate")]
			public string TaxRate { get; set; }

			[JsonProperty("Tax Amount")]
			public string TaxAmount { get; set; }

			[JsonProperty("Details of Tax Exemption")]
			public string DetailsOfTaxExemption { get; set; }

			[JsonProperty("Amount Exempted from Tax")]
			public string AmountExemptedFromTax { get; set; }

			[JsonProperty("Total Excluding Tax")]
			public string TotalExcludingTax { get; set; }

			[JsonProperty("Invoice line net amount")]
			public string InvoiceLineNetAmount { get; set; }

			[JsonProperty("Nett Amount")]
			public string NettAmount { get; set; }

			[JsonProperty("TaxCategory schemeID")]
			public string TaxCategorySchemeID { get; set; }

			[JsonProperty("TaxCategory schemeAgencyID")]
			public string TaxCategorySchemeAgencyID { get; set; }

			[JsonProperty("TaxCategory schemeAgency code")]
			public string TaxCategorySchemeAgencyCode { get; set; }

			[JsonProperty("Consolidated Classification Code")]
			public string ConsolidatedClassificationCode { get; set; }

			[JsonProperty("Consolidated Description")]
			public string ConsolidatedDescription { get; set; }
		}

		public class LoginResponse
		{
			public int StatusCode { get; set; }
			public string Message { get; set; }
			public bool IsSuccess { get; set; }
			public LoginData Data { get; set; } = new LoginData();
		}

		public class LoginData
		{
			public string Token { get; set; }
			public DateTime TokenLifeTime { get; set; }
		}


		public class ApiResponse
		{
			[JsonProperty("statusCode")]
			public int StatusCode { get; set; }

			[JsonProperty("message")]
			public string Message { get; set; }

			[JsonProperty("isSuccess")]
			public bool IsSuccess { get; set; }

			[JsonProperty("data")]
			public string Data { get; set; }

			public SubmissionData ParsedData =>
				string.IsNullOrEmpty(Data) ? null : JsonConvert.DeserializeObject<SubmissionData>(Data);
		}

		public class SubmissionData
		{
			[JsonProperty("submissionUid")]
			public string SubmissionUid { get; set; }

			[JsonProperty("acceptedDocuments")]
			public List<Document> AcceptedDocuments { get; set; }

			[JsonProperty("rejectedDocuments")]
			public List<Document> RejectedDocuments { get; set; }
		}

		public class Document
		{
			[JsonProperty("uuid")]
			public string Uuid { get; set; }

			[JsonProperty("invoiceCodeNumber")]
			public string InvoiceCodeNumber { get; set; }
		}

		public class DocumentStatusResponse
		{
			[JsonProperty("statusCode")]
			public int StatusCode { get; set; }

			[JsonProperty("message")]
			public string Message { get; set; }

			[JsonProperty("isSuccess")]
			public bool IsSuccess { get; set; }

			[JsonProperty("data")]
			public DocumentData Data { get; set; }
		}
		public class DocumentData
		{
			[JsonProperty("uniquedocumentID")]
			public string UniqueDocumentID { get; set; }

			[JsonProperty("uniqueIDofthesubmission")]
			public string UniqueIDofTheSubmission { get; set; }

			[JsonProperty("longId")]
			public string LongId { get; set; }

			[JsonProperty("typeName")]
			public string TypeName { get; set; }

			[JsonProperty("typeVersionName")]
			public string TypeVersionName { get; set; }

			[JsonProperty("issuerTin")]
			public string IssuerTin { get; set; }

			[JsonProperty("issuerName")]
			public string IssuerName { get; set; }

			[JsonProperty("receiverId")]
			public string ReceiverId { get; set; }

			[JsonProperty("receiverName")]
			public string ReceiverName { get; set; }

			[JsonProperty("dateTimeReceived")]
			public DateTime DateTimeReceived { get; set; }

			[JsonProperty("dateTimeValidated")]
			public DateTime DateTimeValidated { get; set; }

			[JsonProperty("totalSales")]
			public string TotalSales { get; set; }

			[JsonProperty("totalDiscount")]
			public string TotalDiscount { get; set; }

			[JsonProperty("netAmount")]
			public string NetAmount { get; set; }

			[JsonProperty("total")]
			public string Total { get; set; }

			[JsonProperty("status")]
			public string Status { get; set; }

			[JsonProperty("createdByUserId")]
			public string CreatedByUserId { get; set; }

			[JsonProperty("documentStatusReason")]
			public string DocumentStatusReason { get; set; }

			[JsonProperty("cancelDateTime")]
			public DateTime? CancelDateTime { get; set; }

			[JsonProperty("rejectRequestDateTime")]
			public DateTime? RejectRequestDateTime { get; set; }

			[JsonProperty("validationResults")]
			public string ValidationResults { get; set; }  // Still a raw string in this version

			[JsonProperty("internalId")]
			public string InternalId { get; set; }

			[JsonProperty("dateTimeIssued")]
			public DateTime DateTimeIssued { get; set; }

			[JsonProperty("validationLink")]
			public string ValidationLink { get; set; }
		}

		public class CancelDocument
        {
			[JsonProperty("uuid")]
			public string uuid { get; set; }
			[JsonProperty("status")]
			public string status { get; set; }
			[JsonProperty("reason")]
			public string reason { get; set; }
        }
		public class CancelDocumentData
        {
			[JsonProperty("uuid")]
			public string uuid { get; set; }
			[JsonProperty("status")]
			public string status { get; set; }
			[JsonProperty("error")]
			public string error { get; set; }
        }
		public class EmailRequest
		{
			[JsonProperty("base64pdf")]
			public object Base64Pdf { get; set; }
			[JsonProperty("emailaddress")]
			public string EmailAddress { get; set; }
			[JsonProperty("invoiceno")]
			public string InvoiceNo { get; set; }
		}
	}
}
