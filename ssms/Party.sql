USE PartyServDb; SET NOCOUNT ON;

;WITH M AS (SELECT Id,Name FROM dbo.Menu)
INSERT dbo.Party
(Name,Image,Type,Status,Cost,BeginTime,EndTime,CreatedDate,Description,Slots,Address,Latitude,Longitude,[User],Menu)
SELECT p.Name,NULL,p.Type,p.Status,p.TotalCost,
       p.BeginTime,p.EndTime,GETDATE(),p.Descp,p.Slots,p.Addr,NULL,NULL,p.[User],M.Id
FROM (VALUES
 (N'Sinh nhật bé Na 6 tuổi',N'Private', N'Open',      200000*50,  DATEADD(DAY,7,GETDATE()),  DATEADD(DAY,7,DATEADD(HOUR,3,GETDATE())),  N'Trọn gói sinh nhật cho bé', 50, N'Vinhomes Grand Park, TP.Thủ Đức', N'user.mai',  N'Sinh nhật Kids'),
 (N'Lễ kỷ niệm công ty ABC 10 năm',N'Corporate',N'Open', 350000*120, DATEADD(DAY,14,GETDATE()), DATEADD(DAY,14,DATEADD(HOUR,4,GETDATE())), N'Sân khấu, âm thanh, backdrop', 120, N'Nhà Văn hóa Thanh Niên, Q.1', N'admin', N'Hải sản'),
 (N'Tất niên Phòng Kinh doanh',N'Corporate',N'Open', 220000*60, DATEADD(DAY,40,GETDATE()), DATEADD(DAY,40,DATEADD(HOUR,3,GETDATE())),  N'Tổng kết doanh số', 60, N'Q.7, TP.HCM', N'user.tuan', N'Cơ bản miền Nam'),
 (N'Đám cưới Hà & Long', N'Ceremony', N'Planning', 420000*180, DATEADD(DAY,60,GETDATE()), DATEADD(DAY,60,DATEADD(HOUR,5,GETDATE())),   N'Tiệc cưới cao cấp', 180, N'Trống Đồng, Hoàn Kiếm', N'user.thao', N'Sang trọng'),
 (N'Liên hoan lớp 12A1', N'Private', N'Open', 150000*45, DATEADD(DAY,10,GETDATE()), DATEADD(DAY,10,DATEADD(HOUR,3,GETDATE())),          N'Gặp mặt cuối năm', 45, N'Cầu Giấy, Hà Nội', N'user.mai', N'Bún/Phở ấm bụng')
) AS p(Name,Type,Status,TotalCost,BeginTime,EndTime,Descp,Slots,Addr,[User],MenuName)
JOIN M ON M.Name=p.MenuName
WHERE NOT EXISTS (SELECT 1 FROM dbo.Party x WHERE x.Name=p.Name);
