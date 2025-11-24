# 🔧 Fix NATS Connection Error

## 🔴 **ERROR:**
```
System.InvalidOperationException: 'NATS connection string not found: polyclinic-nats'
```

## 🎯 **CAUSE:**
Missing NATS connection string in `appsettings.json`

## ✅ **SOLUTION:**

### **Step 1: Start NATS Server**

```bash
# Option A: Using Docker (Recommended)
docker run -d -p 4222:4222 -p 8222:8222 --name nats nats

# Option B: Using Aspire (Automatic)
dotnet run --project Polyclinic.AppHost
```

### **Step 2: Verify Connection String**

Check that both `appsettings.json` files have NATS connection:

**Api.Host/appsettings.json:**
```json
{
  "ConnectionStrings": {
    "polyclinic": "mongodb://localhost:27017",
    "polyclinic-nats": "nats://localhost:4222"  ← ADD THIS
  }
}
```

**Generator.Nats.Host/appsettings.json:**
```json
{
  "ConnectionStrings": {
    "polyclinic-nats": "nats://localhost:4222"  ← ADD THIS
  }
}
```

### **Step 3: Restart Application**

```bash
# Stop current app (Ctrl+C)
# Start again
dotnet run --project Polyclinic.Api.Host
```

---

## 🚀 **RECOMMENDED: Use Aspire**

Instead of running services manually, use Aspire AppHost:

```bash
dotnet run --project Polyclinic.AppHost
```

**Benefits:**
- ✅ Auto-starts NATS
- ✅ Auto-starts MongoDB
- ✅ Auto-configures connections
- ✅ Manages all services
- ✅ Dashboard at http://localhost:15888

---

## 📋 **MANUAL STARTUP ORDER**

If you want to run manually:

```bash
# Terminal 1: NATS
docker run -p 4222:4222 nats

# Terminal 2: MongoDB
docker run -p 27017:27017 mongo

# Terminal 3: API (Consumer)
dotnet run --project Polyclinic.Api.Host

# Terminal 4: Generator (Producer)
dotnet run --project Polyclinic.Generator.Nats.Host
```

---

## ✅ **VERIFICATION**

After starting, you should see:

```
[INFO] NATS JetStream initialized with stream polyclinic-stream
[INFO] Created consumers for stream polyclinic-stream
```

No more errors! 🎉

---

## 🐛 **TROUBLESHOOTING**

### **Error: "Connection refused"**
→ NATS server not running
→ Solution: Start NATS with Docker

### **Error: "Stream not found"**
→ Stream not created yet
→ Solution: Run Generator first to create stream

### **Error: "Port already in use"**
→ Another service using port 4222
→ Solution: Stop other NATS instances

---

## 💡 **TIP**

Use Aspire AppHost for easiest setup:
```bash
dotnet run --project Polyclinic.AppHost
```

Then open dashboard: http://localhost:15888
