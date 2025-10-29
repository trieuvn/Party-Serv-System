USE PartyServDb; SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM dbo.News WHERE Subject=N'Ra mắt thực đơn Hải sản')
INSERT dbo.News(Name,Subject,Description,Status,CreatedDate,[User])
VALUES (N'Thông báo',N'Ra mắt thực đơn Hải sản',N'Thêm nhiều món tôm mực tươi ngon cho mùa hè.',N'Active',GETDATE(),N'admin');

IF NOT EXISTS (SELECT 1 FROM dbo.News WHERE Subject=N'Ưu đãi tháng 10')
INSERT dbo.News(Name,Subject,Description,Status,CreatedDate,[User])
VALUES (N'Tin khuyến mãi',N'Ưu đãi tháng 10',N'Giảm 15% cho đơn đặt tiệc trên 50 khách.',N'Active',GETDATE(),N'admin');
