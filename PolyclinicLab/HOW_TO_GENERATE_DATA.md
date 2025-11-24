# 🚀 How to Generate Data - Step by Step

## 📋 **OVERVIEW**

Generator API tạo fake data và gửi qua NATS. Có 2 cách:
1. **Swagger UI** (Dễ nhất - Recommended)
2. **cURL/Postman** (Cho advanced users)

---

## 🎯 **METHOD 1: SWAGGER UI (RECOMMENDED)**

### **Step 1: Open Swagger**

Generator đang chạy tại: `http://localhost:7072`

Mở Swagger UI:
```
http://localhost:7072/swagger
```

### **Step 2: Find the Endpoint**

Bạn sẽ thấy:
```
┌──────────────────────────────────────────────────┐
│  Polyclinic.Generator.Nats.Host                  │
│                                                  │
│  Generator                                       │
│  ├─ GET /api/generator                           │
│  │  Generates and sends contracts in batches    │
│  │  with ACK-based retry logic                  │
│  └─ [Try it out]                                 │
└──────────────────────────────────────────────────┘
```

### **Step 3: Click "Try it out"**

Click vào **GET /api/generator**, sau đó click **"Try it out"**

### **Step 4: Enter Parameters**

```
┌──────────────────────────────────────────────────┐
│  Parameters                                      │
├──────────────────────────────────────────────────┤
│  batchSize *        integer (query)              │
│  ┌────────────────────────────────────────────┐  │
│  │ 10                                         │  │
│  └────────────────────────────────────────────┘  │
│  Number of contracts per batch (default: 10)    │
│                                                  │
│  payloadLimit *     integer (query)              │
│  ┌────────────────────────────────────────────┐  │
│  │ 100                                        │  │
│  └────────────────────────────────────────────┘  │
│  Total number of contracts to generate          │
│                                                  │
│  waitTime *         integer (query)              │
│  ┌────────────────────────────────────────────┐  │
│  │ 2                                          │  │
│  └────────────────────────────────────────────┘  │
│  Delay in seconds between batches              │
└──────────────────────────────────────────────────┘
```

### **Step 5: Click "Execute"**

Click nút **"Execute"** màu xanh

### **Step 6: View Response**

Sau vài giây/phút, bạn sẽ thấy:

```
┌──────────────────────────────────────────────────┐
│  Response                                        │
├──────────────────────────────────────────────────┤
│  Code: 200                                       │
│  Details                                         │
│                                                  │
│  Response body                                   │
│  {                                               │
│    "totalGenerated": 300,                        │
│    "patientsGenerated": 100,                     │
│    "doctorsGenerated": 100,                      │
│    "appointmentsGenerated": 100,                 │
│    "startTime": "2024-11-24T10:00:00Z",          │
│    "endTime": "2024-11-24T10:03:20Z",            │
│    "duration": "00:03:20",                       │
│    "batchIds": [...],                            │
│    "totalBatches": 30                            │
│  }                                               │
└──────────────────────────────────────────────────┘
```

---

## 💻 **METHOD 2: CURL (COMMAND LINE)**

### **Quick Test:**
```bash
curl "http://localhost:7072/api/generator?batchSize=5&payloadLimit=10&waitTime=0"
```

### **Normal Test:**
```bash
curl "http://localhost:7072/api/generator?batchSize=10&payloadLimit=100&waitTime=2"
```

### **Heavy Load:**
```bash
curl "http://localhost:7072/api/generator?batchSize=50&payloadLimit=500&waitTime=1"
```

---

## 📊 **PARAMETER GUIDE**

### **batchSize** (Số lượng mỗi batch)
- **Nhỏ (5):** Chậm nhưng ổn định
- **Trung bình (10):** Cân bằng
- **Lớn (50):** Nhanh nhưng có thể overload

### **payloadLimit** (Tổng số cần tạo)
- **Test (10):** Nhanh, để test
- **Demo (100):** Vừa phải, để demo
- **Production (500+):** Nhiều data

### **waitTime** (Delay giữa các batch - giây)
- **0:** Không đợi, nhanh nhất
- **1-2:** Cân bằng
- **5+:** Chậm, tránh overload

---

## 🎯 **COMMON SCENARIOS**

### **Scenario 1: Quick Test**
```
batchSize:     5
payloadLimit:  10
waitTime:      0
```
**Result:** 30 contracts in ~5 seconds
**Use case:** Test xem API hoạt động

### **Scenario 2: Demo for Teacher**
```
batchSize:     10
payloadLimit:  100
waitTime:      2
```
**Result:** 300 contracts in ~3 minutes
**Use case:** Demo cho giáo viên

### **Scenario 3: Populate Database**
```
batchSize:     20
payloadLimit:  200
waitTime:      1
```
**Result:** 600 contracts in ~10 minutes
**Use case:** Có data để test UI/API

### **Scenario 4: Load Testing**
```
batchSize:     50
payloadLimit:  1000
waitTime:      0
```
**Result:** 3000 contracts in ~20 minutes
**Use case:** Test performance

---

## 📝 **WHAT HAPPENS BEHIND THE SCENES**

```
1. User clicks "Execute"
   ↓
2. Generator creates fake data:
   - Patients: passport, name, gender, birthdate, etc.
   - Doctors: passport, name, specialization, experience
   - Appointments: date, room, doctorId, patientId
   ↓
3. Generator sends to NATS:
   - Subject: polyclinic.patients
   - Subject: polyclinic.doctors
   - Subject: polyclinic.appointments
   ↓
4. NATS stores messages in JetStream
   ↓
5. Consumer (Api.Host) receives messages
   ↓
6. Consumer saves to MongoDB
   ↓
7. Consumer sends ACK back to Generator
   ↓
8. Generator returns summary to user
```

---

## ✅ **VERIFICATION**

### **Check Logs:**
```
[INFO] Generating 100 contracts via 10 batches with 2s delay
[INFO] Sent batch a1b2c3d4 (10 patients) to polyclinic.patients
[INFO] Received ACK for batch a1b2c3d4: 10/10 inserted
[INFO] Sent batch b2c3d4e5 (10 doctors) to polyclinic.doctors
[INFO] Received ACK for batch b2c3d4e5: 10/10 inserted
...
[INFO] Finished sending 300 messages in 200s
```

### **Check MongoDB:**
```bash
# Connect to MongoDB
mongosh

# Use database
use polyclinic

# Count documents
db.patients.countDocuments()    // Should be 100
db.doctors.countDocuments()     // Should be 100
db.appointments.countDocuments() // Should be 100
```

### **Check via API:**
```bash
# Get all patients
curl http://localhost:5000/api/patients

# Get all doctors
curl http://localhost:5000/api/doctors

# Get all appointments
curl http://localhost:5000/api/appointments
```

---

## 🐛 **TROUBLESHOOTING**

### **Error: "NATS connection string not found"**
→ NATS server not running
→ Solution: Start NATS with Docker
```bash
docker run -d -p 4222:4222 --name nats nats
```

### **Error: "Timeout waiting for ACK"**
→ Consumer (Api.Host) not running
→ Solution: Start Api.Host
```bash
dotnet run --project Polyclinic.Api.Host
```

### **Error: "MongoDB connection failed"**
→ MongoDB not running
→ Solution: Start MongoDB
```bash
docker run -d -p 27017:27017 --name mongo mongo
```

### **Response takes too long**
→ payloadLimit too large or waitTime too high
→ Solution: Use smaller values for testing

---

## 💡 **TIPS**

1. **Start small:** Test với `payloadLimit=10` trước
2. **Use Swagger:** Dễ hơn cURL
3. **Check logs:** Xem progress trong console
4. **Monitor NATS:** Check NATS dashboard tại http://localhost:8222
5. **Use Aspire:** Easiest way to run everything

---

## 🎉 **SUCCESS!**

Khi thấy response như này là thành công:
```json
{
  "totalGenerated": 300,
  "patientsGenerated": 100,
  "doctorsGenerated": 100,
  "appointmentsGenerated": 100,
  "duration": "00:03:20"
}
```

**Congratulations! You've successfully generated data!** 🚀
