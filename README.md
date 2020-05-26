# poc-distributed-tracing

<img src="/img/poc_tracing.png">
<br>

### Set the environment variables
It's necessary configure some properties in the `appsettings.json` file. An example is shown bellow:

```json
"JAEGER_SERVICE_NAME": "order-api",
"JAEGER_AGENT_HOST": "localhost",
"JAEGER_AGENT_PORT": "6831",
"JAEGER_SAMPLER_TYPE": "const"
```

It's necessary configure in `Order.API` properties in the `appsettings.json` file.

```json
"ENDPOINT_PAYMENT_API": "http://localhost:61966/"
```

### Usage

```bash
docker run -d -p6831:6831/udp -p16686:16686 jaegertracing/all-in-one:latest
```

```bash
cd src/Order.API/
dotnet run
```

```bash
cd src/Payment.API/
dotnet run
```

```bash
cd src/Delivery.BackgroundTasks/
dotnet run
```

### Self-Test

```bash
curl --location --request POST 'http://localhost:5000/order' \
--header 'Content-Type: application/json' \
--data-raw '{
	"Id": "09bf4884-3009-43a2-97be-e2d857ea5844",
	"Description": "Ordem AA",
	"Amount": 100
}'
```

<img src="/img/jaeger.png">
<br>
