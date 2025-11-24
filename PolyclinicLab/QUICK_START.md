# 🚀 Quick Start - Generator API

## 1️⃣ Start Generator Service

```bash
dotnet run --project Polyclinic.Generator.Nats.Host
```

## 2️⃣ Open Swagger UI

```
http://localhost:5001/swagger
```

## 3️⃣ Test API

### Option A: Swagger UI (Recommended)
1. Click on `GET /api/generator`
2. Click "Try it out"
3. Enter parameters:
   - batchSize: `10`
   - payloadLimit: `100`
   - waitTime: `2`
4. Click "Execute"

### Option B: cURL
```bash
curl "http://localhost:5001/api/generator?batchSize=10&payloadLimit=100&waitTime=2"
```

### Option C: PowerShell
```powershell
Invoke-RestMethod "http://localhost:5001/api/generator?batchSize=10&payloadLimit=100&waitTime=2"
```

## 📊 Response Example

```json
{
  "totalGenerated": 300,
  "patientsGenerated": 100,
  "doctorsGenerated": 100,
  "appointmentsGenerated": 100,
  "patients": [...],
  "doctors": [...],
  "appointments": [...]
}
```

## 🎯 Common Scenarios

| Scenario | batchSize | payloadLimit | waitTime |
|----------|-----------|--------------|----------|
| Quick Test | 5 | 10 | 0 |
| Normal Load | 10 | 100 | 2 |
| Heavy Load | 50 | 500 | 1 |
| Slow Stream | 3 | 100 | 5 |

## 📖 Full Documentation

- **GENERATOR_API_GUIDE.md** - Complete API guide
- **REFACTORING_SUMMARY.md** - What changed
- **MIGRATION_CHECKLIST.md** - Verification checklist
- **example-api-calls.http** - HTTP examples

## ✅ That's it!

Generator is now **Controller-based** and ready to use! 🎉
