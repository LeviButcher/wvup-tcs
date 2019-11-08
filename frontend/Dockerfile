# React app located in /app/build

FROM node:13-alpine
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . ./
RUN npm run build
RUN npm i -g serve
ENTRYPOINT [ "serve", "-s", "/app/build"]
