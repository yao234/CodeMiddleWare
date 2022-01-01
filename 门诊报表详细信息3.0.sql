USE [master]
GO
/****** Object:  StoredProcedure [dbo].[ClinicMoneyCount]    Script Date: 09/22/2021 17:30:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
if OBJECT_ID('ClinicMoneyCount') is not null
drop proc ClinicMoneyCount;
go
create proc [dbo].[ClinicMoneyCount] @StartTime nvarchar(20),@EndTime nvarchar(20),@MedicareType nvarchar(20)
as
begin
	declare @strsql nvarchar(max)
	--拿到门诊的所有未撤销的收费信息
	if exists(select 1 from sys.views where name='CM_DrugAndCureItemAll')
		drop view CM_DrugAndCureItemAll;
--由于consulting表的customcode有不唯一的code所以在进行连接时需要根据名称以及编码和价格三个字段进行判断，并且为了防止后期连接项目列表时有多个不准确值，所以在此刻应筛选项目编码不为空的值
set @strsql='
create view CM_DrugAndCureItemAll
as
select a.id,b.RegID,a.ITEMCODE,a.ADVICECODE,a.REGCODE,CREATETIME,CASH,a.STATE,c.ClassifyCode,''2'' as [type],b.ChargeType from his.HIS.CM_CUREADVICE a 
left join his.dbo.ClinicRegister b
on a.ADVICECODE=b.CureAdvice or a.ADVICECODE=b.CheckAdvice
left join BaseData.dbo.Consulting c
on a.ITEMCODE=c.CustomCode and a.ITEMNAME=c.Name and a.PRICE=c.Price
where a.Register is null and STATE<>5 and a.CANCELER=''0'' and c.ClassifyCode is not null 
union
select a.id,RegId,a.DRUGCODE as ITEMCODE,a.ADVICECODE,a.REGCODE,CREATETIME,a.CASH,a.STATE,c.ClassifyCode,''1'' as [type],b.ChargeType from his.HIS.CM_DRUGADVICE a
left join BaseData.dbo.DM_DICT c
on a.DRUGCODE=c.CODE
left join his.dbo.ClinicRegister b
on a.ADVICECODE=b.DrugAdvice
where c.ClassifyCode is not null and a.STATE<>5 and a.CANCELER=''0''';
		exec(@strsql)
	
	--根据试图下面的classifyCode找到对应的itemname
	--通过CM_DrugAndCureItemAll试图与BaseData.dbo.ItemInfo相连接并获取相关数据的ItemName信息
	if exists(select 1 from sys.views where name='CM_groupView')
		drop view CM_groupView;
set @strsql='
create view cm_groupview as
(select RegID,RegCode,Name ITEMNAME,code, cash as CASH ,a.ChargeType from CM_DrugAndCureItemAll a
left join BASEDATA.DBO.ITEMINFO item on item.REMARKS like ''%''+a.ClassifyCode+''%'' and item.REMARKS<>'''')';
		exec(@strsql);

--由于挂号费在项目列表里为空,因此如果我们进行连接会将挂号表的那些regid一起连接
--由于后台有挂号费的显示，而此存储过程是显示明细信息，并没有挂号费，又因为明细信息的项目编码在数据库中remarks不为空，因此我们只需查询明细的费用
	if exists(select 1 from sys.views where name='mz_countView')
	  drop view mz_countView;
set @strsql='
create view mz_countView
as(select a.RegID,b.LS,b.Register,a.ItemName,b.BillCode,b.BalanceDate,b.MedicareType,a.Cash,b.BalanceTotal,b.AccountPayTotal,b.SocialFunds,b.CashPayTotal from CM_groupView a
left join his.dbo.MedicareBalance b
on (a.RegID=b.RegID ) or (a.RegID=b.BillCode )
where (b.BillCode<>'''' or b.BillCode is not null) and b.MedicareType=''11'' and b.Status=''3'' and b.Register is null)'
	  exec(@strsql)
	
	--由于在前面已经判断了Register不为空，所以在此时也是取不到挂号费的费用
	if exists(select 1 from sys.views where name='FinallyView')
		drop view FinallyView;
set @strsql='
create view FinallyView as(select RegID,BillCode,MedicareType,BalanceDate,BalanceTotal,SocialFunds,CashPayTotal,AccountPayTotal,床位费=ISNULL(SUM(床位费),0),西药费=ISNULL(SUM(西药费),0),中成药费=ISNULL(SUM(中成药费),0),检查费=ISNULL(SUM(检查费),0),治疗费=ISNULL(SUM(治疗费),0),化验费=ISNULL(SUM(化验费),0),输氧费=ISNULL(SUM(输氧费),0),放射费=ISNULL(SUM(放射费),0),其他=ISNULL(BalanceTotal-SocialFunds-AccountPayTotal-CashPayTotal,0),挂号费=ISNULL(SUM(挂号费),0),门诊诊察费=ISNULL(SUM(门诊诊察费),0),中草药费用=ISNULL(SUM(中草药费用),0),中草药=ISNULL(SUM(中草药),0),耗材=ISNULL(SUM(耗材),0) from mz_countView a
pivot( sum(cash) for a.ItemName in (床位费,西药费,中成药费,检查费,治疗费,化验费,输氧费,放射费,其他,挂号费,门诊诊察费,中草药费用,中草药,耗材)) as p
group by RegID,BillCode,MedicareType,BalanceDate,BalanceTotal,SocialFunds,CashPayTotal,AccountPayTotal)';
		exec(@strsql);	
		
	set @strsql='select RegID,BillCode,床位费,西药费,中成药费,检查费,治疗费,化验费,输氧费,放射费,其他,挂号费,门诊诊察费,中草药费用,中草药,耗材 from FinallyView where MedicareType=@MedicareType and BalanceDate between @StartTime and @EndTime;'	 
	--由于编译机制的原因，这里需用exec进行动态执行sql，否则会出现在新的数据库中编译不通过的问题
	--并且sp_executesql存储过程的参数不支持varchar类型，需要varchar改为nvarchar
	exec sp_executesql @strSql,N'@StartTime nvarchar(20),@EndTime nvarchar(20),@MedicareType nvarchar(20)',@StartTime,@EndTime,@MedicareType;
end
go
USE [master]
GO
/****** Object:  StoredProcedure [dbo].[proc_ClinicSettlementReport]    Script Date: 09/22/2021 17:31:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
if OBJECT_ID('proc_ClinicSettlementReport') is not null
drop proc proc_ClinicSettlementReport;
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


	if exists(select 1 from sys.views where name='cm_FinallyViewsettleCountView')
		drop view cm_FinallyViewsettleCountView;
	set @strSql='
create view cm_FinallyViewsettleCountView
as
select RegID,BillCode,[status] as condition,MedicareType,BalanceDate,ISNULL(SUM(BalanceTotal),0) as BalanceTotal,ISNULL(SUM(CashPayTotal),0) as CashPayTotal,ISNULL(SUM(AccountPayTotal),0) as AccountPayTotal,床位费=ISNULL(SUM(床位费),0),西药费=ISNULL(SUM(西药费),0),中成药费=ISNULL(SUM(中成药费),0),检查费=ISNULL(SUM(检查费),0),治疗费=ISNULL(SUM(治疗费),0),化验费=ISNULL(SUM(化验费),0),输氧费=ISNULL(SUM(输氧费),0),放射费=ISNULL(SUM(放射费),0),挂号费=isnull(SUM(挂号费),0),门诊诊察费=isnull(SUM(门诊诊察费),0),其他=ISNULL(SUM(其他),0),中草药费用=ISNULL(SUM(中草药费用),0),中草药=ISNULL(SUM(中草药),0),耗材=ISNULL(SUM(耗材),0) from cm_settleCountView a
pivot( sum(cash) for a.ItemName in (床位费,西药费,中成药费,检查费,治疗费,化验费,输氧费,放射费,挂号费,门诊诊察费,其他,中草药费用,中草药,耗材)) as p
group by BillCode,RegID,MedicareType,BalanceDate,BalanceTotal,CashPayTotal,AccountPayTotal,[status]
' 
exec(@strSql);
	set @strSql='select RegID,BillCode,condition,床位费,西药费,中成药费,检查费,治疗费,化验费,输氧费,放射费,挂号费,门诊诊察费,其他,中草药费用,中草药,耗材 from cm_FinallyViewsettleCountView where MedicareType=@MedicareType and BalanceDate between @StartTime and @EndTime';
----	--由于编译机制的原因，这里需用exec进行动态执行sql，否则会出现在新的数据库中编译不通过的问题
    exec sp_executesql @strSql,N'@StartTime nvarchar(20),@EndTime nvarchar(20),@MedicareType nvarchar(20)',@StartTime,@EndTime,@MedicareType;
end
go
begin
	update BaseData.dbo.ITEMINFO set REMARKS='8888' where NAME='放射费' 
	or CODE='1800000000';
end
go
begin
update
BaseData.dbo.REPORT
set DEFINE='<?xml version="1.0" encoding="UTF-8"?>
<Report xmlns="http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition" xmlns:rd="http://schemas.microsoft.com/SQLServer/reporting/reportdesigner">
  <Description>
  </Description>
  <Author>
  </Author>
  <PageHeight>210mm</PageHeight>
  <PageWidth>297mm</PageWidth>
  <DataSources>
    <DataSource Name="DS1">
      <ConnectionProperties>
        <DataProvider>SQL</DataProvider>
        <ConnectString>
        </ConnectString>
      </ConnectionProperties>
    </DataSource>
  </DataSources>
  <Width>7.5in</Width>
  <TopMargin>.25in</TopMargin>
  <LeftMargin>.25in</LeftMargin>
  <RightMargin>.25in</RightMargin>
  <BottomMargin>.25in</BottomMargin>
  <DataSets>
    <DataSet Name="Data">
      <Query>
        <DataSourceName>DS1</DataSourceName>
        <CommandText>
        </CommandText>
      </Query>
      <Fields>
        <Field Name="dayin_datetime">
          <DataField>dayin_datetime</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="title">
          <DataField>title</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="Organization">
          <DataField>Organization</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="coordinate1">
          <DataField>coordinate1</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="coordinate2">
          <DataField>coordinate2</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="coordinate3">
          <DataField>coordinate3</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="coordinate4">
          <DataField>coordinate4</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="coordinate5">
          <DataField>coordinate5</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="coordinate6">
          <DataField>coordinate6</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="coordinate7">
          <DataField>coordinate7</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="coordinate8">
          <DataField>coordinate8</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="coordinate9">
          <DataField>coordinate9</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="accounting">
          <DataField>accounting</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="Tabulation">
          <DataField>Tabulation</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="condition">
          <DataField>condition</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="DecondName">
          <DataField>DecondName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="DecondCash">
          <DataField>DecondCash</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="DrugName">
          <DataField>DrugName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="DrugCash">
          <DataField>DrugCash</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="ZiFeiInformation">
          <DataField>ZiFeiInformation</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name="TuiFeiCodeInformation">
          <DataField>TuiFeiCodeInformation</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
      </Fields>
    </DataSet>
  </DataSets>
  <Body>
    <ReportItems>
      <Table Name="Table2">
        <DataSetName>Data</DataSetName>
        <NoRows>Query returned no rows!</NoRows>
        <Style>
          <BorderStyle>
            <Default>Solid</Default>
          </BorderStyle>
          <BorderColor />
          <BorderWidth />
        </Style>
        <TableColumns>
          <TableColumn>
            <Width>97.7pt</Width>
          </TableColumn>
          <TableColumn>
            <Width>81.8pt</Width>
          </TableColumn>
          <TableColumn>
            <Width>105.9pt</Width>
          </TableColumn>
          <TableColumn>
            <Width>83.4pt</Width>
          </TableColumn>
          <TableColumn>
            <Width>102.6pt</Width>
          </TableColumn>
          <TableColumn>
            <Width>87.3pt</Width>
          </TableColumn>
          <TableColumn>
            <Width>80.9pt</Width>
          </TableColumn>
        </TableColumns>
        <Header>
          <TableRows>
            <TableRow>
              <Height>27pt</Height>
              <TableCells>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox16">
                      <Value>=Fields!Organization.Value+''门诊工作日报表''</Value>
                      <Style xmlns="http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition">
                        <TextAlign>Center</TextAlign>
                        <BorderStyle>
                          <Default>Solid</Default>
                        </BorderStyle>
                        <FontWeight>Bold</FontWeight>
                        <BorderColor />
                        <BorderWidth />
                        <VerticalAlign>Bottom</VerticalAlign>
                        <FontSize>16pt</FontSize>
                      </Style>
                    </Textbox>
                  </ReportItems>
                  <ColSpan>7</ColSpan>
                </TableCell>
              </TableCells>
            </TableRow>
            <TableRow>
              <Height>23.9pt</Height>
              <TableCells>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox1">
                      <Value>=Fields!dayin_datetime.Value</Value>
                      <Style xmlns="http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition">
                        <TextAlign>Center</TextAlign>
                        <BorderStyle>
                          <Default>Solid</Default>
                        </BorderStyle>
                        <FontWeight>Bold</FontWeight>
                        <BorderColor />
                        <BorderWidth />
                        <VerticalAlign>Bottom</VerticalAlign>
                        <FontSize>12pt</FontSize>
                      </Style>
                    </Textbox>
                  </ReportItems>
                  <ColSpan>7</ColSpan>
                </TableCell>
              </TableCells>
            </TableRow>
            <TableRow>
              <Height>20.8pt</Height>
              <TableCells>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox5">
                      <Value>项目</Value>
                      <Style>
                        <TextAlign>Center</TextAlign>
                        <BorderStyle>
                          <Default>Solid</Default>
                        </BorderStyle>
                        <FontWeight>Bold</FontWeight>
                        <BorderColor />
                        <BorderWidth />
                        <VerticalAlign>Bottom</VerticalAlign>
                      </Style>
                    </Textbox>
                  </ReportItems>
                </TableCell>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox10">
                      <Value>自费</Value>
                      <Style>
                        <TextAlign>Center</TextAlign>
                        <BorderStyle>
                          <Default>Solid</Default>
                        </BorderStyle>
                        <FontWeight>Bold</FontWeight>
                        <BorderColor />
                        <BorderWidth />
                        <VerticalAlign>Bottom</VerticalAlign>
                      </Style>
                    </Textbox>
                  </ReportItems>
                </TableCell>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox11">
                      <Value>退费</Value>
                      <Style>
                        <TextAlign>Center</TextAlign>
                        <BorderStyle>
                          <Default>Solid</Default>
                        </BorderStyle>
                        <FontWeight>Bold</FontWeight>
                        <BorderColor />
                        <BorderWidth />
                        <VerticalAlign>Bottom</VerticalAlign>
                      </Style>
                    </Textbox>
                  </ReportItems>
                </TableCell>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox12">
                      <Value>自费合计</Value>
                      <Style>
                        <TextAlign>Center</TextAlign>
                        <BorderStyle>
                          <Default>Solid</Default>
                        </BorderStyle>
                        <FontWeight>Bold</FontWeight>
                        <BorderColor />
                        <BorderWidth />
                        <VerticalAlign>Bottom</VerticalAlign>
                      </Style>
                    </Textbox>
                  </ReportItems>
                </TableCell>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox13">
                      <Value>医保</Value>
                      <Style>
                        <TextAlign>Center</TextAlign>
                        <BorderStyle>
                          <Default>Solid</Default>
                        </BorderStyle>
                        <FontWeight>Bold</FontWeight>
                        <BorderColor />
                        <BorderWidth />
                        <VerticalAlign>Bottom</VerticalAlign>
                      </Style>
                    </Textbox>
                  </ReportItems>
                </TableCell>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox14">
                      <Value>医保退费</Value>
                      <Style>
                        <TextAlign>Center</TextAlign>
                        <BorderStyle>
                          <Default>Solid</Default>
                        </BorderStyle>
                        <FontWeight>Bold</FontWeight>
                        <BorderColor />
                        <BorderWidth />
                        <VerticalAlign>Bottom</VerticalAlign>
                      </Style>
                    </Textbox>
                  </ReportItems>
                </TableCell>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox15">
                      <Value>医保合计</Value>
                      <Style>
                        <TextAlign>Center</TextAlign>
                        <BorderStyle>
                          <Default>Solid</Default>
                        </BorderStyle>
                        <FontWeight>Bold</FontWeight>
                        <BorderColor />
                        <BorderWidth />
                        <VerticalAlign>Bottom</VerticalAlign>
                      </Style>
                    </Textbox>
                  </ReportItems>
                </TableCell>
              </TableCells>
            </TableRow>
          </TableRows>
          <RepeatOnNewPage>true</RepeatOnNewPage>
        </Header>
        <Details>
          <TableRows>
            <TableRow>
              <Height>20.7pt</Height>
              <TableCells>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox28">
                      <Value>=Fields!title.Value</Value>
                      <CanGrow>true</CanGrow>
                      <Style>
                        <BorderStyle>
                          <Default>Solid</Default>
                        </BorderStyle>
                        <BorderColor />
                        <BorderWidth />
                      </Style>
                    </Textbox>
                  </ReportItems>
                </TableCell>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox30">
                      <Value>=Fields!coordinate1.Value</Value>
                      <CanGrow>true</CanGrow>
                      <Style>
                        <BorderStyle>
                          <Default>Solid</Default>
                        </BorderStyle>
                        <BorderColor />
                        <BorderWidth />
                      </Style>
                    </Textbox>
                  </ReportItems>
                </TableCell>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox31">
                      <Value>=Fields!coordinate2.Value</Value>
                      <CanGrow>true</CanGrow>
                      <Style>
                        <BorderStyle>
                          <Default>Solid</Default>
                        </BorderStyle>
                      </Style>
                    </Textbox>
                  </ReportItems>
                </TableCell>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox32">
                      <Value>=Fields!coordinate3.Value</Value>
                      <CanGrow>true</CanGrow>
                      <Style>
                        <BorderStyle>
                          <Default>Solid</Default>
                        </BorderStyle>
                      </Style>
                    </Textbox>
                  </ReportItems>
                </TableCell>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox33">
                      <Value>=Fields!coordinate4.Value</Value>
                      <CanGrow>true</CanGrow>
                      <Style>
                        <BorderStyle>
                          <Default>Solid</Default>
                        </BorderStyle>
                      </Style>
                    </Textbox>
                  </ReportItems>
                </TableCell>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox34">
                      <Value>=Fields!coordinate5.Value</Value>
                      <CanGrow>true</CanGrow>
                      <Style>
                        <BorderStyle>
                          <Default>Solid</Default>
                        </BorderStyle>
                      </Style>
                    </Textbox>
                  </ReportItems>
                </TableCell>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox35">
                      <Value>=Fields!coordinate6.Value</Value>
                      <CanGrow>true</CanGrow>
                      <Style>
                        <BorderStyle>
                          <Default>Solid</Default>
                        </BorderStyle>
                      </Style>
                    </Textbox>
                  </ReportItems>
                </TableCell>
              </TableCells>
            </TableRow>
          </TableRows>
        </Details>
        <Left>0.0pt</Left>
        <Top>0.8pt</Top>
        <PageBreakAtEnd>true</PageBreakAtEnd>
        <Footer>
          <TableRows>
            <TableRow>
              <Height>29.6pt</Height>
              <TableCells>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox58">
                      <Value>=Fields!ZiFeiInformation.Value</Value>
                      <Style>
                        <BorderStyle>
                          <Default>Solid</Default>
                          <Left>Solid</Left>
                          <Right>Solid</Right>
                          <Top>Solid</Top>
                          <Bottom>Solid</Bottom>
                        </BorderStyle>
                        <BorderColor>
                        </BorderColor>
                        <BorderWidth />
                        <TextAlign>Center</TextAlign>
                      </Style>
                    </Textbox>
                  </ReportItems>
                  <ColSpan>7</ColSpan>
                </TableCell>
              </TableCells>
            </TableRow>
            <TableRow>
              <Height>31.0pt</Height>
              <TableCells>
                <TableCell>
                  <ReportItems>
                    <Textbox Name="Textbox66">
                      <Value>=Fields!TuiFeiCodeInformation.Value</Value>
                      <Style>
                        <BorderStyle>
                          <Default>Solid</Default>
                        </BorderStyle>
                        <BorderColor />
                        <BorderWidth />
                        <VerticalAlign>Top</VerticalAlign>
                      </Style>
                      <CanGrow>true</CanGrow>
                    </Textbox>
                  </ReportItems>
                  <ColSpan>7</ColSpan>
                </TableCell>
              </TableCells>
            </TableRow>
          </TableRows>
        </Footer>
        <PageBreakAtStart>true</PageBreakAtStart>
      </Table>
    </ReportItems>
    <Height>177.6pt</Height>
    <Columns>1</Columns>
  </Body>
  <PageFooter>
    <Height>71.4pt</Height>
    <PrintOnFirstPage>true</PrintOnFirstPage>
    <PrintOnLastPage>true</PrintOnLastPage>
    <ReportItems>
      <Textbox Name="Textbox4">
        <Height>12.00pt</Height>
        <Width>72.27pt</Width>
        <Value>收款人：</Value>
        <Left>112.00pt</Left>
        <Top>0.00pt</Top>
        <Style>
          <BorderStyle />
          <BorderColor />
          <BorderWidth />
          <FontWeight>Bold</FontWeight>
        </Style>
      </Textbox>
      <Textbox Name="Textbox6" xmlns="http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition">
        <Height>12pt</Height>
        <Width>1in</Width>
        <Value>付款人：</Value>
        <Left>287.7pt</Left>
        <Top>0.8pt</Top>
        <Style>
          <BorderStyle />
          <BorderColor />
          <BorderWidth />
          <FontWeight>Bold</FontWeight>
        </Style>
      </Textbox>
      <Textbox Name="Textbox2" xmlns="http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition">
        <Height>11.99pt</Height>
        <Width>196.32pt</Width>
        <Value>=''制表时间：''+Globals!ExecutionTime</Value>
        <Left>437.8pt</Left>
        <Top>1.6pt</Top>
        <Style>
          <BorderStyle />
          <BorderColor />
          <BorderWidth />
          <FontWeight>Bold</FontWeight>
        </Style>
      </Textbox>
    </ReportItems>
  </PageFooter>
  <PageHeader>
    <Height>0.0pt</Height>
    <PrintOnFirstPage>true</PrintOnFirstPage>
    <PrintOnLastPage>true</PrintOnLastPage>
  </PageHeader>
  <DataElementName>Report</DataElementName>
  <DataElementStyle>AttributeNormal</DataElementStyle>
</Report>
' where NAME='门诊收入日报表'
end


 