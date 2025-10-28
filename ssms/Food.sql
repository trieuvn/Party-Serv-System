USE PartyServDb;
SET NOCOUNT ON;

;WITH C AS (SELECT Id, Name FROM dbo.Category)
INSERT dbo.Food(Name,Description,Unit,Cost,Image,Discount,CategoryId)
SELECT v.Name, v.Descp, 1, v.Cost, NULL, v.Discount,
       (SELECT Id FROM C WHERE Name=v.CateName)
FROM (VALUES
 -- Khai vị
 (N'Bánh mì bơ tỏi',    N'Bánh mì nướng bơ tỏi',                           15000, NULL, N'Món khai vị'),
 (N'Gỏi cuốn tôm thịt', N'Khai vị đặc trưng Nam Bộ',                        25000, NULL, N'Món khai vị'),
 (N'Salad rau trộn',    N'Xà lách, dưa leo, cà chua sốt dầu giấm',          30000, 10,   N'Món khai vị'),
 -- Món chính
 (N'Gà chiên nước mắm', N'Đặc sản Sài Gòn',                                 85000, NULL, N'Món chính'),
 (N'Bò lúc lắc',        N'Thịt bò xào rau củ',                              120000, NULL, N'Món chính'),
 (N'Tôm hấp bia',       N'Tôm thẻ hấp bia',                                 140000, NULL, N'Món chính'),
 (N'Mực chiên giòn',    N'Mực tươi chiên xù',                               125000, NULL, N'Món chính'),
 (N'Cá hồi áp chảo',    N'Sốt bơ chanh',                                    180000, NULL, N'Món chính'),
 -- Canh/Lẩu
 (N'Canh chua cá lóc',  N'Vị miền Tây',                                     60000, NULL, N'Canh/Lẩu'),
 (N'Lẩu thái hải sản',  N'Chua cay, tôm mực đầy đủ',                        220000, NULL, N'Canh/Lẩu'),
 -- Khác
 (N'Cơm trắng',         N'Gạo ST25',                                        10000, NULL, N'Món chính'),
 (N'Khoai tây nghiền',  N'Ăn kèm bò/ cá hồi',                               30000, NULL, N'Món chính'),
 (N'Rau củ hấp',        N'Cà rốt, bông cải xanh, đậu cô ve',                35000, NULL, N'Món chính'),
 (N'Phở bò tái',        N'Bánh phở truyền thống',                           60000, NULL, N'Món chính'),
 (N'Bún bò Huế',        N'Nước dùng đậm đà',                                55000, NULL, N'Món chính'),
 -- Tráng miệng & Đồ uống
 (N'Bánh flan',         N'Món tráng miệng',                                 15000, 5,    N'Tráng miệng'),
 (N'Bánh kem vani',     N'Bánh sinh nhật kem vani',                         250000, NULL, N'Tráng miệng'),
 (N'Trà trái cây',      N'Trà gạo rang, trái cây theo mùa',                 12000, NULL, N'Đồ uống'),
 (N'Trà sữa trân châu', N'Đồ uống yêu thích giới trẻ',                      25000, NULL, N'Đồ uống')
) AS v(Name,Descp,Cost,Discount,CateName)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Food f WHERE f.Name=v.Name);
