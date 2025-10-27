USE PartyServDb;
SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Category WHERE Name=N'Món khai vị')  INSERT dbo.Category(Name) VALUES (N'Món khai vị');
IF NOT EXISTS (SELECT 1 FROM dbo.Category WHERE Name=N'Món chính')    INSERT dbo.Category(Name) VALUES (N'Món chính');
IF NOT EXISTS (SELECT 1 FROM dbo.Category WHERE Name=N'Canh/Lẩu')     INSERT dbo.Category(Name) VALUES (N'Canh/Lẩu');
IF NOT EXISTS (SELECT 1 FROM dbo.Category WHERE Name=N'Tráng miệng')  INSERT dbo.Category(Name) VALUES (N'Tráng miệng');
IF NOT EXISTS (SELECT 1 FROM dbo.Category WHERE Name=N'Đồ uống')      INSERT dbo.Category(Name) VALUES (N'Đồ uống');
