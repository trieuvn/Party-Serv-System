USE PartyServDb; SET NOCOUNT ON;

;WITH M AS (SELECT Id,Name FROM dbo.Menu),
     F AS (SELECT Id,Name FROM dbo.Food)
INSERT dbo.MenuDetail(Menu,Food,Amount)
SELECT M.Id, F.Id, X.Amount
FROM (VALUES
 (N'Cơ bản miền Nam',N'Gỏi cuốn tôm thịt',10),(N'Cơ bản miền Nam',N'Gà chiên nước mắm',8),
 (N'Cơ bản miền Nam',N'Canh chua cá lóc',8),(N'Cơ bản miền Nam',N'Cơm trắng',20),(N'Cơ bản miền Nam',N'Bánh flan',20),
 (N'Gia đình',N'Salad rau trộn',10),(N'Gia đình',N'Bò lúc lắc',8),(N'Gia đình',N'Cơm trắng',20),(N'Gia đình',N'Trà trái cây',20),
 (N'Hải sản',N'Tôm hấp bia',12),(N'Hải sản',N'Mực chiên giòn',10),(N'Hải sản',N'Lẩu thái hải sản',5),
 (N'Sang trọng',N'Súp bí đỏ',15),(N'Sang trọng',N'Cá hồi áp chảo',10),(N'Sang trọng',N'Khoai tây nghiền',10),
 (N'Chay thanh đạm',N'Salad rau trộn',15),(N'Chay thanh đạm',N'Rau củ hấp',15),(N'Chay thanh đạm',N'Cơm trắng',20),(N'Chay thanh đạm',N'Trà trái cây',20),
 (N'Sinh nhật Kids',N'Bánh mì bơ tỏi',20),(N'Sinh nhật Kids',N'Gà chiên nước mắm',10),(N'Sinh nhật Kids',N'Bánh kem vani',2),(N'Sinh nhật Kids',N'Trà sữa trân châu',20),
 (N'Bún/Phở ấm bụng',N'Phở bò tái',10),(N'Bún/Phở ấm bụng',N'Bún bò Huế',10),(N'Bún/Phở ấm bụng',N'Rau củ hấp',10),
 (N'Vegetarian Plus',N'Súp bí đỏ',12),(N'Vegetarian Plus',N'Salad rau trộn',12),(N'Vegetarian Plus',N'Rau củ hấp',12),(N'Vegetarian Plus',N'Bánh flan',12)
) AS X(MenuName,FoodName,Amount)
JOIN M ON M.Name=X.MenuName
JOIN F ON F.Name=X.FoodName
WHERE NOT EXISTS (SELECT 1 FROM dbo.MenuDetail d WHERE d.Menu=M.Id AND d.Food=F.Id);
