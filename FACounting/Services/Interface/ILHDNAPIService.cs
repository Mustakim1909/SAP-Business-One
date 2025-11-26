using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EInvoice.Models.Advin;

namespace EInvoice.Services.Interface
{
    public interface ILHDNAPIService
    {
        (LoginResponse objResults, String rawResponse) fnLogin();
        (ApiResponse objResults, String rawResponse) fnSubmitDocument(String strJSON, String strToken);
         (DocumentStatusResponse objResults, String rawResponse) fnCheckStatus(String strUUID, String strToken);
        (DocumentStatusResponse objResults, String rawResponse) fnCancelInvoice(String strJSON, String strToken);
        (DocumentStatusResponse objResults, String rawResponse) fnSendEmail(String strJSON, String strToken);

    }
}
