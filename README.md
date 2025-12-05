# Лабораторная работа – Поликлиника

Данный репозиторий содержит лабораторную работу по предмету, посвящённую моделированию предметной области **Поликлиника**.  
В проекте реализована распределённая микросервисная архитектура с использованием .NET Aspire, NATS JetStream для обмена сообщениями, MongoDB для хранения данных, а также Clean Architecture с разделением на слои Domain, Application и Infrastructure.

---

## Описание проекта

Проект моделирует работу информационной системы поликлиники с использованием современных подходов к разработке enterprise-приложений:

- **Микросервисная архитектура** с использованием .NET Aspire для оркестрации
- **Event-driven подход** через NATS JetStream для асинхронной обработки данных
- **Clean Architecture** с чётким разделением ответственности между слоями
- **MongoDB** для персистентного хранения данных
- **Background services** для валидации и обработки данных в реальном времени

Система включает генератор тестовых данных, валидатор appointments и REST API для работы с данными поликлиники.

---

## Архитектура

Проект построен на основе **Clean Architecture** и состоит из следующих компонентов:

### Core слои
- **Polyclinic.Domain** - доменные сущности (Doctor, Patient, Appointment)
- **Polyclinic.Application.Contracts** - DTO и контракты для обмена данными
- **Polyclinic.Application** - бизнес-логика и use cases

### Infrastructure слои
- **Polyclinic.Infrastructure.Mongo** - репозитории для работы с MongoDB
- **Polyclinic.Infrastructure.InMemory** - in-memory реализация для тестирования
- **Polyclinic.Infrastructure.Nats** - consumer для обработки сообщений из NATS

### Микросервисы
- **Polyclinic.Api.Host** - REST API для работы с данными поликлиники
- **Polyclinic.Generator.Nats.Host** - генератор тестовых appointments с отправкой в NATS
- **Polyclinic.Validator.Nats** - background service для валидации appointments

### Инфраструктура
- **Polyclinic.AppHost** - .NET Aspire orchestrator для запуска всех сервисов
- **Polyclinic.ServiceDefaults** - общие настройки для всех сервисов
- **Polyclinic.Tests** - модульные тесты

---

## Технологический стек

- **.NET 8.0**
- **.NET Aspire** - оркестрация микросервисов
- **NATS JetStream** - message broker для event-driven архитектуры
- **MongoDB** - NoSQL база данных
- **Bogus** - генерация тестовых данных
- **xUnit** - модульное тестирование
- **Swagger/OpenAPI** - документация API

---

## Структура проекта

```
PolyclinicLab
│
├── Polyclinic.Domain                    # Доменные сущности
├── Polyclinic.Application.Contracts     # DTO и контракты
├── Polyclinic.Application               # Бизнес-логика
│
├── Polyclinic.Infrastructure.Mongo      # MongoDB репозитории
├── Polyclinic.Infrastructure.InMemory   # In-memory реализация
├── Polyclinic.Infrastructure.Nats       # NATS consumer
│
├── Polyclinic.Api.Host                  # REST API сервис
├── Polyclinic.Generator.Nats.Host       # Генератор данных
├── Polyclinic.Validator.Nats            # Валидатор appointments
│
├── Polyclinic.AppHost                   # Aspire orchestrator
├── Polyclinic.ServiceDefaults           # Общие настройки
└── Polyclinic.Tests                     # Модульные тесты
```

---

## Запуск проекта

### Требования
- .NET 8.0 SDK
- Docker (для MongoDB и NATS)

### Запуск через .NET Aspire

```bash
dotnet run --project Polyclinic.AppHost
```

Aspire автоматически запустит:
- MongoDB
- NATS JetStream
- API сервис
- Generator сервис
- Validator сервис
- Aspire Dashboard