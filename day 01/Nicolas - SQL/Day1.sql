DECLARE @INPUT VARCHAR(MAX) = '<insert input here>'

DECLARE 
  @olddelim nvarchar(32) = char(13) + char(10) + char(13) + char(10), 
  @olddelim2 nvarchar(32) = char(13) + char(10), 
  @newdelim nchar(1)     = NCHAR(9999); -- pencil (✏)

----
SELECT  TOP 1 x.value, SUM(y.Calorie)
FROM STRING_SPLIT(replace(@input, @olddelim, @newdelim) , @newdelim) x
	CROSS APPLy (SELECT cast(z.Value as int) Calorie 
				 FROM STRING_SPLIT(REPLACE(x.value, @olddelim2, @newdelim) , @newdelim) z
				) Y
GROUP BY x.value
ORDER BY  SUM(y.Calorie) DESC

SELECT SUM(TotaalElf)
FROM (
	SELECT  top 3 x.value, SUM(y.Calorie) TotaalElf
	FROM STRING_SPLIT(replace(@input, @olddelim, @newdelim) , @newdelim) x
		CROSS APPLy (SELECT cast(z.Value as int) Calorie 
					 FROM STRING_SPLIT(replace(x.value, @olddelim2, @newdelim) , @newdelim) z
					) Y
GROUP BY x.value
ORDER BY  SUM(y.Calorie) desc) x