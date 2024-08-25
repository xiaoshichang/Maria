import json
import time
import os
import subprocess
import ctypes
import functools


class ServerGroupStart(object):

    def __init__(self, args):
        self.args = args
        self.server_group_config = json.load(open(args.ConfigPath))

    def start(self):
        raise NotImplementedError()

    def start_process(self, name):
        raise NotImplementedError()


class ServerGroupStartNT(ServerGroupStart):

    def __init__(self, args):
        super().__init__(args)
        self.pids = []
        self.handles = []

    def start(self):
        self.start_process("GMServer")
        time.sleep(0.1)
        game_server_configs = self.server_group_config.get("GameServers", [])
        for i, config in enumerate(game_server_configs):
            self.start_process("GameServer%d" % (i + 1))
            time.sleep(0.1)
        gate_server_configs = self.server_group_config.get("GateServers", [])
        for i, config in enumerate(gate_server_configs):
            self.start_process("GateServer%d" % (i + 1))
            time.sleep(0.1)

        # sleep a small interval, then we can find them using os api
        time.sleep(1)
        self.adjust_console_window()

    def start_process(self, name):
        cmd = [os.path.abspath(self.args.ExecutablePath)]
        flag = 0
        flag |= subprocess.CREATE_NEW_CONSOLE  # create server program in new console, or it will exit when the console run python.exe exit
        env = os.environ.copy()
        env["ConfigPath"] = os.path.abspath(self.args.ConfigPath)
        env["ServerName"] = name
        env["NativeDllSearchPath"] = os.path.abspath(self.args.DllSearchPath)
        print("start server process: %s, %s, %s" % (cmd, env["ConfigPath"], env["ServerName"]))
        p = subprocess.Popen(cmd, creationflags=flag, env=env)
        self.pids.append(p.pid)

    def adjust_console_window(self):
        user32 = ctypes.windll.user32
        callback = ctypes.CFUNCTYPE(ctypes.c_bool, ctypes.c_int, ctypes.c_int)

        f = functools.partial(ServerGroupStartNT.find_windows_callback, self)
        user32.EnumWindows(callback(f), 0)

        for i, handler in enumerate(self.handles):
            row = int(i / 3)
            col = int(i % 3)
            r = user32.MoveWindow(handler, col * 800, row * 500, 800, 500, 1)

    def find_windows_callback(self, hwnd, extra):
        user32 = ctypes.windll.user32
        process_id = ctypes.c_int(0)
        process_id_point = ctypes.pointer(process_id)
        user32.GetWindowThreadProcessId(hwnd, process_id_point)
        if process_id.value in self.pids:
            self.handles.append(hwnd)


class ServerGroupStartLinux(ServerGroupStart):

    def __init__(self, args):
        super().__init__(args)

    def start(self):
        pass

    def start_process(self, name):
        pass
