SET executable=..\..\Server\MariaServer\Maria.Server\bin\Debug\net8.0\Maria.Server.exe
SET config=Config\ServerGroupConfig.json
SET dllSearchPath=..\..\Server\MariaServerNative\cmake-build-debug

python ServerGroupOperation.py start %executable% %config% %dllSearchPath%