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
    public class CNSQLb1f : SystemFormBase
	{
		private static ILHDNAPIService _ILhdnApiService;
		SAPbouiCOM.Form oForm;
		SAPbouiCOM.DBDataSource db;
		SAPbobsCOM.Documents oInvoice;
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
            this.ComboBox2 = ((SAPbouiCOM.ComboBox)(this.GetItem("Item_2").Specific));
            this.cboCustomer = ((SAPbouiCOM.EditText)(this.GetItem("4").Specific));
			this.btncninv = ((SAPbouiCOM.Button)(this.GetItem("btncninv").Specific));
			this.btncninv.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btncninv_ClickAfter);
			this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_3").Specific));
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
		private SAPbouiCOM.Button btnCancel;
		private SAPbouiCOM.Button btnChkS;
		private SAPbouiCOM.Button btncninv;
		private SAPbouiCOM.EditText cboCustomer;


		private void OnCustomInitialize()
		{
			btnGen.Item.Left = btnCancel.Item.Left + 10 + btnCancel.Item.Width;  // X position
			btnGen.Item.Top = btnCancel.Item.Top;   // Y position
													//btnGen.Item.LinkTo = "2";
			btnGen.Item.Width = btnCancel.Item.Width + 5;
			btnGen.Item.LinkTo = btnCancel.Item.UniqueID;//btnGen.Item.LinkTo = "2";


			btnChkS.Item.Left = btnGen.Item.Left + 10 + btnGen.Item.Width;  // X position
			btnChkS.Item.Top = btnGen.Item.Top;   // Y position
												  //btnGen.Item.LinkTo = "2";
			btnChkS.Item.Width = btnGen.Item.Width + 5;
			btnChkS.Item.LinkTo = btnGen.Item.UniqueID;//btnGen.Item.LinkTo = "2";

			btncninv.Item.Left = btnChkS.Item.Left + btnChkS.Item.Width + 10;
			btncninv.Item.Top = btnChkS.Item.Top;
			btncninv.Item.Width = btnChkS.Item.Width + 5;
			btncninv.Item.LinkTo = btnChkS.Item.UniqueID;
			this.cboCustomer.LostFocusAfter += new SAPbouiCOM._IEditTextEvents_LostFocusAfterEventHandler(cboCustomer_LostFocusAfter);
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
						CreditNote.BaseB2B("ORIN", "RIN1", "RIN2", "RIN9", "RIN11", Program.oCompany, null, strToken);

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
			    CreditNote.fnCheckStatusUpdate("ORIN", Program.oCompany);
				SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Checking Status Completed", SAPbouiCOM.BoMessageTime.bmt_Medium, false);
			}
			catch (Exception ex)
			{
				SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(ex.Message, 1, "Ok");
				return;
				//SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, true);
			}

		}
		private void btnAdd_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
		{
			try
			{
				db = oForm.DataSources.DBDataSources.Item("ORIN");
				oInvoice.UserFields.Fields.Item("U_OriginalInvoiceNumber").Value = ComboBox2.Selected.Value;
				oInvoice.UserFields.Fields.Item("U_OriginalInvoiceIRBMUniqueIdentifierNumber").Value = ComboBox2.Selected.Description;

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

		private void cboCustomer_LostFocusAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
		{
			try
			{
				string selectedCardCode = cboCustomer.Value.Trim(); 

				if (string.IsNullOrEmpty(selectedCardCode))
				{
					return;
				}
				CreditNote.FillComboFromDB(ComboBox2, Program.oCompany, selectedCardCode);
			}
			catch (Exception ex)
			{
				SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(ex.Message, 1, "Ok");
			}
		}
		private void btncninv_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
		{
			try
			{
				SAPbouiCOM.Application SBO_Application = SAPbouiCOM.Framework.Application.SBO_Application;

				// Check if popup is already open
				foreach (SAPbouiCOM.Form f in SBO_Application.Forms)
				{
					if (f.UniqueID == "CanPopup")
					{
						f.Select();
						return;
					}
				}

				// Create the new popup form
				SAPbouiCOM.FormCreationParams oCreationParams = (SAPbouiCOM.FormCreationParams)SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
				oCreationParams.UniqueID = "CanPopup";
				oCreationParams.FormType = "CancelForm";
				oCreationParams.BorderStyle = SAPbouiCOM.BoFormBorderStyle.fbs_Fixed;
				oCreationParams.Modality = SAPbouiCOM.BoFormModality.fm_Modal;

				SAPbouiCOM.Form oForm = SBO_Application.Forms.AddEx(oCreationParams);
				oForm.Title = "Cancel Invoice";
				oForm.Width = 400;
				oForm.Height = 180;

				// Add Static Text Label
				SAPbouiCOM.Item oLbl = oForm.Items.Add("lblReason", SAPbouiCOM.BoFormItemTypes.it_STATIC);
				oLbl.Top = 20;
				oLbl.Left = 20;
				oLbl.Width = 100;
				SAPbouiCOM.StaticText oStatic = (SAPbouiCOM.StaticText)oLbl.Specific;
				oStatic.Caption = "Cancel Reason:";

				// Add Edit Text
				SAPbouiCOM.Item oEditItem = oForm.Items.Add("txtReason", SAPbouiCOM.BoFormItemTypes.it_EDIT);
				oEditItem.Top = 40;
				oEditItem.Left = 20;
				oEditItem.Width = 340;
				SAPbouiCOM.EditText oEdit = (SAPbouiCOM.EditText)oEditItem.Specific;
				oEdit.String = "";

				// Add Button
				// ===== Submit Button (Left side) =====
				SAPbouiCOM.Item oBtnSubmit = oForm.Items.Add("btnSubmit", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
				oBtnSubmit.Top = 90;
				oBtnSubmit.Left = 80;  // Left aligned
				oBtnSubmit.Width = 100;
				SAPbouiCOM.Button oButtonSubmit = (SAPbouiCOM.Button)oBtnSubmit.Specific;
				oButtonSubmit.Caption = "Submit";

				// ===== Cancel Button (Right side) =====
				SAPbouiCOM.Item oBtnCancel = oForm.Items.Add("btnCancel", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
				oBtnCancel.Top = 90;
				oBtnCancel.Left = 200;  // Right aligned
				oBtnCancel.Width = 100;
				SAPbouiCOM.Button oButtonCancel = (SAPbouiCOM.Button)oBtnCancel.Specific;
				oButtonCancel.Caption = "Cancel";

				// Center the form on screen
				// Center the popup on parent form (invoice form)
				SAPbouiCOM.Form oParentForm = SBO_Application.Forms.Item(pVal.FormUID);

				int parentLeft = oParentForm.Left;
				int parentTop = oParentForm.Top;
				int parentWidth = oParentForm.Width;
				int parentHeight = oParentForm.Height;

				int popupWidth = oForm.Width;
				int popupHeight = oForm.Height;

				oForm.Left = parentLeft + (parentWidth - popupWidth) / 2;
				oForm.Top = parentTop + (parentHeight - popupHeight) / 2;


				oForm.Visible = true;
			}
			catch (Exception ex)
			{
				SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Error: " + ex.Message);
			}
		}
		private void Form_LoadAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            throw new System.NotImplementedException();

        }

        public SAPbouiCOM.ComboBox ComboBox2;
        private SAPbouiCOM.StaticText StaticText0;
    }
}
