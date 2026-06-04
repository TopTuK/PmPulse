#!/bin/bash
set -e

# Get the password from environment variable, default to "orleans"
ORLEANS_PASSWORD="${POSTGRES_PASSWORD:-orleans}"

# Use default postgres user and database if not set
POSTGRES_USER="${POSTGRES_USER:-postgres}"
POSTGRES_DB="${POSTGRES_DB:-$POSTGRES_USER}"

# Execute Orleans SQL from sql/ (not the initdb root — root *.sql would run twice via entrypoint)
SCRIPT_DIR="$(dirname "$0")"
SQL_DIR="$SCRIPT_DIR/sql"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "orleans" -f "$SQL_DIR/02-postgresql-main.sql"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "orleans" -f "$SQL_DIR/03-postgresql-clustering.sql"

# Grant all privileges on OrleansQuery table to orleans user
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "orleans" <<-EOSQL
    -- Grant all privileges on OrleansQuery table to orleans user
    GRANT ALL PRIVILEGES ON TABLE OrleansQuery TO orleans;
    
    -- Also grant usage on the schema (public schema)
    GRANT USAGE ON SCHEMA public TO orleans;
    GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO orleans;
    GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO orleans;
    
    -- Set default privileges for future tables
    ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO orleans;
    ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO orleans;
EOSQL

