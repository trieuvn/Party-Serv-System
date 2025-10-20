USE PartyServDb; SET NOCOUNT ON;

;WITH F AS (SELECT Id,Name FROM dbo.Food),
     G AS (SELECT Id,Name FROM dbo.Ingredient)
INSERT dbo.FoodIngredient(Food,Ingredient,Amount)
SELECT F.Id, G.Id, X.Amount
FROM (VALUES
 (N'Bánh mì bơ tỏi',N'Bánh mì',1),(N'Bánh mì bơ tỏi',N'Bơ',1),(N'Bánh mì bơ tỏi',N'Tỏi',1),
 (N'Gỏi cuốn tôm thịt',N'Tôm',1),(N'Gỏi cuốn tôm thịt',N'Bánh phở',1),(N'Gỏi cuốn tôm thịt',N'Rau thơm',1),
 (N'Salad rau trộn',N'Xà lách',2),(N'Salad rau trộn',N'Dưa leo',1),(N'Salad rau trộn',N'Cà chua',1),
 (N'Gà chiên nước mắm',N'Gà',1),(N'Gà chiên nước mắm',N'Dầu ăn',1),(N'Gà chiên nước mắm',N'Nước mắm',1),
 (N'Bò lúc lắc',N'Bò',2),(N'Bò lúc lắc',N'Hành tây',1),(N'Bò lúc lắc',N'Hạt tiêu',1),
 (N'Tôm hấp bia',N'Tôm',2),(N'Tôm hấp bia',N'Muối',1),
 (N'Mực chiên giòn',N'Mực',2),(N'Mực chiên giòn',N'Dầu ăn',1),
 (N'Cá hồi áp chảo',N'Cá hồi',1),(N'Cá hồi áp chảo',N'Bơ',1),(N'Cá hồi áp chảo',N'Hạt tiêu',1),
 (N'Canh chua cá lóc',N'Cà chua',2),(N'Canh chua cá lóc',N'Dứa',1),(N'Canh chua cá lóc',N'Nước mắm',1),
 (N'Cơm trắng',N'Gạo',1),
 (N'Phở bò tái',N'Bánh phở',1),(N'Phở bò tái',N'Bò',1),
 (N'Bún bò Huế',N'Bún',1),(N'Bún bò Huế',N'Bò',1),
 (N'Bánh flan',N'Sữa',1),(N'Bánh flan',N'Trứng',2),(N'Bánh flan',N'Đường',1),
 (N'Trà trái cây',N'Xoài',1),(N'Trà trái cây',N'Dứa',1),
 (N'Trà sữa trân châu',N'Sữa',1),(N'Trà sữa trân châu',N'Đường',1)
) AS X(FoodName,IngName,Amount)
JOIN F ON F.Name=X.FoodName
JOIN G ON G.Name=X.IngName
WHERE NOT EXISTS (SELECT 1 FROM dbo.FoodIngredient fi WHERE fi.Food=F.Id AND fi.Ingredient=G.Id);
