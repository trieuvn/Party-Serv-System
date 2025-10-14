USE PartyServDb; SET NOCOUNT ON;

;WITH I AS (SELECT Id,Name FROM dbo.Ingredient)
INSERT dbo.Provider(Ingredient,Name,Description,PhoneNumber,Address,Latitude,Longitude,Cost)
SELECT I.Id, P.Name, P.Descp, P.Phone, P.Addr, NULL, NULL, P.Cost
FROM (VALUES
 (N'Gà',      N'Thực Phẩm An Khang', N'Gà thả vườn',             N'0938000001', N'Quận 9, TP.HCM',  80000),
 (N'Bò',      N'Vissan Q.8',         N'Thịt bò mát',              N'02838500002',N'Quận 8, TP.HCM', 180000),
 (N'Sữa',     N'Vinamilk',           N'Sữa tươi thanh trùng',     N'02839100003',N'Bình Thạnh',     28000),
 (N'Bột mì',  N'Interflour',         N'Bột mì số 11',             N'02837700004',N'Quận 7',         18000),
 (N'Đường',   N'Biên Hòa Sugar',     N'Đường tinh luyện',         N'02513600005',N'Biên Hòa',       17000),
 (N'Cà chua', N'Nông Sản Văn Thánh', N'Cà chua Đà Lạt',           N'0909000006', N'Bình Thạnh',     25000),
 (N'Xà lách', N'Rau Sạch Củ Chi',    N'Hydroponic',               N'0909000007', N'Củ Chi',         22000),
 (N'Tôm',     N'Hải sản Phan Thiết', N'Tôm thẻ đông lạnh',        N'0909000008', N'Thủ Đức',        180000),
 (N'Mực',     N'Hải sản Vũng Tàu',   N'Mực ống',                  N'0909000009', N'Q.4',            160000),
 (N'Cá hồi',  N'Hải sản Hạ Long',    N'Phi lê cá hồi Nauy',       N'02439990010',N'Cầu Giấy, HN',   420000),
 (N'Gạo',     N'Gạo ST25 Minh Long', N'Gạo thơm ST25',            N'0909000011', N'Tân Bình',       22000)
) AS P(IngName,Name,Descp,Phone,Addr,Cost)
JOIN I ON I.Name=P.IngName
WHERE NOT EXISTS(SELECT 1 FROM dbo.Provider x WHERE x.Name=P.Name AND x.Ingredient=I.Id);
