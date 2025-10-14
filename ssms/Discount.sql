USE PartyServDb; SET NOCOUNT ON;

INSERT dbo.Discount(Value,CreatedDate,ExpiredDate,CouponCode,IsValid)
SELECT v.Value, GETDATE(), DATEADD(MONTH,v.Months,GETDATE()), v.Code, 1
FROM (VALUES
 (10,3,N'CHAO2025'), (15,2,N'TIEC15'), (20,1,N'VIP20'), (5,12,N'SINHNHAT5')
) AS v(Value,Months,Code)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Discount d WHERE d.CouponCode=v.Code);
