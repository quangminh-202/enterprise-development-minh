# Система управления поликлиникой

Комплексная система управления здравоохранением, построенная на современных .NET технологиях и микросервисной архитектуре. Данный проект демонстрирует практики разработки корпоративного уровня с распределённой архитектурой, событийно-ориентированным дизайном и чётким разделением ответственности.

---

## Обзор проекта

Система управления поликлиникой представляет собой полнофункциональное healthcare-приложение со следующими возможностями:

- **Микросервисная архитектура** с оркестрацией через .NET Aspire
- **Событийно-ориентированный дизайн** с использованием NATS JetStream для обработки данных в реальном времени
- **Clean Architecture** с чётким разделением между слоями Domain, Application и Infrastructure
- **Современный веб-интерфейс** на базе Blazor WebAssembly и компонентов Blazorise
- **MongoDB** для масштабируемого хранения данных
- **Аналитика в реальном времени** и возможности отчётности
- **Фоновые сервисы** для валидации данных и автоматизированной обработки

Система управляет пациентами, врачами, записями на приём и предоставляет комплексную аналитику для медицинских операций.

---

## Архитектура

Проект следует принципам **Clean Architecture** и состоит из следующих компонентов:

### Основные слои
- **Polyclinic.Domain** - доменные сущности (Doctor, Patient, Appointment) и бизнес-правила
- **Polyclinic.Application.Contracts** - DTO и контракты для обмена данными
- **Polyclinic.Application** - бизнес-логика, сервисы и use cases

### Инфраструктурные слои
- **Polyclinic.Infrastructure.Mongo** - репозитории MongoDB и доступ к данным
- **Polyclinic.Infrastructure.InMemory** - in-memory реализация для тестирования
- **Polyclinic.Infrastructure.Nats** - NATS consumers и обработчики событий

### Микросервисы
- **Polyclinic.Api.Host** - REST API сервис для управления медицинскими данными
- **Polyclinic.Generator.Nats.Host** - генератор тестовых данных с интеграцией NATS
- **Polyclinic.Validator.Nats** - фоновый сервис для валидации записей на приём

### Клиентские приложения
- **Polyclinic.Client.Wasm** - современный Blazor WebAssembly frontend с адаптивным UI

### Инфраструктура и оркестрация
- **Polyclinic.AppHost** - .NET Aspire оркестратор для управления сервисами
- **Polyclinic.ServiceDefaults** - общие конфигурации и сервисы
- **Polyclinic.Tests** - комплексные unit и интеграционные тесты

---

## Технологический стек

- **.NET 8.0** - основная платформа разработки
- **.NET Aspire** - оркестрация микросервисов и управление конфигурацией
- **Blazor WebAssembly** - современный SPA фронтенд
- **Blazorise** - UI компоненты для Bootstrap
- **NATS JetStream** - message broker для событийно-ориентированной архитектуры
- **MongoDB** - NoSQL база данных для масштабируемого хранения
- **AutoMapper** - маппинг между доменными объектами и DTO
- **Bogus** - генерация тестовых данных
- **xUnit** - модульное тестирование
- **Swagger/OpenAPI** - документация и тестирование API

---

## Структура проекта

```
PolyclinicLab/
│
├── Polyclinic.Domain/                    # Доменные сущности и бизнес-правила
├── Polyclinic.Application.Contracts/     # DTO и контракты
├── Polyclinic.Application/               # Бизнес-логика и сервисы
│
├── Polyclinic.Infrastructure.Mongo/      # MongoDB репозитории
├── Polyclinic.Infrastructure.InMemory/   # In-memory реализация
├── Polyclinic.Infrastructure.Nats/       # NATS consumers
│
├── Polyclinic.Api.Host/                  # REST API микросервис
├── Polyclinic.Generator.Nats.Host/       # Генератор данных
├── Polyclinic.Validator.Nats/            # Валидатор записей
│
├── Polyclinic.Client.Wasm/               # Blazor WebAssembly клиент
│
├── Polyclinic.AppHost/                   # Aspire оркестратор
├── Polyclinic.ServiceDefaults/           # Общие настройки
└── Polyclinic.Tests/                     # Тесты
```

---

## Запуск проекта

### Системные требования
- .NET 8.0 SDK
- Docker Desktop (для MongoDB и NATS)
- Современный веб-браузер

### Запуск через .NET Aspire
```bash
dotnet run --project Polyclinic.AppHost
```