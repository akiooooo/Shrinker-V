# Компоненты Shrinker V by akiooo

Для запуска требуется Windows x64 с .NET Framework 4.8. EXE включает ShrinkerV.Core, три управляемые библиотеки и данные runtime. Для интеграции через C# достаточно ShrinkerV.Core.dll из того же релиза. CodeWalker читает таблицы с диска, поэтому при первом задании процесс распаковывает их во временную папку и удаляет её при штатном завершении.

| Компонент | Автор и условия |
| --- | --- |
| SharpDX и SharpDX.Mathematics 4.2.0 | Alexandre Mutel и участники SharpDX. MIT. [Полный текст лицензии](licenses/SharpDX-LICENSE.txt), [исходники версии 4.2.0](https://github.com/sharpdx/SharpDX/tree/v4.2.0). |
| CodeWalker.Core 1.0.0.0 | dexyfex и авторы компонентов, указанные в [Notice.txt](licenses/CodeWalker-Notice.txt). Условия публикации исходного кода находятся в [Readme_Src.txt](licenses/CodeWalker-Readme_Src.txt). Ссылки на исходные проекты и сторонние компоненты приведены в [репозитории CodeWalker](https://github.com/dexyfex/CodeWalker). |
| Данные для GTA V | Таблицы чтения и записи RPF, образцы AWC и YPT и три ресурса для режима удаления карты. Права на материалы игры принадлежат соответствующим правообладателям. |

Владелец проекта подтвердил разрешение на публикацию данной сборки CodeWalker.Core и комплекта runtime в составе Shrinker V by akiooo. Это подтверждение относится к поставке проекта. Лицензия MIT в таблице относится к SharpDX. Тексты уведомлений входят в исходники и встроены в библиотеку.

## Состав сборки

| Файл | SHA-256 |
| --- | --- |
| CodeWalker.Core.dll | `0DE982EAA2F0BC93BE5021192E5DFF2A2D16F6F9E1B99639840511DAF85F4400` |
| SharpDX.dll | `A0AF255EA4B09A8CDB995B8C6FD1075E46F098E23C2351C974E6DED9B8B620CF` |
| SharpDX.Mathematics.dll | `4C9615496970EA84320E2A6E99F8FB828E3C7790384DF5585D93FC368885D94E` |

Исходный проект берёт DLL из `vendor/managed`, а данные из `vendor/runtime.zip`. Для сборки со своим комплектом можно передать пути через `build.ps1 -DependencyDir` и `-RuntimeDir`. Переменная `SHRINKER_V_RUNTIME_DIR` позволяет явно выбрать внешний каталог данных при запуске.
