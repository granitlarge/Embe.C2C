#!/usr/bin/env python3

import sqlite3
from shapely import wkb

DB_PATH = r"C:\Users\Miro\Downloads\gadm_410-levels\my_gadm_410-levels.gpkg"

TABLES = (
    "adm_0",
    "adm_1",
    "adm_2",
    "adm_3",
    "adm_4",
    "adm_5",
)

FETCH_BATCH = 5000
WRITE_BATCH = 1000

ENVELOPE_SIZES = (0, 32, 48, 48, 64)

def load_geometry(blob):
    offset = 8 + ENVELOPE_SIZES[(blob[3] >> 1) & 7]
    return wkb.loads(blob[offset:])


def process_table(conn, table):
    new_table = f"{table}_new"

    print(f"\n{table}")

    conn.execute(f'DROP TABLE IF EXISTS "{new_table}"')

    conn.execute(f"""
        CREATE TABLE "{new_table}" (
            fid INTEGER PRIMARY KEY,
            longitude REAL,
            latitude REAL
        )
    """)

    cur = conn.cursor()
    cur.execute(f'SELECT fid, geom FROM "{table}"')

    insert_sql = f'''
        INSERT INTO "{new_table}"
        VALUES (?, ?, ?)
    '''

    rows_done = 0
    inserts = []

    conn.execute("BEGIN")

    while rows := cur.fetchmany(FETCH_BATCH):
        for fid, blob in rows:
            try:
                c = load_geometry(blob).centroid
                inserts.append((fid, c.x, c.y))
            except Exception as e:
                print(f"\nFailed fid={fid}: {e}")

            rows_done += 1

            if len(inserts) >= WRITE_BATCH:
                conn.executemany(insert_sql, inserts)
                inserts.clear()

            if rows_done % 100 == 0:
                print(f"\r{rows_done:,}", end="", flush=True)

    if inserts:
        conn.executemany(insert_sql, inserts)

    conn.commit()

    print(f"\r{rows_done:,}")
    print("Finished.")

def main():
    with sqlite3.connect(DB_PATH) as conn:
        conn.execute("PRAGMA journal_mode=WAL")
        conn.execute("PRAGMA synchronous=NORMAL")

        for table in TABLES:
            process_table(conn, table)

    print("\nAll tables complete.")

if __name__ == "__main__":
    main()