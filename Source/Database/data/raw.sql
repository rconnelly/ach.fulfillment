SET IDENTITY_INSERT [ach].[Universe] ON
INSERT [ach].[Universe] ([UniverseId], [Login], [PasswordHash], [Created], [Modified], [Deleted]) VALUES (1, N'test', N'', CAST(0x0000A06100F9988B AS DateTime), NULL, 0)
SET IDENTITY_INSERT [ach].[Universe] OFF
SET IDENTITY_INSERT [ach].[Currency] ON
INSERT [ach].[Currency] ([CurrencyId], [UniverseId], [Name], [Description], [CurrencyCode], [Hidden], [Position], [Created], [Modified]) VALUES (1, 1, N'UAH', NULL, N'UAH', 0, 0, CAST(0x0000A06100F9988F AS DateTime), NULL)
INSERT [ach].[Currency] ([CurrencyId], [UniverseId], [Name], [Description], [CurrencyCode], [Hidden], [Position], [Created], [Modified]) VALUES (2, 1, N'USD', NULL, N'USD', 0, 0, CAST(0x0000A06100F9988F AS DateTime), NULL)
INSERT [ach].[Currency] ([CurrencyId], [UniverseId], [Name], [Description], [CurrencyCode], [Hidden], [Position], [Created], [Modified]) VALUES (3, 1, N'EUR', NULL, N'EUR', 0, 0, CAST(0x0000A06100F9988F AS DateTime), NULL)
SET IDENTITY_INSERT [ach].[Currency] OFF