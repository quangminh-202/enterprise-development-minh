# 🎯 Polyclinic Generator API Guide

## 📋 Overview

Generator service đã được **refactor từ Background Service sang API Controller** để có control tốt hơn khi demo và testing.

## 🔄 Thay Đổi Chính

### ❌ Cũ: Background Service
- Tự động chạy khi app start
- Không control được
- Phải restart để thay đổi config

### ✅ Mới: API Controller
- Manual trigger qua HTTP
- Full control với query parameters
- Retry logic cho partial failures
- Swagger UI để test dễ dàng

---

## 🚀 Cách Sử Dụng

### 1. Start Generator Service

```bash
dotnet run --project Polyclinic.Generator.Nats.Host
```

Service sẽ chạy tại: `http://localhost:5001` (hoặc port khác)

### 2. Truy Cập Swagger UI

Mở browser: `http://localhost:5001/swagger`

### 3. Gọi API Generate

**Endpoint:** `GET /api/generator`

**Query Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `batchSize` | int | 10 | Số lượng contracts mỗi batch |
| `payloadLimit` | int | 100 | Tổng số contracts cần tạo |
| `waitTime` | int | 2 | Delay giữa các batch (giây) |

**Ví dụ:**

```bash
# Generate 100 contracts, mỗi batch 10, đợi 2 giây
curl "http://localhost:5001/api/generator?batchSize=10&payloadLimit=100&waitTime=2"

# Generate nhanh: 50 contracts, batch 25, không đợi
curl "http://localhost:5001/api/generator?batchSize=25&payloadLimit=50&waitTime=0"

# Generate chậm: 200 contracts, batch 5, đợi 5 giây
curl "http://localhost:5001/api/generator?batchSize=5&payloadLimit=200&waitTime=5"
```

---

## 📊 Response Format

```json
{
  "totalGenerated": 300,
  "patientsGenerated": 100,
  "doctorsGenerated": 100,
  "appointmentsGenerated": 100,
  "patients": [
    {
      "passport": "ABC123XYZ",
      "fullName": "John Doe",
      "gender": "Male",
      "birthDate": "1990-05-15T00:00:00",
      "address": "123 Main St",
      "bloodType": "A",
      "rhFactor": "Positive",
      "phone": "555-1234"
    }
  ],
  "doctors": [...],
  "appointments": [...]
}
```

---

## 🔧 Architecture

```
┌─────────────────────────────────────────────────────┐
│              GeneratorController                     │
│  - Nhận HTTP request với parameters                 │
│  - Generate contracts theo batch                    │
│  - Retry logic nếu gửi fail                         │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────┐
│            NatsProducerService                       │
│  - Kết nối NATS JetStream                           │
│  - Gửi messages theo subject                        │
│  - Return SendResult với status                     │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────┐
│              NATS JetStream                          │
│  Stream: polyclinic-stream                          │
│  Subjects:                                          │
│    - polyclinic.patients                            │
│    - polyclinic.doctors                             │
│    - polyclinic.appointments                        │
└─────────────────────────────────────────────────────┘
```

---

## 🎓 Retry Logic

Controller có **sophisticated retry mechanism**:

```csharp
while (remaining > 0) {
    var result = await sendFunc(currentBatch);
    
    if (!result.Success) {
        // Retry sau 1 giây
        await Task.Delay(1000);
        continue;
    }
    
    remaining -= result.Inserted;
    
    if (remaining > 0) {
        // Chỉ retry phần chưa gửi được
        logger.LogWarning("{Remaining} items not inserted, retrying...", remaining);
    }
}
```

**Lợi ích:**
- ✅ Không mất data nếu network hiccup
- ✅ Chỉ retry phần fail (không duplicate)
- ✅ Automatic backoff với delay

---

## 🧪 Testing Scenarios

### Scenario 1: Quick Test
```bash
curl "http://localhost:5001/api/generator?batchSize=5&payloadLimit=10&waitTime=0"
```
→ Nhanh, ít data, test xem API hoạt động

### Scenario 2: Normal Load
```bash
curl "http://localhost:5001/api/generator?batchSize=10&payloadLimit=100&waitTime=2"
```
→ Giống production, có delay giữa batches

### Scenario 3: Heavy Load
```bash
curl "http://localhost:5001/api/generator?batchSize=50&payloadLimit=500&waitTime=1"
```
→ Test performance, nhiều data

### Scenario 4: Slow Continuous
```bash
curl "http://localhost:5001/api/generator?batchSize=3&payloadLimit=100&waitTime=5"
```
→ Simulate real-time data entry

---

## 📝 Configuration

File: `appsettings.json`

```json
{
  "Nats": {
    "StreamName": "polyclinic-stream",
    "PatientSubject": "polyclinic.patients",
    "DoctorSubject": "polyclinic.doctors",
    "AppointmentSubject": "polyclinic.appointments"
  }
}
```

---

## 🆚 So Sánh Với Background Service

| Feature | Background Service | API Controller |
|---------|-------------------|----------------|
| **Trigger** | Automatic | Manual (HTTP) |
| **Control** | Config file only | Query parameters |
| **Testing** | Phải restart app | Call API ngay |
| **Flexibility** | Low | High |
| **Demo** | Khó control | Dễ demo |
| **Production** | Good for continuous | Good for on-demand |

---

## 💡 Tips

1. **Dùng Swagger UI** - Dễ test hơn curl
2. **Start với small numbers** - Test trước khi generate nhiều
3. **Monitor logs** - Xem progress và errors
4. **Check NATS dashboard** - Verify messages đã gửi
5. **Adjust waitTime** - Tránh overwhelm consumer

---

## 🐛 Troubleshooting

### Problem: "JetStream context not initialized"
**Solution:** Đảm bảo NATS server đang chạy

### Problem: "Failed to send batch"
**Solution:** Check NATS connection string trong config

### Problem: API không response
**Solution:** Request đang chạy, đợi hoàn thành (có thể lâu nếu payloadLimit lớn)

---

## 🎉 Kết Luận

API Controller approach **tốt hơn cho Lab 4** vì:
- ✅ Giáo viên có thể control chính xác
- ✅ Demo dễ dàng với Swagger
- ✅ Test nhiều scenarios khác nhau
- ✅ Không cần restart app
- ✅ Có retry logic rõ ràng

**Ready to demo!** 🚀
