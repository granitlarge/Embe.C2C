INSERT INTO "AdminAreas" (
    "Id",
    "ParentId",
    "Level",
    "Name",
    "Type",
    "EngType",
    "Point"
)
SELECT
    j->>'Id',
    j->>'ParentId',
    (j->>'Level')::int,
    j->>'Name',
    j->>'Type',
    j->>'EngType',
    ST_SetSRID(
        ST_MakePoint(
            (j->>'Longitude')::double precision,
            (j->>'Latitude')::double precision
        ),
        4326
    )::geography
FROM jsonb_array_elements(
    (pg_read_file('C:/Users/Miro/source/repos/Embe.C2C/data/ADM_202607072036.json')::jsonb)->'ADM'
) AS j;
