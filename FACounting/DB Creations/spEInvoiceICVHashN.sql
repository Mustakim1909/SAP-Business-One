create PROCEDURE  "spEInvoiceICVHashN"

(

@ObjType nvarchar(254),
@DocEntry int,
@ICV int


) 
 
LANGUAGE as  SQLSCRIPT
--READS SQL DATA
AS 
BEGIN

declare @Discount decimal;
declare @Difference decimal;
declare @intCount int;
declare @i int;
--declare @intPresentValue int;
declare @intAlreadyEntry int;


insert into "@EINICV" 


(

"Code",
"Name",
"U_EIN_ICV",
"U_EIN_DocEntry",
"U_EIN_CreateDate",
"U_EIN_DocTime",
"U_EIN_ObjType",
"U_EIN_Hash",
"U_EIN_STAT",
"U_EIN_PIH",
"U_EIN_DocNum",

"U_EIN_UUID",
"U_EIN_INVHASH",
"U_EIN_ZCRSTATUS",
"U_EIN_ZSCODE",
"U_EIN_ZREQ",
"U_EIN_ZRESP",
"U_EIN_GENXML",
"U_EIN_SIGNXML",
"U_EIN_ZXML",
"U_EIN_XMLPATH",
"U_EIN_VALIDATEXML"


)



select * from

(

select 
@ICV "ICV"
,cast(a."DocEntry" as nvarchar(254))||'-'||a."ObjType" "Name"
,ICV "ICV"
,a."DocEntry"
,a."CreateDate"
,a."DocTime"
,a."ObjType"
,a."U_EIN_INVHASH"
,a."U_EIN_ZSCODE"
,a."U_EIN_PIH"
,a."DocNum"  

,a."U_EIN_UUID"
,a."HASH"
,a."U_EIN_ZCRSTATUS"
,a."ZSCODE"
,a."U_EIN_ZREQ"
,a."U_EIN_ZRESP"
,a."U_EIN_GENXML"
,a."U_EIN_SIGNXML"
,a."U_EIN_ZXML"
,a."U_EIN_XMLPATH"
,a."U_EIN_VALIDATEXML"
from 
(
select "DocEntry","CreateDate","DocTime","ObjType","U_EIN_INVHASH","U_EIN_ZSCODE","U_EIN_PIH","DocNum"
,"U_EIN_UUID","U_EIN_INVHASH" "HASH","U_EIN_ZCRSTATUS","U_EIN_ZSCODE" "ZSCODE","U_EIN_ZREQ","U_EIN_ZRESP","U_EIN_GENXML","U_EIN_SIGNXML","U_EIN_ZXML","U_EIN_XMLPATH","U_EIN_VALIDATEXML" from "OINV"
where "ObjType" = @ObjType and "DocEntry" = :DocEntry and "U_EIN_INVHASH" is not null and "U_EIN_ZSCODE" not in ('InternalServerError','NotImplemented','BadGateway','ServiceUnavailable','GatewayTimeout','HttpVersionNotSupported')

union all
select "DocEntry","CreateDate","DocTime","ObjType","U_EIN_INVHASH","U_EIN_ZSCODE","U_EIN_PIH","DocNum"
,"U_EIN_UUID","U_EIN_INVHASH" "HASH","U_EIN_ZCRSTATUS","U_EIN_ZSCODE" "ZSCODE" ,"U_EIN_ZREQ","U_EIN_ZRESP","U_EIN_GENXML","U_EIN_SIGNXML","U_EIN_ZXML","U_EIN_XMLPATH","U_EIN_VALIDATEXML" from "ORIN"
where "ObjType" = :ObjType and "DocEntry" = :DocEntry and "U_EIN_INVHASH" is not null and "U_EIN_ZSCODE" not in ('InternalServerError','NotImplemented','BadGateway','ServiceUnavailable','GatewayTimeout','HttpVersionNotSupported')
union all
select "DocEntry","CreateDate","DocTime","ObjType","U_EIN_INVHASH","U_EIN_ZSCODE","U_EIN_PIH","DocNum"
,"U_EIN_UUID","U_EIN_INVHASH" "HASH","U_EIN_ZCRSTATUS","U_EIN_ZSCODE" "ZSCODE","U_EIN_ZREQ","U_EIN_ZRESP","U_EIN_GENXML","U_EIN_SIGNXML","U_EIN_ZXML","U_EIN_XMLPATH","U_EIN_VALIDATEXML" from "ODPI"
where "ObjType" = :ObjType and "DocEntry" = :DocEntry and "U_EIN_INVHASH" is not null and "U_EIN_ZSCODE" not in ('InternalServerError','NotImplemented','BadGateway','ServiceUnavailable','GatewayTimeout','HttpVersionNotSupported')
 
) a

order by a."CreateDate",a."DocTime",a."DocEntry"

) b;





END