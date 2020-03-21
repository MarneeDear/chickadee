# OVARC APRS TALK

HI everyone. Today I am going to talk about something that has been a big focus and hobby of mine in the past year.

## What does APRS stand for, anyway?

Automatic Packet Reporting System

APRS lives in the world of digital packet radio. So it's one of those modes that makes lots of scratchy weird noises.

APRS was created by Bob Bruninga, WB4APR

And you can hear it on
144.390

This is the commonly used frequency, but technically you can use other FM frequencies.


## So it's just another digital mode?

No!

APRS is much, much more! Let's dive in.

### What Wikipedia thinks it is

APRS is a system for real-time digital communications of information of immediate value in the local area.

### What the APRS specification thinks it is

APRS is a packet communications protocol for disseminating live data to everyone on a network in real time.

## What does that mean, anyway?

Let's break it down ... .

## APRS is two things

* A communications protocol.
* Data formats

## Communications protocol

Let's dive into how it works and why APRS is special

## How data moves from station to station

Talk about the animation

## The Stations

* Transmitting and receiving stations
* Digipeaters
* IGates

## Repeating and Data Sharing

Some stations will transmit and receive APRS data, but they just work like a regular radio.

But we also have stations with specialized functions.

* IGates
* Digipeaters

## IGates

IGates share data on other networks. They usually send packets to a service called aprs.fi, which then publishes the data and plots station positions on a map.

But you could setup your system and publish to that. For example, you might want to setup your own APRS.fi on the local AREDN WiFi mesh network.

## Digipeater path

Every APRS packet will have a PATH. This tells the digipeatrs how to repeat the message and helps prevent congestion while improving reliability.

The path tells the digipeater if it needs to transmit the packet or if it has reached as far as it needs to go.

"How many jumps will the packet take?"
"Do I need to repeat this or are we good?

Let's look at that again. 

Depending on the PATH the packet may or may not make it to the furthest digipeaters.

## The data formats

APRS has lots of them.

Here is a big list!

Notice the one with the yellow arrow. USER DEFINED. This means we can create our own data formats for whatever we want.

## Sample packets from aprs.fi

Whooo boy!

## Whoa, what was all of that?

It sure looks like a bunch of gobbldeygook, but the APRS specification will tell you have to decode the messages to know what they mean.

## Let's look at a couple of examples.

Jim is funny.

## Position Report

Most of the packets are some kind of position report, which tells the networks where the station is located. This can be used to track a moving object or mobile station.

And aprs.fi will put you on the map!

## Weather report

Another common one is weather reports.

Basically this tell the network the local weather conditions.

How's the weather out there?

## What is APRS good for, anyway?

Common uses

So, that's cool and all but what good is APRS anyway.

Let's look at a few examples.

### Announcements

You can send out a message about a special event it's location. For example, a Ham Fest.

### Event communications support and participant tracking

You could use it an event like 24 hours in the old pueblo so share data about participant status, medical aid, and trail conditions.

### Telemetry

You could use it to track the location of your weather balloon, or other flying experiments!

### Real-time crisis operations

There are two big reasons why APRS is great in crisis operations.

* No coordination
    * Share data over a large area with little to no coordination.
* Anyone can do it
    * The equipment you need it inexpensive
    * It is easy to standup an APRS station

### Create your own!

Remember that User Defined data format? You can create your data format to suit your particular event or communications needs. That's fun!

## How can I do that?

Sound great? But how can you create your user defined data format and use it? 

Well, I did it with programming. 

## Let's dive into it.

I learned a lot during the process of programming my own APRS client. Let's dive into it!

## AX.25 Data Link Layer Protocol

First thing! I learned that APRS is centered around something called AX.25. See the Information Field? This is where the APRS data will go.

## Turns your data into noise

AX.25 is an old timey data link layer protocol. Basically it turns your data into screechy modem noises.

Try to play it from APRSDroid

## Linux support

And it turns out that Linux ships with support for AX.25. Pretty cool!

## Data link layer programming??

Ok, so when I figured this out I thought:

"Am I going to have to write an interface to a data link layer protocol?" I mean, Im a good programmer, but I'm not that kind of programmer.

I secretly have no idea what I am doing.

## Direwolf software

As it turns out, NO, I don't have to interface with the data link layer. I can just used Direwolf.

Direwolf is something called a "software terminal node controller". It is going to do all the heavy lifting for me. 
It is going to turn my data into an AX.25 packet, which will go out over the audio port on my computer. 

It will also decode any transmissions it receives over the audio port on my computer.

## Write files and read files

So then all I have to do is write to a file that Direolf will process.

And I can read from a file that Direwolf will produce.

Go through the diagram.

## Kiss interface

Ok, what exactly do we need to write to a file? What will the data look like?

Direwolf provides something called the KISS interface. The KISS interface processes TNC2 MON formatted frames.

Let's find out what that looks like.

## What do we put in the file?

Notice that it looks like a simplified version of the AX.25 frame.

Talk about the structure.

## The software I wrote

I developed some software that will produce APRS packets in the TNC2 MON format, and can process TNC2 MON formatted frames. 

So far it does not support all data formats, but it does support:

* Position report without timestamp
* Message
* A user defined type

I call my project `Chickadee`. It is open source and I would love to have contributors!

It works on the .Net Core, which means it is cross-platform. I can produce a program that will run on Windows, Mac, or Linux.

## What other software is there?

There are lots of other APRS software programs. Here are a few of them

* Xastir
* APRSDroid
* PinPoint
* APRSISCE/32

## Why would I use Chickadee?

* You need a cross-platform APRS message generator. (I want to run this on my Raspberry Pi and my Windows PC!)
* You want to develop your own APRS data format for a special event
* You want to integrate APRS into your own software application
* Open source and free!
* You want to get into programming or try it out. With Chickadee you can combine programming and ham radio!

## Readable code!
