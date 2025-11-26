using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbouiCOM.Framework;
using SAPbobsCOM;
using Utility;
using Serilog;

namespace EInvoice
{
     
	//Test
   public class clsMain
    {
       clsMenu objMenu = new clsMenu();
       public clsMain()
       {

           

       }


       public void fnExecuteAddOn()
       {
			Log.Information("ExecuteAddOn Service Called");
			fnTaxinvoice();
			objMenu.fnAddMenues();
        }




        public void fnTaxinvoice()
        {
            Log.Information("TaxInvoice Service Called");
            Utility.clsCreations.CreateUserFields("OINV", "ADVIN_Status", "Status", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 254, "", "", Program.oCompany);
            Utility.clsCreations.CreateUserFields("OINV", "ADVIN_SubmId", "Submission UUID", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 254, "", "", Program.oCompany);
            Utility.clsCreations.CreateUserFields("OINV", "ADVIN_UUID", "UUID", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 254, "", "", Program.oCompany);
            Utility.clsCreations.CreateUserFields("OINV", "ADVIN_InvNo", "Invoice Number", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 254, "", "", Program.oCompany);
            Utility.clsCreations.CreateUserFields("OINV", "ADVIN_ErrMsg", "Error", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 0, "", "", Program.oCompany);
            Utility.clsCreations.CreateUserFields("OINV", "ADVIN_Req", "Requested Json", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 0, "", "", Program.oCompany);
            Utility.clsCreations.CreateUserFields("OINV", "ADVIN_QrUrl", "Validation Link", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 254, "", "", Program.oCompany);
            Utility.clsCreations.CreateUserFields("OINV", "LongId", "LongId", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 254, "", "", Program.oCompany);
            Utility.clsCreations.CreateUserFields("OINV", "ValidatedDateTime", "Validation Datetime", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 254, "", "", Program.oCompany);
            //Utility.clsCreations.CreateUserFields("ORIN", "OriginalInvoiceNumber", "OriginalInvoiceNumber", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 254, "", "", Program.oCompany);
           // Utility.clsCreations.CreateUserFields("ORIN", "OriginalInvoiceIRBMUniqueNo", "InvoiceIRBMUniqueNo", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 254, "", "", Program.oCompany);
            Utility.clsCreations.CreateUserFields("OINV", "TaxRate", "Tax Rate", BoFieldTypes.db_Numeric, BoFldSubTypes.st_None, 20, "", "", Program.oCompany);
            Utility.clsCreations.CreateUserFields("OINV", "PaymentMode", "Payment Mode", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 50, "", "", Program.oCompany);

            Utility.clsCreations.CreateUserFields("INV1", "ClassificationCode", "ClassificationCode", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 20, "", "", Program.oCompany);
            Utility.clsCreations.CreateUserFields("INV1", "ClassificationClass", "ClassificationClass", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 50, "", "", Program.oCompany);
            Utility.clsCreations.CreateUserFields("INV1", "ProductTariffCode", "ProductTariffCode", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 20, "", "", Program.oCompany);
            Utility.clsCreations.CreateUserFields("INV1", "ProductTariffClass", "ProductTariffClass", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 50, "", "", Program.oCompany);

           // Utility.clsCreations.CreateUserFields("OITM", "ClassificationCode", "ClassificationCode", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 20, "", "", Program.oCompany);
          //  Utility.clsCreations.CreateUserFields("OITM", "ClassificationClass", "ClassificationClass", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 50, "", "", Program.oCompany);
           // Utility.clsCreations.CreateUserFields("OITM", "ProductTariffCode", "ProductTariffCode", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 20, "", "", Program.oCompany);
           // Utility.clsCreations.CreateUserFields("OITM", "ProductTariffClass", "ProductTariffClass", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 50, "", "", Program.oCompany);

            Utility.clsCreations.CreateUserFields("OCRD", "Category", "Category", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 50, "", "", Program.oCompany);
            Utility.clsCreations.CreateUserFields("OCRD", "BusinessActivityDesc", "BusinessActivityDescription", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 254, "", "", Program.oCompany);
            Utility.clsCreations.CreateUserFields("OCRD", "MSICCode", "MSIC Code", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 50, "", "", Program.oCompany);
            Utility.clsCreations.CreateUserFields("OCRD", "TaxEx01emptionReason", "Tax Excemption Reason", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, 254, "", "", Program.oCompany);


        }



    }
}
