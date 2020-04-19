![logo](logo.png)

## About Chickadee

Chickadee is an APRS client that integrates with DireWolf and can run on Windows, Linux, and Mac.

## About DireWolf

DireWolf is software that acts as a Terminal Node Controller (TNC). It serves as the interface between the computer and the radio through the audio port. It handles encoding packets to send over the radio and decoding packets that are received.

## Chickadee features

* A self-hosted web service providing an interface
    * To view received APRS packets
    * Craft APRS packets to send
* A command-line interface (CLI) which can be used to create APRS messages
* A worker service that runs in the background
    * The service periodically reads and saving received packets from DireWolf
    * The service periodically checks for new messages to send and writes them to DireWolf
