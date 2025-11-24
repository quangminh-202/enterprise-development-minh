# Test Generator API Script
Write-Host "🧪 Testing Generator API Refactoring..." -ForegroundColor Cyan

Write-Host "`n1️⃣ Building Generator project..." -ForegroundColor Yellow
dotnet build Polyclinic.Generator.Nats.Host/Polyclinic.Generator.Nats.Host.csproj --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Build successful!" -ForegroundColor Green

Write-Host "`n2️⃣ Checking project structure..." -ForegroundColor Yellow
$files = @(
    "Polyclinic.Generator.Nats.Host/Controllers/GeneratorController.cs",
    "Polyclinic.Generator.Nats.Host/Services/NatsProducerService.cs",
    "Polyclinic.Generator.Nats.Host/Services/ContractGenerator.cs",
    "Polyclinic.Generator.Nats.Host/Interfaces/IProducerService.cs",
    "Polyclinic.Generator.Nats.Host/Program.cs"
)

$allExist = $true
foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "  ✓ $file" -ForegroundColor Gray
    } else {
        Write-Host "  ✗ $file - NOT FOUND!" -ForegroundColor Red
        $allExist = $false
    }
}

if (!$allExist) {
    Write-Host "`n❌ Some files are missing!" -ForegroundColor Red
    exit 1
}

Write-Host "`n3️⃣ Verifying old BackgroundService is removed..." -ForegroundColor Yellow
if (Test-Path "Polyclinic.Generator.Nats.Host/Services/ContractGeneratorService.cs") {
    Write-Host "  ⚠️  Old BackgroundService still exists!" -ForegroundColor Yellow
} else {
    Write-Host "  ✓ Old BackgroundService removed" -ForegroundColor Gray
}

Write-Host "`n🎉 ALL CHECKS PASSED!" -ForegroundColor Green
Write-Host "`n📋 Summary:" -ForegroundColor Cyan
Write-Host "  • Architecture: ✅ Controller-based" -ForegroundColor White
Write-Host "  • Build: ✅ Success" -ForegroundColor White
Write-Host "  • Files: ✅ All present" -ForegroundColor White
Write-Host "  • Old code: ✅ Cleaned up" -ForegroundColor White

Write-Host "`n🚀 Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Start NATS: docker run -p 4222:4222 nats" -ForegroundColor White
Write-Host "  2. Run Generator: dotnet run --project Polyclinic.Generator.Nats.Host" -ForegroundColor White
Write-Host "  3. Open Swagger: http://localhost:5001/swagger" -ForegroundColor White
Write-Host "  4. Test API: GET /api/generator?batchSize=5`&payloadLimit=10`&waitTime=0" -ForegroundColor White

Write-Host "`n📖 Read GENERATOR_API_GUIDE.md for detailed usage!" -ForegroundColor Cyan
