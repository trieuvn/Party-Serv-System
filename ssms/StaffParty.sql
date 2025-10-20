USE PartyServDb; SET NOCOUNT ON;

;WITH P AS (SELECT Id,Name FROM dbo.Party)
INSERT dbo.StaffParty(Staff,Party)
SELECT s.Staff, p.Id
FROM (VALUES
 (N'staff.hcm', N'Sinh nhật bé Na 6 tuổi'),
 (N'staff.hcm', N'Lễ kỷ niệm công ty ABC 10 năm'),
 (N'staff.hn',  N'Đám cưới Hà & Long'),
 (N'staff.hcm', N'Tất niên Phòng Kinh doanh'),
 (N'staff.hn',  N'Liên hoan lớp 12A1')
) AS s(Staff,PartyName)
JOIN P ON P.Name=s.PartyName
WHERE NOT EXISTS (SELECT 1 FROM dbo.StaffParty sp WHERE sp.Staff=s.Staff AND sp.Party=P.Id);
