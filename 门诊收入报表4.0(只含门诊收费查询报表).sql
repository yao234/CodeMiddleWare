USE [master]
GO
/****** Object:  StoredProcedure [dbo].[proc_ClinicSettlementReport]    Script Date: 10/15/2021 23:47:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
if OBJECT_ID('proc_ClinicSettlementReport')  is not null
drop proc [proc_ClinicSettlementReport];
go
create proc [dbo].[proc_ClinicSettlementReport] @StartTime nvarchar(20),@EndTime nvarchar(20),@MedicareType nvarchar(20)
as
begin
 declare @strSql nvarchar(max);
 if exists(select 1 from sys.views where name='cm_settleView')
	drop view cm_settleView;
 set @strSql='
create view cm_settleView 
as
select a.id,suoshu,b.RegID,
case
	when c.ClassifyCode is null then ''其他''
	else ''''
end  as otherName
,a.CREATETIME,a.ITEMCODE,a.ADVICECODE,a.REGCODE,CASH,a.STATE,c.ClassifyCode,''2'' as [type],b.ChargeType from his.HIS.CM_CUREADVICE a 
left join his.dbo.ClinicRegister b
on a.ADVICECODE=b.CureAdvice or a.ADVICECODE=b.CheckAdvice
left join BaseData.dbo.Consulting c
on a.ITEMCODE=c.CustomCode and a.ITEMNAME=c.Name and a.PRICE=c.Price
where a.Register is null and STATE<>5 and a.CANCELER=''0''
and ITEMNAME not like ''%挂号费%'' and ITEMNAME not like ''%门诊诊察费%''
union
select a.id,suoshu,RegId,
case
	when c.ClassifyCode is null then ''其他''
	else ''''
end 
,a.CREATETIME,a.DRUGCODE as ITEMCODE,a.ADVICECODE,a.REGCODE,a.CASH,a.STATE,c.ClassifyCode,''1'' as [type],b.ChargeType from his.HIS.CM_DRUGADVICE a
left join BaseData.dbo.DM_DICT c
on a.DRUGCODE=c.CODE
left join his.dbo.ClinicRegister b
on a.ADVICECODE=b.DrugAdvice
where c.ClassifyCode is not null and a.STATE<>5 and a.CANCELER=''0''';
exec(@strSql);

--将挂号费和其他的收入类别信息进行相连
	if exists(select 1 from sys.views where name='cm_settleViewgroup')
		drop view cm_settleViewgroup;
set @strSql='
create view cm_settleViewgroup
as
select  RegID,suoshu,a.CREATETIME,Name ITEMNAME,code, cash as CASH ,a.ChargeType from cm_settleView a
left join BASEDATA.DBO.ITEMINFO item on item.REMARKS like ''%''+a.ClassifyCode+''%''  where item.REMARKS<>'''' and (a.otherName is null or a.otherName='''')
union all
select  
b.RegID,suoshu,a.CREATETIME,a.ITEMNAME,b.RegCode as code,a.CASH,b.ChargeType from his.HIS.CM_CUREADVICE a 
left join his.dbo.ClinicRegister b
on a.ADVICECODE=b.CureAdvice or a.ADVICECODE=b.CheckAdvice where (a.STATE<>5 and b.RegID is not null and a.Register is not null) or (a.ITEMNAME like ''%门诊诊察费%'' and a.ITEMNAME like ''%挂号费%'' and a.STATE<>5)
union all
select a.RegID,suoshu,a.createtime,otherName as ItemName,'''' as code,a.cash as Cash,a.ChargeType from cm_settleView a 
where a.otherName<>'''' and a.otherName is not null';
exec(@strSql)

	if exists(select 1 from sys.views where name='cm_YiBaoView')
		drop view cm_YiBaoView;
--set @strSql='
--create view cm_settleCountView
--as(
--select b.RegID,b.Status,b.Register,b.BillCode,b.MedicareType,a.ItemName,a.Cash,a.ChargeType,b.BalanceDate,b.CashPayTotal,b.BalanceTotal,b.AccountPayTotal from cm_settleViewgroup a
--left join his.dbo.MedicareBalance b
--on a.RegID=b.RegID or a.RegID=b.BillCode
--where b.MedicareType=''11'' and b.BillCode<>'''' and b.Status in (''2'',''3''))';
--医保的结算和撤销
set @strSql='
create view cm_YiBaoView
as
select b.RegID,a.suoshu,b.Status,b.Register,b.BillCode,b.MedicareType,a.ItemName,a.Cash,a.ChargeType,b.BalanceDate,b.CashPayTotal,b.BalanceTotal,b.AccountPayTotal from cm_settleViewgroup a
left join his.dbo.MedicareBalance b
on a.suoshu=b.BillCode and a.regid=b.RegID
where suoshu is not null and b.Status=''3'' and  b.MedicareType=''11''
union all
select b.RegID,a.suoshu,b.Status,b.Register,b.BillCode,b.MedicareType,a.ItemName,a.Cash,a.ChargeType,b.BalanceDate,b.CashPayTotal,b.BalanceTotal,b.AccountPayTotal from cm_settleViewgroup a
left join his.dbo.MedicareBalance b
on a.suoshu=b.BillCode and a.regid=b.RegID
where suoshu is not null and b.Status=''2'' and b.MedicareType=''11'''
exec (@strSql);

	if exists(select 1 from sys.views where name='cm_NotYiBaoView')
	drop view cm_NotYiBaoView;
	set @strSql='
create view cm_NotYiBaoView
as
(select b.RegID,a.suoshu,b.Status,b.Register,b.BillCode,b.MedicareType,a.ItemName,a.Cash,a.ChargeType,b.BalanceDate,b.CashPayTotal,b.BalanceTotal,b.AccountPayTotal from cm_settleViewgroup a
left join his.dbo.MedicareBalance b
on a.regid=b.BillCode  
where a.regid not in(
select c.regid from cm_YiBaoView c
group by c.regid) and 
b.Status=''2'' and b.MedicareType=''11'' )
union all
(select b.RegID,a.suoshu,b.Status,b.Register,b.BillCode,b.MedicareType,a.ItemName,a.Cash,a.ChargeType,b.BalanceDate,b.CashPayTotal,b.BalanceTotal,b.AccountPayTotal from cm_settleViewgroup a
left join his.dbo.MedicareBalance b
on a.regid=b.BillCode  
where a.regid not in(
select c.regid from cm_YiBaoView c
group by c.regid) and 
b.Status=''3'' and b.MedicareType=''11''
and b.Register is null)
';
exec(@strSql);


if exists(select 1 from sys.views where name='cm_settleCountView')
		drop view cm_settleCountView;
set @strSql='
create view cm_settleCountView
as
((select * from cm_YiBaoView)
union all
(select * from cm_NotYiBaoView))';
exec (@strSql)

--对门诊诊察费进行过滤
if exists(select 1 from sys.views where name='cm_settleCountView_Filter')
	drop view cm_settleCountView_Filter;
set @strSql='
create view cm_settleCountView_Filter
as
select 
 RegID,suoshu,[Status],Register,
 ItemName=case 
	when ItemName like ''%门诊诊察费%'' then ''门诊诊察费''
	else ItemName
 end
 ,BillCode,MedicareType,Cash,ChargeType,BalanceDate,CashPayTotal,BalanceTotal,AccountPayTotal
 from cm_settleCountView';
exec (@strSql);


	if exists(select 1 from sys.views where name='cm_FinallyViewsettleCountView')
		drop view cm_FinallyViewsettleCountView;
	set @strSql='
create view cm_FinallyViewsettleCountView
as
select RegID,BillCode,[status] as condition,MedicareType,BalanceDate,ISNULL(SUM(BalanceTotal),0) as BalanceTotal,ISNULL(SUM(CashPayTotal),0) as CashPayTotal,ISNULL(SUM(AccountPayTotal),0) as AccountPayTotal,床位费=ISNULL(SUM(床位费),0),西药费=ISNULL(SUM(西药费),0),中成药费=ISNULL(SUM(中成药费),0),检查费=ISNULL(SUM(检查费),0),治疗费=ISNULL(SUM(治疗费),0),化验费=ISNULL(SUM(化验费),0),输氧费=ISNULL(SUM(输氧费),0),放射费=ISNULL(SUM(放射费),0),挂号费=isnull(SUM(挂号费),0),门诊诊察费=isnull(SUM(门诊诊察费),0),其他=ISNULL(SUM(其他),0),中草药费用=ISNULL(SUM(中草药费用),0),中草药=ISNULL(SUM(中草药),0),耗材=ISNULL(SUM(耗材),0) from cm_settleCountView_Filter a
pivot( sum(cash) for a.ItemName in (床位费,西药费,中成药费,检查费,治疗费,化验费,输氧费,放射费,挂号费,门诊诊察费,其他,中草药费用,中草药,耗材)) as p
group by BillCode,RegID,MedicareType,BalanceDate,BalanceTotal,CashPayTotal,AccountPayTotal,[status]
' 
exec(@strSql);
	set @strSql='select RegID,BillCode,condition,床位费,西药费,中成药费,检查费,治疗费,化验费,输氧费,放射费,挂号费,门诊诊察费,其他,中草药费用,中草药,耗材 from cm_FinallyViewsettleCountView where MedicareType=@MedicareType and BalanceDate between @StartTime and @EndTime';
----	--由于编译机制的原因，这里需用exec进行动态执行sql，否则会出现在新的数据库中编译不通过的问题
    exec sp_executesql @strSql,N'@StartTime nvarchar(20),@EndTime nvarchar(20),@MedicareType nvarchar(20)',@StartTime,@EndTime,@MedicareType;
end

--exec [proc_ClinicSettlementReport] '2021/10/14 00:00:00','2021/10/14 23:59:59','11'
