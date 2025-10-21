# Build stage
FROM node:22-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./
RUN npm ci

# Copy source code
COPY . .

# Build the application (override vite config to output to dist)
RUN npx vite build --outDir /app/dist

# Production stage
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Copy built files
COPY --from=build /app/dist .

# Copy nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]

