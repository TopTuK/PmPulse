#!/bin/bash
set -e

# Get the password from environment variable, default to "orleans"
ORLEANS_PASSWORD="${POSTGRES_PASSWORD:-orleans}"

# Use default postgres user and database if not set
POSTGRES_USER="${POSTGRES_USER:-postgres}"
POSTGRES_DB="${POSTGRES_DB:-$POSTGRES_USER}"

# Set the postgres user password explicitly (for Aspire health checks)
# This ensures the postgres user has the password set even if POSTGRES_PASSWORD wasn't used during init
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    -- Set password for postgres user if it exists (required for Aspire health checks)
    DO \$\$
    BEGIN
        IF EXISTS (SELECT FROM pg_catalog.pg_user WHERE usename = 'postgres') THEN
            ALTER USER postgres WITH PASSWORD '$ORLEANS_PASSWORD';
        END IF;
    END
    \$\$;
    
    -- Create the orleans user if it doesn't exist
    DO \$\$
    BEGIN
        IF NOT EXISTS (SELECT FROM pg_catalog.pg_user WHERE usename = 'orleans') THEN
            CREATE USER orleans WITH PASSWORD '$ORLEANS_PASSWORD';
        ELSE
            -- Update password if user exists
            ALTER USER orleans WITH PASSWORD '$ORLEANS_PASSWORD';
        END IF;
    END
    \$\$;
EOSQL

# Create the orleans database (must be done outside of a transaction)
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -tc "SELECT 1 FROM pg_database WHERE datname = 'orleans'" | grep -q 1 || \
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -c "CREATE DATABASE orleans OWNER orleans;"

# Grant privileges
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "orleans" <<-EOSQL
    GRANT ALL PRIVILEGES ON DATABASE orleans TO orleans;
EOSQL

# Note: 02-postgresql-main.sql and 03-postgresql-clustering.sql will be executed by 02-postgresql-main.sh
# which runs after this script completes

