SET IDENTITY_INSERT [dbo].[RuleTypeSet] ON
INSERT INTO [dbo].[RuleTypeSet] ([Id], [Name], [Description], [HasSchedule], [IsEvent]) VALUES (1, N'ScheduledTask', N'Scheduled Tasks', 0, 0)
INSERT INTO [dbo].[RuleTypeSet] ([Id], [Name], [Description], [HasSchedule], [IsEvent]) VALUES (2, N'ManualTask', N'Task for manual run', 0, 0)
INSERT INTO [dbo].[RuleTypeSet] ([Id], [Name], [Description], [HasSchedule], [IsEvent]) VALUES (3, N'WorkItemEvent', N'Process work item changes', 0, 0)
SET IDENTITY_INSERT [dbo].[RuleTypeSet] OFF

SET IDENTITY_INSERT [dbo].[ScheduleTypeSet] ON
INSERT INTO [dbo].[ScheduleTypeSet] ([Id], [Name], [Period], [Step]) VALUES (1, N'Each 5 mins', 0, 5)
INSERT INTO [dbo].[ScheduleTypeSet] ([Id], [Name], [Period], [Step]) VALUES (2, N'Each hour', 1, 1)
INSERT INTO [dbo].[ScheduleTypeSet] ([Id], [Name], [Period], [Step]) VALUES (3, N'Each 4 hours', 1, 4)
SET IDENTITY_INSERT [dbo].[ScheduleTypeSet] OFF