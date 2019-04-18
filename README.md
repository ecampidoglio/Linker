# Linker ![](https://github.com/ecampidoglio/Linker/blob/master/Icon.png)

|  VSTS | AppVeyor | Travis CI |
|  :--: | :------: | :-------: |
| [![Visual Studio Team Services](https://img.shields.io/vso/build/megakemp/24a2406e-4c2e-40a4-a766-7ad55e45178f/1.svg?style=flat)](https://megakemp.visualstudio.com/Linker/_build/index?definitionId=1&_a=completed) | [![AppVeyor](https://ci.appveyor.com/api/projects/status/72o5vk00ta9p37j7?svg=true)](https://ci.appveyor.com/project/ecampidoglio/linker) | [![Travis CI](https://travis-ci.org/ecampidoglio/Linker.svg?branch=netcore)](https://travis-ci.org/ecampidoglio/Linker) |

Linker is a _really_ simple web-based URL shortening service.

## Background
When I set out to create a [Pluralsight course](http://bit.ly/ps-cake) about my favorite built tool, [Cake](https://cakebuild.net), I needed a .NET web application for my demos. The idea of the course was to take an existing web app and show how to use Cake to create a *build* and *deployment* pipeline for it from scratch. By the end, you'd have a Cake script that can take the application all the way from source code to running software. That idea eventually became [*Building and Deploying Applications with Cake*](http://bit.ly/ps-cake), which you can read more about [here](https://megakemp.com/2017/10/20/cake-at-pluralsight/).

Back to the app. Right off the bat, I had three requirements for it:

1. Be simple
2. Be realistic
3. Don't be boring

In other words, I wanted an app that *felt* realistic—alas, not the ASP.NET MVC sample application—but also simple enough so not to become a [cognitive burden](https://en.wikipedia.org/wiki/Cognitive_load). The last thing you want in a course (or presentation) is to spend half of the time explaining the domain of your *demo* app to the audience.

Of course, I could have gone for one of the classics like the music collection database or the online pet store we all know too well. But I wanted something *fun* and, if possible, *useful*.

You would never guess what I ended up making: a URL shortening service. Meet [Linker](http://lnker.net).

## Overview

Linker consists of a RESTful API backed by a database and consumed by a web frontend. Here's an overview:

![Linker's Architecture](https://megakemp.com/assets/cake-at-pluralsight/demo-application.png)

This repository contains the complete source code, with tests and everything. Of course, there's also the complete [build and deployment script](https://github.com/ecampidoglio/Linker/blob/53dfd94147e6ea9f408190901eeefb6332cc57b2/build.cake) written with Cake.

## Platform

Since Cake is cross-platform, Linker should be as well. So I wrote two versions of it:

* One that runs only on Windows on top of the [.NET Framework](https://docs.microsoft.com/en-us/dotnet/framework/)
* One that runs on Windows, macOS or Linux built on top of [.NET Core](https://docs.microsoft.com/en-us/dotnet/core/get-started)

You'll find the Windows-only version in the [`pluralsight-net4.6`](https://github.com/ecampidoglio/Linker/tree/pluralsight-net4.6) branch, while the cross-platform one is in a branch called [`pluralsight-netcore1.0`](https://github.com/ecampidoglio/Linker/tree/pluralsight-netcore1.0). These are the exact versions featured in the [Pluralsight course](http://bit.ly/ps-cake) and will stay as they are for reference.

As time went on, however, I realized that if Linker was going to stay valuable, it had to be kept up-to-date with the current technology. So, I eventually upgraded the .NET Core version from 1.0 to [2.1 LTS](https://devblogs.microsoft.com/dotnet/announcing-net-core-2-1/).

.NET Core is clearly the future when it comes to .NET development; it's also cross-platform, leaving little value in maintaining a separate version that only runs on Windows. For these reasons, I decided to make the .NET Core 2.1 version the _canonical version_ by [merging it](https://github.com/ecampidoglio/Linker/commit/08a80e5dce4f7a10f0725a589e53598d12f0483e) into `master`. That became the officially maintained version of Linker going forward.

## What to Do with It

Although Linker came into existence to serve the needs of my Pluralsight course [Building and Deploying Applications with Cake](http://bit.ly/ps-cake), it has proven to be equally useful for other educational purposes. In fact, I'll go as far as to say that if you ever need a demo app for a course, a talk or a workshop, you should feel free to use Linker for it. To put in one sentence:

> Linker is a simple and complete ASP.NET Core web application, ready to go. :rocket:

Of course, if you would like to improve it in any way, pull requests are always welcome.

## Additional Resources

You can see Linker in action in the following online resources:

- [Building and Deploying Applications with Cake](https://www.pluralsight.com/courses/cake-applications-deploying-building) (Pluralsight Course)
- [Cake + .NET Core = Write Once, Build Anywhere](https://youtu.be/FKbykwvB_MU) (NDC London 2018 Conference Talk)
