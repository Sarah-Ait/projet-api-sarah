# syntax=docker/dockerfile:1

# ---- Build stage ----
FROM node:20-alpine AS build
WORKDIR /app

ARG API_URL=http://localhost:5065

COPY package*.json ./
RUN npm ci

COPY . ./
RUN sed -i "s|http://localhost:5065|${API_URL}|g" src/app/core/api.config.ts
RUN npm run build -- --configuration=production

# ---- Runtime stage ----
FROM nginx:alpine AS runtime

COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html

EXPOSE 80
