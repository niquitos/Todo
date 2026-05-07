FROM node:20-alpine AS base
WORKDIR /app

FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM base AS final
WORKDIR /app
COPY package*.json ./
COPY --from=build /app/dist ./dist
EXPOSE 3000
CMD ["npm", "run", "preview", "--", "--host", "0.0.0.0", "--port", "3000"]
