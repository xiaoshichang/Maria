import os


class ServerGroupKill(object):

    def __init__(self, args):
        self.args = args

    def kill(self):
        raise NotImplementedError()


class ServerGroupKillNT(ServerGroupKill):

    def __init__(self, args):
        super().__init__(args)

    def kill(self):
        os.system("taskkill /f /im  " + self.args.ExecutableName)
