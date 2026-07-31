# RoomRental API — Backend

REST API для системы бронирования помещений (переговорные комнаты, коворкинги, актовые залы). Реализован на ASP.NET Core 8 с применением принципов Clean Architecture.

Клиентская часть (Angular SPA): https://github.com/teteik/room-rental-ui

---

## Стек технологий

- **C# / .NET 8** — основной язык и платформа
- **ASP.NET Core Web API** — REST API фреймворк
- **Entity Framework Core 8** — ORM для работы с БД
- **PostgreSQL** — реляционная база данных
- **JWT (JSON Web Tokens)** — аутентификация
- **BCrypt** — безопасное хеширование паролей
- **ASP.NET Core Identity** — управление пользователями и ролями (RBAC)
- **Swagger / OpenAPI** — интерактивная документация API
- **Clean Architecture** — архитектурный паттерн

---

## Архитектура проекта

Проект построен по принципам **Clean Architecture**, что обеспечивает разделение ответственности, независимость бизнес-логики от фреймворков и легкую тестируемость.


```text
RoomRental/
├── RoomRental.Domain/          # Слой домена (Ядро, не зависит от внешних библиотек)
│   ├── Entities/               # Бизнес-сущности: Room, RoomImage, Booking, Client, ApplicationUser
│   └── Enums/                  # Перечисления: BookingStatus
│
├── RoomRental.Infrastructure/  # Слой инфраструктуры (Реализация технических деталей)
│   ├── Data/                   # AppDbContext (конфигурация EF Core, связи между сущностями)
│   └── Migrations/             # Файлы миграций базы данных
│
└── RoomRental.API/             # Слой презентации (Точка входа в приложение)
    ├── Controllers/            # REST API контроллеры (Rooms, Bookings, Auth)
    ├── DTOs/                   # Модели запросов и ответов (Request/Response)
    ├── wwwroot/images/         # Статические файлы (загруженные изображения)
    ├── appsettings.json        # Конфигурация (строки подключения, JWT настройки)
    └── Program.cs              # Конфигурация DI-контейнера, Middleware и Swagger
```

---

## Ключевые фичи

### Аутентификация и авторизация
- Регистрация пользователей с хешированием паролей через BCrypt
- Выпуск JWT-токенов с настраиваемым временем жизни
- Ролевая модель (RBAC): `Admin` и `User`
- Защита эндпоинтов атрибутами `[Authorize(Roles = "Admin")]`
- Автоматический сидер администратора при первом запуске

### Логика бронирования
- Проверка пересечений временных слотов на уровне БД
- Автоматический расчет стоимости на основе длительности
- Статусы бронирования: `Pending`, `Confirmed`, `Cancelled`
- Защита от удаления комнаты с историей бронирований

### ️ Работа с файлами
- Мульти-загрузка изображений одним запросом (`IFormFileCollection`)
- Валидация расширений (JPG, PNG, WebP) на стороне сервера
- Генерация уникальных имен файлов через `Guid`
- Транзакционное сохранение: запись на диск + метаданные в БД
- Поддержка сортировки фото через эндпоинт `PUT /api/rooms/{id}/images/order`

### Оптимизация производительности
- **Решение проблемы N+1:** использование `Include()` для eager loading связанных сущностей
- **Динамическая фильтрация:** построение LINQ-запросов через `IQueryable` (условия добавляются только если параметры переданы)
- **Case-insensitive поиск:** приведение к нижнему регистру в LINQ-запросах
- **Оптимизация SQL:** EF Core генерирует один эффективный запрос вместо множества мелких

---

## API Endpoints

### Аутентификация (Auth)
| Метод | Endpoint | Описание | Доступ |
|-------|----------|----------|--------|
| POST | `/api/Auth/register` | Регистрация нового пользователя | Публичный |
| POST | `/api/Auth/login` | Вход, возврат JWT-токена | Публичный |

### Комнаты (Rooms)
| Метод | Endpoint | Описание | Доступ |
|-------|----------|----------|--------|
| GET | `/api/Rooms` | Список комнат (с поддержкой фильтрации и поиска) | Публичный |
| GET | `/api/Rooms/{id}` | Детальная информация о комнате | Публичный |
| POST | `/api/Rooms` | Создание новой комнаты | Admin |
| PUT | `/api/Rooms/{id}` | Редактирование данных комнаты | Admin |
| DELETE | `/api/Rooms/{id}` | Удаление комнаты | Admin |
| GET | `/api/Rooms/{id}/schedule?date=YYYY-MM-DD` | Получение списка занятых слотов на выбранную дату | Публичный |

### Фотографии (Rooms/Images)
| Метод | Endpoint | Описание | Доступ |
|-------|----------|----------|--------|
| POST | `/api/Rooms/{id}/images` | Мульти-загрузка фото (`multipart/form-data`) | Admin |
| DELETE | `/api/Rooms/{id}/images/{imageId}` | Удаление конкретного фото | Admin |
| PUT | `/api/Rooms/{id}/images/order` | Обновление порядка отображения фото (для Drag-and-Drop) | Admin |

### Бронирования (Bookings)
| Метод | Endpoint | Описание | Доступ |
|-------|----------|----------|--------|
| GET | `/api/Bookings` | Получение списка бронирований | User, Admin |
| POST | `/api/Bookings` | Создание нового бронирования с проверкой пересечений | User, Admin |
| GET | `/api/Bookings/{id}` | Детали конкретного бронирования | User, Admin |
| POST | `/api/Bookings/{id}/confirm` | Подтверждение бронирования | Admin |
| POST | `/api/Bookings/{id}/cancel` | Отмена бронирования | User, Admin |

### Клиенты (Clients)
| Метод | Endpoint | Описание | Доступ |
|-------|----------|----------|--------|
| GET | `/api/Clients` | Список клиентов | Admin |
| POST | `/api/Clients` | Создание записи о клиенте | Admin |
| GET | `/api/Clients/{id}` | Детали клиента | Admin |
| PUT | `/api/Clients/{id}` | Редактирование клиента | Admin |
| DELETE | `/api/Clients/{id}` | Удаление клиента | Admin |

>  *Полная интерактивная документация со всеми схемами запросов/ответов доступна через Swagger UI после запуска API.*

---

## 🛠 Установка и локальный запуск

### Требования
- **.NET 8 SDK**
- **PostgreSQL 14+** 

## Запуск через Docker (рекомендуется)
Если у вас установлен Docker Desktop
1. Запустите контейнеры:
```bash
docker-compose up --build -d
```
2. Важно: Примените миграции к базе данных внутри контейнера (при первом запуске):
```bash
cd RoomRental.API
dotnet ef database update --connection "Host=localhost;Port=5433;Database=room_rental_db;Username=postgres;Password=mysecretpassword"
```
После этого API будет доступно по адресу:
Swagger UI: http://localhost:5282/swagger


## Альтернативный способ:
### 1. Клонирование репозитория
```bash
git clone https://github.com/teteik/RoomRental.git
cd RoomRental
```

### 2. Настройка базы данных
Создай базу данных в PostgreSQL:
```bash
createdb room_rental_db
# Или через psql:
psql -U postgres -c "CREATE DATABASE room_rental_db;"
```

Обнови строку подключения в файле `RoomRental.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=room_rental_db;Username=postgres;Password=твой_пароль"
  }
}
```

### 3. Применение миграций
Перейди в папку API и примени миграции для создания структуры БД:
```bash
cd RoomRental.API
dotnet ef database update
```
*Эта команда также выполнит сидинг данных и создаст учетную запись администратора.*

### 4. Запуск приложения
```bash
dotnet run
```
API будет доступно по адресу: `https://localhost:5282`  
Swagger UI: `https://localhost:5282/swagger`

---

## Тестовые данные

При первом запуске (после применения миграций) автоматически создается администратор:
- **Email:** `admin@roomrental.com`
- **Пароль:** `Admin123!`

Для создания обычного пользователя используй эндпоинт `/api/Auth/register` через Swagger или клиентское приложение.

---

## Roadmap (Планы по улучшению)

- [ ] Настройка гибкого расписания доступности комнат (24/7, рабочие часы, выходные)
- [ ] Пагинация для списков комнат и клиентов (`Skip`/`Take`)
- [ ] Покрытие доменного слоя Unit-тестами (xUnit + Moq)
- [ ] Интеграционные тесты API (WebApplicationFactory)
- [x] Docker-контейнеризация (Dockerfile + docker-compose для API и БД)
- [ ] Настройка CI/CD пайплайна через GitHub Actions
- [ ] Структурированное логирование через Serilog
- [ ] Вынос хранения файлов в облачное хранилище (S3 / Azure Blob Storage)

