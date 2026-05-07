FROM node:20-alpine AS base
WORKDIR /app

FROM node:20-alpine AS build
WORKDIR /app
COPY package.json ./
RUN npm install
COPY . .
RUN npm run build

FROM base AS final
WORKDIR /app
COPY --from=build /app/dist ./dist
EXPOSE 3000
CMD ["npx", "serve", "-l", "3000", "dist"]
