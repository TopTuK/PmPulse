#!/bin/bash
# Emulate the deploy-prod GitHub Actions job locally.
# Builds images, tags them like CI, and runs docker-compose.production.yml.

set -euo pipefail

ROOT="$(cd "$(dirname "$0")" && pwd)"
cd "$ROOT"

IMAGE_TAG="${IMAGE_TAG:-$(git rev-parse HEAD)}"

echo "==> Building images (build-images job)"
chmod +x ./build-images.sh
./build-images.sh

echo "==> Tagging images for ghcr.io (deploy uses IMAGE_TAG=$IMAGE_TAG)"
docker tag pmpulse-silohost:latest "ghcr.io/toptuk/pmpulse-silohost:${IMAGE_TAG}"
docker tag pmpulse-webapi:latest "ghcr.io/toptuk/pmpulse-webapi:${IMAGE_TAG}"

echo "==> Preparing .env (override with your secrets or export POSTGRES_PASSWORD, SENTRY_DSN, SILO_SENTRY_DSN)"
cat > .env <<EOF
POSTGRES_PASSWORD=${POSTGRES_PASSWORD:-orleans}
SENTRY_DSN=${SENTRY_DSN:-}
SILO_SENTRY_DSN=${SILO_SENTRY_DSN:-}
IMAGE_TAG=${IMAGE_TAG}
EOF

echo "==> Ensuring external network proxy exists (used by reverse proxy in production)"
docker network inspect proxy >/dev/null 2>&1 || docker network create proxy

echo "==> Ensuring TLS certs exist for Kestrel HTTPS"
mkdir -p certs
if [[ ! -f certs/pmi.moscow.crt ]]; then
  openssl req -x509 -newkey rsa:2048 \
    -keyout certs/pmi.moscow.key -out certs/pmi.moscow.crt \
    -days 365 -nodes -subj "/CN=localhost"
fi

echo "==> Stopping existing stack (same as deploy-prod)"
docker compose -f docker-compose.production.yml down 2>/dev/null || true

echo "==> Starting production compose (pull skipped — using locally built images)"
docker compose -f docker-compose.production.yml up -d
docker image prune -f

echo ""
echo "Production stack is up. Check status:"
docker compose -f docker-compose.production.yml ps
echo ""
echo "HTTP:  http://localhost:8080"
echo "HTTPS: https://localhost:8443"
echo "Logs:  docker compose -f docker-compose.production.yml logs -f"
