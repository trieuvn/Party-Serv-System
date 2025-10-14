USE PartyServDb; SET NOCOUNT ON;

;WITH P AS (SELECT Id,Name FROM dbo.Party),
     F AS (SELECT Id,Name FROM dbo.Food)
INSERT dbo.PriceHistory(Party,Food,Cost,Amount)
SELECT P.Id, F.Id, X.Cost, X.Amount
FROM (VALUES
 (N'Sinh nhật bé Na 6 tuổi',N'Bánh mì bơ tỏi',15000,50),
 (N'Sinh nhật bé Na 6 tuổi',N'Gà chiên nước mắm',85000,40),
 (N'Sinh nhật bé Na 6 tuổi',N'Bánh kem vani',250000,2),
 (N'Sinh nhật bé Na 6 tuổi',N'Trà sữa trân châu',25000,50),
 (N'Lễ kỷ niệm công ty ABC 10 năm',N'Tôm hấp bia',140000,120),
 (N'Lễ kỷ niệm công ty ABC 10 năm',N'Mực chiên giòn',125000,100),
 (N'Lễ kỷ niệm công ty ABC 10 năm',N'Lẩu thái hải sản',220000,30)
) AS X(PartyName,FoodName,Cost,Amount)
JOIN P ON P.Name=X.PartyName
JOIN F ON F.Name=X.FoodName
WHERE NOT EXISTS (SELECT 1 FROM dbo.PriceHistory ph WHERE ph.Party=P.Id AND ph.Food=F.Id);
