![alt text][logo]

[logo]: https://raw.githubusercontent.com/MarneeDear/chickadee/master/logo.png "Chickadee"

> APRS is a protocol for widely spreading messages over an ad-hoc radio network while avoiding redundancy and network congestion.

# Chickadee (F# for APRS)

A system for sending APRS messages built in F#, because functional programming and static typing make everything better.

> Mac and Linux users: You too can work on this code. Install the .NET Core SDK for your system and you can do all the things. I recommend installing Visual Studio Code with the Ionide plugin for development.

A work in progress.

## Prior Art

Here is a similar system using a Kantronics packet radio setup that functions like a BBS with keyboard-to-keyboard communications.

[![Packet Radio](http://img.youtube.com/vi/FJEVWMuz6Xg/0.jpg)](http://www.youtube.com/watch?v=FJEVWMuz6Xg "Kantronics Packet Radio Mail and BBS Operations")

## The architecture

### System

TODO draw a diagram

#### Requirements

* .NET Core SDK 3.1 and above
* [Dire Wolf v1.5](https://github.com/wb2osz/direwolf/releases/tag/1.5)
* SQLite3

#### Dependencies

See the project files, but here are the highlights.

* .NET Core SDK v3.1 and above 
* [Argu](https://github.com/fsprojects/Argu) for the CLI
* [Expecto](https://github.com/haf/expecto) for unit tests

### Program

The design calls for 3 main parts:

* A service that runs on the APRS server (work in progress).
  * This service picks up new APRS messages from Dire Wolf. The messages are written in TNC2MON format to a designated folder by the DireWolf `kissutil`.
* A CLI that can be used to write TNC2MON format frames.
  * They can be written to a folder monitored by the `kissutil`. When the `kissutil` detects a new file DireWolf will process the frames and transmit.
  * Presently only produces TNC2MON formatted messages with `Lat/Lon Position Report without Timestamp`, and a plain text message
* A web app (UI) that can be used to compose APRS packets that will be used by the `kissutil`. I have plans to support a number of APRS data formats. At the moment you can compose Message types but not Position Reports through the web UI.
* A background service that reads from the `kissutil` and saves the received packets to the database.

## How to setup and run

First, make sure you have .NET Core SDK 3.1 or above installed.

After cloning this repo you can restore the dependencies, run the migrations, run the tests, run the CLI, run the Web UI, or run the background server with [FAKE](https://fake.build/) script. FAKE is a DSL for build tasks that will run any `dotnet` command.

Once you can cloned the repo, `cd` into the root of the repo and do the setup steps:

1. Install the dotnet tools
    1. `dotnet tool restore`
2. Install the dependencies (packages)
    1. `dotnet packet install`
    2. This will run `paket`, download dependencies, and link references in all projects
3. Run the data migrations
    1. `dotnet fake build target MigrateUp`
    2. This will create the database where sent and received packets are stored
4. Run the tests
    1. `dotnet fake build target Test`
    2. This will run tests. If any tests fail you will not be able to run the application.
5. Run the build and launch the Web UI
    1. `dotnet fake build target Build`. If the build passes (no errors) you can run the web UI
    2. `dotnet fake build target RunWeb`
    3. This will launch the web UI in a browser window.

### Run the CLI and see the possible commands

From the root project folder (the folder that was created when you cloned this repo)

```cmd
dotnet run --project src/chickadee.cli/ -- --help
```

You should see the help output looking like this:

```cmd
USAGE: chick [--help] --sender <sender> [--destination <destination>] [--path <path>] [--savefilepath <save>] [--parseframe <frame>]
             [<subcommand> [<options>]]

SUBCOMMANDS:

    --positionreport, --rpt <rpt>
                          Position Reports require a Latitude and Longitude. See Position Report usage for more.
    --custommessage, --msg <msg>
                          Unformatted message. Anything you want but cuts off at 63 chars. in length

    Use 'chick <subcommand> --help' for additional information.

OPTIONS:

    --sender, -s <sender> Your Call Sign. This is required for all commands.
    --destination, -d <destination>
                          To whom is this intended? This could also be a an application from the To Calls list http://aprs.org/aprs11/tocalls.txt
    --path, -p <path>     Only option is WIDE1-1
    --savefilepath, --save-to <save>
                          Send output to a file in this location to be used by Dire Wolf's kissutil
    --parseframe, --parse-frame <frame>
                          Provide a raw frame and I'll check it out to see what it means
    --help                display this list of options.
```

There are also sub-commands for the `position report`. You can get help for those too.

```cmd
dotnet run --project src/chickadee.cli/ -- --rpt --help
```

You should see some helpful stuff like this.

```cmd
USAGE: chick --positionreport [--help] latitude <latitude> longitude <longitude> [symbol <symbol>] [comment <comment>]

OPTIONS:

    latitude, --lat <latitude>
                          Your current latitude in decimal coordinates (simple standard) format
    longitude, --lon <longitude>
                          Your current longitude in decimal coordinates (simple standard) format
    symbol, -s <symbol>   Optional. Default is House (-). If you want to use House, do not use the symbol argument because dashes do not parse.
    comment, -c <comment> Optional. What do you want to say? <comment> must be 43 characters or fewer.
    --help                display this list of options.
```

#### Create a TNC2MON formatted frame with position report

```cmd
dotnet run --project src/chickadee.cli/ -- --save-to XMIT --sender KG7SIO-7 --destination APDW15 --path WIDE1-1 --rpt latitude 36.106964 longitude -112.112999 symbol b comment "You can't stop the signal."
```

This will create a TNC2MON formatted frame with a lat/lon position report that looks like this:

```text
KG7SIO>APDW15:WIDE1-1:=36.106964N/112.112999WbYou can't stop the signal.
```

The CLI will save it to the folder (and path) specified in `--save-to`. In this case the XMIT folder (if you have one) in your `present working directory.`

Let's break this down:

* Who is sending this packet?
  * `KG7SIO`
* The destination in this case is the Dire Wolf v1.5 `TOCALL` as [specified in APRS 1.1.](http://www.aprs.org/aprs11/tocalls.txt). The destination field can be overridden to indicate the sending application.
  * `APDW15`
* Your position is `36.106964 degrees N` and `112.112999 degrees W`
* Your APRS symbol is `b` for `bicycle`
* Your comment is `You can't stop the signal.`

#### Create a TNC2MON formatted frame with unformatted message (string)

```bash
dotnet run --project src/chickadee.cli/ -- --save-to XMIT --sender KG7SIO-7 --destination APDW15 --path WIDE1-1 --msg -a "KG7SIO" -m "Join Oro Valley Amateur Radio Club"
```

This will create a TNC2MON formatted frame with a custom message. The output should look like this:

```cmd
This is what you want me to do [SaveFilePath "XMIT"; Sender "KG7SIO-7"; Destination "APDW15"; Path "WIDE1-1";
 CustomMessage [Addressee "KG7SIO"; Message "The spice must flow."]]
Successfully parsed your packet. Here is what I got.
APRS PACKET: KG7SIO-7>APDW15,WIDE1-1::KG7SIO   :The spice must flow.{00000
SENDER : CallSign "KG7SIO-7"
DESTINATION : CallSign "APDW15"
INFORMATION : ":KG7SIO   :The spice must flow.{00000"
ADDRESSEE: CallSign "KG7SIO"
MESSAGE: MessageText "The spice must flow."
NUMBER: Some MessageNumber "00000"
```

The APRS packet:

```cmd
APRS PACKET: KG7SIO-7>APDW15,WIDE1-1::KG7SIO   :The spice must flow.{00000
```

The CLI will save it to the folder (and path) specified in `--save-to`. In this case the XMIT folder (if you have one) in your `present working directory.`

#### Parse a given packet

Do you have a packet and want to know what is means? You can use the CLI to parse the data formats that are currently supported. You can also use the CLI to find out the type of data format of the data formats that are not fully supported at this time.

##### Data formats current supported

* Messages
  * Message
  * Message Acknowledgements
  * Message Rejections
* Position Reports

##### How to parse a packet

```cmd
dotnet run --project src/chickadee.cli/ -- --save-to XMIT --sender KG7SIO-7 --parseframe "KG7SIO-7>APDW15,WIDE1-1::KG7SIO   :The spice must flow.{00000"
```

You should see output like this:

```cmd
This is what you want me to do [SaveFilePath "XMIT"; Sender "KG7SIO-7";
 ParseFrame "KG7SIO-7>APDW15,WIDE1-1::KG7SIO   :The spice must flow.{00000"]
Successfully parsed your packet. Here is what I got.
APRS PACKET: KG7SIO-7>APDW15,WIDE1-1::KG7SIO   :The spice must flow.{00000
SENDER : CallSign "KG7SIO-7"
DESTINATION : CallSign "APDW15"
INFORMATION : ":KG7SIO   :The spice must flow.{00000"
ADDRESSEE: CallSign "KG7SIO"
MESSAGE: MessageText "The spice must flow."
NUMBER: Some MessageNumber "00000"
```


## Developers and contributors

Contributors welcome. Please follow the [F# Style Guide](https://docs.microsoft.com/en-us/dotnet/fsharp/style-guide/) and [open source contributors guide.](https://opensource.guide/how-to-contribute/)

### How to get started

#### Setup the appsettings file for the Web UI

TODO

#### Steps to build and run

There is a `DOCKERFILE` if you are so inclined, but I haven't tested it.
> The Dockerfile has not been tested lately and may not work so good. MMD 10/26/2019 

## Publish and deploy to a Raspberry PI

> This assumes some familiarity with Linux and Raspberry Pi.

To create a package that you can copy to and run on a Raspberry Pi, you first need to publish a `self-contained` package specifying the `linux-arm` runtime identifier.

Find out more about publishing and runtime identifiers [here](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog).

```bash
dotnet publish -r linux-arm --self-contained -o ../../publish src/chickadee.cli
```

This will create a bunch of files in the `publish` directory, two folders up from the src directory `src/chickadee.cl`. You can change that path to suit your needs.

Once that is done you can copy to your Raspberry Pi and setup a command reference in `usr/bin`. TODO

1. Use `scp` or `rsync` to copy the publish directory and all of its contents to a location on the Raspberry Pi. It can be your home directory.

#### Setup a command alias in two ways

##### Use `bash aliases` to create a command alias that points to the location of the files you copied to the Pi.

1. Use your favorite editor to open your .bashrc file (for example, `nano`)
2. At the end of the file add a line that looks like this
```text
alias ckdee='<path to your published files>/chickadee.cli'
```
3. Save the file and go back to the command line
4. On the command line, execute `bashrc`
```bash
. ~/.bashrc
```
3. On the command line, test that this worked
```bash
ckdee --help
```

This makes the `ckdee` command only available to you. To make it available to all users you will need to copy the published directory to `/usr/local/bin`.

To make this less ambiguous, rename the `publish` directory to `aprs-chickadee`.

1. Use `cp` to copy `aprs-chickadee` to `usr/local/bin`
2. 

## Working with Dire Wolf and the `kissutil`

Follow the Dire Wolf user guide to install Dire Wolf on your system. Follow the user guide to configure and start Dire Wolf.

### Start the `kissutil` specifying the read and write folder

```bash
kissutil -o REC -f XMIT
```

`-o` specifies the folder to which Dire Wolf will write the received APRS messages.

`-f` specifies the folder from which Dire Wolf will read the messages Chickadee will send.

### Use Chickadee to send a message through `kissutil`

Write a Position Report without Timestamp to the `XMIT` folder.

```bash
dotnet run --project src/chickadee.cli/ -- --save-to XMIT --sender KG7SIO --destination APDW15 --path WIDE1-1 --msg "Join Oro Valley Amateur Radio Club"
```

## DireWolf tips

Debugging tip:  Use the DireWolf "-d n" command line option to print the KISS frames in hexadecimal so we can see what is being sent.

```bash
direwolf -d n
```

### Running DireWolf with RTL-SDR devices

For testing I setup my Baofeng connected to my desktop PC, running DireWolf and Chickadee, and a Nooelc RTL-SDR connected to a Raspberry Pi 3 running DireWolf, RTL-FM, and Chickadee. The RTL-SDR software seems only work on Linux.

DireWolf provides documentation about use SDR in the guide `Raspberry-Pi-SDR-IGate.pdf` (look in the reference-materials of this repo), but I will summarize what I do, here. The guide describes how and what to install to get it to work.

#### Calibrating RTL-SDR

Attach the RTL_SDR and run the `rtl_test`. (You will need to install the rtl-sdr software first).

```bash
rtl_test -p60
```

Let this run for at least 5 minutes. The last PPM reported with the value you will use.

```text
real sample rate: 2048132 current PPM: 65 cumulative PPM: 65
real sample rate: 2048124 current PPM: 61 cumulative PPM: 63
real sample rate: 2048129 current PPM: 63 cumulative PPM: 63
```

Run `direwolf` with the PPM calibration value. The flag is `-p`

```bash
rtl_fm -p 63 -f 144.39M - | direwolf -d n -c sdr.conf -r 24000 -D 1 - 
```
