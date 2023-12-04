Select GeographicalId_PR,BookId,* From App_Accounts
Select top 100 * From App_EmailSMSSchedule order by EmailSMSScheduleId desc
Delete From App_Accounts Where AccountId = 31029

Select * From App_Accounts_Temp

Delete From App_Accounts_Temp Where TempAccountId = 12947

Select * From App_Accounts_Temp Where AccountName = 'rohan'

update App_Accounts set DateOfDeath = 2020-02-17  Where AccountId = 31008