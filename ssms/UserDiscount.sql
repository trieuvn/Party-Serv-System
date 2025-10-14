USE PartyServDb; SET NOCOUNT ON;

INSERT dbo.UserDiscount([User],Discount,Amount)
SELECT 'user.tuan', d.Id, 1
FROM dbo.Discount d
WHERE d.CouponCode=N'VIP20'
  AND NOT EXISTS (SELECT 1 FROM dbo.UserDiscount ud WHERE ud.[User]='user.tuan' AND ud.Discount=d.Id);

INSERT dbo.UserDiscount([User],Discount,Amount)
SELECT 'user.mai', d.Id, 2
FROM dbo.Discount d
WHERE d.CouponCode=N'TIEC15'
  AND NOT EXISTS (SELECT 1 FROM dbo.UserDiscount ud WHERE ud.[User]='user.mai' AND ud.Discount=d.Id);
