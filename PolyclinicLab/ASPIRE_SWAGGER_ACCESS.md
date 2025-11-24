# 🎯 Access Swagger via Aspire Dashboard

## ✅ **ĐÃ CẤU HÌNH XONG!**

Generator giờ đã có **External HTTP Endpoints** trong Aspire!

---

## 🚀 **CÁCH SỬ DỤNG:**

### **BƯỚC 1: Restart AppHost**

```bash
# Stop current app (Ctrl+C)
dotnet run --project Polyclinic.AppHost
```

### **BƯỚC 2: Mở Aspire Dashboard**

Dashboard tự động mở tại:
```
http://localhost:15888
```

### **BƯỚC 3: Tìm Generator trong Resources**

Trong bảng **Resources**, tìm dòng **generator**:

```
┌──────────────────────────────────────────────────────────┐
│ Name       │ State   │ URLs                              │
├──────────────────────────────────────────────────────────┤
│ generator  │ Running │ http://localhost:7072  ← CLICK    │
└──────────────────────────────────────────────────────────┘
```

### **BƯỚC 4: Click vào Link**

Click vào `http://localhost:7072`

### **BƯỚC 5: Thêm /swagger**

Trong address bar, thêm `/swagger`:
```
http://localhost:7072/swagger
```

---

## 🎯 **DIRECT LINKS:**

Sau khi AppHost chạy, dùng các links này:

### **Generator Root:**
```
http://localhost:7072
```

### **Swagger UI:**
```
http://localhost:7072/swagger
```

### **API Endpoint:**
```
http://localhost:7072/api/generator?batchSize=10&payloadLimit=100&waitTime=2
```

### **Aspire Dashboard:**
```
http://localhost:15888
```

---

## 📊 **WHAT CHANGED:**

### **Before:**
```csharp
builder.AddProject<Projects.Polyclinic_Generator_Nats_Host>("generator")
       .WithReference(nats)
       .WaitFor(nats);
// ❌ No external endpoints - link not clickable in Aspire
```

### **After:**
```csharp
builder.AddProject<Projects.Polyclinic_Generator_Nats_Host>("generator")
       .WithReference(nats)
       .WaitFor(nats)
       .WithExternalHttpEndpoints();  // ✅ Added this!
// ✅ Link now clickable in Aspire Dashboard
```

---

## 💡 **BENEFITS:**

1. ✅ **Easy Access** - Click link trong Aspire Dashboard
2. ✅ **No Manual Port** - Không cần nhớ port number
3. ✅ **Consistent** - Giống như API Host
4. ✅ **Professional** - Proper Aspire configuration

---

## 🎬 **DEMO FLOW:**

```
1. Start AppHost
   ↓
2. Aspire Dashboard opens (http://localhost:15888)
   ↓
3. See "generator" with clickable link
   ↓
4. Click link → Opens http://localhost:7072
   ↓
5. Add /swagger → http://localhost:7072/swagger
   ↓
6. Test API with Swagger UI
   ↓
7. Generate data! 🎉
```

---

## ✅ **VERIFICATION:**

After restart, you should see in Aspire Dashboard:

```
Resources:
┌────────────┬─────────┬──────────────────────────┐
│ Name       │ State   │ URLs                     │
├────────────┼─────────┼──────────────────────────┤
│ mongodb    │ Running │ tcp://localhost:57471    │
│ nats       │ Running │ tcp://localhost:57475    │
│ api        │ Running │ https://localhost:7044   │ ← Clickable
│ generator  │ Running │ http://localhost:7072    │ ← Clickable NOW!
└────────────┴─────────┴──────────────────────────┘
```

---

## 🚀 **READY TO USE!**

Giờ bạn có thể:
1. ✅ Click vào generator link trong Aspire
2. ✅ Thêm `/swagger` để mở Swagger UI
3. ✅ Test API dễ dàng
4. ✅ Generate data ngay!

**Much more convenient!** 🎉
