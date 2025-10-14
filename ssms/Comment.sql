USE PartyServDb; SET NOCOUNT ON;

DECLARE @news1 int = (SELECT TOP 1 Id FROM dbo.News WHERE Subject=N'Ra mắt thực đơn Hải sản');
DECLARE @news2 int = (SELECT TOP 1 Id FROM dbo.News WHERE Subject=N'Ưu đãi tháng 10');

IF @news1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Comment WHERE [User]='user.mai' AND [News]=@news1)
INSERT dbo.Comment([User],[News],Stars,Description)
VALUES (N'user.mai',@news1,5,N'Món mực rất tươi, gia vị vừa miệng!');

IF @news2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Comment WHERE [User]='user.tuan' AND [News]=@news2)
INSERT dbo.Comment([User],[News],Stars,Description)
VALUES (N'user.tuan',@news2,4,N'Giảm giá tốt, tư vấn nhanh.');
