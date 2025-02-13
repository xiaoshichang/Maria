SET Config=Config\ServerGroupConfig.json
SET TelnetToolScript=..\ServerProcessTelnet\ServerProcessTelnet.py

python ServerGroupOperation.py stop %Config% %TelnetToolScript%
pause