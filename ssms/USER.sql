USE PartyServDb; SET NOCOUNT ON;

-- Admin & Staff (TPH trong bảng User)
IF NOT EXISTS (SELECT 1 FROM dbo.[User] WHERE Username='admin')
INSERT dbo.[User](Username,Password,FirstName,LastName,Email,PhoneNumber,Role,Avatar,Discriminator,Salary)
VALUES (N'admin',N'123456',N'Triệu',N'Nguyễn',N'admin@party.vn',N'0909000000',N'Admin',NULL,N'Staff',15000000);

IF NOT EXISTS (SELECT 1 FROM dbo.[User] WHERE Username='staff.hcm')
INSERT dbo.[User] VALUES (N'staff.hcm',N'123456',N'Linh',N'Phạm',N'staff.hcm@party.vn',N'0909000001',N'Staff',NULL,N'Staff',12000000);
IF NOT EXISTS (SELECT 1 FROM dbo.[User] WHERE Username='staff.hn')
INSERT dbo.[User] VALUES (N'staff.hn',N'123456',N'Nam',N'Ngô',N'staff.hn@party.vn',N'0909000002',N'Staff',NULL,N'Staff',11000000);
IF NOT EXISTS (SELECT 1 FROM dbo.[User] WHERE Username='staff.dn')
INSERT dbo.[User] VALUES (N'staff.dn',N'123456',N'Hương',N'Nguyễn',N'staff.dn@party.vn',N'0909000003',N'Staff',NULL,N'Staff',10000000);

-- Khách hàng
IF NOT EXISTS (SELECT 1 FROM dbo.[User] WHERE Username='user.tuan')
INSERT dbo.[User](Username,Password,FirstName,LastName,Email,PhoneNumber,Role,Avatar,Discriminator,Salary)
VALUES (N'user.tuan',N'123456',N'Tuấn',N'Phạm',N'tuan@party.vn',N'0901234567',N'User',NULL,N'User',NULL);
IF NOT EXISTS (SELECT 1 FROM dbo.[User] WHERE Username='user.mai')
INSERT dbo.[User] VALUES (N'user.mai',N'123456',N'Mai',N'Đỗ',N'mai@party.vn',N'0902345678',N'User',NULL,N'User',NULL);
IF NOT EXISTS (SELECT 1 FROM dbo.[User] WHERE Username='user.thao')
INSERT dbo.[User] VALUES (N'user.thao',N'123456',N'Thảo',N'Lê',N'thao@party.vn',N'0903456789',N'User',NULL,N'User',NULL);
