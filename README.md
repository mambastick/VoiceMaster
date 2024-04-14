# 🔊 VoiceMaster
**VoiceMaster** - это **Discord бот**, который поможет вам настроить временные голосовые каналы на вашем Discord сервере.

## 🏗️ Установка
<details>
<summary>Docker</summary>

### 🐳 Установка с помощью Docker

1. Склонируйте репозиторий следующей командой:
```
git clone https://github.com/mambastick/VoiceMaster/
```

2. Отредактируйте файл `docker-compose.yml`. Вам нужно заменить все **environment**, в значении которых вы видите `YOUR`, а именно:
- `BOT_TOKEN` - Этот токен нужно получить на сайте [Discord Developer Portal](https://discord.com/developers/applications/)
- `WEBHOOK_ID` и `WEBHOOK_URL` - Нужно создать на своем Discord сервере Webhook. Инструкция по созданию Webhook есть на [Discord Support](https://support.discord.com/hc/ru/articles/228383668-Использование-Webhooks)
- `DATABASE_NAME` и `MYSQL_DATABASE` - Имя базы данных
- `DATABASE_USER` и `MYSQL_USER`- Имя пользователя, который будет иметь доступ к `DATABASE_NAME`
- `DATABASE_PASSWORD` и `MYSQL_PASSWORD` - Пароль от пользователя `DATABASE_USER`, который имеет доступ к базе данных
- `MYSQL_ROOT_PASSWORD` - Пароль от root пользователя базы данных

3. Запустить проект, используя команду:
```
docker-compose up -d
```
</details>

<details>
<summary>Ручная установка</summary>

### 🛠️ Установка в ручном режиме
1. Склонируйте репозиторий следующей командой:
```
git clone https://github.com/mambastick/VoiceMaster/
```

2. Откройте директорию `VoiceMaster`, в которой лежит `VoiceMaster.csproj`
3. Запустите компиляцию проекта:
- Для Linux:
```
dotnet build -c Release -r linux-x64 -o /путь/к/папке/назначения
```
- Для Windows:
```
dotnet build -c Release -r win-x64 -o /путь/к/папке/назначения
```

4. После успешной компиляции перейдите в `/путь/к/папке/назначения`, который вы указали при компиляции и создайте там файл `.env`, в который поместите следующее:
```
# Токен бота, который нужно получить на https://discord.com/developers/applications/
BOT_TOKEN=

# Webhook, который необходимо создать на вашем Discord сервере, инстркуцию можно найти здесь: https://support.discord.com/hc/ru/articles/228383668-Использование-Webhooks
WEBHOOK_ID=
WEBHOOK_URL=

# У вас уже должна быть создана база данных MySQL и необходимый пользователь, поэтому просто вставьте сюда свои данные
DATABASE_HOST=
DATABASE_PORT=
DATABASE_NAME=
DATABASE_USER=
DATABASE_PASSWORD=
```

5. Запустите проект, используя исполняемый файл `VoiceMaster`
</details>

## 📘 Инструкция по использованию
1. **Добавить Discord бота** на Discord сервер
2. **Нажать на зеленую кнопку** `Установить` в приветственном сообщении бота или же **использовать команду** `/setup`
3. На вашем Discord сервере появится категория `VoiceMaster`, в которой будет голосовой канал `Создать канал`, **зайдя в этот канал**, **вы** автоматически **создадите** свой временный канал

## 🔗 Зависимости проекта
- [DSharpPlus](https://www.nuget.org/packages/DSharpPlus)
  - [DSharpPlus.SlashCommands](https://www.nuget.org/packages/DSharpPlus.SlashCommands)
- [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore)
  - [Microsoft.EntityFrameworkCore.Tools](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools)
  - [MySql.EntityFrameworkCore](https://www.nuget.org/packages/MySql.EntityFrameworkCore)
- [Polly](https://www.nuget.org/packages/Polly/)
- [Serilog](https://www.nuget.org/packages/Serilog/)
  - [Serilog.Extensions.Logging](https://www.nuget.org/packages/Serilog.Extensions.Logging)
  - [Serilog.Sinks.Async](https://www.nuget.org/packages/Serilog.Sinks.Async)
  - [Serilog.Sinks.Console](https://www.nuget.org/packages/Serilog.Sinks.Console)
  - [Serilog.Sinks.Discord](https://www.nuget.org/packages/Serilog.Sinks.Discord)
  - [Serilog.Sinks.File](https://www.nuget.org/packages/Serilog.Sinks.File)
