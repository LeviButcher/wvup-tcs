# React app located in /app/build

FROM node:13-alpine
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . ./
RUN chmod 755 startScript
ENTRYPOINT [ "/app/startScript" ]
