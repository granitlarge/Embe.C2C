DECLARE @json NVARCHAR(MAX);

SELECT @json = BulkColumn
FROM OPENROWSET(
    BULK 'C:\Users\Miro\source\repos\Embe.C2C\data\ADM_202607072036.json',
    SINGLE_NCLOB
) AS j;

INSERT INTO AdminAreas (id, parentid, level, name, type, engtype, Point)
SELECT
    id, parentid, level, name, type, engtype, geography::Point(latitude, longitude, 4326)
FROM OPENJSON(@json, '$.ADM')
WITH (
    Id nvarchar(450) '$.Id',
    ParentId nvarchar(450) '$.ParentId',
    Level int '$.Level',
    Name nvarchar(max) '$.Name',
    Type nvarchar(max) '$.Type',
    EngType nvarchar(max) '$.EngType',
    Longitude float '$.Longitude',
    Latitude float '$.Latitude'
)
