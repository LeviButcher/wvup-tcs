# Stage 1 - the build process
FROM node:10 as build-deps
ARG REACT_APP_BACKEND
WORKDIR /usr/src/app
COPY package.json package-lock.json ./
RUN npm install
COPY . ./

ENV REACT_APP_BACKEND $REACT_APP_BACKEND
RUN REACT_APP_BACKEND=${REACT_APP_BACKEND} npm run build

# Stage 2 - the production environment
FROM nginx:1.12-alpine
COPY --from=build-deps /usr/src/app/build /var/www
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
