USE PartyServDb; SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Partner WHERE Name=N'Riverside Palace')
INSERT dbo.Partner(Name,Subject,Description)
VALUES (N'Riverside Palace',N'Đối tác địa điểm',N'Trung tâm tiệc cưới – hội nghị tại Q.4, TP.HCM');

IF NOT EXISTS (SELECT 1 FROM dbo.Partner WHERE Name=N'Trống Đồng Palace')
INSERT dbo.Partner(Name,Subject,Description)
VALUES (N'Trống Đồng Palace',N'Đối tác địa điểm',N'Hệ thống trung tâm sự kiện tại Hà Nội');
