import os


class ServerGroupStop(object):

    def __init__(self, args):
        self.args = args

    def stop(self):
        raise NotImplementedError()


class ServerGroupStopNT(ServerGroupStop):

    def __init__(self, args):
        super().__init__(args)

    def stop(self):
        os.system("taskkill /f /im  " + self.args.ExecutableName)


class ServerGroupStopLinux(ServerGroupStop):
    def __init__(self, args):
        super().__init__(args)

    def stop(self):
        pass
