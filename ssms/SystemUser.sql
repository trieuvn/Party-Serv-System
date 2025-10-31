USE PartyServDb; SET NOCOUNT ON;

-- Admin & Staff (TPH trong bảng User)
-- [ĐÃ SỬA] Đổi [User] thành [SystemUser]
IF NOT EXISTS (SELECT 1 FROM dbo.[SystemUser] WHERE Username='admin')
INSERT dbo.[SystemUser](Username,Password,FirstName,LastName,Email,PhoneNumber,Role,Avatar,Salary)
VALUES (N'admin',N'123456',N'Triệu',N'Nguyễn',N'admin@party.vn',N'0909000000',N'Admin',NULL,15000000);

-- [ĐÃ SỬA] Đổi [User] thành [SystemUser]
IF NOT EXISTS (SELECT 1 FROM dbo.[SystemUser] WHERE Username='staff.hcm')
INSERT dbo.[SystemUser] VALUES (N'staff.hcm',N'123456',N'Linh',N'Phạm',N'staff.hcm@party.vn',N'0909000001',N'Staff',NULL,12000000);
-- [ĐÃ SỬA] Đổi [User] thành [SystemUser]
IF NOT EXISTS (SELECT 1 FROM dbo.[SystemUser] WHERE Username='staff.hn')
INSERT dbo.[SystemUser] VALUES (N'staff.hn',N'123456',N'Nam',N'Ngô',N'staff.hn@party.vn',N'0909000002',N'Staff',NULL,11000000);
-- [ĐÃ SỬA] Đổi [User] thành [SystemUser]
IF NOT EXISTS (SELECT 1 FROM dbo.[SystemUser] WHERE Username='staff.dn')
INSERT dbo.[SystemUser] VALUES (N'staff.dn',N'123456',N'Hương',N'Nguyễn',N'staff.dn@party.vn',N'0909000003',N'Staff',NULL,10000000);

-- Khách hàng
-- [ĐÃ SỬA] Đổi [User] thành [SystemUser]
IF NOT EXISTS (SELECT 1 FROM dbo.[SystemUser] WHERE Username='user.tuan')
INSERT dbo.[SystemUser](Username,Password,FirstName,LastName,Email,PhoneNumber,Role,Avatar,Salary)
VALUES (N'user.tuan',N'123456',N'Tuấn',N'Phạm',N'tuan@party.vn',N'0901234567',N'User',NULL,NULL);
-- [ĐÃ SỬA] Đổi [User] thành [SystemUser]
IF NOT EXISTS (SELECT 1 FROM dbo.[SystemUser] WHERE Username='user.mai')
INSERT dbo.[SystemUser] VALUES (N'user.mai',N'123456',N'Mai',N'Đỗ',N'mai@party.vn',N'0902345678',N'User',NULL,NULL);
-- [ĐÃ SỬA] Đổi [User] thành [SystemUser]
IF NOT EXISTS (SELECT 1 FROM dbo.[SystemUser] WHERE Username='user.thao')
INSERT dbo.[SystemUser] VALUES (N'user.thao',N'123456',N'Thảo',N'Lê',N'thao@party.vn',N'0903456789',N'User',NULL,NULL);