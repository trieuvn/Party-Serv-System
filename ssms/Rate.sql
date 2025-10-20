USE PartyServDb; SET NOCOUNT ON;

DECLARE @pid1 int = (SELECT Id FROM dbo.Party WHERE Name=N'Sinh nhật bé Na 6 tuổi');
DECLARE @pid2 int = (SELECT Id FROM dbo.Party WHERE Name=N'Đám cưới Hà & Long');

IF @pid1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Rate WHERE [User]='user.mai' AND Party=@pid1)
INSERT dbo.Rate([User],Party,Stars,Comment) VALUES (N'user.mai',@pid1,5,N'Dịch vụ chu đáo, bé rất thích!');

IF @pid2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Rate WHERE [User]='user.thao' AND Party=@pid2)
INSERT dbo.Rate([User],Party,Stars,Comment) VALUES (N'user.thao',@pid2,4,N'Món lên nhanh, trang trí đẹp.');
