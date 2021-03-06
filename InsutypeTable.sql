USE [BaseData]
GO
/****** Object:  Table [dbo].[InsutypeTable]    Script Date: 01/14/2022 16:41:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InsutypeTable](
	[Code] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[catorytype] [int] NULL,
	[customtype] [int] NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[InsutypeTable] ON
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (0, N'', NULL, NULL, 1)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (1, N'insutype', 0, NULL, 2)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (2, N'med_type', 0, NULL, 3)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (3, N'pay_loc', 0, NULL, 4)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (4, N'cvlserv_lv', 0, NULL, 5)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (5, N'mdtrt_cert_type', 0, NULL, 6)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (6, N'测试2', 0, -2147483648, 7)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (11, N'普通门诊', 2, 11, 8)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (12, N'门诊挂号', 2, 11, 9)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (13, N'急诊', 2, NULL, 10)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (15, N'特药', 2, NULL, 11)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (21, N'普通住院', 2, 21, 12)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (22, N'外伤住院', 2, 21, 13)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (23, N'转外住院', 2, 21, 14)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (24, N'急诊转住院', 2, NULL, 15)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (25, N'异地就医', 2, NULL, 16)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (28, N'日间手术', 2, NULL, 17)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (41, N'药店购药', 2, NULL, 18)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (51, N'生育门诊', 2, 11, 19)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (52, N'生育住院', 2, 21, 20)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (71, N'家庭病床', 2, NULL, 21)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (91, N'其他门诊', 2, 11, 22)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (92, N'其他住院', 2, 21, 23)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (93, N'其他购药', 2, NULL, 24)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (310, N'职工基本医疗保险', 1, NULL, 25)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (320, N'公务员医疗补助', 1, NULL, 26)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (321, N'高层次人才', 1, NULL, 27)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (330, N'大额医疗费用补助', 1, NULL, 28)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (340, N'离休人员医疗保障', 1, NULL, 29)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (350, N'一至六级残废军人医疗补助', 1, NULL, 30)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (370, N'企业补充医疗保险', 1, NULL, 31)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (390, N'城乡居民基本医疗保险', 1, NULL, 32)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (392, N'城乡居民大病医疗保险', 1, NULL, 33)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (399, N'涟康宝', 1, NULL, 34)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (503, N'测试', 1, 11, 35)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (510, N'生育保险', 1, NULL, 36)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (1401, N'门诊慢性病', 2, 11, 37)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (1402, N'门诊特殊病', 2, 11, 38)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (1403, N'门诊大病', 2, 11, 39)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (2301, N'转外住院', 2, 21, 40)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (2302, N'异地就医', 2, NULL, 41)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (4101, N'定点药店购药', 2, NULL, 42)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (4102, N'定点特殊药品', 2, NULL, 43)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (5301, N'计划生育门诊', 2, 11, 44)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (5302, N'计划生育住院', 2, 21, 45)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (5303, N'计划生育手术费', 2, NULL, 46)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (9901, N'单病种', 2, NULL, 47)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (9902, N'家庭病床', 2, NULL, 48)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (9903, N'门诊两病', 2, 11, 49)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (9904, N'门诊特检特治', 2, 11, 50)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (9905, N'日间病床', 2, NULL, 51)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (9906, N'血液特殊病', 2, NULL, 52)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (9907, N'门诊特检特制(阳性)', 2, 11, 53)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (39904, N'建国前老工人医疗保险', 1, NULL, 54)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (140201, N'门诊特殊病', 2, 11, 55)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (210303, N'精神病住院', 2, 21, 56)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (530102, N'计划生育门诊', 2, 11, 57)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (530202, N'计划生育住院', 2, 21, 58)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (990301, N'转诊转院', 2, NULL, 59)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (990602, N'血液特殊病', 2, NULL, 60)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (990802, N'特殊项目报销', 2, NULL, 61)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (990901, N'特殊病住院', 2, 21, 62)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (990902, N'门诊专项用药', 2, 11, 63)
INSERT [dbo].[InsutypeTable] ([Code], [Name], [catorytype], [customtype], [ID]) VALUES (991202, N'分疗程间断住院', 2, 21, 64)
SET IDENTITY_INSERT [dbo].[InsutypeTable] OFF
