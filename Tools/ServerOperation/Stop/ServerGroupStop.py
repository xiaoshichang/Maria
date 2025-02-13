import os
import json
import subprocess


class ServerGroupStop(object):

    def __init__(self, args):
        self.args = args
        self.server_group_config = json.load(open(args.ConfigPath))
        self.tool_script_path = os.path.abspath(self.args.TelnetToolScript)

    def stop(self):
        port = self.server_group_config["GMServer"]["TelnetPort"]
        cmd = ["python", self.tool_script_path, "--port", str(port), "--command", "$shutdown"]
        print(cmd)
        subprocess.Popen(cmd)
