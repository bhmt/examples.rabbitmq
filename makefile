.PHONY: consumer
consumer:
	dotnet run --no-cache --project src/bhmt.mq.consumer --urls "http://*:5100;https://*:5101"

.PHONY: producer
producer:
	dotnet run --no-cache --project src/bhmt.mq.producer --urls "http://*:5200;https://*:5201"

.PHONY: up
up:
	docker-compose up -d