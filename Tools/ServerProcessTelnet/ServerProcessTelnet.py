import socket
import os
import argparse

delimiter = "\r\n"

parser = argparse.ArgumentParser(description='a command line tool used to telnet to server progress.')
parser.add_argument("--port", help="the port of the socket to connect.")
parser.print_help()


class TelnetTool(object):

    def __init__(self, port):
        self.port = port
        self.socket = None
        self.buffer = b''

    def run(self):
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.socket.setblocking(True)

        address = ("localhost", self.port)
        self.socket.connect(address)

        os.system('cls' if os.name == 'nt' else 'clear')

        message = self.receive()
        print(message)

        while True:
            raw = input(">>> ")
            processed = self.process_user_input(raw)
            self.socket.send(processed)
            message = self.receive()
            print(message)

    def receive(self):
        sep = delimiter.encode("ascii")
        while sep not in self.buffer:
            data = self.socket.recv(1024)
            if not data:
                return None
            self.buffer += data
        line, sep, self.buffer = self.buffer.partition(sep)
        return line.decode("ascii")

    @staticmethod
    def process_user_input(raw):
        replaced = raw.replace(delimiter, "\n")
        formatted = replaced + delimiter
        return formatted.encode("ascii")


if __name__ == '__main__':
    args = parser.parse_args()
    tool = TelnetTool(int(args.port))
    tool.run()
