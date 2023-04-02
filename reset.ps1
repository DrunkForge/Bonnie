docker stop postgres
docker rm postgres
docker volume rm bonnie_postgres_data

# docker stop mongo
# docker rm mongo
# docker volume rm bonnie_mongo_data

docker-compose -f docker-compose.infrastructure.yml up -d postgres
