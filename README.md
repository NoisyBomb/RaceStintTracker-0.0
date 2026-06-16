# RaceStintTracker

**RaceStintTracker** — это веб-приложение для планирования стинтов в гонках на выносливость. Оно помогает командам распределить пилотов по стинтам, рассчитать время смен и отслеживать прогресс гонки в реальном времени.

---

## Что умеет приложение

### Стинты
- Автоматическая генерация плана стинтов на основе параметров гонки
- Цветовая кодировка пилотов для удобного визуального восприятия
- Редактирование стинтов вручную — можно поменять пилота или количество кругов
- Фильтрация стинтов по выбранной гонке

### Гонки
- Создание гонок с указанием трассы, времени круга, продолжительности, расхода топлива и времени пит-стопа
- Список всех созданных гонок

### Пилоты
- Добавление и удаление пилотов
- Отображение количества стинтов каждого пилота

---

## Как запустить

### Что нужно установить

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) — бесплатная программа для запуска контейнеров

> Больше ничего устанавливать не нужно — всё остальное запустится внутри Docker автоматически.

### Шаги

**1. Скачай проект**

Нажми зелёную кнопку `Code` → `Download ZIP`, распакуй архив. Или если знаешь что такое git:
```bash
git clone https://github.com/ваш-username/RaceStintTracker.git
cd RaceStintTracker
```

**2. Создай файл с паролем**

В папке проекта создай файл `.env` (именно с точкой в начале) и вставь в него:
```
POSTGRES_DB=racestints
POSTGRES_USER=appuser
POSTGRES_PASSWORD=придумай_любой_пароль
```

**3. Запусти**

Открой терминал в папке проекта и выполни:
```bash
docker-compose up --build -d
```

Первый запуск займёт несколько минут — скачаются все необходимые компоненты.

**4. Открой приложение**

Перейди в браузере по адресу: [http://localhost](http://localhost)

---

## Как пользоваться

### Первый запуск

1. Перейди на вкладку **Пилоты** и добавь участников гонки
   <div align="center">
   <img width="767" height="753" alt="image" src="https://github.com/user-attachments/assets/87c0d5e9-a778-436c-9628-b5dc746ba35d" />
   <img width="767" height="837" alt="image" src="https://github.com/user-attachments/assets/89b7df38-320c-4fc7-a0c2-77a4157e4d52" />
   </div>

3. Перейди на вкладку **Гонка** и создай гонку, заполнив параметры:
   - Название и трасса
   - Длительность круга (например `00:02:04`)
   - Продолжительность гонки (например `12:00:00`)
   - Расход топлива на круг и объём бака
   - Время пит-стопа
     <div align="center">
     <img width="771" height="822" alt="image" src="https://github.com/user-attachments/assets/2fce91c7-da2e-4a1b-a544-4fe2c96bb932" />
     <img width="687" height="262" alt="image" src="https://github.com/user-attachments/assets/c1d73324-12e0-4a51-b977-44130a014531" />
     </div>

4. Перейди на вкладку **Стинты**:
   - Выбери гонку из списка
   - Отметь галочками пилотов которые будут участвовать
   - Укажи время старта гонки
   - Нажми **Сгенерировать план**
     <div align="center">
     <img width="1346" height="591" alt="image" src="https://github.com/user-attachments/assets/af33aaec-a444-43d2-a216-574623e2de26" />
     <img width="1310" height="690" alt="image" src="https://github.com/user-attachments/assets/7950dc7c-5cca-4059-a5bd-41dca4913317" />
     </div>



### Редактирование стинта

В таблице стинтов нажми кнопку ✎ напротив нужного стинта — можно поменять пилота и количество кругов.
<div align="center">
<img width="1307" height="542" alt="image" src="https://github.com/user-attachments/assets/ef7637b3-0f24-440e-aec3-c34bbb993f20" />
<img width="1305" height="440" alt="image" src="https://github.com/user-attachments/assets/e2193c86-cc11-40ca-bc47-dfa4bae4fecd" />
</div>


---

## Остановка и перезапуск

Остановить приложение:
```bash
docker-compose down
```

Запустить снова (данные сохранятся):
```bash
docker-compose up -d
```

Полный сброс вместе с базой данных:
```bash
docker-compose down -v
```

---

## Технологии

| Компонент | Технология |
|-----------|-----------|
| Бэкенд | .NET 10, Entity Framework Core |
| Фронтенд | React 19, TypeScript, Vite |
| База данных | PostgreSQL 16 |
| Веб-сервер | nginx |
| Инфраструктура | Docker, docker-compose |
