
GO
/****** Object:  StoredProcedure [dbo].[App_Accounts_ValidateSubmisson_IPM]    Script Date: 24-Mar-23 12:29:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--select * from App_Accounts_Address where
--[App_Accounts_ValidateSubmisson_IPM] 31012
ALTER Procedure [dbo].[App_Accounts_ValidateSubmisson_IPM]
 @RecordKeyId as bigint=0

As 
Begin
	SET NOCOUNT ON;
    
Select  'Personal Details' as Name,count(Accountid) as Rec_count,  count(Accountid) rownnum from app_accounts where AccountId=@RecordKeyId and 
(AccountName<>'' 
--updated by renu on 03/02/2021
--and AccountAlias<>'' --and  isnull(Detail2,'')<>'' and isnull(Detail3,'')<> ''
and (AccountRegType in('NI','NC') Or GeographicalId is not null )
 and  isnull(accountaddress,'')<>'')
union all
Select 'Bank Details' as Name,count(Accountid) as Rec_count, count(Accountid) rownnum from app_accounts where 
AccountId=@RecordKeyId and (BankName<>'' and BankAcNo<>'' 
and (  AccountRegType in('NI','NC') OR isnull(BankIFSCCode,'')<>'')
and isnull(BankBranchName,'')<> '' )
--union all
--Select 'Work Registration Details' as Name,isnull(max(Rec_count),0) ,isnull(min(rownnum),0) from(select count(Accountid) as Rec_count,row_number()over( order by Accountid) rownnum from App_Accounts_WorkRegistration
 --where AccountId=@RecordKeyId group by Accountid)MT where rownnum=1
union all
Select  'Document Details' as Name,isnull(max(Rec_count),0) as Rec_count,isnull(min(rownnum),0) as rownnum   from
(select dense_Rank()over( order by DocumentLookupId)  as Rec_count, row_number()over( order by Accountid) rownnum from App_Accounts_Doc 
where AccountId=@RecordKeyId	 )MT   



	--SELECT accountid,accountname,AccountAlias, BankBranchName,GeographicalId,GeographicalId_PR,accountaddress   FROM App_Accounts where accounttype='C'
	--FROM App_Accounts_Address (NoLock)
	--Where (@RecordKeyId=0 Or AddressId=@RecordKeyId)

End




