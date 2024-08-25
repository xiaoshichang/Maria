import argparse
import os
from Start.ServerGroupStart import ServerGroupStartNT
from Stop.ServerGroupStop import ServerGroupStopNT


def start_server_group(args):
    print("start_server_group")
    if os.name == "nt":
        starter = ServerGroupStartNT(args)
    else:
        raise NotImplementedError()
    starter.start()


def stop_server_group(args):
    if os.name == "nt":
        stopper = ServerGroupStopNT(args)
    else:
        raise NotImplementedError()
    stopper.stop()


def parser_args():
    parser = argparse.ArgumentParser()
    subparser = parser.add_subparsers(help="operations to be perform")

    subparserStart = subparser.add_parser("start", help="Start server group")
    subparserStart.add_argument("ExecutablePath", help="server exe path")
    subparserStart.add_argument("ConfigPath", help="config path of server group")
    subparserStart.add_argument("DllSearchPath", help="the path to search native dll")
    subparserStart.set_defaults(func=start_server_group)

    subparserStop = subparser.add_parser("stop", help="Stop server group")
    subparserStop.add_argument("ExecutableName", help="server exe name")
    subparserStop.set_defaults(func=stop_server_group)

    return parser.parse_args()


def main():
    args = parser_args()
    args.func(args)


if __name__ == "__main__":
    main()





