#!/bin/bash
set -e

# Get the password from environment variable, default to "orleans"
ORLEANS_PASSWORD="${POSTGRES_PASSWORD:-orleans}"

# Use default postgres user and database if not set
POSTGRES_USER="${POSTGRES_USER:-postgres}"
POSTGRES_DB="${POSTGRES_DB:-$POSTGRES_USER}"

# Execute PostgreSQL-Main.sql against the orleans database
# This script runs after the orleans database is created by 01-init-orleans.sh
# The SQL file is in the same directory as this script
SCRIPT_DIR="$(dirname "$0")"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "orleans" -f "$SCRIPT_DIR/postgresql-main.sql"

# Execute PostgreSQL-Clustering.sql after PostgreSQL-Main.sql
# The SQL file is in the same directory as this script
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "orleans" -f "$SCRIPT_DIR/postgresql-clustering.sql"

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

