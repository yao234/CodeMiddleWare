USE [his]
GO
/****** Object:  StoredProcedure [dbo].[ClinicRegisterPeople]    Script Date: 10/10/2021 17:48:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
if OBJECT_ID('ClinicRegisterPeople') is not null
drop proc [ClinicRegisterPeople]
go
create proc [dbo].[ClinicRegisterPeople] @StartTime nvarchar(20),@EndTime nvarchar(20)
as
begin
 --����Һ��˴δ洢����
 declare @sqlstr nvarchar(max);
 --��ҽ������Һ�
 if OBJECT_ID('CashPay_ClinicRegisterPeopleView') is not null
	drop view CashPay_ClinicRegisterPeopleView;
 set @sqlstr='create view CashPay_ClinicRegisterPeopleView
as
select ClinicType,c.DepartmentName,
ClinicTypeName=
case
	when ClinicType=''11'' then ''��ͨ����''
	when ClinicType=''16'' then ''�������Բ�''
end  
,b.BalanceDate,b.Register from ClinicRegister a
left join MedicareBalance b
on a.RegID=b.BillCode
left join his.dbo.DepartmentType c
on a.DeptID=c.ID
where b.Status=3 and MedicareType=''11'' and b.Register is not null
';
 
 exec(@sqlstr);
 
 --ҽ������Һ�
 if	OBJECT_ID('HealthCure_ClinicRegisterPeopleView') is not null
	drop view HealthCure_ClinicRegisterPeopleView;
 set @sqlstr='
create view HealthCure_ClinicRegisterPeopleView
as
select ClinicType,c.DepartmentName,
ClinicTypeName=
case
	when ClinicType=''11'' then ''��ͨ����''
	when ClinicType=''16'' then ''�������Բ�''
end  
,b.BalanceDate,b.Register from ClinicRegister a
left join MedicareBalance b
on a.RegID=b.RegID
left join his.dbo.DepartmentType c
on a.DeptID=c.ID
where b.Status=3 and MedicareType=''11'' and b.Register is not null
';
 
 exec(@sqlstr);
 
 --select * from HealthCure_ClinicRegisterPeopleView
 
 if OBJECT_ID('ClinicRegisterPeopleView') is not null
	drop view ClinicRegisterPeopleView;

set @sqlstr='create view ClinicRegisterPeopleView
as
select * from CashPay_ClinicRegisterPeopleView
union
select * from HealthCure_ClinicRegisterPeopleView';

exec (@sqlstr);
	
	
	set @sqlstr='select  departmentName,clinictypename,register,COUNT(1) as HumanCount from ClinicRegisterPeopleView
where BalanceDate between @StartTime and @EndTime
group by departmentName,clinictypename,register';
	exec sp_executesql @sqlstr,N'@StartTime nvarchar(20),@EndTime nvarchar(20)',@StartTime,@EndTime;
end
go
USE [his]
GO
/****** Object:  StoredProcedure [dbo].[ClinicCureInformation]    Script Date: 10/10/2021 17:48:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
if OBJECT_ID('ClinicCureInformation') is not null
drop proc ClinicCureInformation
go
create proc [dbo].[ClinicCureInformation] @StartTime nvarchar(20),@EndTime nvarchar(20)
as
begin
/*�˴洢����Ϊ������������˴α�*/
/*���ݿ���ͳ����ط���*/
	declare @sqlstr nvarchar(max);
	--����������
	if object_id('PayCash_ClinicCureInformation_View') is not null
	begin	
		drop table PayCash_ClinicCureInformation_View;
	end 
	/*ע�⣺���ﲻ����view�����ñ�������ӣ�sp_executesql ���ܴ�����ͼ*/
set @sqlstr='
select * into PayCash_ClinicCureInformation_View from(
select c.DepartmentName,a.RegID,sum(b.BalanceTotal) as BalanceTotal,sum(b.CashPayTotal) as CashPayTotal,
SUM(b.BalanceTotal-b.CashPayTotal) as HealthCurePayTotal,
''1'' as CountType from his.dbo.ClinicRegister a
inner join his.dbo.MedicareBalance b
on a.RegID=b.BillCode
left join his.dbo.DepartmentType c
on a.DeptID=c.ID
where b.MedicareType=''11'' and b.Status=3
and b.BalanceDate between @StartTime and @EndTime
group by c.DepartmentName,a.RegID) as p
';
	exec sp_executesql @sqlstr,N'@StartTime nvarchar(20),@EndTime nvarchar(20)',@StartTime,@EndTime;
  
    --ҽ���������
    if object_id('HealthCure_ClinicCureInformation_View') is not null
		drop table HealthCure_ClinicCureInformation_View;
	
	set @sqlstr='
select * into HealthCure_ClinicCureInformation_View from
(select c.DepartmentName,a.RegID,sum(b.BalanceTotal) as BalanceTotal,sum(b.CashPayTotal) as CashPayTotal,SUM(b.BalanceTotal-b.CashPayTotal) as HealthCurePayTotal,''2'' as CountType from his.dbo.ClinicRegister a
inner join his.dbo.MedicareBalance b
on a.RegID=b.RegID
left join his.dbo.DepartmentType c
on a.DeptID=c.ID
where b.MedicareType=''11'' and b.Status=''3''
and b.BalanceDate between @StartTime and @EndTime
group by c.DepartmentName,a.RegID) as d
';
  exec sp_executesql @sqlstr,N'@StartTime nvarchar(20),@EndTime nvarchar(20)',@StartTime,@EndTime;

    
    /*���������ͼ*/
    --�м��
    if exists(select 1 from sys.views where name='Center_ClinicCureInformation_View')
		drop view Center_ClinicCureInformation_View;
	set @sqlstr='
create view Center_ClinicCureInformation_View
as
(select * from PayCash_ClinicCureInformation_View)
union
(select * from HealthCure_ClinicCureInformation_View)
';
exec (@sqlstr);

	set @sqlstr='select DepartmentName,CountType,COUNT(1) as ClinicPeople,SUM(BalanceTotal) as BalanceTotal,SUM(CashPayTotal) as CashPayTotal,SUM(HealthCurePayTotal) as HealthCurePayTotal from Center_ClinicCureInformation_View
group by DepartmentName,CountType
';
	exec (@sqlstr);
end
go
USE [his]
GO
/****** Object:  StoredProcedure [dbo].[ClinicWorkCount]    Script Date: 10/10/2021 17:48:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
if OBJECT_ID('ClinicWorkCount') is not null
drop proc ClinicWorkCount
go
create proc [dbo].[ClinicWorkCount] @StartTime nvarchar(20),@EndTime nvarchar(20),@MedicareType nvarchar(20)
as
begin
/*conditionΪ״̬��
 100:��ҽ��ҩƷ��
 200:��ҽ�����Ʒ�
 300:��ҽ���Һŷ�
 1001:ҽ��ҩƷ��
 2002:ҽ�����Ʒ�
 3003:ҽ���Һŷ�
*/
	declare @sqlstr nvarchar(max);
	--�����ҽ��
	--��ҽ�� ҩƷ
	--��ҽ�� ���� ���ڹҺŷѿɵ�������,�������������ǲ�ͳ�ƣ����������ظ�
	if exists(select 1 from sys.views where name='ClinicWorkCount_Clinc_NotYiBaoView')
		drop view ClinicWorkCount_Clinc_NotYiBaoView;
	set @sqlstr='
create view ClinicWorkCount_Clinc_NotYiBaoView
as
select b.DeptID,b.DoctorID,a.SuoShu,b.RegID,a.CASH,''100'' as condition  from his.HIS.CM_DRUGADVICE a
left join his.dbo.ClinicRegister b
on a.ADVICECODE=b.DrugAdvice
where a.STATE<>5 and a.CANCELER=''0''
and b.ChargeType=''2''
and a.SuoShu is not null
union all
select b.DeptID,b.DoctorID,a.SuoShu,b.RegID,a.CASH,''200'' as condition from his.his.CM_CUREADVICE a
left join his.dbo.ClinicRegister b
on a.ADVICECODE=b.CureAdvice or a.ADVICECODE=b.CheckAdvice
where a.STATE<>5  and (a.CANCELER=''0'' or (a.CANCELER is null) or a.CANCELER='''')
and b.ChargeType=''2'' and a.Register is null and a.ITEMNAME not like ''%�Һŷ�%'' and a.ITEMNAME not like ''%��������%''
and a.SuoShu is not null';
    exec (@sqlstr);

	--��ҽ���Էѣ�����ͼ������Ϊ�����ֽ���ý���
	--ClinicWorkCount_Clinc_NotYiBaoView��ͼ������ɸѡ��ֻͳ�ƽ����״̬Ϊ3����Ϣ
	if exists(select 1 from sys.views where name='ClinicWorkCount_Clinc_NotYiBaoView_filter')
		drop view ClinicWorkCount_Clinc_NotYiBaoView_filter;
	set @sqlstr='
create view ClinicWorkCount_Clinc_NotYiBaoView_filter
as
select a.DeptID,a.DoctorID,a.SuoShu,b.BillCode as RegID,a.condition,a.cash,b.BalanceDate,b.BalanceTotal,b.CashPayTotal from ClinicWorkCount_Clinc_NotYiBaoView a
left join his.dbo.MedicareBalance b
on a.regid=b.BillCode
where b.Register is null and b.Status=3 and b.MedicareType=''11''';
	exec (@sqlstr);
	
	--����ҽ��
	if exists(select 1 from sys.views where name='ClinicWorkCount_Clinc_YiBaoView')
		drop view ClinicWorkCount_Clinc_YiBaoView;
	set @sqlstr='
create view ClinicWorkCount_Clinc_YiBaoView
as
select b.DeptID,b.DoctorID,a.SuoShu,b.RegID,a.CASH,''1001'' as condition  from his.HIS.CM_DRUGADVICE a
left join his.dbo.ClinicRegister b
on a.ADVICECODE=b.DrugAdvice
where a.STATE<>5 and a.CANCELER=''0''
and b.ChargeType=''1''
and a.SuoShu is not null
union all
select b.DeptID,b.DoctorID,a.SuoShu,b.RegID,a.CASH,''2002'' as condition from his.his.CM_CUREADVICE a
left join his.dbo.ClinicRegister b
on a.ADVICECODE=b.CureAdvice or a.ADVICECODE=b.CheckAdvice
where a.STATE<>5  and (a.CANCELER=''0'' or (a.CANCELER is null) or a.CANCELER='''')
and b.ChargeType=''1'' and a.Register is null and (a.ITEMNAME not like ''%�Һŷ�%'' and a.ITEMNAME not like ''%��������%'')
and a.SuoShu is not null';
	exec (@sqlstr);
	
	--ҽ�����ݹ���
	if exists(select 1 from sys.views where name='ClinicWorkCount_Clinc_YiBaoView_Filter')
	   drop view ClinicWorkCount_Clinc_YiBaoView_Filter;
	set @sqlstr='
create view ClinicWorkCount_Clinc_YiBaoView_Filter
as
select a.DeptID,a.DoctorID,a.SuoShu,a.RegID,a.CASH,a.condition,b.BalanceDate,b.BalanceTotal,b.CashPayTotal from ClinicWorkCount_Clinc_YiBaoView a
left join his.dbo.MedicareBalance b
on a.RegID=b.RegID
where b.Register is null and b.MedicareType=''11''
and b.Status=''3''';
	exec (@sqlstr);
	
	--��ҽ���ҺŷѺ�ҽ���Һŷ�
	if exists(select 1 from sys.views where name='ClinicWorkCount_Clinc_RegisterView')
		drop view ClinicWorkCount_Clinc_RegisterView;
	 set @sqlstr='
create view ClinicWorkCount_Clinc_RegisterView
as
(select b.DeptID,b.DoctorID,a.SuoShu,b.RegID,a.CASH,''300'' as condition from his.his.CM_CUREADVICE a
left join his.dbo.ClinicRegister b
on a.ADVICECODE=b.CureAdvice or a.ADVICECODE=b.CheckAdvice
where a.STATE<>5  and (a.CANCELER=''0'' or (a.CANCELER is null) or a.CANCELER='''')
and b.ChargeType=''2'' and a.Register is not null and (a.ITEMNAME like ''%�Һŷ�%'' or a.ITEMNAME like ''%��������%'')
and a.SuoShu is not null)
union all
(select b.DeptID,b.DoctorID,a.SuoShu,b.RegID,a.CASH,''3003'' as condition from his.his.CM_CUREADVICE a
left join his.dbo.ClinicRegister b
on a.ADVICECODE=b.CureAdvice or a.ADVICECODE=b.CheckAdvice
where a.STATE<>5  and (a.CANCELER=''0'' or (a.CANCELER is null) or a.CANCELER='''')
and b.ChargeType=''1'' and a.Register is not null and (a.ITEMNAME like ''%�Һŷ�%'' or a.ITEMNAME like ''%��������%'')
and a.SuoShu is not null)';
	exec (@sqlstr);
	
	--�Һŷ����ݹ���
	if exists(select 1 from sys.views where name='ClinicWorkCount_Clinc_RegisterView_Filter')
		drop view ClinicWorkCount_Clinc_RegisterView_Filter;
	set @sqlstr='
create view ClinicWorkCount_Clinc_RegisterView_Filter
as
select a.DeptID,a.DoctorID,a.SuoShu,a.RegID,a.condition,a.cash,b.BalanceDate,b.BalanceTotal,b.CashPayTotal from ClinicWorkCount_Clinc_RegisterView a
left join his.dbo.MedicareBalance b
on a.RegID=b.RegID or a.RegID=b.BillCode
where b.Status=''3'' and b.MedicareType=''11'' and b.Register is not null';
	exec (@sqlstr);
	
	
	--���ι���(����)
	if exists(select 1 from sys.views where name='ClinicWorkCount_SecondFilter')
		drop view ClinicWorkCount_SecondFilter;
	set @sqlstr='
create view ClinicWorkCount_SecondFilter
as
select DeptID,RegID,(select DepartmentName from his.dbo.DepartmentType where ID=DeptID) as DepartmentName,DoctorID,(select NAME from BaseData.dbo.EMPLOYEE where EMPID=DoctorID) as DoctorNAME,SuoShu,condition,cash,BalanceDate,BalanceTotal,CashPayTotal from ClinicWorkCount_Clinc_NotYiBaoView_filter
union all
select DeptID,RegID,(select DepartmentName from his.dbo.DepartmentType where ID=DeptID) as DepartmentName,DoctorID,(select NAME from BaseData.dbo.EMPLOYEE where EMPID=DoctorID) as DoctorNAME,SuoShu,condition,cash,BalanceDate,BalanceTotal,CashPayTotal from ClinicWorkCount_Clinc_YiBaoView_Filter
union all
select DeptID,RegID,(select DepartmentName from his.dbo.DepartmentType where ID=DeptID) as DepartmentName,DoctorID,(select NAME from BaseData.dbo.EMPLOYEE where EMPID=DoctorID) as DoctorNAME,SuoShu,condition,cash,BalanceDate,BalanceTotal,CashPayTotal from ClinicWorkCount_Clinc_RegisterView_Filter';
    exec (@sqlstr);
    
    --���ι���
	set @sqlstr='select DepartmentName,DoctorID,DoctorNAME,RegID as Suoshu,condition,BalanceTotal,CashPayTotal,SUM(Cash) as cash from ClinicWorkCount_SecondFilter where balanceDate between @StartTime and @EndTime group by DepartmentName,DoctorID,DoctorNAME,RegID,condition,BalanceTotal,CashPayTotal'
    exec sp_executesql @sqlstr,N'@StartTime nvarchar(20),@EndTime nvarchar(20)',@StartTime,@EndTime;
end
