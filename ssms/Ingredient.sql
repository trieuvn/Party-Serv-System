USE PartyServDb; SET NOCOUNT ON;

DECLARE @I TABLE(Name nvarchar(50));
INSERT INTO @I(Name) VALUES
(N'Đường'),(N'Bột mì'),(N'Trứng'),(N'Sữa'),(N'Bơ'),
(N'Muối'),(N'Hạt tiêu'),(N'Gà'),(N'Bò'),(N'Gạo'),
(N'Cà chua'),(N'Xà lách'),(N'Hành tây'),(N'Tỏi'),(N'Dầu ăn'),
(N'Tôm'),(N'Mực'),(N'Cá hồi'),(N'Dưa leo'),(N'Cà rốt'),
(N'Khoai tây'),(N'Nấm'),(N'Bánh phở'),(N'Bún'),(N'Bánh mì'),
(N'Dứa'),(N'Xoài'),(N'Rau thơm'),(N'Nước mắm'),(N'Nước tương');

INSERT dbo.Ingredient(Name,Description)
SELECT i.Name, N'Nguyên liệu: ' + i.Name
FROM @I i
WHERE NOT EXISTS (SELECT 1 FROM dbo.Ingredient g WHERE g.Name=i.Name);
