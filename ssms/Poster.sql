USE PartyServDb; SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Poster WHERE Name=N'Tết 2025')
INSERT dbo.Poster(Name,Image) VALUES (N'Tết 2025',N'/content/posters/tet-2025.jpg');

IF NOT EXISTS (SELECT 1 FROM dbo.Poster WHERE Name=N'Summer Seafood')
INSERT dbo.Poster(Name,Image) VALUES (N'Summer Seafood',N'/content/posters/seafood-summer.jpg');
