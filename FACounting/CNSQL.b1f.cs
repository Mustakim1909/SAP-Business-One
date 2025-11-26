using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EInvoice.Services;
using EInvoice.Services.Interface;
using SAPbouiCOM.Framework;
using static EInvoice.Models.Advin;
//Test
namespace EInvoice
{
	[FormAttribute("179", "CNSQL.b1f")]
    class CNSQLb1f : SystemFormBase
	{
		private static ILHDNAPIService _ILhdnApiService;
		public CNSQLb1f()
		{
			_ILhdnApiService = new LHDNAPIService();
		}

		/// <summary>
		/// Initialize components. Called by framework after form created.
		/// </summary>
		public override void OnInitializeComponent()
		{
			this.btnGen = ((SAPbouiCOM.Button)(this.GetItem("btnGen").Specific));
			this.btnGen.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btnGen_ClickAfter);
			this.btnCancel = ((SAPbouiCOM.Button)(this.GetItem("2").Specific));
			this.btnChkS = ((SAPbouiCOM.Button)(this.GetItem("btnChkS").Specific));
			this.btnChkS.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btnChkS_ClickAfter);
			this.OnCustomInitialize();

		}

		/// <summary>
		/// Initialize form event. Called by framework before form creation.
		/// </summary>
		public override void OnInitializeFormEvents()
		{
            this.DataAddAfter += new SAPbouiCOM.Framework.FormBase.DataAddAfterHandler(this.Form_DataAddAfter);
            this.DataAddBefore += new SAPbouiCOM.Framework.FormBase.DataAddBeforeHandler(this.Form_DataAddBefore);
            this.DataUpdateBefore += new SAPbouiCOM.Framework.FormBase.DataUpdateBeforeHandler(this.Form_DataUpdateBefore);
            // this.LoadAfter += new LoadAfterHandler(this.Form_LoadAfter);
           // this.LoadAfter += new LoadAfterHandler(this.Form_LoadAfter);

        }

        private SAPbouiCOM.Button btnGen;

		private void OnCustomInitialize()
		{
			btnGen.Item.Left = btnCancel.Item.Left + 10 + btnCancel.Item.Width;  // X position
			btnGen.Item.Top = btnCancel.Item.Top;   // Y position
													//btnGen.Item.LinkTo = "2";
			btnGen.Item.Width = btnCancel.Item.Width+5;//btnGen.Item.LinkTo = "2";


			btnChkS.Item.Left = btnGen.Item.Left + 10 + btnGen.Item.Width;  // X position
			btnChkS.Item.Top = btnGen.Item.Top;   // Y position
													 //btnGen.Item.LinkTo = "2";
			btnChkS.Item.Width = btnGen.Item.Width + 5;//btnGen.Item.LinkTo = "2";


		}

		private void btnGen_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
		{
			try
			{

				SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Processing please wait", SAPbouiCOM.BoMessageTime.bmt_Medium, false);

				String strToken = "";
				(LoginResponse objResults, String rawResponse)  = _ILhdnApiService.fnLogin();
				if (objResults.IsSuccess)
				{
					if (objResults.Data.Token != null)
					{
						strToken = objResults.Data.Token;
						Invoice.BaseB2B("ORIN", "RIN1", "RIN2", "RIN9", "RIN11", Program.oCompany, null, strToken);

					}

				}

				//	EINFLICK.FLICK.BaseB2B("ORIN", "RIN1", "RIN12", "RIN9", "RIN11", Program.oCompany);
			//	EINFLICK.FLICK.fnCheckStatusUpdate("ORIN", Program.oCompany);
				SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Processed", SAPbouiCOM.BoMessageTime.bmt_Medium, false);


			}
			catch (Exception ex)
			{
				SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(ex.Message, 1, "Ok");
				return;
				//SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, true);
			}

		}

		private void btnChkS_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
		{
			try
			{

				SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Checking Status please wait", SAPbouiCOM.BoMessageTime.bmt_Medium, false);
			    Invoice.fnCheckStatusUpdate("ORIN", Program.oCompany);
				SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Checking Status Completed", SAPbouiCOM.BoMessageTime.bmt_Medium, false);
			}
			catch (Exception ex)
			{
				SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(ex.Message, 1, "Ok");
				return;
				//SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, true);
			}

		}


		private void Form_DataAddBefore(ref SAPbouiCOM.BusinessObjectInfo pVal, out bool BubbleEvent)
		{

			BubbleEvent = false;
			try
			{
				
			}
			catch (Exception ex)
			{
				SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(ex.Message, 1, "Ok");
				BubbleEvent = false;
				return;

			}

			BubbleEvent = true;

		}
		private void Form_DataAddAfter(ref SAPbouiCOM.BusinessObjectInfo pVal)
		{
			try
			{
				//SystemCore.Core64.BaseB2BSQL("ORIN", "RIN1", "RIN12", "RIN9", "RIN11", Program.oCompany);

			}
			catch (Exception ex)
			{

				SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, true);
			}




		}

		private void Form_DataUpdateBefore(ref SAPbouiCOM.BusinessObjectInfo pVal, out bool BubbleEvent)
		{
			BubbleEvent = false;
			try
			{
				//SystemCore.Core64.ValidateUpdate("ORIN", Program.oCompany);
			}
			catch (Exception ex)
			{

				SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(ex.Message, 1, "Ok");

				return;
			}
			BubbleEvent = true;

		}

		private SAPbouiCOM.Button btnCancel;
		private SAPbouiCOM.Button btnChkS;

        private void Form_LoadAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            throw new System.NotImplementedException();

        }
    }
}
