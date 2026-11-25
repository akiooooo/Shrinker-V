# Локальные зависимости

Сборка использует три управляемые DLL из совместимого комплекта CodeWalker. Если у вас есть прежняя рабочая сборка, эти файлы находятся в `.waka_safe_runtime/managed_runtime`. Скопируйте их в `local/managed` или передайте каталог в `build.ps1 -DependencyDir`.

| Файл | SHA-256 проверенной сборки |
| --- | --- |
| CodeWalker.Core.dll | `0DE982EAA2F0BC93BE5021192E5DFF2A2D16F6F9E1B99639840511DAF85F4400` |
| SharpDX.dll | `A0AF255EA4B09A8CDB995B8C6FD1075E46F098E23C2351C974E6DED9B8B620CF` |
| SharpDX.Mathematics.dll | `4C9615496970EA84320E2A6E99F8FB828E3C7790384DF5585D93FC368885D94E` |

Для зашифрованных RPF используйте согласованный комплект данных из своего рабочего окружения. В прежней сборке он лежал в `.waka_safe_runtime/archivefix_runtime`. Сами файлы хранятся локально. Исполняемый файл ArchiveFix для этой версии не требуется.

| Режим | Файлы в каталоге runtime |
| --- | --- |
| Работа с AES и NG | `gtav_aes_key.dat`, `gtav_ng_key.dat`, `gtav_ng_decrypt_tables.dat`, `gtav_ng_encrypt_tables.dat`, `gtav_ng_encrypt_luts.dat`, `gtav_hash_lut.dat` |
| Замена звука | `embedded_timer.awc` |
| Замена эффектов | `embedded_cut_iaaj.ypt` |
| Удаление геометрии карты | `minimal_ydr.stub`, `minimal_yft.stub`, `minimal_ybn.stub` |

Три файла для карты в старом комплекте называются `dt1_rd1_props_busroof_007.ydr`, `warrener_null2.yft` и `ch2_roads_14.ybn` соответственно. Скопируйте их под именами из таблицы для использования этого режима. При отсутствии файла для выбранной замены API выдаёт ошибку до начала записи архивов.
