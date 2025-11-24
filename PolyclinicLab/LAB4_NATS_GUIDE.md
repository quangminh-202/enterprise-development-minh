# Lab 4: NATS JetStream Implementation

## Tổng quan kiến trúc

```
Generator Service → NATS JetStream → API Host → MongoDB
```

### Các thành phần:

1. **Polyclinic.Generator**: Shared library
   - `IProducerService`: Interface cho message producers
   - `GeneratorService`: Background service sinh contracts với Bogus
   - Không có reference đến server projects

2. **Polyclinic.Infrastructure.Nats**: Thư viện NATS JetStream
   - `PolyclinicNatsConsumer`: Consumer với JetStream push model
   - `PolyclinicPayloadDeserializer`: Custom deserializer
   - Retry logic tích hợp trong JetStream

3. **Polyclinic.Generator.Nats.Host**: Generator service host
   - `PolyclinicNatsProducer`: Implementation của IProducerService
   - Gửi batch contracts qua NATS JetStream
   - Sử dụng Aspire.NATS.Net integration

4. **Polyclinic.Api.Host**: API nhận và lưu contracts
   - `PolyclinicNatsConsumer`: Background service với JetStream
   - Lưu vào MongoDB thông qua Application Services
   - Message acknowledgment (Ack)

5. **Polyclinic.AppHost**: Aspire orchestration
   - Quản lý MongoDB, NATS, API, Generator

## NATS JetStream Configuration

### Stream: `polyclinic-stream`
### Subjects:
- `polyclinic.patients` - Patient contracts
- `polyclinic.doctors` - Doctor contracts  
- `polyclinic.appointments` - Appointment contracts

## Cấu hình

### Generator (appsettings.json)
```json
{
  "ConnectionStrings": {
    "nats": "nats://localhost:4222"
  },
  "Generator": {
    "BatchSize": 5,
    "DelayMs": 2000
  }
}
```

### API Host (appsettings.json)
```json
{
  "ConnectionStrings": {
    "polyclinic": "mongodb://localhost:27017",
    "nats": "nats://localhost:4222"
  }
}
```

## Retry Logic

Cả Publisher và Subscriber đều có retry logic khi connect:
- Max retries: 10 lần
- Delay: 3 giây (exponential backoff)
- Tự động reconnect nếu NATS chưa sẵn sàng

## Chạy ứng dụng

### Cách 1: Chạy với Aspire (Recommended)

```bash
dotnet run --project Polyclinic.AppHost
```

Aspire sẽ tự động:
- Start MongoDB container
- Start NATS container
- Start API Host
- Start Generator
- Quản lý dependencies và health checks

### Cách 2: Chạy thủ công

1. Start MongoDB:
```bash
docker run -d -p 27017:27017 --name mongodb mongo:latest
```

2. Start NATS:
```bash
docker run -d -p 4222:4222 --name nats nats:latest
```

3. Start API Host:
```bash
dotnet run --project Polyclinic.Api.Host
```

4. Start Generator:
```bash
dotnet run --project Polyclinic.Generator.Nats.Host
```

## Kiểm tra hoạt động

1. Mở Aspire Dashboard (thường là http://localhost:15888)
2. Xem logs của Generator - sẽ thấy "Published Patient/Doctor/Appointment"
3. Xem logs của API - sẽ thấy "Saved Patient/Doctor/Appointment"
4. Kiểm tra MongoDB để xem data đã được lưu

## Streaming Implementation

Generator gửi contracts theo batch:
- Mỗi batch: 5 contracts (configurable)
- Delay giữa các batch: 2 giây (configurable)
- Chạy liên tục cho đến khi stop

API nhận và xử lý streaming:
- 3 subscribers song song (patients, doctors, appointments)
- Mỗi message được xử lý độc lập
- Error handling cho từng message

## Đặc điểm kỹ thuật

✅ Generator là standalone app (không reference server projects)
✅ Chỉ reference Contracts library
✅ Streaming data qua NATS
✅ Retry logic khi connect broker
✅ Aspire orchestration với NATS
✅ Background services cho producer/consumer
✅ Error handling và logging

## Troubleshooting

**Generator không connect được NATS:**
- Kiểm tra NATS đã chạy chưa
- Xem logs, retry sẽ tự động chạy 10 lần
- Kiểm tra connection string trong appsettings.json

**API không nhận được messages:**
- Kiểm tra NatsConsumerService đã start chưa
- Xem logs để confirm subscription
- Kiểm tra NATS subjects có đúng không

**Data không lưu vào MongoDB:**
- Kiểm tra MongoDB connection
- Xem logs của Application Services
- Verify migrations đã chạy
