USE PartyServDb; SET NOCOUNT ON;

INSERT dbo.Food(Name,Description,Unit,Cost,Image,Discount)
SELECT v.Name, v.Descp, 1, v.Cost, NULL, v.Discount
FROM (VALUES
 (N'Bánh mì bơ tỏi', N'Bánh mì nướng bơ tỏi',                           15000, NULL),
 (N'Gỏi cuốn tôm thịt', N'Khai vị đặc trưng Nam Bộ',                    25000, NULL),
 (N'Salad rau trộn', N'Xà lách, dưa leo, cà chua sốt dầu giấm',         30000, 10),
 (N'Gà chiên nước mắm', N'Đặc sản Sài Gòn',                             85000, NULL),
 (N'Bò lúc lắc', N'Thịt bò xào rau củ',                                 120000, NULL),
 (N'Tôm hấp bia', N'Tôm thẻ hấp bia Hà Nội',                            140000, NULL),
 (N'Mực chiên giòn', N'Mực tươi chiên xù',                              125000, NULL),
 (N'Cá hồi áp chảo', N'Sốt bơ chanh',                                   180000, NULL),
 (N'Canh chua cá lóc', N'Vị miền Tây',                                  60000, NULL),
 (N'Cơm trắng', N'Gạo ST25',                                            10000, NULL),
 (N'Bún bò Huế', N'Nước dùng đậm đà',                                   55000, NULL),
 (N'Phở bò tái', N'Bánh phở truyền thống',                              60000, NULL),
 (N'Bánh flan', N'Món tráng miệng',                                     15000, 5),
 (N'Trà trái cây', N'Trà gạo rang, trái cây theo mùa',                   12000, NULL),
 (N'Lẩu thái hải sản', N'Chua cay, tôm mực đầy đủ',                      220000, NULL),
 (N'Bánh kem vani', N'Bánh sinh nhật kem vani',                          250000, NULL),
 (N'Súp bí đỏ', N'Khai vị, kem tươi',                                    45000, NULL),
 (N'Khoai tây nghiền', N'Ăn kèm bò/ cá hồi',                              30000, NULL),
 (N'Rau củ hấp', N'Cà rốt, bông cải xanh, đậu cô ve',                    35000, NULL),
 (N'Trà sữa trân châu', N'Đồ uống yêu thích giới trẻ',                    25000, NULL)
) AS v(Name,Descp,Cost,Discount)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Food f WHERE f.Name=v.Name);
