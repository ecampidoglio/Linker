# Linker ![](https://github.com/ecampidoglio/Linker/blob/master/Icon.png)

|  Azure Pipelines | AppVeyor | GitHub Actions | TeamCity | Travis CI | Coveralls |
|  :-------------: | :------: | :------------: | :------: | :-------: | :-------: |
| [![Azure Pipelines](https://img.shields.io/azure-devops/build/megakemp/24a2406e-4c2e-40a4-a766-7ad55e45178f/3.svg)](https://megakemp.visualstudio.com/Linker/_build/latest?definitionId=3&branchName=master) | [![AppVeyor](https://img.shields.io/appveyor/ci/ecampidoglio/linker.svg)](https://ci.appveyor.com/project/ecampidoglio/linker) | [![GitHub Actions](https://github.com/ecampidoglio/Linker/workflows/Build/badge.svg)](https://github.com/ecampidoglio/Linker/actions?workflow=Build) | [![TeamCity](https://img.shields.io/teamcity/http/delivery-megakemp.northeurope.cloudapp.azure.com:8080/s/Linker_Build_master.svg)](http://delivery-megakemp.northeurope.cloudapp.azure.com:8080/viewType.html?buildTypeId=Linker_Build_master&guest=1) | [![Travis CI](https://img.shields.io/travis/ecampidoglio/Linker.svg)](https://travis-ci.org/ecampidoglio/Linker) | [![Coveralls](https://coveralls.io/repos/github/ecampidoglio/Linker/badge.svg?branch=master)](https://coveralls.io/github/ecampidoglio/Linker?branch=master) |

Linker is a _really_ simple web-based URL shortening service.

## Background

When I set out to create a [Pluralsight course](http://bit.ly/ps-cake) about my favorite build tool, [Cake](https://cakebuild.net), I needed a sample app for my demos. The idea behind the course was to take an existing web app and show you how to use Cake to create a *build* and *deployment* pipeline for it from scratch. By the end, you'd have a Cake script that can take the application all the way from source code to software running on a server. That idea eventually turned into [*Building and Deploying Applications with Cake*](http://bit.ly/ps-cake), which you can read more about [here](https://megakemp.com/2017/10/20/cake-at-pluralsight/).

Right from the start, I had three requirements for my demo app:

1. Be simple
2. Be realistic
3. Don't be boring

I wanted an app that *felt* realistic—alas, not the usual ASP.NET MVC sample app—but that was also simple enough not to become a [cognitive burden](https://en.wikipedia.org/wiki/Cognitive_load). The last thing you want in a course is having to spend half the time explaining what your demo app does.

I could have gone for one of the classics like the fake CRM or the online pet store, but that would be boring. Remember, I wanted to do something *fun* and, possibly, *useful*.

So, I decided to make a URL shortening service: meet [Linker](https://lnker.net).

## Overview

Linker consists of a RESTful API backed by a relational database and consumed by a web frontend. Here's an overview:

![Linker's Architecture](https://megakemp.com/assets/cake-at-pluralsight/demo-application.png)

This repository contains the complete source code, with tests and everything. There's also the complete [build and deployment script](https://github.com/ecampidoglio/Linker/blob/53dfd94147e6ea9f408190901eeefb6332cc57b2/build.cake) written with Cake.

## Platform

Cake is cross-platform and so is Linker. Originally, I wrote two versions of it:

* One that runs only on Windows on top of the [.NET Framework](https://docs.microsoft.com/en-us/dotnet/framework/)
* One that runs on Windows, macOS or Linux built on top of [.NET Core](https://docs.microsoft.com/en-us/dotnet/core/get-started)

You'll find the Windows-only version in the [`pluralsight-net4.6`](https://github.com/ecampidoglio/Linker/tree/pluralsight-net4.6) branch, while the cross-platform one is in a branch called [`pluralsight-netcore1.0`](https://github.com/ecampidoglio/Linker/tree/pluralsight-netcore1.0). These are the exact versions featured in the [Pluralsight course](http://bit.ly/ps-cake) and will stay as they are for reference.

As time went on, however, I realized that if Linker was going to stay valuable, it had to be kept up-to-date with the current technology. So, I eventually upgraded the .NET Core version from 1.0 to [2.1 LTS](https://devblogs.microsoft.com/dotnet/announcing-net-core-2-1/).

.NET Core is clearly the future when it comes to .NET development; it's also cross-platform, leaving little value in maintaining a separate version that only runs on Windows. For these reasons, I decided to make the .NET Core 2.1 version the _canonical version_ by [merging it](https://github.com/ecampidoglio/Linker/commit/08a80e5dce4f7a10f0725a589e53598d12f0483e) into `master`. That became the officially maintained version of Linker going forward.

## What to Do with It

Although Linker came into existence to serve the needs of my Pluralsight course [Building and Deploying Applications with Cake](http://bit.ly/ps-cake), it has proven to be equally useful for other educational purposes. In fact, I'll go as far as to say that if you ever find yourself in need of a demo app for a course, a talk or a workshop, feel free to use Linker. To put in one sentence:

> Linker is a simple and complete ASP.NET Core web application, ready to go. :rocket:

Of course, if you would like to improve it in any way, pull requests are always welcome.

## Additional Resources

You can see Linker in action in the following online resources:

- [Building and Deploying Applications with Cake](https://www.pluralsight.com/courses/cake-applications-deploying-building) (Pluralsight Course)
- [Cake + .NET Core = Write Once, Build Anywhere](https://youtu.be/FKbykwvB_MU) (NDC London 2018 Conference Talk)
