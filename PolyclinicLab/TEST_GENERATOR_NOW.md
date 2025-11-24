# 🧪 TEST GENERATOR - STEP BY STEP

## ✅ **CODE ĐÃ SẴN SÀNG!**

Generator giờ chạy ở **TEST MODE** - không cần NATS, response ngay!

---

## 🚀 **BƯỚC 1: RESTART APP**

**QUAN TRỌNG:** Phải restart để load code mới!

### **Cách 1: Restart trong VS Code**
1. Nhấn `Ctrl+C` trong terminal đang chạy AppHost
2. Chạy lại:
   ```bash
   dotnet run --project Polyclinic.AppHost
   ```

### **Cách 2: Stop và Start lại**
1. Stop app (nút Stop trong VS Code)
2. Start lại (F5 hoặc Run)

---

## 🧪 **BƯỚC 2: MỞ SWAGGER**

Sau khi app chạy, mở browser:

```
http://localhost:7072/swagger
```

Hoặc click vào link **generator** trong Aspire Dashboard (sẽ tự động redirect đến Swagger)

---

## 🎯 **BƯỚC 3: TEST API**

### **Test 1: Quick Test (5 giây)**

Parameters:
```
batchSize:     5
payloadLimit:  10
waitTime:      0
```

Click **"Execute"**

**Expected Result:**
```json
{
  "totalGenerated": 30,
  "patientsGenerated": 10,
  "doctorsGenerated": 10,
  "appointmentsGenerated": 10,
  "startTime": "2024-11-24T...",
  "endTime": "2024-11-24T...",
  "duration": "00:00:01.5",
  "batchIds": [...],
  "totalBatches": 6
}
```

**Time:** ~1-2 seconds ⚡

---

### **Test 2: Normal Test (10 giây)**

Parameters:
```
batchSize:     10
payloadLimit:  20
waitTime:      0
```

Click **"Execute"**

**Expected Result:**
```json
{
  "totalGenerated": 60,
  "patientsGenerated": 20,
  "doctorsGenerated": 20,
  "appointmentsGenerated": 20,
  "duration": "00:00:02.5",
  "totalBatches": 6
}
```

**Time:** ~2-3 seconds ⚡

---

## 📊 **EXPECTED BEHAVIOR**

### **✅ SUCCESS:**
- Response trong vài giây
- Status code: 200
- JSON response với summary
- Không có errors

### **❌ IF STILL LOADING:**

**Possible Issues:**

1. **Chưa restart app**
   → Solution: Stop và start lại AppHost

2. **Browser cache**
   → Solution: Hard refresh (Ctrl+Shift+R)

3. **Wrong port**
   → Solution: Check port trong Aspire Dashboard

4. **Code chưa compile**
   → Solution: Build lại
   ```bash
   dotnet build Polyclinic.Generator.Nats.Host
   ```

---

## 🔍 **CHECK LOGS**

Trong console, bạn sẽ thấy:

```
[INFO] Generating 10 contracts via 5 batches with 0s delay
[INFO] Simulating send of batch a1b2c3d4 (5 patients) to polyclinic.patients
[INFO] Batch a1b2c3d4: Simulated 5 patients sent successfully
[INFO] Simulating send of batch b2c3d4e5 (5 doctors) to polyclinic.doctors
[INFO] Batch b2c3d4e5: Simulated 5 doctors sent successfully
...
[INFO] Finished sending 30 messages in 1.5s
```

**Key words:** "Simulating" và "Simulated" - nghĩa là đang chạy test mode

---

## 🐛 **TROUBLESHOOTING**

### **Problem: Still loading forever**

**Solution 1: Force restart**
```bash
# Kill all dotnet processes
taskkill /F /IM dotnet.exe

# Start again
dotnet run --project Polyclinic.AppHost
```

**Solution 2: Run Generator standalone**
```bash
# Run Generator directly (not via Aspire)
dotnet run --project Polyclinic.Generator.Nats.Host
```

Then test at: `http://localhost:5001/swagger`

**Solution 3: Test with cURL**
```bash
curl "http://localhost:7072/api/generator?batchSize=5&payloadLimit=10&waitTime=0"
```

---

### **Problem: Port not found**

Check actual port in Aspire Dashboard or logs:
```
Now listening on: http://localhost:XXXX
```

Use that port instead of 7072

---

### **Problem: 404 Not Found**

Make sure URL is correct:
```
✅ http://localhost:7072/swagger
✅ http://localhost:7072/api/generator
❌ http://localhost:7072/Generator
❌ http://localhost:7072/api/Generator
```

---

## ✅ **VERIFICATION CHECKLIST**

Before testing, verify:

- [ ] AppHost is running
- [ ] Generator shows "Running" in Aspire Dashboard
- [ ] Can access http://localhost:7072
- [ ] Swagger UI loads
- [ ] Can see GET /api/generator endpoint
- [ ] Parameters are visible (batchSize, payloadLimit, waitTime)

If all checked, click Execute and it should work! 🎉

---

## 🎉 **SUCCESS INDICATORS**

You'll know it works when:

1. ✅ Response comes back in < 5 seconds
2. ✅ Status code is 200
3. ✅ JSON has "totalGenerated", "duration", etc.
4. ✅ Logs show "Simulating" messages
5. ✅ No errors in console

---

## 📝 **WHAT'S HAPPENING**

Current code is in **TEST MODE**:
- ❌ NOT connecting to NATS
- ❌ NOT sending real messages
- ✅ ONLY simulating success
- ✅ Returns fake data immediately

This is to verify API works before fixing NATS connection.

---

## 🚀 **NEXT STEPS**

After confirming API works:

1. ✅ API responds quickly
2. ✅ No loading issues
3. ✅ JSON response correct

Then we can:
- Fix NATS connection
- Enable real message sending
- Test with Consumer

---

## 💡 **TIP**

If you see response like this, it's working:

```json
{
  "totalGenerated": 30,
  "patientsGenerated": 10,
  "doctorsGenerated": 10,
  "appointmentsGenerated": 10,
  "startTime": "2024-11-24T12:30:00Z",
  "endTime": "2024-11-24T12:30:02Z",
  "duration": "00:00:02",
  "batchIds": [
    "guid-1", "guid-2", "guid-3", 
    "guid-4", "guid-5", "guid-6"
  ],
  "totalBatches": 6
}
```

**That's SUCCESS!** 🎉

---

## 🆘 **STILL NOT WORKING?**

Share:
1. Screenshot of Swagger UI
2. Console logs
3. Error messages (if any)
4. Port number from Aspire Dashboard

And I'll help debug! 😊
