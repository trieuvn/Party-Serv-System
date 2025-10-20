USE PartyServDb; SET NOCOUNT ON;

INSERT dbo.Menu(Name,Description,Cost,Status,Image,Discount)
SELECT m.Name, m.Descp, m.Cost, N'Active', NULL, m.Discount
FROM (VALUES
 (N'Cơ bản miền Nam', N'Gỏi cuốn, Gà chiên, Canh chua, Cơm, Bánh flan',  220000, NULL),
 (N'Gia đình',        N'Salad, Bò lúc lắc, Cơm trắng, Trà trái cây',     180000, 10),
 (N'Hải sản',         N'Tôm hấp bia, Mực chiên giòn, Lẩu thái hải sản',  350000, NULL),
 (N'Sang trọng',      N'Súp bí đỏ, Cá hồi áp chảo, Khoai tây nghiền',    420000, NULL),
 (N'Chay thanh đạm',  N'Salad, Rau củ hấp, Cơm trắng, Trà trái cây',     160000, NULL),
 (N'Sinh nhật Kids',  N'Bánh mì bơ tỏi, Gà chiên, Bánh kem, Trà sữa',    200000, 5),
 (N'Bún/Phở ấm bụng', N'Phở bò, Bún bò, Rau củ hấp, Trà đá',              150000, NULL),
 (N'Vegetarian Plus', N'Súp bí đỏ, Salad, Rau củ hấp, Bánh flan',         210000, NULL)
) AS m(Name,Descp,Cost,Discount)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Menu x WHERE x.Name=m.Name);
