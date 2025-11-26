using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Serilog;
using SAPbouiCOM.Framework;

namespace EInvoice
{


    public class clsMenu
    {
        //Test Comments for TFS

        //public clsMenu()
        //{



        //}

        public void fnAddMenues()
        {
            Log.Information("AddMenues Service Called");
            SAPbouiCOM.Menus oMenus;
            SAPbouiCOM.MenuItem oMenuItem;
            oMenus = Application.SBO_Application.Menus;
            SAPbouiCOM.MenuCreationParams oCreationPackage;

            oCreationPackage = (SAPbouiCOM.MenuCreationParams)Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams);
            oMenuItem = Application.SBO_Application.Menus.Item("43520");// 'Modules
            oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_POPUP;
			oCreationPackage.UniqueID = "EInvAddOn";
            oCreationPackage.String = "ADVIN EInvoice";
            oCreationPackage.Enabled = true;
            oCreationPackage.Position = -1;
            oMenus = oMenuItem.SubMenus;


			try
            {
                //If the manu already exists this code will fail
                oMenus.AddEx(oCreationPackage);
            }
            catch {
                Log.Error("Exception in AddMenues");
            }


            
 

         


        }
        public void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {

				SAPbouiCOM.Form oForm = Application.SBO_Application.Forms.ActiveForm;



			}
            catch 
            {
                //Application.SBO_Application.MessageBox(ex.ToString(), 1, "Ok", "", "");
            }
           
        }

       
        private SAPbouiCOM.Application SBO_Application;

        private SAPbouiCOM.Form oOrderForm;

        private SAPbouiCOM.Item oNewItem;

        private SAPbouiCOM.Item oItem;

        private SAPbouiCOM.Folder oFolderItem;

        private SAPbouiCOM.OptionBtn oOptionBtn;

        private SAPbouiCOM.CheckBox oCheckBox;

        private SAPbouiCOM.PictureBox oPictureBox;

        private int i;
        int lastFormType = -1;
        public void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            lastFormType = pVal.FormType;
            BubbleEvent = true;

            if ((((pVal.FormType == 133 || pVal.FormType == 179 || pVal.FormType == 141 || pVal.FormType == 181) & pVal.EventType != SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD) & (pVal.Before_Action == true)))
            {

                // get the event sending form
                oOrderForm = Application.SBO_Application.Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);

                if (((pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD) & (pVal.Before_Action == true)))
                {

                    // add a new folder item to the form
                    //oNewItem = oOrderForm.Items.Add("ZATCA", SAPbouiCOM.BoFormItemTypes.it_FOLDER);

                    // use an existing folder item for grouping and setting the
                    // items properties (such as location properties)
                    // use the 'Display Debug Information' option (under 'Tools')
                    // in the application to acquire the UID of the desired folder
                    //oItem = oOrderForm.Items.Item("138");


                    //oNewItem.Top = oItem.Top;
                    //oNewItem.Height = oItem.Height;
                    //oNewItem.Width = oItem.Width;
                    //oNewItem.Left = oItem.Left + oItem.Width;


                    //               oFolderItem = ((SAPbouiCOM.Folder)(oNewItem.Specific));

                    //oFolderItem.Caption = "ZATCA";
                    //               oFolderItem.Pane = 135;
                    // group the folder with the desired folder item
                    //oFolderItem.GroupWith("138");

                    // add your own items to the form

                    oOrderForm.PaneLevel = 1;

                }

                if (pVal.ItemUID == "ZATCA" & pVal.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED & pVal.Before_Action == true)
                {

                    // when the new folder is clicked change the form's pane level
                    // by doing so your items will apear on the new folder
                    // assuming they were placed correctly and their pane level
                    // was also set accordingly
                    //oOrderForm.PaneLevel = 5;


                }

            }
            if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
            {

            }



           /* if (pVal.FormType == 133 && pVal.ItemUID == "38" && pVal.ColUID == "1" &&
     pVal.EventType == SAPbouiCOM.BoEventTypes.et_VALIDATE && !pVal.BeforeAction)
            {
                try
                {
                    SAPbouiCOM.Form oForm = Application.SBO_Application.Forms.Item(FormUID);
                    SAPbouiCOM.Matrix oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item("38").Specific;
                    //SAPbouiCOM.EditText oUDF = (SAPbouiCOM.EditText)oForm.Items.Item("U_ClassificationCode").Specific;

                    string itemCode = ((SAPbouiCOM.EditText)oMatrix.Columns.Item("1").Cells.Item(pVal.Row).Specific).Value;

                    SAPbobsCOM.Recordset rs = (SAPbobsCOM.Recordset)Program.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    string query = $"SELECT U_ClassificationCode, U_ClassificationClass, U_ProductTariffCode, U_ProductTariffClass FROM OITM WHERE ItemCode = '{itemCode}'";
                    rs.DoQuery(query);

                    if (!rs.EoF)
                    {
                        string classificationCode = rs.Fields.Item("U_ClassificationCode").Value?.ToString() ?? "";
                        string classificationClass = rs.Fields.Item("U_ClassificationClass").Value?.ToString() ?? "";
                        string tariffCode = rs.Fields.Item("U_ProductTariffCode").Value?.ToString() ?? "";
                        string tariffClass = rs.Fields.Item("U_ProductTariffClass").Value?.ToString() ?? "";
                        oMatrix.SetCellWithoutValidation(pVal.Row, "U_ClassificationCode", classificationCode);
                        oMatrix.SetCellWithoutValidation(pVal.Row, "U_ClassificationClas", classificationClass);
                        oMatrix.SetCellWithoutValidation(pVal.Row, "U_ProductTariffCode", tariffCode);
                        oMatrix.SetCellWithoutValidation(pVal.Row, "U_ProductTariffClass", tariffClass);
                    }

                }
                catch (Exception ex)
                {
                    Application.SBO_Application.MessageBox("UDF Fill Error: " + ex.Message);
                }
            }*/

            if (pVal.FormTypeEx == "CancelForm" &&
      pVal.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED &&
      !pVal.BeforeAction)
            {
                SAPbouiCOM.Form oForm = SAPbouiCOM.Framework.Application.SBO_Application.Forms.Item(FormUID);

                if (pVal.ItemUID == "btnSubmit")
                {
                    string reason = ((SAPbouiCOM.EditText)oForm.Items.Item("txtReason").Specific).Value;

                    if (string.IsNullOrWhiteSpace(reason))
                    {
                        SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Please enter a reason.");
                        return;
                    }
                    oForm.Close();
                    SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Sending Cancel Request", SAPbouiCOM.BoMessageTime.bmt_Medium, false);
                    if (lastFormType == 133)
                    {
                        Invoice.fnCancelDocument("OINV", reason, Program.oCompany);  // Your method
                    }
                    else if(lastFormType == 141)
                    {
                        SBInvoice.fnCancelDocument("OPCH", reason, Program.oCompany);
                    }
                    
                    SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Cancel Request Completed", SAPbouiCOM.BoMessageTime.bmt_Medium, false);// Close popup after submit
                }
                else if (pVal.ItemUID == "btnCancel")
                {
                    oForm.Close(); 
                }
            }
        }

            private void AddToPermissionTree(String strPermissionName, String strPermissionID, String strFormType, String strParentPermissionID)
		{
			long RetVal;
			//			long ErrCode;
			string ErrMsg = "";
			SAPbobsCOM.UserPermissionTree oPermission;

			oPermission = (SAPbobsCOM.UserPermissionTree)Program.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserPermissionTree);

			oPermission.Name = strPermissionName;
			oPermission.PermissionID = strPermissionID;
			if (strFormType.Length > 0)
			{
				oPermission.UserPermissionForms.FormType = strFormType;
			}
			if (strParentPermissionID.Length > 0)
			{
				oPermission.ParentID = strParentPermissionID;
			}
			oPermission.Options = SAPbobsCOM.BoUPTOptions.bou_FullNone;

			RetVal = oPermission.Add();

			int temp_int = (int)(RetVal);
			string temp_string = ErrMsg;
			Program.oCompany.GetLastError(out temp_int, out temp_string);
			if (RetVal != 0)
			{
				//MessageBox.Show(temp_string);
			}
			else
			{
				//grpPermission.Enabled = false;
				//grpSetPermission.Enabled = true;
			}
		}
       


    
        public  void SBO_Application_FormDataEvent(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
           
            if (BusinessObjectInfo.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD
              && BusinessObjectInfo.ActionSuccess && !BusinessObjectInfo.BeforeAction)
            {
               

            }

           

              
            }



        }
    }



