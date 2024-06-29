# TM API
![Рисунок1](https://github.com/lexa56720/TMApi/assets/84237338/be88c313-edf9-46ac-8c2d-2d9a8ee8f426)

## Описание
Представляет собой собой API и серверную часть мессенджера TM.
Документация по всем методам API находится [тут](https://github.com/lexa56720/TMApi/wiki/API).

В репозитории включены следующие элементы:
* 'ApiTypes' - Все типы которые использует API 
* 'TMApi' - библиотека для запросов к API 
* 'TMClient' - тестовый клиент
* 'TMServer' - сервер мессенджера TM

## Установка
Распаковать и запустить. Перед запуском рекомендуется настроить конфигурационный файл сервера.

## Использование
Можно использовать для развёртки своего собственного сервера. Также библиотеку TMApi можно использовать для создания собтсвенных клиентов мессенджера.

## Зависимости
Проект использует следующие DLL-библиотеки:
- `ApiTypes/Lib/AutoSerializer.dll` (https://github.com/lexa56720/CSDTP)
- `ApiTypes/Lib/CSDTP.dll` (https://github.com/lexa56720/CSDTP)
- `ApiTypes/Lib/PerformanceUtils.dll` (https://github.com/lexa56720/CSDTP)
