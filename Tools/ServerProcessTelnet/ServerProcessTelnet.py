import socket
import os
import argparse

delimiter = "\r\n"

parser = argparse.ArgumentParser(description='a command line tool used to telnet to server progress.')
parser.add_argument("--port", help="the port of the socket to connect.")
parser.add_argument("--command", help="command to be sent to server process")
parser.print_help()


class TelnetTool(object):

    def __init__(self, args):
        self.port = int(args.port)
        self.command = args.command
        self.socket = None
        self.buffer = b''

    def run(self):
        self.connect()
        self.clear_screen()
        self.receive_welcome()

        if self.command is None:
            self.start_interact_mode()
        else:
            self.start_command_mode()

    def connect(self):
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.socket.setblocking(True)
        address = ("localhost", self.port)
        self.socket.connect(address)

    @staticmethod
    def clear_screen():
        os.system('cls' if os.name == 'nt' else 'clear')

    def receive_welcome(self):
        message = self.receive()
        print(message)

    def start_command_mode(self):
        processed = self.process_user_input(self.command)
        self.socket.send(processed)
        while True:
            message = self.receive()
            print(message)
            if message is None:
                print("exit...")
                break

    def start_interact_mode(self):
        while True:
            raw = input(">>> ")
            processed = self.process_user_input(raw)
            self.socket.send(processed)
            message = self.receive()
            print(message)

    def receive(self):
        try:
            sep = delimiter.encode("ascii")
            while sep not in self.buffer:
                data = self.socket.recv(1024)
                if not data:
                    return None
                self.buffer += data
            line, sep, self.buffer = self.buffer.partition(sep)
            return line.decode("ascii")
        except ConnectionResetError:
            return None

    @staticmethod
    def process_user_input(raw):
        replaced = raw.replace(delimiter, "\n")
        formatted = replaced + delimiter
        return formatted.encode("ascii")


if __name__ == '__main__':
    args = parser.parse_args()
    tool = TelnetTool(args)
    tool.run()
