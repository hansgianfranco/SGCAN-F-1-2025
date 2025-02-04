FROM node:18 AS build
WORKDIR /app
RUN npm install -g pnpm
COPY package.json pnpm-lock.yaml ./
RUN pnpm install
COPY . .
RUN pnpm build

FROM node:18 AS runtime
WORKDIR /app
RUN npm install -g pnpm
COPY --from=build /app .
EXPOSE 3000

CMD ["pnpm", "dev"]