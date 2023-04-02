docker-compose -f docker-compose.infrastructure.yml down
docker volume rm df_mongo_data
docker volume rm df_postgres_data
docker network rm df-network
