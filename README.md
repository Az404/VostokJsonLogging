# VostokJsonLogging

## ConsoleLogJson

Реализует асинхронное логирование в формате JSON-объектов на основе `ConsoleLog`. Каждая строка лога представляет собой сериализованный (с ограничениями) экземпляр `Vostok.Logging.Abstractions.LogEvent`.

```json
{"Level":1,"Timestamp":"2022-10-19T16:37:59.7642418+05:00","Message":"msg","Properties":{"key":"value"},"Exception":{"Type":"System.FormatException","Message":"Invalid format","StackTrace":"<...>","InnerException":null}}
```

В отличие от оригинального `ConsoleLog`, `ConsoleLogJson` не позволяет указать `OutputTemplate`, т.к. в JSON-объекте присутствуют все возможные поля, включая все `Properties`.

Сериализованные объекты можно восстановить обратно в `LogEvent` и вновь залогировать.

Ограничения:

- `MessageTemplate`
    - Не сохраняется при сериализации.
- `Message`
    - В сериализованном `LogEvent` содержит итоговое сообщение лога (результат подстановки `Properties` в `MessageTemplate`, все указанные форматы учитываются).
    - При десериализации записывается в `MessageTemplate`.
- `Properties`
    - Поскольку значения могут быть произвольными объектами, при сериализации выполняется их преобразование в строки.
        - В `MessageTemplate` может быть указан формат рендеринга объекта, например `{intValue:X}`. Решено не использовать его по 2 причинам:
            - так формат значений `Properties` всегда будет фиксированным
            - нужна сложная реализация (разбор шаблона internal-средствами Vostok)
        - Даты форматируются по ISO8601: `"2022-10-19T16:37:59.7642418+05:00"`
        - Остальные объекты форматируются идентично рендерингу с шаблоном `{key}`
        - Референсный код:

          [logging.hercules/HerculesTagsBuilderExtensions.cs at 43a2363a1ccf92ea52dc555eb28792b1fe8e0c87 · vostok/logging.hercules](https://github.com/vostok/logging.hercules/blob/43a2363a1ccf92ea52dc555eb28792b1fe8e0c87/Vostok.Logging.Hercules/HerculesTagsBuilderExtensions.cs#L24)

- `Exception`
    - Полноценная сериализация невозможна (выброшенное исключение содержит ссылку на метод) и не требуется для целей логирования.
        - Сохраняются только те поля, которые участвуют в построении строкового представления. Пример из кода Vostok:

          [logging.formatting/ExceptionToken.cs at fd7c6e990d00215bd55b6bfb40567c524f78cdcc · vostok/logging.formatting](https://github.com/vostok/logging.formatting/blob/fd7c6e990d00215bd55b6bfb40567c524f78cdcc/Vostok.Logging.Formatting/Tokens/ExceptionToken.cs#L26)

        - `AggregateException` (и другие типы исключений с дополнительными данными) специально не обрабатываются.
          Для `AggregateException` будет сохранён только первый `InnerException`, остальные будут представлены только сообщениями в поле `Message`.
    - Для целей логирования десериализация в конкретный тип (такой как `FormatException`) не требуется.
        - Десериализация происходит в фиксированный тип `ExceptionDataWrapper`, сериализованное представление которого идентично сериализации исходного исключения.